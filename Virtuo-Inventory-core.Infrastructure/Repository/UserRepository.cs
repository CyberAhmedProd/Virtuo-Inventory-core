using VirtuoInventory.Application.Interfaces;
using VirtuoInventory.Core.Entities;
using VirtuoInventory.Infrastructure.Sql;
using Dapper;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace VirtuoInventory.Infrastructure.Repository
{
    public class UserRepository : IUserRepository
    {
        #region ===[ Private Members ]=============================================================

        private readonly IConfiguration configuration;

        #endregion

        #region ===[ Constructor ]=================================================================

        public UserRepository(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public async Task<List<User>> GetAll()
        {
            try
            {
                using (IDbConnection connection = new SqlConnection(configuration.GetConnectionString("DBConnection")))
                {
                    var users = await connection.QueryAsync<User>(UserQuery.AllUsers);

                    return users.ToList();
                }
            }
            catch (Exception ex)
            {
                // Log the exception or handle it as needed
                throw new Exception(ex.Message);
            }
        }

        public async Task<User?> GetById(int id) // Updated return type to User? to handle nullability
        {
            try
            {
                using (IDbConnection connection = new SqlConnection(configuration.GetConnectionString("DBConnection")))
                {
                    var user = (await connection.QueryAsync<User>(UserQuery.UserById, new { id = id })).SingleOrDefault();

                    return user;
                }
            }
            catch (Exception ex)
            {
                // Log the exception or handle it as needed
                throw new Exception(ex.Message);
            }
        }

        public async Task<User?> AuthenticateUser(string username) // Updated return type to User? to handle nullability
        {
            try
            {
                using (IDbConnection connection = new SqlConnection(configuration.GetConnectionString("DBConnection")))
                {
                    var user = (await connection.QueryAsync<User>(UserQuery.AuthenticateUser
                        , new { UserName = username })).FirstOrDefault();

                    return user;
                }
            }
            catch (Exception ex)
            {
                // Log the exception or handle it as needed
                throw new Exception(ex.Message);
            }
        }

        public async Task<int> InsertUser(User user)
        {
            try
            {
                using (IDbConnection connection = new SqlConnection(configuration.GetConnectionString("DBConnection")))
                {
                    // Check if the username is unique
                    bool isUnique = await IsUniqueUserName(user.Username);
                    if (!isUnique)
                    {
                        throw new Exception("The username is already taken. Please choose a different username.");
                    }

                    var result = await connection.ExecuteScalarAsync<int>(UserQuery.InsertUser, user);

                    return result; // Return the inserted user's ID as an integer
                }
            }
            catch (Exception ex)
            {
                // Log the exception or handle it as needed
                throw new Exception(ex.Message);
            }
        }
        public async Task<bool> UpdateUser(User user)
        {
            try
            {
                using (IDbConnection connection = new SqlConnection(configuration.GetConnectionString("DBConnection")))
                {
                    var rowsAffected = await connection.ExecuteAsync(UserQuery.UpdateUser, user);
                    return rowsAffected > 0;
                }
            }
            catch (Exception ex)
            {
                // Log the exception or handle it as needed
                throw new Exception(ex.Message);
            }
        }

        public async Task<bool> UpdateUserPassword(int id, string newPassword)
        {
            try
            {
                using (IDbConnection connection = new SqlConnection(configuration.GetConnectionString("DBConnection")))
                {
                    var rowsAffected = await connection.ExecuteAsync(UserQuery.UpdateUserPassword, new { Id = id, Password = newPassword });
                    return rowsAffected > 0;
                }
            }
            catch (Exception ex)
            {
                // Log the exception or handle it as needed
                throw new Exception(ex.Message);
            }
        }

        public async Task<bool> DeleteUser(int id)
        {
            try
            {
                using (IDbConnection connection = new SqlConnection(configuration.GetConnectionString("DBConnection")))
                {
                    var rowsAffected = await connection.ExecuteAsync(UserQuery.DeleteUser, new { Id = id });
                    return rowsAffected > 0;
                }
            }
            catch (Exception ex)
            {
                // Log the exception or handle it as needed
                throw new Exception(ex.Message);
            }
        }
        private async Task<bool> IsUniqueUserName(string username)
        {
            try
            {
                using (IDbConnection connection = new SqlConnection(configuration.GetConnectionString("DBConnection")))
                {
                    var count = await connection.ExecuteScalarAsync<int>(UserQuery.CheckUserNameUnicity, new { UserName = username });
                    return count == 0; // Returns true if the username is unique
                }
            }
            catch (Exception ex)
            {
                // Log the exception or handle it as needed
                throw new Exception(ex.Message);
            }
        }
        #endregion
    }
}
