using Microsoft.AspNetCore.Mvc;
using Portfolio.Business;
using Portfolio.DataAccess;

namespace PortfolioAPI.Controllers
{
    [ApiController]
    [Route("api/Experiences")]
    public class ExperienceController : Controller
    {
        private readonly clsExperience __Experience;

        public ExperienceController(clsExperience Experience)
        {
            __Experience = Experience;
        }

        [HttpGet]
        [ProducesResponseType(typeof(List<ExperienceDTO>), 200)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<List<ExperienceDTO>>> GetAll()
        {
            try { return Ok(await __Experience.getAllExperiences()); }
            catch (Exception ex) { return StatusCode(500, new { message = "Internal Server Error", error = ex.Message }); }
        }

        [HttpGet("{id:long}")]
        [ProducesResponseType(typeof(ExperienceDTO), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<ExperienceDTO>> GetById(long id)
        {
            try
            {
                var item = await __Experience.getExperienceById(id);
                if (item == null || item.ID == 0) return NotFound();
                return Ok(item);
            }
            catch (Exception ex) { return StatusCode(500, new { message = "Internal Server Error", error = ex.Message }); }
        }

        [HttpGet("person/{personId:long}")]
        [ProducesResponseType(typeof(List<ExperienceDTO>), 200)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<List<ExperienceDTO>>> GetByPerson(long personId)
        {
            try { return Ok(await __Experience.getExperiencesByPerson(personId)); }
            catch (Exception ex) { return StatusCode(500, new { message = "Internal Server Error", error = ex.Message }); }
        }

        [HttpPost]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<ActionResult> Create([FromBody] ExperienceDTO dto)
        {
            try
            {
                if (dto == null) return BadRequest();
                var id = await __Experience.addNewExperience(dto);
                if (id <= 0) return StatusCode(500);
                return CreatedAtAction(nameof(GetById), new { id }, null);
            }
            catch (Exception ex) { return StatusCode(500, new { message = "Internal Server Error", error = ex.Message }); }
        }

        [HttpPut("{id:long}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult> Update(long id, [FromBody] ExperienceDTO dto)
        {
            try
            {
                if (dto == null || id != dto.ID) return BadRequest();
                var ok = await __Experience.updateExperienceById(dto);
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
                var ok = await __Experience.deleteExperienceById(id);
                if (!ok) return NotFound();
                return NoContent();
            }
            catch (Exception ex) { return StatusCode(500, new { message = "Internal Server Error", error = ex.Message }); }
        }
    }
}
