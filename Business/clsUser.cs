using Portfolio.DataAccess;

namespace Portfolio.Business
{
    public class clsUser
    {
        private readonly IUser __User;

        public clsUser(IUser User)
        {
            __User = User;
        }

        public async Task<List<UserWithoutPasswordDTO>> getAllUsers()
        {
            var Users = await __User.getAll();
            return Users;
        }

        public async Task<UserWithoutPasswordDTO> getUserById(long ID)
        {
            var User = await __User.getById(ID);
            return User;
        }

        public async Task<UserWithoutPasswordDTO> getUserByCredentials(string Username, string Password)
        {
            var User = await __User.getUserByCredentials(Username, Password);
            return User;
        }

        public async Task<long> addNewUser(UserDTO user)
        {
            if (!clsValidation.IsValidUserDTO(user))
                return 0;

            var newId = await __User.addNew(user);
            return newId;
        }

        public async Task<bool> updateUserById(UserDTO user)
        {
            if (!clsValidation.IsValidUserDTO(user))
                return false;

            if (!clsValidation.IsValidId(user.ID))
                return false;

            var result = await __User.updateById(user);
            return result;
        }

        public async Task<bool> toogleActive(long ID, bool Active)
        {
            if (!clsValidation.IsValidId(ID))
                return false;

            var result = await __User.toogleActive(ID, Active);
            return result;
        }

        public async Task<bool> changePassword(long ID, string currentPassword, string newPassword)
        {
            if (!clsValidation.IsValidId(ID))
                return false;

            if (clsValidation.IsNullOrEmpty(currentPassword) || !clsValidation.IsWithinLength(currentPassword, 5, 100))
                return false;

            if (clsValidation.IsNullOrEmpty(newPassword) || !clsValidation.IsWithinLength(newPassword, 5, 100))
                return false;

            var result = await __User.changePassword(ID, currentPassword, newPassword);
            return result;
        }

        public async Task<bool> deleteUserById(long ID)
        {
            if (!clsValidation.IsValidId(ID))
                return false;

            var result = await __User.deleteById(ID);
            return result;
        }
    }
}
