using System.IO;

using chapter09.lib.Data;
using chapter09.lib.ML;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace chapter09.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UploadController : ControllerBase
    {
        private readonly FileClassificationPredictor _predictor;

        public UploadController(FileClassificationPredictor predictor)
        {
            _predictor = predictor;
        }

        private static byte[] GetBytesFromPost(IFormFile file)
        {
            using (var ms = new BinaryReader(file.OpenReadStream()))
            {
                return ms.ReadBytes((int)file.Length);
            }
        }

        [HttpPost]
        public FileClassificationResponseItem Post(IFormFile file)
        {
            if (file == null)
            {
                return null;
            }

            var fileBytes = GetBytesFromPost(file);

            var responseItem = new FileClassificationResponseItem(fileBytes);

            return _predictor.Predict(responseItem);
        }
    }
}