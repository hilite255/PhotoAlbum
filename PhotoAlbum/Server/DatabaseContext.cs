using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PhotoAlbum.Shared.Model;

namespace PhotoAlbum.Server
{
    public class DatabaseContext : IdentityDbContext<IdentityUser>
    {
        public DatabaseContext(DbContextOptions options) : base(options) { }
        public DatabaseContext() : base() { }

        public DbSet<Photo> Photos { get; set; }
        public DbSet<IdentityUser> Users {  get; set; }
    }
}
