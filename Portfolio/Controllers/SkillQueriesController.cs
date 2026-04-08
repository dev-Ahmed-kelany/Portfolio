using Microsoft.AspNetCore.Mvc;
using Portfolio.Business;
using Portfolio.DataAccess;

namespace PortfolioAPI.Controllers
{
    [ApiController]
    [Route("api/SkillQueries")]
    public class SkillQueriesController : Controller
    {
        private readonly clsSkillQueries __SkillQueries;

        public SkillQueriesController(clsSkillQueries SkillQueries)
        {
            __SkillQueries = SkillQueries;
        }

        [HttpGet("person/{personId:long}")]
        [ProducesResponseType(typeof(List<SkillQueryResultDTO>), 200)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<List<SkillQueryResultDTO>>> GetSkillsByPerson(long personId)
        {
            try { return Ok(await __SkillQueries.getSkillsByPerson(personId)); }
            catch (Exception ex) { return StatusCode(500, new { message = "Internal Server Error", error = ex.Message }); }
        }

        [HttpGet("project/{projectId:long}")]
        [ProducesResponseType(typeof(List<SkillQueryResultDTO>), 200)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<List<SkillQueryResultDTO>>> GetSkillsByProject(long projectId)
        {
            try { return Ok(await __SkillQueries.getSkillsByProject(projectId)); }
            catch (Exception ex) { return StatusCode(500, new { message = "Internal Server Error", error = ex.Message }); }
        }
    }
}
