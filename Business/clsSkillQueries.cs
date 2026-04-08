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
            var Skills = await __SkillQueries.getSkillsByPerson(PersonID);
            return Skills;
        }

        public async Task<List<SkillQueryResultDTO>> getSkillsByProject(long ProjectID)
        {
            var Skills = await __SkillQueries.getSkillsByProject(ProjectID);
            return Skills;
        }
    }
}
