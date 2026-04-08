using Microsoft.AspNetCore.Mvc;
using Portfolio.Business;
using Portfolio.DataAccess;

namespace PortfolioAPI.Controllers
{
    [ApiController]
    [Route("api/Projects")]
    public class ProjectController : Controller
    {
        private readonly clsProject __Project;

        public ProjectController(clsProject Project)
        {
            __Project = Project;
        }

        [HttpGet]
        [ProducesResponseType(typeof(List<ProjectDTO>), 200)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<List<ProjectDTO>>> GetAll()
        {
            try { return Ok(await __Project.getAllProjects()); }
            catch (Exception ex) { return StatusCode(500, new { message = "Internal Server Error", error = ex.Message }); }
        }

        [HttpGet("{id:long}")]
        [ProducesResponseType(typeof(ProjectDTO), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<ProjectDTO>> GetById(long id)
        {
            try
            {
                var item = await __Project.getProjectById(id);
                if (item == null || item.ID == 0) return NotFound();
                return Ok(item);
            }
            catch (Exception ex) { return StatusCode(500, new { message = "Internal Server Error", error = ex.Message }); }
        }

        [HttpGet("person/{personId:long}")]
        [ProducesResponseType(typeof(List<ProjectDTO>), 200)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<List<ProjectDTO>>> GetByPerson(long personId)
        {
            try { return Ok(await __Project.getProjectsByPerson(personId)); }
            catch (Exception ex) { return StatusCode(500, new { message = "Internal Server Error", error = ex.Message }); }
        }

        [HttpPost]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<ActionResult> Create([FromBody] ProjectDTO dto)
        {
            try
            {
                if (dto == null) return BadRequest();
                var id = await __Project.addNewProject(dto);
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
        public async Task<ActionResult> Update(long id, [FromBody] ProjectDTO dto)
        {
            try
            {
                if (dto == null || id != dto.ID) return BadRequest();
                var ok = await __Project.updateProjectById(dto);
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
                var ok = await __Project.deleteProjectById(id);
                if (!ok) return NotFound();
                return NoContent();
            }
            catch (Exception ex) { return StatusCode(500, new { message = "Internal Server Error", error = ex.Message }); }
        }
    }
}
