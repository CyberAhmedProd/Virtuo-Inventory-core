
namespace VirtuoInventory.Infrastructure.Sql
{
    class UserQuery
    {
        //Get All Users
        public static string AllUsers => "SELECT [Id], [FirstName], [LastName], [UserName],[Role] FROM [dbo].[User]";

        //Get User By Id
        public static string UserById => "SELECT [Id], [FirstName], [LastName], [UserName],[Role] FROM [dbo].[User] WHERE [Id] = @Id";

        //Authenticate User
        public static string AuthenticateUser =>
            @"SELECT [Id],[Password], [FirstName], [LastName], [UserName],[Role]
			FROM [dbo].[User]
			WHERE [UserName] = @UserName";

        //Add User
        public static string InsertUser => 
            @"INSERT INTO [dbo].[User] ([FirstName], [LastName], [UserName],[Password],[Role]) 
            OUTPUT INSERTED.[Id]
            VALUES (@FirstName, @LastName, @UserName, @Password, @Role)";
        //Update User
        public static string UpdateUser =>
            @"UPDATE [dbo].[User]
            SET [FirstName] = @FirstName, 
                [LastName] = @LastName, 
                [UserName] = @UserName, 
                [Role] = @Role
            WHERE [Id] = @Id";
        //Update User Password
        public static string UpdateUserPassword =>
            @"UPDATE [dbo].[User]
            SET [Password] = @Password
            WHERE [Id] = @Id";
        //Delete User
        public static string DeleteUser =>
            @"DELETE FROM [dbo].[User]
            WHERE [Id] = @Id";

        // Check Unicity
        // Check Unicity of UserName
        public static string CheckUserNameUnicity =>
            @"SELECT COUNT(1)
            FROM [dbo].[User]
            WHERE [UserName] = @UserName";

    }
}

