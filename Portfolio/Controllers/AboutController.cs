using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Portfolio.Business;
using Portfolio.DataAccess;
using System.Security.Claims;

namespace PortfolioAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/About")]
    public class AboutController : Controller
    {
        private readonly clsAbout __About;

        public AboutController(clsAbout About)
        {
            __About = About;
        }

        /// <summary>
        /// Get all about records
        /// </summary>
        [Authorize(Roles = "Admin")]
        [HttpGet]
        [ProducesResponseType(typeof(List<AboutDTO>), 200)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<List<AboutDTO>>> GetAll()
        {
            try
            {
                var items = await __About.getAllAbout();
                return Ok(items);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal Server Error", error = ex.Message });
            }
        }

        /// <summary>
        /// Get about by id
        /// </summary>
        [HttpGet("{id:long}")]
        [ProducesResponseType(typeof(AboutDTO), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<AboutDTO>> GetById(long id)
        {
            try
            {
                var item = await __About.getAboutById(id);
                if (item == null || item.ID == 0) return NotFound();
                return Ok(item);
            }
            catch (Exception ex) { return StatusCode(500, new { message = "Internal Server Error", error = ex.Message }); }
        }

        /// <summary>
        /// Get about by person id
        /// </summary>
        [AllowAnonymous]
        [HttpGet("person/{personId:long}")]
        [ProducesResponseType(typeof(AboutDTO), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<AboutDTO>> GetByPerson(long personId)
        {
            try
            {
                var item = await __About.getAboutByPerson(personId);
                if (item == null || item.ID == 0) return NotFound();
                return Ok(item);
            }
            catch (Exception ex) { return StatusCode(500, new { message = "Internal Server Error", error = ex.Message }); }
        }

        [HttpPost]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<ActionResult> Create([FromBody] AboutDTO dto)
        {
            try
            {
                if (dto == null) return BadRequest();
                var id = await __About.addNewAbout(dto);
                if (id <= 0) return StatusCode(500);
                return CreatedAtAction(nameof(GetById), new { id }, null);
            }
            catch (Exception ex) { return StatusCode(500, new { message = "Internal Server Error", error = ex.Message }); }
        }

        [HttpPut("{id:long}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<ActionResult> Update(long id, [FromBody] AboutDTO dto)
        {
            try
            {
                if (dto == null || id != dto.ID) return BadRequest();
                var ok = await __About.updateAboutById(dto);
                if (!ok) return NotFound();
                return NoContent();
            }
            catch (Exception ex) { return StatusCode(500, new { message = "Internal Server Error", error = ex.Message }); }
        }

        [HttpDelete("{id:long}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult> Delete(long id)
        {
            try
            {
                var ok = await __About.deleteAboutById(id);
                if (!ok) return NotFound();
                return NoContent();
            }
            catch (Exception ex) { return StatusCode(500, new { message = "Internal Server Error", error = ex.Message }); }
        }
    }
}
