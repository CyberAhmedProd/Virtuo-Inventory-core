using VirtuoInventory.Api.Models.Enum;

namespace VirtuoInventory.Api.Models
{
    public class AddUserRequest
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Username { get; set; }
        public string? Password { get; set; }
        public RoleEnum Role { get; set; }
    }
}
