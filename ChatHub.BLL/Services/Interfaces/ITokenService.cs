using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatHub.BLL.Services.Interfaces
{
    public interface ITokenService
    {
        string GetUserId();
        string GetDepartmentId();
        string GetRole();
        string GetUsername();
    }
}
