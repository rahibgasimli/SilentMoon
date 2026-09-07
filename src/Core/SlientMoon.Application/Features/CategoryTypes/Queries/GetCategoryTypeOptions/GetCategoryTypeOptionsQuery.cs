using Application.Abstractions.Messaging;
using SlientMoon.Application.DTOs.Categories;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SlientMoon.Application.Features.CategoryTypes.Queries.GetCategoryTypeOptions
{
    public class GetCategoryTypeOptionsQuery : IQuery<List<CategoryTypeOptionDto>>
    {
    }

    public class GetCategoryTypeOptionsQueryHandler : IQueryHandler<GetCategoryTypeOptionsQuery, List<CategoryTypeOptionDto>>
    {
        private readonly IUow _uow;

        public GetCategoryTypeOptionsQueryHandler(IUow uow)
        {
            _uow = uow;
        }

        public async Task<Result<List<CategoryTypeOptionDto>>> Handle(GetCategoryTypeOptionsQuery query, CancellationToken ct)
        {
            return await _uow.CategoryRepository.GetCategoryTypeOptionsAsync(ct);
        }
    }
}
