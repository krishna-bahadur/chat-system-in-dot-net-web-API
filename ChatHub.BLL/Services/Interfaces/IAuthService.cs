using ChatHub.BLL.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatHub.BLL.Services.Interfaces
{
    public interface IAuthService
    {
        Task<ServiceResult<object>> Login(LoginModel loginModel);
        Task<ServiceResult<object>> CreateUser(RegisterModel registerModel);
        Task<ServiceResult<object>> logout(string username);
        Task<ServiceResult<object>> CheckEmail(string email);
        Task<ServiceResult<object>> CheckUsername(string username);
        Task<ServiceResult<RegisterModel>> GetUserById(string Id);
        Task<ServiceResult<List<RoleDTO>>> GetRoles();
        Task<ServiceResult<List<RegisterModel>>> GetAllUsers(string param);
        Task<ServiceResult<string>> GetActiveUsers();
        Task<ServiceResult<object>> Logout();
        Task<ServiceResult<RegisterModel>> ChangeUserStatus(string Id);
        Task<ServiceResult<object>> RefreshToken(TokenModel tokenModel);
    }
}
