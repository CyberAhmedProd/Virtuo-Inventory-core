using AuthDemo.Application.Interfaces;
using AuthDemo.Core.Entities;
using AuthDemoApi.Helper;
using AuthDemoApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Data;

namespace AuthDemoApi.Controllers
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
            var user = await _unitOfWork.Users.AuthenticateUser(model.Username, model.Password);

            if (user == null)
                return BadRequest(new { message = "Username or password is incorrect" });

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

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            // Check if the "User" object exists in HttpContext.Items and is not null
            if (!HttpContext.Items.TryGetValue("User", out var userObj) || userObj is not User user)
            {
                return Unauthorized(new { message = "You are not authorized to access this resource." });
            }

            // Ensure the "User" object has the required Role property
            if (user.Role != "Admin")
            {
                return Unauthorized(new { message = "You are not authorized to access this resource." });
            }

            var users = await _unitOfWork.Users.GetAll();
            return Ok(users);
        }
        [HttpPost("InsertUser")]
        public async Task<IActionResult> InsertUser(AddUserRequest model)
        {
            // Validate the request model
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Create a new User entity
            var newUser = new User
            {
                Username = model.Username,
                Password = model.Password, // Ensure password is hashed in a real-world scenario
                FirstName = model.FirstName,
                LastName = model.LastName,
                Role = model.Role
            };

            // Add the user to the repository
            var IdInserted = await _unitOfWork.Users.InsertUser(newUser);
            var user = await _unitOfWork.Users.GetById(IdInserted);

            if (user == null)
                return StatusCode(500, new { message = "An error occurred while adding the user." });

            return Ok(new { message = "User added successfully.", UserId = user.Id });
        }
        #endregion
    }
}