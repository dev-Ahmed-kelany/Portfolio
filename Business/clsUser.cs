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
            var Users = await __User.getAllUsersWithoutPassword();
            return Users;
        }

        public async Task<UserWithoutPasswordDTO> getUserById(long ID)
        {
            var User = await __User.getUserByIdWithoutPassword(ID);
            return User;
        }

        public async Task<UserDTO> getUserByCredentials(string Username, string Password)
        {
            var User = await __User.getUserByCredentials(Username, Password);
            return User;
        }

        public async Task<long> addNewUser(UserDTO user)
        {
            var newId = await __User.addNew(user);
            return newId;
        }

        public async Task<bool> updateUserById(UserDTO user)
        {
            var result = await __User.updateById(user);
            return result;
        }

        public async Task<bool> deleteUserById(long ID)
        {
            var result = await __User.deleteById(ID);
            return result;
        }
    }
}
