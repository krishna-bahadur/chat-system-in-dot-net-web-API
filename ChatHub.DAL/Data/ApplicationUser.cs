using ChatHub.DAL.Entities;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatHub.DAL.Datas
{
    public class ApplicationUser : IdentityUser
    {
        public string? FullName { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime RefreshTokenExpiryTime { get; set; }
        public bool IsActive { get; set; }
        public virtual Department Department { get; set; }
        public string? DepartmentId { get; set; }

    }
}
