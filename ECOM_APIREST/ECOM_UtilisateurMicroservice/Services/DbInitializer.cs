using ECOM_UtilisateurMicroservice.Models;
using Microsoft.EntityFrameworkCore;

namespace ECOM_UtilisateurMicroservice.Services
{
    public class DbInitializer
    {
        private readonly UtilisateurDbContext _context;
        private readonly DummyJsonService _dummyJsonService;
        private readonly ILogger<DbInitializer> _logger;

        public DbInitializer(
            UtilisateurDbContext context, 
            DummyJsonService dummyJsonService,
            ILogger<DbInitializer> logger)
        {
            _context = context;
            _dummyJsonService = dummyJsonService;
        }

        public async Task InitializeDatabaseAsync()
        {
            try 
            {
                await _context.Database.EnsureCreatedAsync();

                if (await _context.Users.AnyAsync())
                {
                    return;
                }

                var dummyUsers = await _dummyJsonService.GetUsersAsync();

                _context.Users.AddRange(dummyUsers);
                await _context.SaveChangesAsync();

            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
} 