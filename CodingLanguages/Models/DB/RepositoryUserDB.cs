using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;

namespace CodingLanguages.Models.DB {
    public class RepositoryUserDB : IRepositoryUsers {

        private string _connectionString = "Server=localhost;database=languages;user=root;password=";
        private DbConnection _conn;

        public async Task ConnectAsync() {
            if (this._conn == null) {
                this._conn = new MySqlConnection(this._connectionString);
            }
            if (this._conn.State != ConnectionState.Open) {
                await this._conn.OpenAsync();
            }
        }

        public async Task DisconnectAsync() {
            if(this._conn?.State == ConnectionState.Open) {
                await this._conn.CloseAsync();
            }
        }

        public async Task<bool> InsertAsync(User user) {
            if (this._conn?.State == ConnectionState.Open) {

                DbCommand cmdInsert = this._conn.CreateCommand();

                cmdInsert.CommandText = "insert into user values(null, @username, @firstname, @lastname, sha2(@pwd, 512), @bd, @email, @country, @gender, 0);";

                DbParameter paramUN = cmdInsert.CreateParameter();
                paramUN.ParameterName = "username";
                paramUN.DbType = DbType.String;
                paramUN.Value = user.Username;

                DbParameter paramFN = cmdInsert.CreateParameter();
                paramFN.ParameterName = "firstname";
                paramFN.DbType = DbType.String;
                paramFN.Value = user.Firstname;

                DbParameter paramLN = cmdInsert.CreateParameter();
                paramLN.ParameterName = "lastname";
                paramLN.DbType = DbType.String;
                paramLN.Value = user.Lastname;

                DbParameter paramPwd = cmdInsert.CreateParameter();
                paramPwd.ParameterName = "pwd";
                paramPwd.DbType = DbType.String;
                paramPwd.Value = user.Password;

                DbParameter paramBd = cmdInsert.CreateParameter();
                paramBd.ParameterName = "bd";
                paramBd.DbType = DbType.Date;
                paramBd.Value = user.Birthdate;

                DbParameter paramEm = cmdInsert.CreateParameter();
                paramEm.ParameterName = "email";
                paramEm.DbType = DbType.String;
                paramEm.Value = user.Email;

                DbParameter paramCount = cmdInsert.CreateParameter();
                paramCount.ParameterName = "country";
                paramCount.DbType = DbType.String;
                paramCount.Value = user.Country;

                DbParameter paramGender = cmdInsert.CreateParameter();
                paramGender.ParameterName = "gender";
                paramGender.DbType = DbType.Int32;
                paramGender.Value = user.Gender;


                cmdInsert.Parameters.Add(paramUN);
                cmdInsert.Parameters.Add(paramFN);
                cmdInsert.Parameters.Add(paramLN);
                cmdInsert.Parameters.Add(paramPwd);
                cmdInsert.Parameters.Add(paramBd);
                cmdInsert.Parameters.Add(paramEm);
                cmdInsert.Parameters.Add(paramCount);
                cmdInsert.Parameters.Add(paramGender);

                return await cmdInsert.ExecuteNonQueryAsync() == 1;
            }
            return false;
        }

        public async Task<bool> UpdateAsync(User newUserData) {
            throw new NotImplementedException();
        }

        public async Task<bool> DeleteAsync(int userId) {
            throw new NotImplementedException();
        }

        public async Task<User> GetUserAsync(int userId)
        {
            User user = null;

            if (this._conn?.State == ConnectionState.Open)
            {

                DbCommand cmdOneUser = this._conn.CreateCommand();
                cmdOneUser.CommandText = "select * from user where user_id = @user_id";

                DbParameter paramUserId = cmdOneUser.CreateParameter();
                paramUserId.ParameterName = "user_id";
                paramUserId.DbType = DbType.Int32;
                paramUserId.Value = userId;

                cmdOneUser.Parameters.Add(paramUserId);

                using (DbDataReader reader = await cmdOneUser.ExecuteReaderAsync())
                {

                    if (reader.Read())
                    {

                        user = new User()
                        {
                            UserID = Convert.ToInt32(reader["user_id"]),
                            Username = Convert.ToString(reader["username"]),
                            Firstname = Convert.ToString(reader["firstname"]),
                            Lastname = Convert.ToString(reader["lastname"]),
                            Birthdate = Convert.ToDateTime(reader["birthdate"]),
                            Email = Convert.ToString(reader["email"]),
                            Country = Convert.ToString(reader["country"]),
                            Gender = (Gender)Convert.ToInt32(reader["gender"]),
                            Admin = Convert.ToInt32(reader["admin"])
                        };
                    }
                }
            }

            return user;
        }

