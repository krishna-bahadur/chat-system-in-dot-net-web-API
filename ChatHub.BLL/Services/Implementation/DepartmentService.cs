using ChatHub.BLL.DTOs;
using ChatHub.BLL.Services.Interfaces;
using ChatHub.DAL.Datas;
using ChatHub.DAL.Entities;
using ChatHub.DAL.Repository.Interfaces;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatHub.BLL.Services.Implementation
{
    public class DepartmentService : IDepartmentService
    {
        private readonly IRepository<Department> _departmentRepository;
        private readonly ChatHubDbContext _dbContext;
        private readonly IUploadImages _uploadImages;
        private readonly ITokenService _tokenService;
        public DepartmentService(
            IRepository<Department> departmentRepository,
            ChatHubDbContext dbContext,
            IUploadImages uploadImages, 
            ITokenService tokenService
            )
        {
            _departmentRepository = departmentRepository;
            _dbContext = dbContext;
            _uploadImages = uploadImages;
            _tokenService = tokenService;
        }
        public async Task<ServiceResult<DepartmentDTO>> CreateDepartment(DepartmentDTO departmentDTO)
        {
            using(var dbContext = await _dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    Department department = new Department()
                    {
                        DepartmentId = Guid.NewGuid().ToString(),
                        DepartmentName = departmentDTO.DepartmentName,
                        DepartmentHead = departmentDTO.DepartmentHead,
                        Phone = departmentDTO.Phone,
                        OrganizationId = _tokenService.GetDepartmentId(),
                    };
                    if(departmentDTO.LogoFile != null)
                    {
                        department.LogoURL = await _uploadImages.UploadImageAsync(departmentDTO.LogoFile);
                        department.LogoName = departmentDTO.LogoFile.FileName;
                    };
                    var department1 = await _departmentRepository.AddAsync(department);
                    await dbContext.CommitAsync();
                    await dbContext.DisposeAsync();
                    return new ServiceResult<DepartmentDTO>(true, new DepartmentDTO().ToDepartmentDTO(department1));
                }catch(Exception ex)
                {
                    await dbContext.RollbackAsync();
                    await dbContext.CommitAsync();
                    return new ServiceResult<DepartmentDTO>(false, errors: new[] { ex.Message });
                }
            }
        }

        public async Task<ServiceResult<DepartmentDTO>> GetDepartmentById(string departmentId)
        {
            var department = await _departmentRepository.GetByIdAsync(departmentId);
            if (department != null)
            {
                return new ServiceResult<DepartmentDTO>(true, new DepartmentDTO().ToDepartmentDTO(department));
            }
            return new ServiceResult<DepartmentDTO>(false, errors: new[] { "Department not found" });
        }

        public async Task<ServiceResult<List<DepartmentDTO>>> GetDepartments()
        {
            var departments = await _departmentRepository.GetAllAsync();
            if(departments.Count > 0)
            {
                return new ServiceResult<List<DepartmentDTO>>(true, new DepartmentDTO().ToDepartmentDTOLists(departments));
            }
            return new ServiceResult<List<DepartmentDTO>>(false, errors: new[] {"Departments not found."});
        }

        public async Task<ServiceResult<DepartmentDTO>> UpdateDepartment(DepartmentDTO departmentDTO)
        {
            using (var dbContext = await _dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    Department department = new Department()
                    {
                        DepartmentId = departmentDTO.DepartmentId,
                        DepartmentName = departmentDTO.DepartmentName,
                        DepartmentHead = departmentDTO.DepartmentHead,
                        Phone = departmentDTO.Phone,
                        OrganizationId = _tokenService.GetDepartmentId(),
                    };
                    if (departmentDTO.LogoFile != null)
                    {
                        department.LogoURL = await _uploadImages.UploadImageAsync(departmentDTO.LogoFile);
                        department.LogoName = departmentDTO.LogoFile.FileName;
                    };
                    var department1 = await _departmentRepository.UpdateAsync(department);
                    await dbContext.CommitAsync();
                    await dbContext.DisposeAsync();
                    return new ServiceResult<DepartmentDTO>(true, new DepartmentDTO().ToDepartmentDTO(department1));
                }
                catch (Exception ex)
                {
                    await dbContext.RollbackAsync();
                    await dbContext.CommitAsync();
                    return new ServiceResult<DepartmentDTO>(false, errors: new[] { ex.Message });
                }
            }
        }
    }
}
