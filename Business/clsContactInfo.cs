using Portfolio.DataAccess;

namespace Portfolio.Business
{
    public class clsContactInfo
    {
        private readonly IContactInfo __ContactInfo;

        public clsContactInfo(IContactInfo ContactInfo)
        {
            __ContactInfo = ContactInfo;
        }

        public async Task<List<ContactInfoDTO>> getAllContactInfo()
        {
            var ContactInfo = await __ContactInfo.getAll();
            return ContactInfo;
        }

        public async Task<ContactInfoDTO> getContactInfoById(long ID)
        {
            var ContactInfo = await __ContactInfo.getById(ID);
            return ContactInfo;
        }

        public async Task<ContactInfoDTO> getContactInfoByPerson(long PersonID)
        {
            var ContactInfo = await __ContactInfo.getByPerson(PersonID);
            return ContactInfo;
        }

        public async Task<long> addNewContactInfo(ContactInfoDTO contactInfo)
        {
            var newId = await __ContactInfo.addNew(contactInfo);
            return newId;
        }

        public async Task<bool> updateContactInfoById(ContactInfoDTO contactInfo)
        {
            var result = await __ContactInfo.updateById(contactInfo);
            return result;
        }

        public async Task<bool> deleteContactInfoById(long ID)
        {
            var result = await __ContactInfo.deleteById(ID);
            return result;
        }
    }
}
