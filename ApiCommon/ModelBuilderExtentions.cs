using System;
using System.Linq;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using WebstoreEntities.Markers;

namespace WebstoreData.Extensions
{
    public static class ModelBuilderExtensions
    {
        private static readonly MethodInfo SetAuditingShadowPropertiesMethodInfo = typeof(ModelBuilderExtensions)
            .GetMethods(BindingFlags.Public | BindingFlags.Static)
            .Single(t => t.IsGenericMethod && t.Name == "SetAuditingShadowProperties");

        private static readonly MethodInfo SetIsDeletedShadowPropertyMethodInfo = typeof(ModelBuilderExtensions)
            .GetMethods(BindingFlags.Public | BindingFlags.Static)
            .Single(t => t.IsGenericMethod && t.Name == "SetIsDeletedShadowProperty");

        private static readonly MethodInfo SetDeleteAuditShadowPropertyMethodInfo = typeof(ModelBuilderExtensions)
            .GetMethods(BindingFlags.Public | BindingFlags.Static)
            .Single(t => t.IsGenericMethod && t.Name == "SetDeleteAuditShadowProperty");

        public static void ShadowProperties(this ModelBuilder modelBuilder)
        {
            foreach (var type in modelBuilder.Model.GetEntityTypes())
            {
                var t = type.ClrType;
                if (typeof(IAudited).IsAssignableFrom(t))
                {
                    var method = SetAuditingShadowPropertiesMethodInfo.MakeGenericMethod(t);
                    method.Invoke(modelBuilder, new object[] {modelBuilder});
                }

                if (typeof(ISoftDelete).IsAssignableFrom(t))
                {
                    var method = SetIsDeletedShadowPropertyMethodInfo.MakeGenericMethod(t);
                    method.Invoke(modelBuilder, new object[] {modelBuilder});
                }

                if (typeof(ISoftDelete).IsAssignableFrom(t) && typeof(IAudited).IsAssignableFrom(t))
                {
                    var method = SetDeleteAuditShadowPropertyMethodInfo.MakeGenericMethod(t);
                    method.Invoke(modelBuilder, new object[] {modelBuilder});
                }
            }
        }

        public static void SetIsDeletedShadowProperty<T>(ModelBuilder builder) where T : class, ISoftDelete
        {
            // define shadow property
            builder.Entity<T>().Property<bool>("IsDeleted");
        }

        public static void SetDeleteAuditShadowProperty<T>(ModelBuilder builder) where T : class, ISoftDelete, IAudited
        {
            builder.Entity<T>().Property<DateTime>("DeletedOn");
            builder.Entity<T>().Property<int>("DeletedById");
        }

        public static void SetAuditingShadowProperties<T>(ModelBuilder builder) where T : class, IAudited
        {
            // define shadow properties
            builder.Entity<T>().Property<DateTime>("CreatedOn");
            builder.Entity<T>().Property<DateTime>("ModifiedOn");
            builder.Entity<T>().Property<int>("CreatedById");
            builder.Entity<T>().Property<int>("ModifiedById");
        }
    }
}