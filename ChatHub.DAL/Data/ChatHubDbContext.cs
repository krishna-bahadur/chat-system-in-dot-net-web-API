using ChatHub.DAL.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatHub.DAL.Datas
{
    public class ChatHubDbContext : IdentityDbContext<ApplicationUser>
    {
        public ChatHubDbContext(DbContextOptions<ChatHubDbContext> options) : base(options) { }
        public DbSet<Organization> Organizations { get; set; }
        public DbSet<Department> Departments { get; set; }

    }
}
