using Azure;
using ChatHub.BLL.DTOs;
using ChatHub.BLL.Services.Implementation;
using ChatHub.BLL.Services.Interfaces;
using ChatHub.DAL.Datas;
using ChatHub.DAL.Entities;
using ChatHub.DAL.Repository.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace ChatHub.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticateController : ControllerBase
    {
        private readonly IAuthService _authService;
        public AuthenticateController(IAuthService authService)
        {
            _authService = authService;
        }
        [AllowAnonymous]
        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel loginModel)
        {
            var result = await _authService.Login(loginModel);
            if (!result.Success)
            {
                return StatusCode(StatusCodes.Status401Unauthorized, result.Errors);
            }
            return StatusCode(StatusCodes.Status200OK, result.Data);
        }
        [HttpPost]
        [Route("create-user")]
        public async Task<IActionResult> CreateUser([FromBody] RegisterModel registerModel)
        {
            var result = await _authService.CreateUser(registerModel);
            if (!result.Success)
            {
                return StatusCode(StatusCodes.Status401Unauthorized, result.Errors);
            }
            return StatusCode(StatusCodes.Status200OK, "User created successfully.");
        }
        [HttpGet]
        [Route("get-all-users/{param}")]
        public async Task<IActionResult> GetAllUsers(string param)
        {
            var result = await _authService.GetAllUsers(param);
            if (!result.Success)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, result.Errors);
            }
            return StatusCode(StatusCodes.Status200OK, result.Data);
        }
        [HttpGet]
        [Route("get-active-users")]
        public async Task<IActionResult> GetActiveUsers()
        {
            var result = await _authService.GetActiveUsers();
            if (!result.Success)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, result.Errors);
            }
            return StatusCode(StatusCodes.Status200OK, result.Data);
        }
        [HttpGet]
        [Route("check-username/{username}")]
        public async Task<IActionResult> CheckUsername(string username)
        {
            var userExists = await _authService.CheckUsername(username);
            var exists = userExists.Data != null;
            return Ok(new { exists });
        }

        [HttpGet]
        [Route("check-email/{email}")]
        public async Task<IActionResult> CheckEmail(string email)
        {
            var userEmailExists = await _authService.CheckEmail(email);
            var exists = userEmailExists.Data != null;
            return Ok(new { exists });
        }

        [HttpGet]
        [Route("getRoles")]
        public async Task<IActionResult> GetRoles()
        {
            var result = await _authService.GetRoles();
            if (!result.Success)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
            return StatusCode(StatusCodes.Status200OK, result.Data);
        }
        [HttpGet]
        [Route("GetUserById/{UserId}")]
        public async Task<IActionResult> GetUserById(string UserId)
        {
            var result = await _authService.GetUserById(UserId);
            if (!result.Success)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
            return StatusCode(StatusCodes.Status200OK, result.Data);
        }
        [HttpGet]
        [Route("change-user-status/{UserId}")]
        public async Task<IActionResult> ChangeUserStatus(string UserId)
        {
            var result = await _authService.ChangeUserStatus(UserId);
            if (!result.Success)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
            return StatusCode(StatusCodes.Status200OK, result.Data);
        }
        [HttpGet]
        [Route("get-users-by-departmentId/{DepartmentId}")]
        public async Task<IActionResult> GetUsersByDepartmentId(string DepartmentId)
        {
            var result = await _authService.GetUsersByDeparmentId(DepartmentId);
            if (!result.Success)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
            return StatusCode(StatusCodes.Status200OK, result.Data);
        }
        [HttpGet]
        [Route("GetUsersByChatTimestamp/{DepartmentId}")]
        public async Task<IActionResult> GetUsersByChatTimestamp(string DepartmentId)
        {
            var result = await _authService.GetUsersByChatTimestamp(DepartmentId);
            if (!result.Success)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
            return StatusCode(StatusCodes.Status200OK, result.Data);
        }
        [HttpPost]
        [Route("ChangePassword")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDTO changePasswordDTO)
        {
            var result = await _authService.ChangePassword(changePasswordDTO);
            if (!result.Success)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, result.Errors);
            }
            return StatusCode(StatusCodes.Status200OK, result.Data);
        }

        [HttpGet]
        [Route("logout")]
        public async Task<IActionResult> Logout()
        {
            var result = await _authService.Logout();
            if (!result.Success)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
            return StatusCode(StatusCodes.Status200OK, "success");
        }
        [HttpPatch]
        [Route("UpdateUser")]
        public async Task<IActionResult> UpdateUser([FromForm] UserDTO userDTO)
        {
            var result = await _authService.UpdateUser(userDTO);
            if (!result.Success)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, result.Errors);
            }
            return StatusCode(StatusCodes.Status200OK);
        }
    }
}
