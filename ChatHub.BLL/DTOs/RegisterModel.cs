using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatHub.BLL.DTOs
{
    public class RegisterModel
    {
        [Required(ErrorMessage = "User Name is required")]
        public string? Username { get; set; }

        [EmailAddress]
        [Required(ErrorMessage = "Email is required")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string? Password { get; set; }
        public string? RoleId { get; set; }
        public string? DepartmentId { get; set; }

        public string? UserId { get; set; }
        public string? DepartmentName { get; set; }
        public string? RoleName { get; set; }
        public bool IsActive { get; set; }
        public string? Fullname { get; set; }

    }
}
