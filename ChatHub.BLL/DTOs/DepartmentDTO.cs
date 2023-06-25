using ChatHub.DAL.Entities;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatHub.BLL.DTOs
{
    public class DepartmentDTO
    {
        public string? DepartmentId { get; set; }
        public string? DepartmentName { get; set; }
        public string? DepartmentHead { get; set; }
        public string? Phone { get; set; }
        public IFormFile? LogoFile { get; set; }
        public string? LogoURL { get; set; }
        public string? LogoName { get; set; }
        public string? OrganizationId { get; set; }

        public DepartmentDTO ToDepartmentDTO(Department department)
        {
            return new DepartmentDTO
            {
                DepartmentId = department.DepartmentId,
                DepartmentName = department.DepartmentName,
                DepartmentHead = department.DepartmentHead,
                Phone = department.Phone,
                OrganizationId = department.OrganizationId,
                LogoName = department.LogoName,
                LogoURL = department.LogoURL
            };
        }
        public List<DepartmentDTO> ToDepartmentDTOLists(List<Department> departments)
        {
            List<DepartmentDTO> departmentDTOs = new List<DepartmentDTO>();
            foreach (var department in departments)
            {
                departmentDTOs.Add(new DepartmentDTO().ToDepartmentDTO(department));
            }
            return departmentDTOs;
        }
    }
}
