using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatHub.BLL.DTOs
{
    public class UserDTO
    {
        public string? UserId { get; set; }
        public string? Fullname { get; set; }
        public string? Username { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? ProfilePictureURL { get; set; }
        public IFormFile? ProfilePicture { get; set; }
        public string? RoleId { get; set; }
        public string? DepartmentId { get; set; }
        public string? DepartmentName { get; set; }
        public string? RoleName { get; set; }
        public bool IsActive { get; set; }
        public string? ProfilePictureULR { get; set; }
        public string? LastMessage { get; set; }
        public DateTime? LastMessageDateTime { get; set; }
    }
}
