using ChatHub.BLL.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatHub.BLL.Services.Interfaces
{
    public interface IInitializeService
    {
        Task<ServiceResult<OrganizationDTO>> Initialize(OrganizationDTO organizationDTO);
        Task<ServiceResult<OrganizationDTO>> CheckInitialize();
        Task<ServiceResult<Object>> ResetSystem();
    }
}
