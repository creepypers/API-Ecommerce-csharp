using Microsoft.EntityFrameworkCore;
using ECOM_UtilisateurMicroservice.Models;

namespace ECOM_UtilisateurMicroservice
{
    public class UtilisateurDbContext : DbContext
    {
        public UtilisateurDbContext(DbContextOptions<UtilisateurDbContext> options) 
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
    }
}
   