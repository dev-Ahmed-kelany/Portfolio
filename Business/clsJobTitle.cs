using Portfolio.DataAccess;

namespace Portfolio.Business
{
    public class clsJobTitle
    {
        private readonly IJobTitle __JobTitle;

        public clsJobTitle(IJobTitle JobTitle)
        {
            __JobTitle = JobTitle;
        }

        public async Task<List<JobTitleDTO>> getAllJobTitles()
        {
            var JobTitles = await __JobTitle.getAll();
            return JobTitles;
        }

        public async Task<JobTitleDTO> getJobTitleById(long ID)
        {
            var JobTitle = await __JobTitle.getById(ID);
            return JobTitle;
        }

        public async Task<List<JobTitleDTO>> getJobTitlesByPerson(long PersonID)
        {
            var JobTitles = await __JobTitle.getByPerson(PersonID);
            return JobTitles;
        }

        public async Task<long> addNewJobTitle(JobTitleDTO jobTitle)
        {
            if (!clsValidation.IsValidJobTitleDTO(jobTitle))
                return 0;

            var newId = await __JobTitle.addNew(jobTitle);
            return newId;
        }

        public async Task<bool> updateJobTitleById(JobTitleDTO jobTitle)
        {
            if (!clsValidation.IsValidJobTitleDTO(jobTitle))
                return false;

            if (!clsValidation.IsValidId(jobTitle.ID))
                return false;

            var result = await __JobTitle.updateById(jobTitle);
            return result;
        }

        public async Task<bool> deleteJobTitleById(long ID)
        {
            if (!clsValidation.IsValidId(ID))
                return false;

            var result = await __JobTitle.deleteById(ID);
            return result;
        }
    }
}
