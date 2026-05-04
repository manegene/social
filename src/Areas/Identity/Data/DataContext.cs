using Kmums.Models.Category;
using Kmums.Models.Contact;
using Kmums.Models.Store;
using Kmums.Models.User;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Kmums.Models.LinkedModels;

namespace Kmums.Areas.Identity.Data;

public class DataContext : IdentityDbContext<UserModel, RolesModel, Guid>
{
    public DataContext(DbContextOptions<DataContext> options)
        : base(options)
    {
    }
    public DbSet<AboutUsModel> About { get; set; }
    public DbSet<PrivacyUsModel> Privacy { get; set; }
    public DbSet<CategoryModel> Category { get; set; }
    public DbSet<HomeModel> Home { get; set; }
    public DbSet<UserPublicModel> PublicProfile { get; set; }
    public DbSet<ContactModel> ContactQueue { get; set; }
    public DbSet<UserImageModel> Images { get; set; }
    public DbSet<Subscription> Subscriptions { get; set; }
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.Entity<UserModel>().ToTable("Customers");
        builder.Entity<RolesModel>().ToTable("CustomerRole");
        builder.Entity<IdentityUserRole<Guid>>(b => b.ToTable("CustomerRoleMappings"));
        builder.Entity<IdentityUserLogin<Guid>>(ext =>
        {
            ext.ToTable("ExternalLoginMappings");
        });
    }

}
