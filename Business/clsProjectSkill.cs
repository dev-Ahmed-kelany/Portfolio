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
            if (!clsValidation.IsValidProjectSkillDTO(projectSkill))
                return 0;

            var newId = await __ProjectSkill.addNew(projectSkill);
            return newId;
        }

        public async Task<bool> updateProjectSkillById(ProjectSkillDTO projectSkill)
        {
            if (!clsValidation.IsValidProjectSkillDTO(projectSkill))
                return false;

            if (!clsValidation.IsValidId(projectSkill.ID))
                return false;

            var result = await __ProjectSkill.updateById(projectSkill);
            return result;
        }

        public async Task<bool> deleteProjectSkillById(long ID)
        {
            if (!clsValidation.IsValidId(ID))
                return false;

            var result = await __ProjectSkill.deleteById(ID);
            return result;
        }
    }
}
