using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SlientMoon.Application.Features.Storage.Commands.UploadFile;
using SlientMoon.Domain.Enums;
using SlientMoon.Domain.Errors;
using System.Threading.Tasks;

namespace SlientMoon.WebApi.Controllers
{
    public class StorageController : BaseController
    {
        [HttpPost("upload")]
        public async Task<IResult> Upload(
            IFormFile file,
            [FromForm] StorageType storageType,
            [FromForm] string? entityId)
        {
            if (file is null)
                return HandleResult(StorageErrors.FileEmpty);

            await using var stream = file.OpenReadStream();

            var command = new UploadFileCommand(
                stream,
                file.FileName,
                file.ContentType,
                storageType,
                entityId
            );

            var result = await Dispatcher.Send(command);

            return HandleResult(result);
        }
    }
}
