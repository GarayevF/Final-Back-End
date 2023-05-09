using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Smartelectronics.Models;
using System.Reflection.Metadata;

namespace Smartelectronics.DataAccessLayer
{
    public class AppDbContext : IdentityDbContext<AppUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Setting> Settings { get; set; }
        public DbSet<Slider> Sliders { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Brand> Brands { get; set; }
        public DbSet<CategoryBrand> CategoryBrands { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }
        public DbSet<Basket> Baskets { get; set; }
        public DbSet<Wishlist> Wishlists { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Specification> Specifications { get; set; }
        public DbSet<CategorySpecification> CategorySpecifications { get; set; }
        public DbSet<ProductCategorySpecification> ProductCategorySpecifications { get; set; }
        public DbSet<SpecificationGroup> SpecificationGroups { get; set; }
        public DbSet<LoanRange> LoanRanges { get; set; }
        public DbSet<LoanCompany> LoanCompanies { get; set; }
        public DbSet<LoanTerm> LoanTerms { get; set; }
        public DbSet<LoanTermLoanRange> LoanTermLoanRanges { get; set; }
        public DbSet<ProductIFLoanRange> ProductIFLoanRanges { get; set; }
        public DbSet<ProductLoanRange> ProductLoanRanges { get; set; }
        public DbSet<Color> Colors { get; set; }
        public DbSet<ProductColor> ProductColors { get; set; }
        
    }
}
