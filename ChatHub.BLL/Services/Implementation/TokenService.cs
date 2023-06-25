using ChatHub.BLL.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatHub.BLL.Services.Implementation
{
    public class TokenService : ITokenService
    {
        private readonly IHttpContextAccessor _contextAccessor;
        public TokenService(IHttpContextAccessor contextAccessor)
        {
            _contextAccessor = contextAccessor;
        }
        private string GetTokenFromAuthorizationHeader()
        {
            string authorization = _contextAccessor.HttpContext.Request.Headers.Authorization;
            var token = authorization.Split(" ")[1];
            return token;
        }
        public string GetDepartmentId()
        {
            var token = GetTokenFromAuthorizationHeader();
            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(token);
            var depId = jwt.Claims.FirstOrDefault(x => x.Type == "depid");
            return depId.Value;
        }

        public string GetUserId()
        {
            var token = GetTokenFromAuthorizationHeader();
            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(token);
            var uid = jwt.Claims.FirstOrDefault(x => x.Type == "uid");
            return uid.Value;
        }
    }
}
