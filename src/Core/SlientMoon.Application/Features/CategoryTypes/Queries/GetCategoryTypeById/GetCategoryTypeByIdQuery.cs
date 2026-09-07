using Application.Abstractions.Messaging;
using SlientMoon.Application.DTOs;
using SlientMoon.Application.DTOs.CategoryType;
using SlientMoon.Domain.Errors;
using System.Threading;
using System.Threading.Tasks;

namespace SlientMoon.Application.Features.CategoryTypes.Queries.GetCategoryTypeById
{
    public class GetCategoryTypeByIdQuery : IQuery<CategoryTypeInput>
    {
        public string Id { get; set; }
    }

    public class GetCategoryTypeByIdQueryHandler : IQueryHandler<GetCategoryTypeByIdQuery, CategoryTypeInput>
    {
        private readonly IUow _uow;

        public GetCategoryTypeByIdQueryHandler(IUow uow)
        {
            _uow = uow;
        }

        public async Task<Result<CategoryTypeInput>> Handle(GetCategoryTypeByIdQuery query, CancellationToken ct)
        {
            var categoryType = await _uow.CategoryTypeRepository.GetByIdAsync(query.Id, ct);
            if (categoryType is null)
                return CategoryTypeErrors.CategoryTypeNotFound;

            return new CategoryTypeInput
            {
                Name = categoryType.Name,
                Slug = categoryType.Slug
            };
        }
    }
}