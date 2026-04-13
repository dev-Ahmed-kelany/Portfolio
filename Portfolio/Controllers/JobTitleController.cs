using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Portfolio.Business;
using Portfolio.DataAccess;

namespace PortfolioAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/JobTitles")]
    public class JobTitleController : Controller
    {
        private readonly clsJobTitle __JobTitle;

        public JobTitleController(clsJobTitle JobTitle)
        {
            __JobTitle = JobTitle;
        }

        [HttpGet]
        [ProducesResponseType(typeof(List<JobTitleDTO>), 200)]
        [ProducesResponseType(500)]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<List<JobTitleDTO>>> GetAll()
        {
            try { return Ok(await __JobTitle.getAllJobTitles()); }
            catch (Exception ex) { return StatusCode(500, new { message = "Internal Server Error", error = ex.Message }); }
        }

        [HttpGet("{id:long}")]
        [ProducesResponseType(typeof(JobTitleDTO), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<JobTitleDTO>> GetById(long id)
        {
            try
            {
                var item = await __JobTitle.getJobTitleById(id);
                if (item == null || item.ID == 0) return NotFound();
                return Ok(item);
            }
            catch (Exception ex) { return StatusCode(500, new { message = "Internal Server Error", error = ex.Message }); }
        }

        [HttpGet("person/{personId:long}")]
        [ProducesResponseType(typeof(List<JobTitleDTO>), 200)]
        [ProducesResponseType(500)]
        [AllowAnonymous]
        public async Task<ActionResult<List<JobTitleDTO>>> GetByPerson(long personId)
        {
            try { return Ok(await __JobTitle.getJobTitlesByPerson(personId)); }
            catch (Exception ex) { return StatusCode(500, new { message = "Internal Server Error", error = ex.Message }); }
        }

        [HttpPost]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<ActionResult> Create([FromBody] JobTitleDTO dto)
        {
            try
            {
                if (dto == null) return BadRequest();
                var id = await __JobTitle.addNewJobTitle(dto);
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
        public async Task<ActionResult> Update(long id, [FromBody] JobTitleDTO dto)
        {
            try
            {
                if (dto == null || id != dto.ID) return BadRequest();
                var ok = await __JobTitle.updateJobTitleById(dto);
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
                var ok = await __JobTitle.deleteJobTitleById(id);
                if (!ok) return NotFound();
                return NoContent();
            }
            catch (Exception ex) { return StatusCode(500, new { message = "Internal Server Error", error = ex.Message }); }
        }
    }
}
