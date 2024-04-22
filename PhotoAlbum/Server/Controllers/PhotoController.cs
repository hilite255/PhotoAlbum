using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PhotoAlbum.Shared.Model;

namespace PhotoAlbum.Server.Controllers
{
    [Route("api/photo")]
    [ApiController]
    public class PhotoController : ControllerBase
    {
        private readonly DatabaseContext _dbcontext;
        private readonly string imagePath = "..\\images";
        public PhotoController(DatabaseContext dbcontext)
        {
            _dbcontext = dbcontext;
        }

        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpGet("{id}")]
        public async Task<ActionResult<Photo>> GetPhoto(int id)
        {
            return await _dbcontext.Photos.SingleOrDefaultAsync(p => p.ID == id);
        }

        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpGet("")]
        public async Task<ActionResult<List<Photo>>> GetMyPhotos()
        {
            if (User == null || User.Identity == null || User.Identity.Name == null)
            {
                return Unauthorized();
            }
            return await _dbcontext.Photos.Where(p => p.Owner == User.Identity.Name).ToListAsync();
        }

        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpPut("{id}")]
        public async Task<IActionResult> AddImage(int id, [FromForm(Name = "image")] IFormFile image)
        {
            if (image == null)
                return NotFound();
            string extension = Path.GetExtension(image.FileName);
            using (var stream = new FileStream($"{imagePath}\\{id}{extension}", FileMode.OpenOrCreate))
            {
                await image.CopyToAsync(stream);
            }

            return Ok();
        }

        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpPost("{name}")]
        public async Task<ActionResult<Photo>> CreatePhoto(string name)
        {
            if (User == null || User.Identity == null || User.Identity.Name == null)
            {
                return Unauthorized();
            }
            var photo = new Photo() { Name = name, UploadTime = DateTime.Now, Owner = User.Identity.Name };
            await _dbcontext.AddAsync(photo);
            await _dbcontext.SaveChangesAsync();

            return photo;
        }

        [HttpGet("image/{imageId}")]
        public IActionResult GetImage(int imageId)
        {
            var files = Directory.GetFiles(imagePath, $"{imageId}.*");
            if (files.Length > 0)
            {
                string extension = Path.GetExtension(files.First());
                return new FileStreamResult(new FileStream(files.First(), FileMode.Open, FileAccess.Read, FileShare.Read), $"image/{extension}");
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
