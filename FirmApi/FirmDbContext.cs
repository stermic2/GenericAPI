using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using WebstoreData.Extensions;
using WebstoreEntities.Entities;
using WebstoreEntities.Markers;

namespace FirmApi
{
    public class FirmDbContext : DbContext
    {
        private static readonly MethodInfo SetGlobalQueryForSoftDeleteMethodInfo = typeof(FirmDbContext)
            .GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .Single(t => t.IsGenericMethod && t.Name == "SetGlobalQueryForSoftDelete");

        private readonly IHttpContextAccessor _accessor;

        public FirmDbContext(DbContextOptions<FirmDbContext> options, IHttpContextAccessor accessor) :
            base(options)
        {
            _accessor = accessor;
        }
        
        public DbSet<Entity> Entities { get; set; }
        public DbSet<StringProperty> StringProperties { get; set; }
        public DbSet<IntProperty> IntProperties { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Entity>()
                .HasMany(x => x.IntProperties)
                .WithOne(x => x.Entity);
            modelBuilder.Entity<Entity>()
                .HasMany(x => x.StringProperties)
                .WithOne(x => x.Entity);
            
            modelBuilder.ShadowProperties();
            SetGlobalQueryFilters(modelBuilder);
        }
        
        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            ChangeTracker.SetShadowProperties(_accessor);
            return await base.SaveChangesAsync(cancellationToken);
        }
        private void SetGlobalQueryFilters(ModelBuilder modelBuilder)
        {
            foreach (var type in modelBuilder.Model.GetEntityTypes())
            {
                var t = type.ClrType;
                if (typeof(ISoftDelete).IsAssignableFrom(t))
                {
                    // softdeletable
                    var method = SetGlobalQueryForSoftDeleteMethodInfo.MakeGenericMethod(t);
                    method.Invoke(this, new object[] {modelBuilder});
                }
            }
        }
    }
}