using ChatHub.BLL.DTOs;
using ChatHub.BLL.Services.Interfaces;
using ChatHub.DAL.Datas;
using ChatHub.DAL.Entities;
using ChatHub.DAL.Repository.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ChatHub.BLL.Services.Implementation
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IRepository<Department> _departmentRepository;
        private readonly IUploadImages _uploadImages;
        private readonly IEncryptionService _encryptionService;


        private readonly ChatHubDbContext _dbContext;
        private readonly IConfiguration _configuration;
        private readonly ITokenService _tokenService;

        public AuthService(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IConfiguration configuration,
            ITokenService tokenService,
            ChatHubDbContext dbContext,
            IRepository<Department> departmentRepository,
            IUploadImages uploadImages,
            IEncryptionService encryptionService
            )
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
            _tokenService = tokenService;
            _dbContext = dbContext;
            _departmentRepository = departmentRepository;
            _uploadImages = uploadImages;
            _encryptionService = encryptionService;
        }


        public async Task<ServiceResult<object>> Login(LoginModel loginModel)
        {
            var user = await _userManager.FindByNameAsync(loginModel.Username);
            if (user != null && await _userManager.CheckPasswordAsync(user, loginModel.Password) && user.IsActive)
            {
                var userRoles = await _userManager.GetRolesAsync(user);

                var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim("uid",user.Id),
                    new Claim("depid",user.DepartmentId),
                };

                foreach (var userRole in userRoles)
                {
                    authClaims.Add(new Claim(ClaimTypes.Role, userRole));
                }

                var token = CreateToken(authClaims);
                var refreshToken = GenerateRefreshToken();

                _ = int.TryParse(_configuration["JWT:RefreshTokenValidityInDays"], out int refreshTokenValidityInDays);

                user.RefreshToken = refreshToken;
                user.RefreshTokenExpiryTime = DateTime.Now.AddDays(refreshTokenValidityInDays);

                await _userManager.UpdateAsync(user);

                return new ServiceResult<object>(true,
                    new
                    {
                        Token = new JwtSecurityTokenHandler().WriteToken(token),
                        RefreshToken = refreshToken,
                        Expiration = token.ValidTo
                    });
            }
            return new ServiceResult<object>(false, errors: new[] { "Invalid username or password." });
        }
        public async Task<ServiceResult<object>> CreateUser(RegisterModel registerModel)
        {
            using (var dbContext = await _dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    var userExists = await _userManager.FindByNameAsync(registerModel.Username);
                    var userEmailExists = await _userManager.FindByEmailAsync(registerModel.Email);
                    if (userExists != null)
                    {
                        return new ServiceResult<object>(false, errors: new[] { "User already exists." });
                    }

                    if (userEmailExists != null)
                    {
                        return new ServiceResult<object>(false, errors: new[] { "Email already exists." });
                    }

                    ApplicationUser user = new()
                    {
                        Email = registerModel.Email,
                        SecurityStamp = Guid.NewGuid().ToString(),
                        UserName = registerModel.Username,
                        DepartmentId = registerModel.DepartmentId,
                        IsActive = true,
                    };
                    var result = await _userManager.CreateAsync(user, registerModel.Password);
                    if (!result.Succeeded)
                    {
                        return new ServiceResult<object>(false, errors: new[] { "User creation failed. Please try again." });
                    }
                    var role = await _roleManager.FindByIdAsync(registerModel.RoleId);
                    if (role == null)
                    {
                        return new ServiceResult<object>(false, errors: new[] { "Role was not found." });
                    }

                    await _userManager.AddToRoleAsync(user, role.Name);

                    await dbContext.CommitAsync();
                    await dbContext.DisposeAsync();
                    return new ServiceResult<object>(true);
                }
                catch (Exception ex)
                {
                    await dbContext.RollbackAsync();
                    await dbContext.DisposeAsync();
                    return new ServiceResult<object>(false, errors: new[] { ex.Message });
                }
            }

        }

        public async Task<ServiceResult<List<RoleDTO>>> GetRoles()
        {
            List<RoleDTO> roleDTOs = new List<RoleDTO>();
            var roles = await _roleManager.Roles.Where(x => x.Name != "superadmin").ToListAsync();
            if (roles.Count > 0)
            {
                foreach (var role in roles)
                {
                    roleDTOs.Add(new RoleDTO()
                    {
                        RoleId = role.Id,
                        RoleName = role.Name
                    });
                }
            }
            return new ServiceResult<List<RoleDTO>>(true, roleDTOs);
        }

        public async Task<ServiceResult<object>> logout(string username)
        {
            throw new NotImplementedException();
        }

        public async Task<ServiceResult<object>> RefreshToken(TokenModel tokenModel)
        {
            throw new NotImplementedException();
        }
        private JwtSecurityToken CreateToken(List<Claim> authClaims)
        {
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));
            _ = int.TryParse(_configuration["JWT:TokenValidityInMinutes"], out int tokenValidityInMinutes);

            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                expires: DateTime.Now.AddMinutes(tokenValidityInMinutes),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                );

            return token;
        }

        private static string GenerateRefreshToken()
        {
            var randomNumber = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        private ClaimsPrincipal? GetPrincipalFromExpiredToken(string? token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"])),
                ValidateLifetime = false
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);
            if (securityToken is not JwtSecurityToken jwtSecurityToken || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                throw new SecurityTokenException("Invalid token");

            return principal;

        }

        public async Task<ServiceResult<object>> CheckEmail(string email)
        {
            var userEmailExists = await _userManager.FindByEmailAsync(email);
            return new ServiceResult<object>(true, userEmailExists);
        }

        public async Task<ServiceResult<object>> CheckUsername(string username)
        {
            var userExists = await _userManager.FindByNameAsync(username);
            return new ServiceResult<object>(true, userExists);
        }

        public async Task<ServiceResult<List<RegisterModel>>> GetAllUsers(string param)
        {
            List<RegisterModel> registerModels = new List<RegisterModel>();
            List<ApplicationUser> users = new List<ApplicationUser>();
            if(!string.IsNullOrEmpty(param))
            {
                if(param == "all")
                {
                   
                    if(_tokenService.GetRole() == "superadmin")
                    {
                        users = await _userManager.Users.ToListAsync();
                        users = users.Where(u => !_userManager.IsInRoleAsync(u, "superadmin").Result).ToList();
                    }
                    else
                    {
                        users = await _userManager.Users.Where(x=>x.DepartmentId == _tokenService.GetDepartmentId()).ToListAsync();
                        //get users without superadmin user role.
                        users = users.Where(u => !_userManager.IsInRoleAsync(u, "superadmin").Result).ToList();
                    }
                  
                }
                if(param == "active")
                {
                    if (_tokenService.GetRole() == "superadmin")
                    {
                        users = await _userManager.Users.Where(x => x.IsActive == true).ToListAsync();
                        //get users without superadmin user role.
                        users = users.Where(u => !_userManager.IsInRoleAsync(u, "superadmin").Result).ToList();
                    }
                    else
                    {
                        users = await _userManager.Users.Where(x => x.IsActive == true && x.DepartmentId == _tokenService.GetDepartmentId()).ToListAsync();
                        //get users without superadmin user role.
                        users = users.Where(u => !_userManager.IsInRoleAsync(u, "superadmin").Result).ToList();
                    }

                }
                if (param == "includingsuperadmin")
                {
                    if (_tokenService.GetRole() == "superadmin")
                    {
                        users = await _userManager.Users.ToListAsync();
                    }
                    else
                    {
                        users = await _userManager.Users.Where(x => x.DepartmentId == _tokenService.GetDepartmentId()).ToListAsync();
                    }
                    
                }
            }

            if (users.Count > 0)
            {
                foreach (var user in users)
                {
                    var departmentName = (await _dbContext.Departments.Where(x => x.DepartmentId == user.DepartmentId).FirstOrDefaultAsync())?.DepartmentName ?? null;
                    registerModels.Add(new RegisterModel()
                    {
                        Fullname = user.FullName,
                        UserId = user.Id,
                        Username = user.UserName,
                        Email = user.Email,
                        DepartmentId = user.DepartmentId,
                        DepartmentName = departmentName,
                        RoleName= (await _userManager.GetRolesAsync(user)).FirstOrDefault(),
                        IsActive = user.IsActive,
                        ProfilePictureULR = user.ProfilePictureURL,
                    });
                }
                return new ServiceResult<List<RegisterModel>>(true, registerModels);
            }
            return new ServiceResult<List<RegisterModel>>(false, errors: new[] { "Users not found." });
        }

        public async Task<ServiceResult<RegisterModel>> GetUserById(string id)
        {
            var user = await _userManager.Users.Where(x => x.Id == id).FirstOrDefaultAsync();

            if (user != null)
            {
                var roles = await _userManager.GetRolesAsync(user);
                var roleId = await _roleManager.FindByNameAsync(roles.FirstOrDefault()); 

                if (roleId != null)
                {
                    var department = await _departmentRepository.GetByIdAsync(user.DepartmentId);
                    RegisterModel registerModel = new RegisterModel()
                    {
                        Fullname = user.FullName,
                        UserId = user.Id,
                        Username = user.UserName,
                        Email = user.Email,
                        RoleId = roleId.Id,
                        DepartmentId = user.DepartmentId,
                        DepartmentName = department?.DepartmentName,
                        Phone = user.PhoneNumber,
                        ProfilePictureULR = user.ProfilePictureURL,
                    };
                    return new ServiceResult<RegisterModel>(true, registerModel);
                }
            }
            return new ServiceResult<RegisterModel>(false, errors: new[] { "User not found or does not have a role." });
        }

        public async Task<ServiceResult<RegisterModel>> ChangeUserStatus(string UserId)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(x=>x.Id == UserId);
            if (user != null)
            {
                user.IsActive = !user.IsActive;
                await _dbContext.SaveChangesAsync();
                RegisterModel registerModel = new RegisterModel()
                {
                    Fullname = user.FullName,
                    UserId = user.Id,
                    Username = user.UserName,
                    Email = user.Email,
                    DepartmentId = user.DepartmentId,
                    IsActive = user.IsActive
                };
                return new ServiceResult<RegisterModel>(true, registerModel);
            }
            return new ServiceResult<RegisterModel>(false, errors: new[] { "User not found or does not have a role." });
        }
        public async Task<ServiceResult<string>> GetActiveUsers()
        {
            List<ApplicationUser> users = new List<ApplicationUser>();
            if(_tokenService.GetRole() == "superadmin")
            {
                users = await _userManager.Users.Where(x => x.RefreshToken != null).ToListAsync();
            }
            else
            {
                users = await _userManager.Users.Where(x => x.RefreshToken != null && x.DepartmentId == _tokenService.GetDepartmentId()).ToListAsync();
            }
            if (users.Count > 0)
            {
                return new ServiceResult<string>(true, users.Count.ToString());
            }
            return new ServiceResult<string>(true, "0");
        }
        public async Task<ServiceResult<object>> Logout()
        {
            var user = await _userManager.FindByIdAsync(_tokenService.GetUserId());
            if (user != null)
            {
                user.RefreshToken = null;
                await _userManager.UpdateAsync(user);
                return new ServiceResult<object>(true);
            }
            return new ServiceResult<object>(false);
        }

        public async Task<ServiceResult<List<RegisterModel>>> GetUsersByDeparmentId(string DepartmentId)
        {
            List<RegisterModel> registerModels = new List<RegisterModel>();
            List<ApplicationUser> users = new List<ApplicationUser>();
            if (!string.IsNullOrEmpty(DepartmentId))
            {
                if(await _departmentRepository.CheckIdIfExists(x=>x.DepartmentId == DepartmentId))
                {
                    users = await _userManager.Users.Where(x => x.DepartmentId == DepartmentId && x.IsActive && x.UserName != _tokenService.GetUsername()).ToListAsync();
                }
                else
                {
                    users = await _userManager.Users.Where(x => x.IsActive && x.UserName != _tokenService.GetUsername()).ToListAsync();
                }
                if(users.Count > 0)
                {
                    foreach (var user in users)
                    {
                        var departmentName = (await _dbContext.Departments.Where(x => x.DepartmentId == user.DepartmentId).FirstOrDefaultAsync())?.DepartmentName ?? null;
                        registerModels.Add(new RegisterModel()
                        {
                            Fullname = user.FullName,
                            UserId = user.Id,
                            Username = user.UserName,
                            Email = user.Email,
                            DepartmentId = user.DepartmentId,
                            DepartmentName = departmentName,
                            RoleName = (await _userManager.GetRolesAsync(user)).FirstOrDefault(),
                            IsActive = user.IsActive,
                            ProfilePictureULR=user.ProfilePictureURL,
                        });
                    }
                }
            }
            return new ServiceResult<List<RegisterModel>>(true, registerModels);
        }

        public async Task<ServiceResult<ChangePasswordDTO>> ChangePassword(ChangePasswordDTO changePasswordDTO)
        {
            var user =await _userManager.FindByNameAsync(_tokenService.GetUsername());
            if(user is not null)
            {
                var changePasswordResult = await _userManager.ChangePasswordAsync(user, changePasswordDTO.CurrentPassword, changePasswordDTO.NewPassword);
                if (!changePasswordResult.Succeeded)
                {
                    return new ServiceResult<ChangePasswordDTO>(false, errors: new[] {changePasswordResult.Errors.FirstOrDefault().Description.ToString()});
                }
            }
            return new ServiceResult<ChangePasswordDTO>(true);
        }

        public async Task<ServiceResult<UserDTO>> UpdateUser(UserDTO userDTO)
        {
            using (var dbContext = await _dbContext.Database.BeginTransactionAsync()) {
                try
                {
                    var userId = _tokenService.GetUserId();
                    if (!string.IsNullOrEmpty(userId))
                    {
                        var user = await _userManager.FindByIdAsync(userId);
                        if (user is not null)
                        {
                            if (!string.IsNullOrEmpty(userDTO.Fullname))
                            {
                                user.FullName = userDTO.Fullname;
                            }
                            if (!string.IsNullOrEmpty(userDTO.Email))
                            {
                                user.Email = userDTO.Email;
                            }
                            if (!string.IsNullOrEmpty(userDTO.Phone))
                            {
                                user.PhoneNumber = userDTO.Phone;
                            }
                            if (userDTO.ProfilePicture != null)
                            {
                                user.ProfilePictureURL = await _uploadImages.UploadImageAsync(userDTO.ProfilePicture);
                                user.ProfilePictureName = userDTO.ProfilePicture.FileName;
                            };
                            var user1 = await _userManager.UpdateAsync(user);
                            await dbContext.CommitAsync();
                            await dbContext.DisposeAsync();
                            return new ServiceResult<UserDTO>(true);

                        }
                    }
                    return new ServiceResult<UserDTO>(false);
                }
                catch(Exception ex)
                {
                    await dbContext.RollbackAsync();
                    await dbContext.DisposeAsync();
                    return new ServiceResult<UserDTO>(false, errors: new[] {ex.Message} );
                }
            }
        }

        public async Task<ServiceResult<List<UserDTO>>> GetUsersByChatTimestamp(string DepartmentId)
        {
            List<UserDTO> userDTOs = new List<UserDTO>();
            List<ApplicationUser> users = new List<ApplicationUser>();

            if (await _departmentRepository.CheckIdIfExists(x => x.DepartmentId == DepartmentId))
            {
                users = await _userManager.Users.Where(x => x.DepartmentId == DepartmentId && x.IsActive && x.UserName != _tokenService.GetUsername()).ToListAsync();
            }
            else
            {
                users = await _userManager.Users.Where(x => x.IsActive && x.UserName != _tokenService.GetUsername()).ToListAsync();
            }

            var usersWithChatTimestamp = new List<(ApplicationUser User, DateTime Timestamp, string? lastmessage, DateTime? lastMessageDateTime)>();

            foreach (var user in users)
            {
                var mostRecentMessage = await _dbContext.Messages
                    .Where(x => x.SenderUsername == _tokenService.GetUsername() && x.ReceiverUsername == user.UserName || x.SenderUsername == user.UserName && x.ReceiverUsername == _tokenService.GetUsername() && !x.IsDeleted && !x.IsDeletedBySender)
                    .OrderByDescending(m => m.DateTime)
                    .FirstOrDefaultAsync();

                var timestamp = mostRecentMessage?.DateTime ?? DateTime.MinValue;

                usersWithChatTimestamp.Add((user, timestamp, mostRecentMessage?.Messages, mostRecentMessage?.DateTime));
            }

            var sortedUsers = usersWithChatTimestamp.OrderByDescending(u => u.Timestamp).Select(u => new {u.User,u.lastmessage,u.lastMessageDateTime});
            foreach (var sorteduser in sortedUsers)
            {
                string decryptedMessage= "";
                if (!string.IsNullOrEmpty(sorteduser?.lastmessage))
                {
                    decryptedMessage = await _encryptionService.Decrypt(sorteduser?.lastmessage);
                }
                userDTOs.Add(new UserDTO()
                {
                    Fullname = sorteduser.User.FullName,
                    UserId = sorteduser.User.Id,
                    Username = sorteduser.User.UserName,
                    Email = sorteduser.User.Email,
                    DepartmentId = sorteduser.User.DepartmentId,
                    IsActive = sorteduser.User.IsActive,
                    ProfilePictureULR = sorteduser.User.ProfilePictureURL,
                    LastMessage = decryptedMessage,
                    LastMessageDateTime = sorteduser.lastMessageDateTime
                });
            }
            return new ServiceResult<List<UserDTO>>(true, userDTOs);
        }
    }
}
