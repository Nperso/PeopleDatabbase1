using Microsoft.EntityFrameworkCore;

namespace PeopleDatabase1
{
    public class PeopleDbContext : DbContext
    {
        public DbSet<Person> People { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseSqlite("Data Source=people.db");
        }
    }
}