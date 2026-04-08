using Portfolio.DataAccess;

namespace Portfolio.Business
{
    public class clsSkill
    {
        private readonly ISkill __Skill;

        public clsSkill(ISkill Skill)
        {
            __Skill = Skill;
        }

        public async Task<List<SkillDTO>> getAllSkills()
        {
            var Skills = await __Skill.getAll();
            return Skills;
        }

        public async Task<SkillDTO> getSkillById(long ID)
        {
            var Skill = await __Skill.getById(ID);
            return Skill;
        }

        public async Task<long> addNewSkill(SkillDTO skill)
        {
            var newId = await __Skill.addNew(skill);
            return newId;
        }

        public async Task<bool> updateSkillById(SkillDTO skill)
        {
            var result = await __Skill.updateById(skill);
            return result;
        }

        public async Task<bool> deleteSkillById(long ID)
        {
            var result = await __Skill.deleteById(ID);
            return result;
        }
    }
}
