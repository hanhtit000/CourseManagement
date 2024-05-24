using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO.Compression;

namespace Project_CreateAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class DownloadController : ControllerBase
    {
        [HttpGet("material")]
        public IActionResult DownloadMaterial(string fileName)
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Upload\\Materials", fileName);
            if (!System.IO.File.Exists(filePath))
            {
                return NotFound("File not found!");
            }
            var fileContent = System.IO.File.ReadAllBytes(filePath);
            var contentType = "application/octet-stream";
            return File(fileContent, contentType, fileName);
        }

        [HttpGet("submission")]
        public IActionResult DownloadSubmission(string fileName)
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Upload\\Submissions", fileName);
            if (!System.IO.File.Exists(filePath))
            {
                return NotFound("File not found!");
            }
            var fileContent = System.IO.File.ReadAllBytes(filePath);
            var contentType = "application/octet-stream";
            return File(fileContent, contentType, fileName);
        }

        [HttpPost("all-submissions")]
        public IActionResult DownloadAllSubmissions([FromBody] List<string> fileNames)
        {
            var zipFileName = DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + "submissions.zip";
            var zipFilePath = Path.Combine(Directory.GetCurrentDirectory(), "Upload", zipFileName);

            using (var zipArchive = ZipFile.Open(zipFilePath, ZipArchiveMode.Create))
            {
                foreach (var fileName in fileNames)
                {
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Upload\\Submissions", fileName);
                    if (System.IO.File.Exists(filePath))
                    {
                        var entryName = Path.GetFileName(filePath);
                        zipArchive.CreateEntryFromFile(filePath, entryName);
                    }
                }
            }

            var zipFileContent = System.IO.File.ReadAllBytes(zipFilePath);
            var contentType = "application/octet-stream";
            return File(zipFileContent, contentType, zipFileName);
        }
    }
}
