using AuthDemo.Application.Interfaces;
using AuthDemo.Core.Entities;
using AuthDemo.Infrastructure.Sql;
using Dapper;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace AuthDemo.Infrastructure.Repository
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
                throw new Exception("An error occurred while retrieving all users.", ex);
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
                throw new Exception($"An error occurred while retrieving the user with ID {id}.", ex);
            }
        }

        public async Task<User?> AuthenticateUser(string username, string password) // Updated return type to User? to handle nullability
        {
            try
            {
                using (IDbConnection connection = new SqlConnection(configuration.GetConnectionString("DBConnection")))
                {
                    var user = (await connection.QueryAsync<User>(UserQuery.AuthenticateUser
                        , new { UserName = username, Password = password })).FirstOrDefault();

                    return user;
                }
            }
            catch (Exception ex)
            {
                // Log the exception or handle it as needed
                throw new Exception("An error occurred while authenticating the user.", ex);
            }
        }

        public async Task<int> InsertUser(User user)
        {
            try
            {
                using (IDbConnection connection = new SqlConnection(configuration.GetConnectionString("DBConnection")))
                {
                    var result = await connection.ExecuteScalarAsync<int>(UserQuery.InsertUser, user);

                    return result; // Return the inserted user's ID as an integer
                }
            }
            catch (Exception ex)
            {
                // Log the exception or handle it as needed
                throw new Exception("An error occurred while inserting the user.", ex);
            }
        }

        #endregion
    }
}
