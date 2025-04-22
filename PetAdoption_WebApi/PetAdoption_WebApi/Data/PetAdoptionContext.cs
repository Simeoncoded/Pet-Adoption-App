using Microsoft.EntityFrameworkCore;
using PetAdoption_WebApi.Models;
using System.Numerics;

namespace PetAdoption_WebApi.Data
{
	public class PetAdoptionContext : DbContext
	{
        //To give access to IHttpContextAccessor for Audit Data with IAuditable
        private readonly IHttpContextAccessor _httpContextAccessor;

        //Property to hold the UserName value
        public string UserName
        {
            get; private set;
        }

        public PetAdoptionContext(DbContextOptions<PetAdoptionContext> options, IHttpContextAccessor httpContextAccessor)
             : base(options)
        {
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            if (_httpContextAccessor.HttpContext != null)
            {
                //We have a HttpContext, but there might not be anyone Authenticated
                UserName = _httpContextAccessor.HttpContext?.User?.Identity?.Name ?? "Unknown";
            }
            else
            {
                //No HttpContext so seeding data
                UserName = "Seed Data";
            }
        }

        public PetAdoptionContext(DbContextOptions<PetAdoptionContext> options)
            : base(options)
        {
            _httpContextAccessor = null!;
            UserName = "Seed Data";
        }
        public DbSet<Pet> Pets { get; set; }
		public DbSet<Adoption> Adoptions { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
            // Prevent deleting a pet that has an adoption record
            modelBuilder.Entity<Pet>()
                   .HasMany(p => p.Adoptions)
                   .WithOne(d => d.Pet)
                   .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Adoption>()
                .HasIndex(a => a.Phone)
                .IsUnique();

		}


        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            OnBeforeSaving();
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default(CancellationToken))
        {
            OnBeforeSaving();
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        private void OnBeforeSaving()
        {
            var entries = ChangeTracker.Entries();
            foreach (var entry in entries)
            {
                if (entry.Entity is IAuditable trackable)
                {
                    var now = DateTime.UtcNow;
                    switch (entry.State)
                    {
                        case EntityState.Modified:
                            trackable.UpdatedOn = now;
                            trackable.UpdatedBy = UserName;
                            break;

                        case EntityState.Added:
                            trackable.CreatedOn = now;
                            trackable.CreatedBy = UserName;
                            trackable.UpdatedOn = now;
                            trackable.UpdatedBy = UserName;
                            break;
                    }
                }
            }
        }

    }
}
