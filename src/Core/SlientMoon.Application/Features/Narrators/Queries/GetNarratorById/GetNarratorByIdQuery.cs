using Application.Abstractions.Messaging;
using SlientMoon.Application.DTOs.Narrators;
using SlientMoon.Domain.Errors;
using System.Threading;
using System.Threading.Tasks;

namespace SlientMoon.Application.Features.Narrators.Queries.GetNarratorById
{
    public class GetNarratorByIdQuery : IQuery<NarratorInput>
    {
        public string Id { get; set; }
    }

    public class GetNarratorByIdQueryHandler : IQueryHandler<GetNarratorByIdQuery, NarratorInput>
    {
        private readonly IUow _uow;

        public GetNarratorByIdQueryHandler(IUow uow)
        {
            _uow = uow;
        }

        public async Task<Result<NarratorInput>> Handle(GetNarratorByIdQuery query, CancellationToken ct)
        {
            var narrator = await _uow.NarratorRepository.GetByIdAsync(query.Id, ct);
            if (narrator is null)
                return NarratorErrors.NarratorNotFound;

            return new NarratorInput
            {
                Name = narrator.Name,
                Gender = narrator.Gender
            };
        }
    }
}
