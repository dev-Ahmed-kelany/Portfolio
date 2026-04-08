using Microsoft.AspNetCore.Mvc;
using Portfolio.Business;
using Portfolio.DataAccess;

namespace PortfolioAPI.Controllers
{
    [ApiController]
    [Route("api/PersonSkills")]
    public class PersonSkillController : Controller
    {
        private readonly clsPersonSkill __PersonSkill;

        public PersonSkillController(clsPersonSkill PersonSkill)
        {
            __PersonSkill = PersonSkill;
        }

        [HttpGet]
        [ProducesResponseType(typeof(List<PersonSkillDTO>), 200)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<List<PersonSkillDTO>>> GetAll()
        {
            try { return Ok(await __PersonSkill.getAllPersonSkills()); }
            catch (Exception ex) { return StatusCode(500, new { message = "Internal Server Error", error = ex.Message }); }
        }

        [HttpGet("{id:long}")]
        [ProducesResponseType(typeof(PersonSkillDTO), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<PersonSkillDTO>> GetById(long id)
        {
            try
            {
                var item = await __PersonSkill.getPersonSkillById(id);
                if (item == null || item.ID == 0) return NotFound();
                return Ok(item);
            }
            catch (Exception ex) { return StatusCode(500, new { message = "Internal Server Error", error = ex.Message }); }
        }

        [HttpGet("person/{personId:long}")]
        [ProducesResponseType(typeof(List<PersonSkillDTO>), 200)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<List<PersonSkillDTO>>> GetByPerson(long personId)
        {
            try { return Ok(await __PersonSkill.getPersonSkillsByPerson(personId)); }
            catch (Exception ex) { return StatusCode(500, new { message = "Internal Server Error", error = ex.Message }); }
        }

        [HttpPost]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<ActionResult> Create([FromBody] PersonSkillDTO dto)
        {
            try
            {
                if (dto == null) return BadRequest();
                var id = await __PersonSkill.addNewPersonSkill(dto);
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
        public async Task<ActionResult> Update(long id, [FromBody] PersonSkillDTO dto)
        {
            try
            {
                if (dto == null || id != dto.ID) return BadRequest();
                var ok = await __PersonSkill.updatePersonSkillById(dto);
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
                var ok = await __PersonSkill.deletePersonSkillById(id);
                if (!ok) return NotFound();
                return NoContent();
            }
            catch (Exception ex) { return StatusCode(500, new { message = "Internal Server Error", error = ex.Message }); }
        }
    }
}
