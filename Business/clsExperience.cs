using Portfolio.DataAccess;

namespace Portfolio.Business
{
    public class clsExperience
    {
        private readonly IExperience __Experience;

        public clsExperience(IExperience Experience)
        {
            __Experience = Experience;
        }

        public async Task<List<ExperienceDTO>> getAllExperiences()
        {
            var Experiences = await __Experience.getAll();
            return Experiences;
        }

        public async Task<ExperienceDTO> getExperienceById(long ID)
        {
            var Experience = await __Experience.getById(ID);
            return Experience;
        }

        public async Task<List<ExperienceDTO>> getExperiencesByPerson(long PersonID)
        {
            var Experiences = await __Experience.getByPerson(PersonID);
            return Experiences;
        }

        public async Task<long> addNewExperience(ExperienceDTO experience)
        {
            if (!clsValidation.IsValidExperienceDTO(experience))
                return 0;

            var newId = await __Experience.addNew(experience);
            return newId;
        }

        public async Task<bool> updateExperienceById(ExperienceDTO experience)
        {
            if (!clsValidation.IsValidExperienceDTO(experience))
                return false;

            if (!clsValidation.IsValidId(experience.ID))
                return false;

            var result = await __Experience.updateById(experience);
            return result;
        }

        public async Task<bool> deleteExperienceById(long ID)
        {
            if (!clsValidation.IsValidId(ID))
                return false;

            var result = await __Experience.deleteById(ID);
            return result;
        }
    }
}
