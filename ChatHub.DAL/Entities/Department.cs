using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatHub.DAL.Entities
{
    public class Department
    {
        public string DepartmentId { get; set; }
        public string? DepartmentName { get; set; }
        public string? DepartmentHead { get; set; }
        public string? Phone { get; set; }
        public string? LogoName { get; set; }
        public string? LogoURL { get; set; }
        public string? OrganizationId { get; set; }
        public Organization Organization { get; set; }

    }
}
