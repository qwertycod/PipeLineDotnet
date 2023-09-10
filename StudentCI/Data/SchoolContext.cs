using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace StudentCI.Data
{
    public class SchoolContext : DbContext
    {
        public SchoolContext(DbContextOptions<SchoolContext> options) : base(options) { }
        public DbSet<Models.Student>? students { get; set; }
        public DbSet<Models.Product>? cars { get; set; }
        public DbSet<Models.Bird>? birds { get; set; }
    }
}
