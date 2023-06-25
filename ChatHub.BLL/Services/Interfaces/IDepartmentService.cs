using ChatHub.BLL.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatHub.BLL.Services.Interfaces
{
    public interface IDepartmentService
    {
        Task<ServiceResult<DepartmentDTO>> CreateDepartment(DepartmentDTO departmentDTO);
        Task<ServiceResult<List<DepartmentDTO>>> GetDepartments();
        Task<ServiceResult<DepartmentDTO>> GetDepartmentById(string departmentId);
    }
}
