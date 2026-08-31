using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SlientMoon.Application.Features.Tracks.Queries.StreamTrack;
using System.Threading;
using System.Threading.Tasks;

namespace SlientMoon.WebApi.Controllers
{
    public class StreamingController : BaseController
    {
        [HttpGet("{id}")]
        public async Task<IResult> StreamTrack([FromRoute] string id, CancellationToken ct)
        {
            string? rangeHeader = Request.Headers.Range.ToString();

            var result = await Dispatcher.Send(new StreamTrackQuery(id, rangeHeader), ct);

            return HandleResult(result);
        }
    }
}