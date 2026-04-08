using Portfolio.DataAccess;

namespace Portfolio.Business
{
    public class clsAbout
    {
        private readonly IAbout __About;

        public clsAbout(IAbout About)
        {
            __About = About;
        }

        public async Task<List<AboutDTO>> getAllAbout()
        {
            var About = await __About.getAll();
            return About;
        }

        public async Task<AboutDTO> getAboutById(long ID)
        {
            var About = await __About.getById(ID);
            return About;
        }

        public async Task<AboutDTO> getAboutByPerson(long PersonID)
        {
            var About = await __About.getByPerson(PersonID);
            return About;
        }

        public async Task<long> addNewAbout(AboutDTO about)
        {
            var newId = await __About.addNew(about);
            return newId;
        }

        public async Task<bool> updateAboutById(AboutDTO about)
        {
            var result = await __About.updateById(about);
            return result;
        }

        public async Task<bool> deleteAboutById(long ID)
        {
            var result = await __About.deleteById(ID);
            return result;
        }
    }
}
