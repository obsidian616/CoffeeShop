using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ImageService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PhotoController : ControllerBase
    {
        private readonly string _uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Uploads", "Products");

        public PhotoController()
        {
            // 確保目錄存在
            if (!Directory.Exists(_uploadPath))
            {
                Directory.CreateDirectory(_uploadPath);
            }
        }

        [HttpPost]
        public async Task<ActionResult<string>> Upload([FromForm] IFormFile imageUpload)
        {
            if (imageUpload == null || imageUpload.Length == 0)
            {
                return BadRequest("未上傳圖片。");
            }

            var filePath = Path.Combine(_uploadPath, imageUpload.FileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await imageUpload.CopyToAsync(stream);
            }

            return Ok(imageUpload.FileName); // 返回文件名
        }

        [HttpDelete("{fileName}")]
        public IActionResult Delete(string fileName)
        {
            var filePath = Path.Combine(_uploadPath, fileName);

            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
                return Ok();
            }

            return NotFound();
        }
    }
}