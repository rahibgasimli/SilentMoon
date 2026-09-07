using SlientMoon.Domain.Common;
using SlientMoon.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SlientMoon.Application.Interfaces.Services
{
    public interface ITranslationService
    {
        Task<Dictionary<string, string>> GetTranslationsAsync(
            IEnumerable<string> keys,
            LanguageCode languageCode,
            CancellationToken cancellationToken = default);

        Task SaveTranslationsAsync<TEntity>(
            string entityId,
            LanguageCode languageCode,
            Dictionary<TranslatableField, string> propertyValues,
            CancellationToken cancellationToken = default) where TEntity : BaseEntity;

        Task DeleteTranslationsAsync<TEntity>(
            string entityId,
            IEnumerable<TranslatableField> fields,
            CancellationToken cancellationToken = default) where TEntity : BaseEntity;

        Task SaveAllTranslationsAsync<TEntity, TTranslationInput>(
            string entityId,
            Dictionary<LanguageCode, TTranslationInput> languages,
            Func<TTranslationInput, Dictionary<TranslatableField, string>> fieldSelector,
            CancellationToken cancellationToken = default) where TEntity : BaseEntity;
    }
}