        public async Task<User> GetUserAsync(string username)
        {
            User user = null;

            if (this._conn?.State == ConnectionState.Open)
            {

                DbCommand cmdOneUser = this._conn.CreateCommand();
                cmdOneUser.CommandText = "select * from user where username = @username";

                DbParameter paramUserId = cmdOneUser.CreateParameter();
                paramUserId.ParameterName = "username";
                paramUserId.DbType = DbType.String;
                paramUserId.Value = username;

                cmdOneUser.Parameters.Add(paramUserId);

                using (DbDataReader reader = await cmdOneUser.ExecuteReaderAsync())
                {

                    if (reader.Read())
                    {

                        user = new User()
                        {
                            UserID = Convert.ToInt32(reader["user_id"]),
                            Username = Convert.ToString(reader["username"]),
                            Firstname = Convert.ToString(reader["firstname"]),
                            Lastname = Convert.ToString(reader["lastname"]),
                            Birthdate = Convert.ToDateTime(reader["birthdate"]),
                            Email = Convert.ToString(reader["email"]),
                            Country = Convert.ToString(reader["country"]),
                            Gender = (Gender)Convert.ToInt32(reader["gender"]),
                            Admin = Convert.ToInt32(reader["admin"])
                        };
                    }
                }
            }

            return user;
        }

        public async Task<bool> IsUniqueEmailAsync(string email) {
            bool ret = true;

            if (this._conn?.State == ConnectionState.Open) {

                DbCommand cmdOneUser = this._conn.CreateCommand();
                cmdOneUser.CommandText = "select * from user where email = @email";

                DbParameter paramUserId = cmdOneUser.CreateParameter();
                paramUserId.ParameterName = "email";
                paramUserId.DbType = DbType.String;
                paramUserId.Value = email;

                cmdOneUser.Parameters.Add(paramUserId);

                using (DbDataReader reader = await cmdOneUser.ExecuteReaderAsync()) {

                    if (reader.Read()) {

                        ret = false;
                    }
                }
            }

            return ret;
        }

        public async Task<bool> IsUniqueUsernameAsync(string username) { 
            bool ret = true;

            if (this._conn?.State == ConnectionState.Open) {

                DbCommand cmdOneUser = this._conn.CreateCommand();
                cmdOneUser.CommandText = "select * from user where username = @username";

                DbParameter paramUserId = cmdOneUser.CreateParameter();
                paramUserId.ParameterName = "username";
                paramUserId.DbType = DbType.String;
                paramUserId.Value = username;

                cmdOneUser.Parameters.Add(paramUserId);

                using (DbDataReader reader = await cmdOneUser.ExecuteReaderAsync()) {

                    if (reader.Read()) {

                        ret = false;
                    }
                }
            }

            return ret;
        }

        public async Task<List<User>> GetAllUsersAsync() {
            throw new NotImplementedException();
        }

        public async Task<User> LoginAsync(User u) {
            User user = null;

            if (this._conn?.State == ConnectionState.Open)
            {

                DbCommand cmdOneUser = this._conn.CreateCommand();
                if(u.Username.Contains('@'))
                {
                    cmdOneUser.CommandText = "select user_id, username, admin from user where email = @email and password = sha2(@pwd, 512);";

                    DbParameter paramEm = cmdOneUser.CreateParameter();
                    paramEm.ParameterName = "email";
                    paramEm.DbType = DbType.String;
                    paramEm.Value = u.Username;

                    cmdOneUser.Parameters.Add(paramEm);

                } else
                {
                    cmdOneUser.CommandText = "select user_id, username, admin from user where username = @username and password = sha2(@pwd, 512);";

                    DbParameter paramUN = cmdOneUser.CreateParameter();
                    paramUN.ParameterName = "username";
                    paramUN.DbType = DbType.String;
                    paramUN.Value = u.Username;

                    cmdOneUser.Parameters.Add(paramUN);
                }

                DbParameter paramPwd = cmdOneUser.CreateParameter();
                paramPwd.ParameterName = "pwd";
                paramPwd.DbType = DbType.String;
                paramPwd.Value = u.Password;

                cmdOneUser.Parameters.Add(paramPwd);

                using (DbDataReader reader = await cmdOneUser.ExecuteReaderAsync())
                {

                    if (reader.Read())
                    {
                        user = new User()
                        {
                            UserID = Convert.ToInt32(reader["user_id"]),
                            Username = Convert.ToString(reader["username"]),
                            Admin = Convert.ToInt32(reader["admin"])
                        };
                    }
                }
            }

            return user;
        }
    }
}
