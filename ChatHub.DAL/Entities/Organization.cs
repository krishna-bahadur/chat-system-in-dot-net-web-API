using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatHub.DAL.Entities
{
    public class Organization
    {
        public string OrganizationId { get; set; }
        public string? OrganizationName { get; set; }
        public string?  Location { get; set; }
        public string?  Phone { get; set; }
        public DateTime EstablishedIn { get; set; }
        public string? CEO { get; set; }
        public string? LogoName { get; set; }
        public string? LogoURL { get; set; }
        public virtual List<Department>? Departments { get; set; }
    }
}
