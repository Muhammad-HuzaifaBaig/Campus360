namespace Campus360.Services{
    public interface IAccountRepository{
        Task<User> GetUserById(string id);
        Task<User> GetUserByName(string name);
        Task<bool> CreateUser(User user);
        Task<bool> UpdateUser(User user);
        Task<bool> DeleteUser(string id);
        Task<bool> Save();
    }
}