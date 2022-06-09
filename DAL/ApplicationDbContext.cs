// =============================
// Email: abdoneem@gmail.com
// 
// =============================

using DAL.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using DAL.Models.Interfaces;

namespace DAL
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string>
    {
        public string CurrentUserId { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Contact> Contacts { get; set; }
        public DbSet<Menu> Menus { get; set; }
        public DbSet<PostType> PostTypes { get; set; }
        public DbSet<PostImage> PostImages { get; set; }
        public DbSet<Setting> Settings { get; set; }
        public ApplicationDbContext(DbContextOptions options) : base(options)
        { }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<ApplicationUser>().HasMany(u => u.Claims).WithOne().HasForeignKey(c => c.UserId).IsRequired().OnDelete(DeleteBehavior.Cascade);

            builder.Entity<ApplicationUser>().HasMany(u => u.Roles).WithOne().HasForeignKey(r => r.UserId).IsRequired().OnDelete(DeleteBehavior.Cascade);

            builder.Entity<ApplicationRole>().HasMany(r => r.Claims).WithOne().HasForeignKey(c => c.RoleId).IsRequired().OnDelete(DeleteBehavior.Cascade);
            builder.Entity<ApplicationRole>().HasMany(r => r.Users).WithOne().HasForeignKey(r => r.RoleId).IsRequired().OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Post>().Property(a => a.Title).HasMaxLength(500).IsRequired();
            builder.Entity<Post>().Property(a => a.TitleEn).HasMaxLength(500);
            builder.Entity<Post>().Property(a => a.MainImageUrl).HasMaxLength(500);
            builder.Entity<Post>().Property(a => a.IconUrl).HasMaxLength(500);
            builder.Entity<PostImage>().Property(a => a.ImageUrl).HasMaxLength(500).IsRequired();
            builder.Entity<PostType>().Property(a => a.Name).HasMaxLength(100).IsRequired();
            builder.Entity<PostType>().Property(a => a.NameEn).HasMaxLength(100).IsRequired();
            builder.Entity<PostType>().Property(a => a.Description).HasMaxLength(500);
            builder.Entity<PostType>().Property(a => a.DescriptionEn).HasMaxLength(500);
            builder.Entity<Menu>().Property(a => a.Name).HasMaxLength(100).IsRequired();
            builder.Entity<Menu>().Property(a => a.NameEn).HasMaxLength(100).IsRequired();
            builder.Entity<Menu>().Property(a => a.Url).HasMaxLength(500).IsRequired();
            builder.Entity<Contact>().Property(a => a.Name).HasMaxLength(100).IsRequired();
            builder.Entity<Contact>().Property(a => a.Phone).HasMaxLength(20).IsRequired();
            builder.Entity<Contact>().Property(a => a.Message).HasMaxLength(1000).IsRequired();
            builder.Entity<Contact>().Property(a => a.Email).HasMaxLength(100).IsRequired();
            builder.Entity<Setting>().Property(a => a.Email).HasMaxLength(100).IsRequired();
            builder.Entity<Setting>().Property(a => a.Phone).HasMaxLength(20).IsRequired();
            builder.Entity<Setting>().Property(a => a.Twitter).HasMaxLength(50);
            builder.Entity<Setting>().Property(a => a.Facebook).HasMaxLength(50);
            builder.Entity<Setting>().Property(a => a.Youtube).HasMaxLength(250);
            builder.Entity<Setting>().Property(a => a.Instagram).HasMaxLength(50);
            builder.Entity<Setting>().Property(a => a.Snapchat).HasMaxLength(50);
            builder.Entity<Setting>().Property(a => a.Linkedin).HasMaxLength(50);
            builder.Entity<Setting>().Property(a => a.Whatsapp).HasMaxLength(50);
            builder.Entity<Setting>().Property(a => a.Address).HasMaxLength(1000);
            builder.Entity<Setting>().Property(a => a.AddressEn).HasMaxLength(1000);
            builder.Entity<Setting>().Property(a => a.Map);

            builder.Entity<Menu>()
              .HasOne(e => e.Parent)
              .WithMany(e => e.Children)
              .HasForeignKey(e => e.ParentId);
            builder.Entity<PostImage>()
                .HasOne(a => a.Post)
                .WithMany(a => a.PostImages)
                .HasForeignKey(a => a.PostId)
                .OnDelete(DeleteBehavior.Cascade);
            builder.Entity<PostImage>()
            .HasKey(pi => new
            {
                pi.PostId,
                pi.ImageUrl
            });
            builder.Entity<Post>()
                .HasOne(a => a.PostType)
                .WithMany(a => a.Posts)
                .HasForeignKey(a => a.TypeId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<Post>()
               .HasOne(a => a.CreatedBy)
               .WithMany(a => a.CreatedPosts)
               .HasForeignKey(a => a.CreatedById)
               .OnDelete(DeleteBehavior.SetNull);

            builder.Entity<Post>()
            .HasOne(a => a.UpdatedBy)
            .WithMany(a => a.UpdatedPosts)
            .HasForeignKey(a => a.UpdatedById);
        }




        public override int SaveChanges()
        {
            UpdateAuditEntities();
            return base.SaveChanges();
        }


        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            UpdateAuditEntities();
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }


        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            UpdateAuditEntities();
            return base.SaveChangesAsync(cancellationToken);
        }


        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default(CancellationToken))
        {
            UpdateAuditEntities();
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }


        private void UpdateAuditEntities()
        {
            var modifiedEntries = ChangeTracker.Entries()
                .Where(x => x.Entity is IAuditableEntity && (x.State == EntityState.Added || x.State == EntityState.Modified));


            foreach (var entry in modifiedEntries)
            {
                var entity = (IAuditableEntity)entry.Entity;
                DateTime now = DateTime.UtcNow;

                if (entry.State == EntityState.Added)
                {
                    entity.CreatedDate = now;
                    entity.CreatedById = CurrentUserId;
                }
                else
                {
                    base.Entry(entity).Property(x => x.CreatedById).IsModified = false;
                    base.Entry(entity).Property(x => x.CreatedDate).IsModified = false;
                }

                entity.UpdatedDate = now;
                entity.UpdatedById = CurrentUserId;
            }
        }
    }
}
