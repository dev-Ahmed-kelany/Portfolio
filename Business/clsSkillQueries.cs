using Portfolio.DataAccess;

namespace Portfolio.Business
{
    public class clsSkillQueries
    {
        private readonly ISkillQueries __SkillQueries;

        public clsSkillQueries(ISkillQueries SkillQueries)
        {
            __SkillQueries = SkillQueries;
        }

        public async Task<List<SkillQueryResultDTO>> getSkillsByPerson(long PersonID)
        {
            if (!clsValidation.IsValidPersonId(PersonID))
                return new List<SkillQueryResultDTO>();

            var Skills = await __SkillQueries.getSkillsByPerson(PersonID);
            return Skills;
        }

        public async Task<List<SkillQueryResultDTO>> getSkillsByProject(long ProjectID)
        {
            if (!clsValidation.IsValidId(ProjectID))
                return new List<SkillQueryResultDTO>();

            var Skills = await __SkillQueries.getSkillsByProject(ProjectID);
            return Skills;
        }
    }
}
