using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PhotoAlbum.Shared.DTOs;
using PhotoAlbum.Shared.Model;

namespace PhotoAlbum.Server.Controllers
{
    [Route("api/photo")]
    [ApiController]
    public class PhotoController : ControllerBase
    {
        private readonly DatabaseContext _dbcontext;
        private readonly string imagePath = ".\\";
        public PhotoController(DatabaseContext dbcontext)
        {
            _dbcontext = dbcontext;
        }

        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpGet("{id}")]
        public async Task<ActionResult<PhotoDTO>> GetPhoto(int id)
        {
            var photo = await _dbcontext.Photos.SingleOrDefaultAsync(p => p.ID == id);
            if (photo == null) return NotFound();
            return Ok(photo.ToDTO());
        }

        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpGet("")]
        public async Task<ActionResult<List<PhotoDTO>>> GetMyPhotos()
        {
            if (User == null || User.Identity == null || User.Identity.Name == null)
            {
                return Unauthorized();
            }
            return await _dbcontext.Photos.Where(p => p.Owner == User.Identity.Name).Select(p => p.ToDTO()).ToListAsync();
        }

        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpPut("{id}")]
        public async Task<IActionResult> AddImage(int id, [FromForm(Name = "image")] IFormFile image)
        {
            if (image == null)
                return NotFound();
            string extension = Path.GetExtension(image.FileName);
            using (var stream = new MemoryStream())
            {
                await image.CopyToAsync(stream);
                var bytes = stream.ToArray();
                var photo = await _dbcontext.Photos.SingleOrDefaultAsync(p => p.ID == id);
                if (photo == null)
                    return NotFound();
                photo.Image = bytes;
                await _dbcontext.SaveChangesAsync();
            }

            return Ok();
        }

        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpPost("{name}")]
        public async Task<ActionResult<PhotoDTO>> CreatePhoto(string name)
        {
            if (User == null || User.Identity == null || User.Identity.Name == null)
            {
                return Unauthorized();
            }
            var photo = new Photo() { Name = name, UploadTime = DateTime.Now, Owner = User.Identity.Name, Image = Array.Empty<byte>() };
            await _dbcontext.AddAsync(photo);
            await _dbcontext.SaveChangesAsync();

            return photo.ToDTO();
        }

        [HttpGet("image/{imageId}")]
        public IActionResult GetImage(int imageId)
        {
            var photo = _dbcontext.Photos.SingleOrDefault(p => p.ID == imageId);
            if (photo == null) return NotFound();
            return new FileStreamResult(new MemoryStream(photo.Image), $"image/jpeg");

            var files = Directory.GetFiles(imagePath, $"{imageId}.*");
            if (files.Length > 0)
            {
                string extension = Path.GetExtension(files.First());
                return new FileStreamResult(new FileStream(files.First(), FileMode.Open, FileAccess.Read, FileShare.Read), $"{extension}");
            }
            return NotFound();
            
        }

        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpDelete("{id}")]
        public async Task<NoContentResult> Delete(int id)
        {
            var photo = await _dbcontext.Photos.SingleOrDefaultAsync(p => p.ID == id);
            if (photo != null)
            {
                _dbcontext.Photos.Remove(photo);
                await _dbcontext.SaveChangesAsync();
            }

            return NoContent();
        }
    }
}
