using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthDemo.Infrastructure.Sql
{
    class UserQuery
    {
        //Get All Users
        public static string AllUsers => "SELECT [Id], [FirstName], [LastName], [UserName],[Role] FROM [dbo].[User]";

        //Get User By Id
        public static string UserById => "SELECT [Id], [FirstName], [LastName], [UserName],[Role] FROM [dbo].[User] WHERE [Id] = @Id";

        //Authenticate User
        public static string AuthenticateUser =>
            @"SELECT [Id], [FirstName], [LastName], [UserName],[Role]
			FROM [dbo].[User]
			WHERE [UserName] = @UserName
			AND [Password] = @Password";

        //Add User
        public static string InsertUser => 
            @"INSERT INTO [dbo].[User] ([FirstName], [LastName], [UserName],[Password],[Role]) 
            OUTPUT INSERTED.[Id]
            VALUES (@FirstName, @LastName, @UserName, @Password, @Role)";
    }
}

