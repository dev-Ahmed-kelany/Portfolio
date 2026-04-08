using Portfolio.DataAccess;

namespace Portfolio.Business
{
    public class clsProjectSkill
    {
        private readonly IProjectSkill __ProjectSkill;

        public clsProjectSkill(IProjectSkill ProjectSkill)
        {
            __ProjectSkill = ProjectSkill;
        }

        public async Task<List<ProjectSkillDTO>> getAllProjectSkills()
        {
            var ProjectSkills = await __ProjectSkill.getAll();
            return ProjectSkills;
        }

        public async Task<ProjectSkillDTO> getProjectSkillById(long ID)
        {
            var ProjectSkill = await __ProjectSkill.getById(ID);
            return ProjectSkill;
        }

        public async Task<long> addNewProjectSkill(ProjectSkillDTO projectSkill)
        {
            var newId = await __ProjectSkill.addNew(projectSkill);
            return newId;
        }

        public async Task<bool> updateProjectSkillById(ProjectSkillDTO projectSkill)
        {
            var result = await __ProjectSkill.updateById(projectSkill);
            return result;
        }

        public async Task<bool> deleteProjectSkillById(long ID)
        {
            var result = await __ProjectSkill.deleteById(ID);
            return result;
        }
    }
}
