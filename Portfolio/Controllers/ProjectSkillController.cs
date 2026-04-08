using Microsoft.AspNetCore.Mvc;
using Portfolio.Business;
using Portfolio.DataAccess;

namespace PortfolioAPI.Controllers
{
    [ApiController]
    [Route("api/ProjectSkills")]
    public class ProjectSkillController : Controller
    {
        private readonly clsProjectSkill __ProjectSkill;

        public ProjectSkillController(clsProjectSkill ProjectSkill)
        {
            __ProjectSkill = ProjectSkill;
        }

        [HttpGet]
        [ProducesResponseType(typeof(List<ProjectSkillDTO>), 200)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<List<ProjectSkillDTO>>> GetAll()
        {
            try { return Ok(await __ProjectSkill.getAllProjectSkills()); }
            catch (Exception ex) { return StatusCode(500, new { message = "Internal Server Error", error = ex.Message }); }
        }

        [HttpGet("{id:long}")]
        [ProducesResponseType(typeof(ProjectSkillDTO), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<ProjectSkillDTO>> GetById(long id)
        {
            try
            {
                var item = await __ProjectSkill.getProjectSkillById(id);
                if (item == null || item.ID == 0) return NotFound();
                return Ok(item);
            }
            catch (Exception ex) { return StatusCode(500, new { message = "Internal Server Error", error = ex.Message }); }
        }

        [HttpPost]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<ActionResult> Create([FromBody] ProjectSkillDTO dto)
        {
            try
            {
                if (dto == null) return BadRequest();
                var id = await __ProjectSkill.addNewProjectSkill(dto);
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
        public async Task<ActionResult> Update(long id, [FromBody] ProjectSkillDTO dto)
        {
            try
            {
                if (dto == null || id != dto.ID) return BadRequest();
                var ok = await __ProjectSkill.updateProjectSkillById(dto);
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
                var ok = await __ProjectSkill.deleteProjectSkillById(id);
                if (!ok) return NotFound();
                return NoContent();
            }
            catch (Exception ex) { return StatusCode(500, new { message = "Internal Server Error", error = ex.Message }); }
        }
    }
}
