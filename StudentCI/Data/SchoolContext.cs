﻿using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace StudentCI.Data
{
    public class SchoolContext : DbContext
    {
        public SchoolContext(DbContextOptions<SchoolContext> options) : base(options) { }
        public DbSet<Models.Product>? products { get; set; }
        public DbSet<Models.Bird>? birds { get; set; }
    }
}
