using System;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using WebstoreEntities.Interfaces;
using WebstoreEntities.Markers;

namespace WebstoreData.Extensions
{
    public static class ChangeTrackerExtensions
    {
        public static void SetShadowProperties(this ChangeTracker changeTracker, IHttpContextAccessor accessor)
        {
            changeTracker.DetectChanges();
            var timestamp = DateTime.UtcNow;
            var userId = accessor.HttpContext?.User.GetUserId() ?? 0;

            foreach (var entry in changeTracker.Entries())
            {
                if (entry.State == EntityState.Added && entry.Entity is IEntity && entry.Property("Id").CurrentValue == null)
                {
                    entry.Property("Id").CurrentValue = Guid.NewGuid().ToString();
                }
                
                if (entry.Entity is IAudited)
                {
                    if (entry.State == EntityState.Added || entry.State == EntityState.Modified)
                    {
                        entry.Property("ModifiedById").CurrentValue = userId;
                        entry.Property("ModifiedOn").CurrentValue = timestamp;
                    }

                    if (entry.State == EntityState.Added)
                    {
                        entry.Property("CreatedById").CurrentValue = userId;
                        entry.Property("CreatedOn").CurrentValue = timestamp;
                    }
                }

                if (IsSoftDeleted(entry))
                {
                    entry.State = EntityState.Modified;
                    entry.Property("IsDeleted").CurrentValue = true;
                    if (entry.Entity is IAudited)
                    {
                        entry.Property("DeletedById").CurrentValue = userId;
                        entry.Property("DeletedOn").CurrentValue = timestamp;
                    }
                }
            }
        }

        private static bool IsSoftDeleted(EntityEntry entry)
        {
            return entry.State == EntityState.Deleted && entry.Entity is ISoftDelete;
        }
    }
}