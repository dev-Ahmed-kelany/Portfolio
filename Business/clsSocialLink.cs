using Portfolio.DataAccess;

namespace Portfolio.Business
{
    public class clsSocialLink
    {
        private readonly ISocialLink __SocialLink;

        public clsSocialLink(ISocialLink SocialLink)
        {
            __SocialLink = SocialLink;
        }

        public async Task<List<SocialLinkDTO>> getAllSocialLinks()
        {
            var SocialLinks = await __SocialLink.getAll();
            return SocialLinks;
        }

        public async Task<SocialLinkDTO> getSocialLinkById(long ID)
        {
            var SocialLink = await __SocialLink.getById(ID);
            return SocialLink;
        }

        public async Task<List<SocialLinkDTO>> getSocialLinksByPerson(long PersonID)
        {
            var SocialLinks = await __SocialLink.getByPerson(PersonID);
            return SocialLinks;
        }

        public async Task<long> addNewSocialLink(SocialLinkDTO socialLink)
        {
            var newId = await __SocialLink.addNew(socialLink);
            return newId;
        }

        public async Task<bool> updateSocialLinkById(SocialLinkDTO socialLink)
        {
            var result = await __SocialLink.updateById(socialLink);
            return result;
        }

        public async Task<bool> deleteSocialLinkById(long ID)
        {
            var result = await __SocialLink.deleteById(ID);
            return result;
        }
    }
}
