using Portfolio.DataAccess;

namespace Portfolio.Business
{
    public class clsSkillCategory
    {
        private readonly ISkillCategory __SkillCategory;

        public clsSkillCategory(ISkillCategory SkillCategory)
        {
            __SkillCategory = SkillCategory;
        }

        public async Task<List<SkillCategoryDTO>> getAllSkillCategories()
        {
            var SkillCategories = await __SkillCategory.getAll();
            return SkillCategories;
        }

        public async Task<SkillCategoryDTO> getSkillCategoryById(long ID)
        {
            var SkillCategory = await __SkillCategory.getById(ID);
            return SkillCategory;
        }

        public async Task<long> addNewSkillCategory(SkillCategoryDTO skillCategory)
        {
            if (!clsValidation.IsValidSkillCategoryDTO(skillCategory))
                return 0;

            var newId = await __SkillCategory.addNew(skillCategory);
            return newId;
        }

        public async Task<bool> updateSkillCategoryById(SkillCategoryDTO skillCategory)
        {
            if (!clsValidation.IsValidSkillCategoryDTO(skillCategory))
                return false;

            if (!clsValidation.IsValidId(skillCategory.ID))
                return false;

            var result = await __SkillCategory.updateById(skillCategory);
            return result;
        }

        public async Task<bool> deleteSkillCategoryById(long ID)
        {
            if (!clsValidation.IsValidId(ID))
                return false;

            var result = await __SkillCategory.deleteById(ID);
            return result;
        }
    }
}
