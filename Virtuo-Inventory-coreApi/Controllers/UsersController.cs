using VirtuoInventory.Application.Interfaces;
using VirtuoInventory.Core.Entities;
using VirtuoInventory.Api.Helper;
using VirtuoInventory.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace VirtuoInventory.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UsersController : ControllerBase
    {
        #region ===[ Private Members ]=============================================================

        private readonly IUnitOfWork _unitOfWork;
        private readonly AppSettings _appSettings;

        #endregion

        #region ===[ Constructor ]=================================================================

        /// <summary>
        /// Initialize UsersController by injecting an object type of IUnitOfWork
        /// </summary>
        public UsersController(IUnitOfWork unitOfWork, IOptions<AppSettings> appSettings)
        {
            this._unitOfWork = unitOfWork;
            this._appSettings = appSettings.Value;
        }

        #endregion

        #region ===[ Public Methods ]==============================================================

        [HttpPost("Authenticate")]
        public async Task<IActionResult> Authenticate(AuthenticateRequest model)
        {
            try
            {
                var user = await _unitOfWork.Users.AuthenticateUser(model.Username);
                if (user == null)
                    return BadRequest(new { message = "Username or password is incorrect" });
                if (!Common.VerifyHashedPassword(model.Password, user.Password))
                    return BadRequest(new { message = "password is incorrect" });


                var token = Common.GenerateJwtToken(user, _appSettings);

                var response = new AuthenticateResponse
                {
                    Id = user.Id,
                    Username = user.Username,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Token = token,
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while authenticating the user.", error = ex.Message });
            }
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                if (!HttpContext.Items.TryGetValue("User", out var userObj) || userObj is not User user)
                {
                    return Unauthorized(new { message = "You are not authorized to access this resource." });
                }

                if (user.Role != "Admin")
                {
                    return Unauthorized(new { message = "You are not authorized to access this resource." });
                }

                var users = await _unitOfWork.Users.GetAll();
                return Ok(users);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving users.", error = ex.Message });
            }
        }
        [HttpPost("InsertUser")]
        public async Task<IActionResult> InsertUser(AddUserRequest model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var newUser = new User
                {
                    Username = model.Username,
                    Password = Common.HashPassword(model.Password),
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Role = model.Role.ToString()
                };

                var IdInserted = await _unitOfWork.Users.InsertUser(newUser);
                var user = await _unitOfWork.Users.GetById(IdInserted);

                if (user == null)
                    return StatusCode(500, new { message = "An error occurred while adding the user." });

                return Ok(new { message = "User added successfully.", UserId = user.Id });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while inserting the user.", error = ex.Message });
            }
        }
        [HttpPut("UpdateUser")]
        public async Task<IActionResult> UpdateUser(UpdateUserRequest model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var user = await _unitOfWork.Users.GetById(model.Id);
                if (user == null)
                    return NotFound(new { message = "User not found." });

                user.FirstName = model.FirstName;
                user.LastName = model.LastName;
                user.Username = model.Username;
                user.Role = model.Role;

                var isUpdated = await _unitOfWork.Users.UpdateUser(user);
                if (!isUpdated)
                    return StatusCode(500, new { message = "An error occurred while updating the user." });

                return Ok(new { message = "User updated successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while updating the user.", error = ex.Message });
            }
        }

        [HttpPut("UpdateUserPassword/{id}")]
        public async Task<IActionResult> UpdateUserPassword(int id, UpdatePasswordRequest model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var hashedPassword = Common.HashPassword(model.NewPassword);

                var isUpdated = await _unitOfWork.Users.UpdateUserPassword(id, hashedPassword);
                if (!isUpdated)
                    return StatusCode(500, new { message = "An error occurred while updating the password." });

                return Ok(new { message = "Password updated successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while updating the password.", error = ex.Message });
            }
        }

        [HttpDelete("DeleteUser/{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            try
            {
                var isDeleted = await _unitOfWork.Users.DeleteUser(id);
                if (!isDeleted)
                    return StatusCode(500, new { message = "An error occurred while deleting the user." });

                return Ok(new { message = "User deleted successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while deleting the user.", error = ex.Message });
            }
        }
        #endregion
    }
}