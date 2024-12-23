using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Blackberries.Services;

namespace Blackberries.Controllers
{
    public class FileController: Controller
    {
        public ActionResult Index(long id) 
        {
            var result = FileService.GetFile(id, HttpContext);

            if (result != null)
            {
                result.Value.stream.Seek(0, SeekOrigin.Begin);

                return base.File(result.Value.stream, result.Value.contentType);
            }

            return new EmptyResult();
        }

        public record UploadFilesModel(long housingId, IEnumerable<IFormFile>? files);

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Upload([FromForm] UploadFilesModel model)
        {
            FileService.RemoveAll(model.housingId, HttpContext);

            if (model.files != null)
            {
                foreach (var file in model.files)
                {
                    if (file != null && file.Length > 0)
                    {
                        FileService.UploadFile(model.housingId, file, HttpContext);
                    }
                }
            }

            return RedirectToAction("Index", "Home");
        }
    }
}
