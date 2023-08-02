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


        //        [HttpPost]
        //        [Route("register-admin")]
        //        public async Task<IActionResult> RegisterAdmin([FromBody] RegisterModel model)
        //        {
        //            var userExists = await _userManager.FindByNameAsync(model.Username);
        //            if (userExists != null)
        //                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseDTO { Status = "Error", Message = "User already exists!" });

        //            ApplicationUser user = new()
        //            {
        //                Email = model.Email,
        //                SecurityStamp = Guid.NewGuid().ToString(),
        //                UserName = model.Username
        //            };
        //            var result = await _userManager.CreateAsync(user, model.Password);
        //            if (!result.Succeeded)
        //                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseDTO { Status = "Error", Message = "User creation failed! Please check user details and try again." });

        //            if (!await _roleManager.RoleExistsAsync(UserRoles.admin))
        //                await _roleManager.CreateAsync(new IdentityRole(UserRoles.admin));
        //            if (!await _roleManager.RoleExistsAsync(UserRoles.user))
        //                await _roleManager.CreateAsync(new IdentityRole(UserRoles.user));

        //            if (await _roleManager.RoleExistsAsync(UserRoles.admin))
        //            {
        //                await _userManager.AddToRoleAsync(user, UserRoles.admin);
        //            }
        //            if (await _roleManager.RoleExistsAsync(UserRoles.admin))
        //            {
        //                await _userManager.AddToRoleAsync(user, UserRoles.user);
        //            }
        //            return Ok(new ResponseDTO { Status = "Success", Message = "User created successfully!" });
        //        }

        //        [HttpPost]
        //        [Route("refresh-token")]
        //        public async Task<IActionResult> RefreshToken(TokenModel tokenModel)
        //        {
        //            if (tokenModel is null)
        //            {
        //                return BadRequest("Invalid client request");
        //            }

        //            string? accessToken = tokenModel.AccessToken;
        //            string? refreshToken = tokenModel.RefreshToken;

        //            var principal = GetPrincipalFromExpiredToken(accessToken);
        //            if (principal == null)
        //            {
        //                return BadRequest("Invalid access token or refresh token");
        //            }

        //#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
        //#pragma warning disable CS8602 // Dereference of a possibly null reference.
        //            string username = principal.Identity.Name;
        //#pragma warning restore CS8602 // Dereference of a possibly null reference.
        //#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.

        //            var user = await _userManager.FindByNameAsync(username);

        //            if (user == null || user.RefreshToken != refreshToken || user.RefreshTokenExpiryTime <= DateTime.Now)
        //            {
        //                return BadRequest("Invalid access token or refresh token");
        //            }

        //            var newAccessToken = CreateToken(principal.Claims.ToList());
        //            var newRefreshToken = GenerateRefreshToken();

        //            user.RefreshToken = newRefreshToken;
        //            await _userManager.UpdateAsync(user);

        //            return new ObjectResult(new
        //            {
        //                accessToken = new JwtSecurityTokenHandler().WriteToken(newAccessToken),
        //                refreshToken = newRefreshToken
        //            });
        //        }

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

        //        [Authorize]
        //        [HttpPost]
        //        [Route("revoke-all")]
        //        public async Task<IActionResult> RevokeAll()
        //        {
        //            var users = _userManager.Users.ToList();
        //            foreach (var user in users)
        //            {
        //                user.RefreshToken = null;
        //                await _userManager.UpdateAsync(user);
        //            }

        //            return NoContent();
        //        }

    }
}
