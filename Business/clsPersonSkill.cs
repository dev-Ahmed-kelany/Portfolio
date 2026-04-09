using Portfolio.DataAccess;

namespace Portfolio.Business
{
    public class clsPersonSkill
    {
        private readonly IPersonSkill __PersonSkill;

        public clsPersonSkill(IPersonSkill PersonSkill)
        {
            __PersonSkill = PersonSkill;
        }

        public async Task<List<PersonSkillDTO>> getAllPersonSkills()
        {
            var PersonSkills = await __PersonSkill.getAll();
            return PersonSkills;
        }

        public async Task<PersonSkillDTO> getPersonSkillById(long ID)
        {
            var PersonSkill = await __PersonSkill.getById(ID);
            return PersonSkill;
        }

        public async Task<List<PersonSkillDTO>> getPersonSkillsByPerson(long PersonID)
        {
            var PersonSkills = await __PersonSkill.getByPerson(PersonID);
            return PersonSkills;
        }

        public async Task<long> addNewPersonSkill(PersonSkillDTO personSkill)
        {
            if (!clsValidation.IsValidPersonSkillDTO(personSkill))
                return 0;

            var newId = await __PersonSkill.addNew(personSkill);
            return newId;
        }

        public async Task<bool> updatePersonSkillById(PersonSkillDTO personSkill)
        {
            if (!clsValidation.IsValidPersonSkillDTO(personSkill))
                return false;

            if (!clsValidation.IsValidId(personSkill.ID))
                return false;

            var result = await __PersonSkill.updateById(personSkill);
            return result;
        }

        public async Task<bool> deletePersonSkillById(long ID)
        {
            if (!clsValidation.IsValidId(ID))
                return false;

            var result = await __PersonSkill.deleteById(ID);
            return result;
        }
    }
}
