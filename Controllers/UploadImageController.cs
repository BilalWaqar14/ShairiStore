using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace ShairiStore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin,Manager")] // Restrict to Admin & Manager
    public class UploadImageController : ControllerBase
    {
        private readonly string _baseUploadPath;

        public UploadImageController()
        {
            // Root folder for all images (in project directory)
            _baseUploadPath = Path.Combine(Directory.GetCurrentDirectory(), "images");
        }

        /// <summary>
        /// Upload controller to upload images in system
        /// </summary>
        /// <param name="file"></param>
        /// <param name="uploadType">int value {1 => Invoice, 2 => Payments, 3 => Orders, 4 => Users}</param>
        /// <returns></returns>
        [HttpPost("image")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UploadImage([FromForm] UploadImageRequest file)
        {
            var postFix = file.ActionType == 1 ? "Invoice" : file.ActionType == 2 ? "Payment" : file.ActionType == 3 ? "Order" : "User";
            if (file.File == null || file.File.Length == 0)
                return BadRequest("No file uploaded.");

            // Create folder if not exists
            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", postFix);
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            // Unique file name
            var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(file.File.FileName);
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.File.CopyToAsync(stream);
            }

            var fileUrl = $"{Request.Scheme}://{Request.Host}/uploads/{postFix}/{uniqueFileName}";

            return Ok(new { FileName = uniqueFileName, Url = fileUrl });
        }


        public class UploadImageRequest
        {
            [Required]
            public IFormFile File { get; set; }

            [Required]
            public int ActionType { get; set; }
        }
    }
}
