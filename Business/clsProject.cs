using Portfolio.DataAccess;

namespace Portfolio.Business
{
    public class clsProject
    {
        private readonly IProject __Project;

        public clsProject(IProject Project)
        {
            __Project = Project;
        }

        public async Task<List<ProjectDTO>> getAllProjects()
        {
            var Projects = await __Project.getAll();
            return Projects;
        }

        public async Task<ProjectDTO> getProjectById(long ID)
        {
            var Project = await __Project.getById(ID);
            return Project;
        }

        public async Task<List<ProjectDTO>> getProjectsByPerson(long PersonID)
        {
            var Projects = await __Project.getByPerson(PersonID);
            return Projects;
        }

        public async Task<long> addNewProject(ProjectDTO project)
        {
            var newId = await __Project.addNew(project);
            return newId;
        }

        public async Task<bool> updateProjectById(ProjectDTO project)
        {
            var result = await __Project.updateById(project);
            return result;
        }

        public async Task<bool> deleteProjectById(long ID)
        {
            var result = await __Project.deleteById(ID);
            return result;
        }
    }
}
