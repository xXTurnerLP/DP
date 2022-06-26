using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace DP.Models.Auth.Contexts
{
    public class UserContext : DbContext
    {
		public UserContext(DbContextOptions options) : base(options)
		{
		}

		public DbSet<User> User { get; set; }
    }
}
