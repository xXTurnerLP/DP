using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DP.Models.Auth;
using Microsoft.EntityFrameworkCore;

namespace DP.Models
{
    public class ApplicationContext : DbContext
    {
		public ApplicationContext(DbContextOptions options) : base(options)
		{
		}

		public DbSet<User> Users { get; set; }
		public DbSet<Session> Sessions { get; set; }
		public DbSet<Offer> Offers { get; set; }
    }
}
