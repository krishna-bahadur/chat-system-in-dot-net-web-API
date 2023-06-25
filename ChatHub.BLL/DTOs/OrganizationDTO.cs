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
    public class OrganizationDTO
    {
        [Required(ErrorMessage ="Organization name is required.")]
        public string? OrganizationName { get; set; }
        [Required(ErrorMessage = "Organization location is required.")]
        public string? Location { get; set; }
        [Required(ErrorMessage = "Organization Phone no. is required.")]
        public string? Phone { get; set; }
        [Required(ErrorMessage = "Organization established date is required.")]
        public DateTime EstablishedIn { get; set; }
        [Required(ErrorMessage = "Organization CEO/Head date is required.")]
        public string? CEO { get; set; }
        public IFormFile? LogoFile { get; set; }
        public string? LogoURL { get; set; }
        public string? LogoName { get; set; }
        
        [Required(ErrorMessage = "Username is required.")]
        public string? Username { get; set; }
        [Required(ErrorMessage = "Password is required.")]
        public string? Password { get; set; }
        [Required(ErrorMessage = "Email is required.")]
        public string? Email { get; set; }

        public OrganizationDTO ToOrganizationDTO(Organization organization)
        {
            return new OrganizationDTO
            {
                OrganizationName = organization.OrganizationName,
                Location = organization.Location,
                Phone = organization.Phone,
                EstablishedIn = organization.EstablishedIn,
                LogoURL = organization.LogoURL,
                LogoName = organization.LogoName,
                CEO = organization.CEO,
            };
        }
    }
}
