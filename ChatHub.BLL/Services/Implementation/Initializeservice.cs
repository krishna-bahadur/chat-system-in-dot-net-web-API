using ChatHub.BLL.DTOs;
using ChatHub.BLL.Services.Interfaces;
using ChatHub.DAL.Datas;
using ChatHub.DAL.Entities;
using ChatHub.DAL.Repository.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatHub.BLL.Services.Implementation
{
    public class Initializeservice : IInitializeService
    {
        private readonly IRepository<Organization> _organizationRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ChatHubDbContext _dbContext;
        private readonly IUploadImages _uploadImages;
        public Initializeservice(
            IRepository<Organization> organizationRepository,
            ChatHubDbContext dbContext,
            IUploadImages uploadImages,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager
            )
        {
            _organizationRepository = organizationRepository;
            _dbContext = dbContext;
            _uploadImages = uploadImages;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<ServiceResult<OrganizationDTO>> CheckInitialize()
        {
            var result = await _organizationRepository.AnyAsync();
            if (result)
            {
                return new ServiceResult<OrganizationDTO>(false, errors: new[] {"System has been Already Initialized."});
            }
            else
            {
                return new ServiceResult<OrganizationDTO>(true);
            }
        }

        public async Task<ServiceResult<OrganizationDTO>> Initialize([FromBody]OrganizationDTO dto)
        {
            using(var dbContext =await _dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    if (dto.LogoFile != null)
                    {
                        dto.LogoURL = await _uploadImages.UploadImageAsync(dto.LogoFile);
                        dto.LogoName = dto.LogoFile.FileName;
                    }
                    Organization organization = new Organization()
                    {
                        OrganizationId = Guid.NewGuid().ToString(),
                        OrganizationName = dto.OrganizationName,
                        Location = dto.Location,
                        Phone = dto.Phone,
                        EstablishedIn = dto.EstablishedIn,
                        LogoURL = dto.LogoURL,
                        LogoName = dto.LogoName,
                        CEO = dto.CEO,
                    };
                    var organization1 =await _organizationRepository.AddAsync(organization);
                    var checkuser = await _userManager.FindByNameAsync(dto.Username);
                    if (checkuser != null)
                    {
                        return new ServiceResult<OrganizationDTO>(false, errors: new[] { "User Already Exists." });
                    }
                   
                    ApplicationUser user = new ApplicationUser()
                    {
                        SecurityStamp = Guid.NewGuid().ToString(),
                        UserName = dto.Username,
                        Email = dto.Email,
                        DepartmentId = organization.OrganizationId,
                        IsActive=true
                    };
                    var result = await _userManager.CreateAsync(user, dto.Password);
                    if (!result.Succeeded)
                    {
                        return new ServiceResult<OrganizationDTO>(false, errors: new[] { "User creation failed. Plase try again." });
                    };
                    if (!await _roleManager.RoleExistsAsync(UserRoles.superadmin))
                    {
                        await _roleManager.CreateAsync(new IdentityRole(UserRoles.superadmin));
                    }
                    if (!await _roleManager.RoleExistsAsync(UserRoles.admin))
                    {
                        await _roleManager.CreateAsync(new IdentityRole(UserRoles.admin));
                    }
                    if (!await _roleManager.RoleExistsAsync(UserRoles.user))
                    {
                        await _roleManager.CreateAsync(new IdentityRole(UserRoles.user));
                    }
                    if (await _roleManager.RoleExistsAsync(UserRoles.superadmin))
                    {
                        await _userManager.AddToRoleAsync(user, UserRoles.superadmin);
                    }

                    dbContext.Commit();
                    dbContext.Dispose();
                    return new ServiceResult<OrganizationDTO>(true);
                    
                }
                catch (Exception ex)
                {
                    dbContext.Rollback();
                    dbContext.Dispose();
                    return new ServiceResult<OrganizationDTO>(false, errors: new[] { ex.Message });
                }
            }
            
        }

        public async Task<ServiceResult<object>> ResetSystem()
        {
            using(var dbContext =  await _dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    // Disable foreign key constraints
                    await _dbContext.Database.ExecuteSqlRawAsync("EXEC sp_MSforeachtable 'ALTER TABLE ? NOCHECK CONSTRAINT all'");

                    // Delete data from all tables
                    var tables = _dbContext.Model.GetEntityTypes().Select(x => x.GetTableName());

                    foreach (var table in tables)
                    {
                       await _dbContext.Database.ExecuteSqlRawAsync($"DELETE FROM {table}");
                    }

                    // Enable foreign key constraints
                    _dbContext.Database.ExecuteSqlRaw("EXEC sp_MSforeachtable 'ALTER TABLE ? WITH CHECK CHECK CONSTRAINT all'");

                    await dbContext.CommitAsync();
                    await dbContext.DisposeAsync();
                    return new ServiceResult<object>(true);
                }
                catch(Exception ex)
                {
                    await dbContext.RollbackAsync();
                    await dbContext.DisposeAsync();
                    return new ServiceResult<object>(false, errors: new[] { ex.Message });
                }
            }
        }
    }
}
