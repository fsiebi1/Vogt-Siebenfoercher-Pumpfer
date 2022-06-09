using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CodingLanguages.Models.DB {
    interface IRepositoryUsers {

        public Task ConnectAsync();
        public Task DisconnectAsync();

        public Task<bool> InsertAsync(User user);
        public Task<bool> UpdateAsync(User user);
        public Task<bool> DeleteAsync(string username);
        public Task<User> GetUserAsync(int userId);
        public Task<User> GetUserAsync(string username);
        public Task<bool> IsUniqueEmailAsync(string email);
        public Task<bool> IsUniqueUsernameAsync(string username);
        public Task<List<User>> GetAllUsersAsync();

        public Task<User> LoginAsync(string username, string password);
        public Task<bool> SetPasswordAsync(string username, string password);

    }
}
