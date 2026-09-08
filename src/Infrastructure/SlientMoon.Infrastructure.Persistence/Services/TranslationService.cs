using Microsoft.EntityFrameworkCore;
using SlientMoon.Application.Common.Extensions;
using SlientMoon.Application.Interfaces.Services;
using SlientMoon.Domain.Common;
using SlientMoon.Domain.Entities;
using SlientMoon.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SlientMoon.Infrastructure.Persistence.Services
{
    public class TranslationService : ITranslationService
    {
        private readonly IUow _uow;

        public TranslationService(IUow uow)
        {
            _uow = uow;
        }

        public async Task<Dictionary<string, string>> GetTranslationsAsync(IEnumerable<string> keys, LanguageCode languageCode, CancellationToken cancellationToken = default)
        {
            var keyList = keys.Distinct().ToList();

            if (!keyList.Any())
                return new Dictionary<string, string>();

            var translations = await _uow.GenericRepository<Translation>()
                .GetQueryable()
                .Where(t => t.LanguageCode == languageCode && keyList.Contains(t.Key))
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            return translations.ToDictionary(t => t.Key, t => t.Value);
        }

        public async Task SaveTranslationsAsync<TEntity>(string entityId, LanguageCode languageCode, Dictionary<TranslatableField, string> propertyValues, CancellationToken cancellationToken = default) where TEntity : BaseEntity
        {
            var keys = propertyValues.Keys
                .Select(field => TranslationExtensions.BuildKey<TEntity>(field.ToString(), entityId))
                .ToList();

            var dbTranslations = await _uow.GenericRepository<Translation>()
                .GetQueryable()
                .Where(t => t.LanguageCode == languageCode && keys.Contains(t.Key))
                .ToDictionaryAsync(t => t.Key, cancellationToken);

            foreach (var (field, value) in propertyValues)
            {
                var key = TranslationExtensions.BuildKey<TEntity>(field.ToString(), entityId);

                if (dbTranslations.TryGetValue(key, out var existingTranslation))
                {
                    existingTranslation.Value = value;
                }
                else
                {
                    var newTranslation = new Translation
                    {
                        Id = Guid.NewGuid().ToString(),
                        Key = key,
                        LanguageCode = languageCode,
                        Value = value
                    };
                    await _uow.GenericRepository<Translation>().AddAsync(newTranslation);
                }
            }
        }

        public async Task DeleteTranslationsAsync<TEntity>(
            string entityId,
            IEnumerable<TranslatableField> fields,
            CancellationToken cancellationToken = default) where TEntity : BaseEntity
        {
            var keys = fields
                .Select(field => TranslationExtensions.BuildKey<TEntity>(field.ToString(), entityId))
                .ToList();

            if (keys.Count == 0)
                return;

            var translations = await _uow.GenericRepository<Translation>()
                .GetQueryable()
                .Where(t => keys.Contains(t.Key))
                .ToListAsync(cancellationToken);

            foreach (var translation in translations)
                _uow.GenericRepository<Translation>().Delete(translation);
        }

        public async Task<Dictionary<LanguageCode, Dictionary<TranslatableField, string>>> GetTranslationMapAsync<TEntity>(
            string entityId,
            IEnumerable<TranslatableField> fields,
            CancellationToken cancellationToken = default) where TEntity : BaseEntity
        {
            var keyToField = fields.ToDictionary(
                field => TranslationExtensions.BuildKey<TEntity>(field.ToString(), entityId),
                field => field);

            var keys = keyToField.Keys.ToList();

            var rows = await _uow.GenericRepository<Translation>()
                .GetQueryable()
                .Where(t => keys.Contains(t.Key))
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            var map = new Dictionary<LanguageCode, Dictionary<TranslatableField, string>>();

            foreach (var row in rows)
                map.Set(row.LanguageCode, keyToField[row.Key], row.Value);

            return map;
        }

        public async Task SaveAllTranslationsAsync<TEntity>(
            string entityId,
            Dictionary<LanguageCode, Dictionary<TranslatableField, string>> languages,
            CancellationToken cancellationToken = default) where TEntity : BaseEntity
        {
            if (languages is null)
                return;

            foreach (var (languageCode, fields) in languages)
            {
                if (fields is null)
                    continue;

                var propertyValues = fields
                    .Where(kv => !string.IsNullOrWhiteSpace(kv.Value))
                    .ToDictionary(kv => kv.Key, kv => kv.Value);

                if (propertyValues.Count == 0)
                    continue;

                await SaveTranslationsAsync<TEntity>(entityId, languageCode, propertyValues, cancellationToken);
            }
        }
    }
}
