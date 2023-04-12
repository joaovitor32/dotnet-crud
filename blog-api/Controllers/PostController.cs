using blog_api.Models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace blog_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostsController : ControllerBase
    {
        private readonly PostContext _dbContext;

        public PostsController(PostContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Post>>> GetPosts()
        {
           
            if(_dbContext.Posts == null)
            {
                return NotFound();
            }

            return await _dbContext.Posts.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Post>> GetPost(int id)
        {
            if (_dbContext.Posts == null)
            {
                return NotFound();
            }

            var post = await _dbContext.Posts.FindAsync(id);

            if (post == null)
            {
                return NotFound(id);
            }

            return post;
        }

        [HttpPost]
        [EnableCors("AllowAllOrigins")]
        public async Task<ActionResult<Post>> AddPost([FromBody] Post post)
        {
            _dbContext.Posts.Add(post);
            await _dbContext.SaveChangesAsync();

            return CreatedAtAction(nameof(GetPost), new { id = post.Id }, post);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutPost(int id, Post post)
        {
            if (id != post.Id)
            {
                return BadRequest();
            }

            _dbContext.Entry(post).State = EntityState.Modified;

            try{
                await _dbContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PostExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return NoContent();
        }

        private bool PostExists(long id)
        {
            return (_dbContext.Posts?.Any(p => p.Id == id)).GetValueOrDefault();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePost(int id)
        {
            if (_dbContext.Posts == null)
            {
                return NotFound(id);
            }

            var post = await _dbContext.Posts.FindAsync(id);
            
            if(post == null)
            {
                return NotFound();
            }

            _dbContext.Posts.Remove(post);
            await _dbContext.SaveChangesAsync();

            return NoContent();
        }
    }
}
