using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;

namespace CoffeeShop.Models.EFModels
{
    public partial class AppDbContext : DbContext
    {
        public AppDbContext()
            : base("name=AppDbContext")
        {
        }

        public virtual DbSet<Function> Functions { get; set; }
        public virtual DbSet<GroupFunction> GroupFunctions { get; set; }
        public virtual DbSet<Group> Groups { get; set; }
        public virtual DbSet<GuestBook> GuestBooks { get; set; }
        public virtual DbSet<Member> Members { get; set; }
        public virtual DbSet<MenuCategory> MenuCategories { get; set; }
        public virtual DbSet<Menu> Menus { get; set; }
        public virtual DbSet<OrderItem> OrderItems { get; set; }
        public virtual DbSet<Order> Orders { get; set; }
        public virtual DbSet<RecommendMenu> RecommendMenus { get; set; }
        public virtual DbSet<RecommendPhoto> RecommendPhotos { get; set; }
        public virtual DbSet<UserGroup> UserGroups { get; set; }
        public virtual DbSet<User> Users { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<GuestBook>()
                .Property(e => e.Name)
                .IsFixedLength();

            modelBuilder.Entity<GuestBook>()
                .Property(e => e.Phone)
                .IsUnicode(false);

            modelBuilder.Entity<Member>()
                .Property(e => e.Phone)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<Member>()
                .Property(e => e.Email)
                .IsUnicode(false);

            modelBuilder.Entity<Member>()
                .Property(e => e.ConfirmCode)
                .IsUnicode(false);

            modelBuilder.Entity<Member>()
                .Property(e => e.EncryptedPassword)
                .IsUnicode(false);

            modelBuilder.Entity<MenuCategory>()
                .HasMany(e => e.Menus)
                .WithOptional(e => e.MenuCategory)
                .HasForeignKey(e => e.CategoryID);

            modelBuilder.Entity<User>()
                .Property(e => e.Account)
                .IsUnicode(false);

            modelBuilder.Entity<User>()
                .Property(e => e.Password)
                .IsUnicode(false);

            modelBuilder.Entity<User>()
                .Property(e => e.Phone)
                .IsFixedLength();

            modelBuilder.Entity<User>()
                .Property(e => e.ConfirmCode)
                .IsUnicode(false);
        }
    }
}
