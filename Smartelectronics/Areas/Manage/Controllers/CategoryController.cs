using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Smartelectronics.DataAccessLayer;
using Smartelectronics.Models;
using Smartelectronics.ViewModels;

namespace Smartelectronics.Areas.Manage.Controllers
{
    [Area("Manage")]
    [Authorize(Roles = "SuperAdmin, Admin")]
    public class CategoryController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public CategoryController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        public async Task<IActionResult> Index(int pageIndex = 1)
        {
            IQueryable<Category> query = _context.Categories
                .Include(c => c.Products.Where(p => p.IsDeleted == false))
                .Where(p => p.IsDeleted == false && p.IsMain)
                .OrderByDescending(c => c.Id);

            //ViewBag.TotalCount = (int)Math.Ceiling((decimal)categories.Count() / 3);
            //ViewBag.PageIndex = pageIndex;

            return View(PageNatedList<Category>.Create(query, pageIndex, 3, 8));
        }

        [HttpGet]
        public async Task<IActionResult> Detail(int? id)
        {
            if (id == null) return BadRequest();

            Category category = await _context.Categories
                .Include(c => c.Children.Where(ca => ca.IsDeleted == false)).ThenInclude(c => c.Products.Where(ctb => ctb.IsDeleted == false)).ThenInclude(e => e.Brand)
                .Include(c => c.Products.Where(p => p.IsDeleted == false))
                .FirstOrDefaultAsync(c => c.IsDeleted == false && c.Id == id);

            if (category == null) return NotFound();

            return View(category);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            ViewBag.MainCategories = await _context.Categories.Where(c => c.IsDeleted == false && c.IsMain).ToListAsync();
            ViewBag.Specifications = await _context.Specifications.Where(c => c.IsDeleted == false).ToListAsync();

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Category category)
        {
            ViewBag.MainCategories = await _context.Categories.Where(c => c.IsDeleted == false && c.IsMain).ToListAsync();
            ViewBag.Specifications = await _context.Specifications.Where(c => c.IsDeleted == false).ToListAsync();

            if (!ModelState.IsValid) return View();

            if (await _context.Categories.AnyAsync(c => c.IsDeleted == false && c.Name.ToLower() == category.Name.Trim().ToLower()))
            {
                ModelState.AddModelError("Name", $"Bu adda {category.Name} category movcuddu");
                return View(category);
            }

            if (category.IsMain)
            {
                category.ParentId = null;
            }
            else
            {
                if (category.ParentId == null)
                {
                    ModelState.AddModelError("ParentId", "Parent mutleq secilmelidir");
                    return View(category);
                }

                if (!await _context.Categories.AnyAsync(c => c.IsDeleted == false && c.Id == category.ParentId && c.IsMain))
                {
                    ModelState.AddModelError("ParentId", "Parent mutleq secilmelidir");
                    return View(category);
                }

                if (category.SpecificationIds != null && category.SpecificationIds.Count() > 0)
                {
                    List<CategorySpecification> categorySpecifications = new List<CategorySpecification>();

                    foreach (int specificationId in category.SpecificationIds)
                    {
                        if (!await _context.Specifications.AnyAsync(c => c.IsDeleted == false && c.Id == specificationId))
                        {
                            ModelState.AddModelError("SpecificationIds", $"{specificationId} id deyeri yanlisdir");
                            return View(category);
                        }

                        CategorySpecification categorySpecification = new CategorySpecification
                        {
                            SpecificationId = specificationId,
                            CategoryId = category.Id,
                            CreatedAt = DateTime.UtcNow.AddHours(4),
                            CreatedBy = "System"
                        };

                        categorySpecifications.Add(categorySpecification);
                    }

                    category.CategorySpecifications = categorySpecifications;
                }

            }

            category.Name = category.Name.Trim();
            category.CreatedAt = DateTime.UtcNow.AddHours(4);
            category.CreatedBy = "System";

            await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Update(int? id)
        {

            if (id == null) return BadRequest();

            Category category = await _context.Categories
                .Include(c => c.CategorySpecifications.Where(pt => pt.IsDeleted == false))
                .FirstOrDefaultAsync(c => c.Id == id && c.IsDeleted == false);

            if (category == null) return NotFound();

            ViewBag.MainCategories = await _context.Categories.Where(c => c.IsDeleted == false && c.IsMain).ToListAsync();
            ViewBag.Specifications = await _context.Specifications.Where(c => c.IsDeleted == false).ToListAsync();

            category.SpecificationIds = category.CategorySpecifications?.Select(x => x.SpecificationId);

            return View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int? id, Category category)
        {
            ViewBag.MainCategories = await _context.Categories.Where(c => c.IsDeleted == false && c.IsMain).ToListAsync();
            ViewBag.Specifications = await _context.Specifications.Where(c => c.IsDeleted == false).ToListAsync();

            if (!ModelState.IsValid) return View(category);

            if (id == null) return BadRequest();

            if (id != category.Id) return BadRequest();

            Category dbCategory = await _context.Categories
                .Include(c => c.CategorySpecifications.Where(pt => pt.IsDeleted == false))
                .FirstOrDefaultAsync(c => c.Id == id && c.IsDeleted == false);

            if (category == null) return NotFound();

            if (await _context.Categories.AnyAsync(c => c.IsDeleted == false && c.Name.ToLower() == category.Name.Trim().ToLower() && c.Id != category.Id))
            {
                ModelState.AddModelError("Name", $"Bu adda {category.Name} category movcuddur");
                return View(category);
            }

            if (dbCategory.IsMain != category.IsMain)
            {
                ModelState.AddModelError("IsMain", "Categorynin veziyyeti deyisdirile bilmez");
                return View(dbCategory);
            }

            if (!dbCategory.IsMain)
            {
                if (category.ParentId != dbCategory.ParentId)
                {
                    if (category.ParentId == null)
                    {
                        ModelState.AddModelError("ParentId", "Parent mutleq secilmelidir");
                        return View(category);
                    }

                    if (!await _context.Categories.AnyAsync(c => c.IsDeleted == false && c.Id == category.ParentId && c.IsMain))
                    {
                        ModelState.AddModelError("ParentId", "Parent mutleq secilmelidir");
                        return View(category);
                    }

                    dbCategory.ParentId = category.ParentId;
                }

                //if (category.SpecificationIds != null && category.SpecificationIds.Count() > 0)
                //{
                //    //List<CategorySpecification> oldcategorySpecifications = category.CategorySpecifications;
                //    //List<CategorySpecification> categorySpecifications = dbCategory.CategorySpecifications;

                //    List<CategorySpecification> oldcategorySpecifications = new List<CategorySpecification>();
                //    List<CategorySpecification> categorySpecifications = new List<CategorySpecification>();

                //    if (category.SpecificationIds != null && category.SpecificationIds.Count() > 0)
                //    {
                //        foreach (int specificationId in category.SpecificationIds)
                //        {
                //            if (await _context.Specifications.AnyAsync(c => c.IsDeleted == false && c.Id == specificationId))
                //            {
                //                CategorySpecification categorySpecification = new CategorySpecification
                //                {
                //                    SpecificationId = specificationId,
                //                    CategoryId = category.Id,
                //                    CreatedAt = DateTime.UtcNow.AddHours(4),
                //                    CreatedBy = "System"
                //                };

                //                categorySpecifications.Add(categorySpecification);
                //            }
                //            else
                //            {
                //                ModelState.AddModelError("SpecificationIds", $"{specificationId} id deyeri yanlisdir");
                //                return View(category);
                //            }
                //        }
                //    }

                //    if (dbCategory.SpecificationIds != null && dbCategory.SpecificationIds.Count() > 0)
                //    {
                //        foreach (int specificationId in dbCategory.SpecificationIds)
                //        {
                //            if (await _context.Specifications.AnyAsync(c => c.IsDeleted == false && c.Id == specificationId))
                //            {
                //                CategorySpecification categorySpecification = new CategorySpecification
                //                {
                //                    SpecificationId = specificationId,
                //                    CategoryId = dbCategory.Id,
                //                    CreatedAt = DateTime.UtcNow.AddHours(4),
                //                    CreatedBy = "System"
                //                };

                //                oldcategorySpecifications.Add(categorySpecification);
                //            }
                            
                //        }
                //    }



                //    foreach (int specificationId in category.SpecificationIds)
                //    {

                //        if (categorySpecifications != null && categorySpecifications.Count() > 0 && categorySpecifications.Any(c => c.SpecificationId == specificationId) &&
                //            (oldcategorySpecifications != null && oldcategorySpecifications.Count() > 0 && !oldcategorySpecifications.Any(c => c.SpecificationId == specificationId) ||
                //            oldcategorySpecifications != null || oldcategorySpecifications.Count() == 0)
                //            )
                //        {
                //            CategorySpecification categorySpecification = new CategorySpecification
                //            {
                //                SpecificationId = specificationId,
                //                CategoryId = category.Id,
                //                CreatedAt = DateTime.UtcNow.AddHours(4),
                //                CreatedBy = "System"
                //            };

                //            categorySpecifications.Add(categorySpecification);

                //            //IEnumerable<ProductCategorySpecification> temp = await _context.ProductCategorySpecifications
                //            //    .Where(a => a.IsDeleted == false && a.CategorySpecification.CategoryId == category.Id).ToListAsync();

                //            //if(temp != null && temp.Count() > 0)
                //            //{
                //            //    foreach (ProductCategorySpecification item in temp)
                //            //    {
                //            //        int? tempId = item.ProductId;
                //            //        Product product = await _context.Products.FirstOrDefaultAsync(p => p.IsDeleted == false && p.Id == tempId);

                //            //        ProductCategorySpecification tempProduct = new ProductCategorySpecification
                //            //        {
                //            //            ProductId = tempId,
                //            //            CategorySpecification = categorySpecification,
                //            //        };

                //            //        if(product != null) product.ProductCategorySpecifications.Add(tempProduct);
                //            //    }
                //            //}

                //        }
                //        else if(categorySpecifications != null && categorySpecifications.Count() > 0 && !categorySpecifications.Any(c => c.SpecificationId == specificationId) &&
                //            (oldcategorySpecifications != null && oldcategorySpecifications.Count() > 0 && !oldcategorySpecifications.Any(c => c.SpecificationId == specificationId) ||
                //            oldcategorySpecifications != null || oldcategorySpecifications.Count() == 0)
                //            )
                //        {
                //            //IEnumerable<ProductCategorySpecification> temp = await _context.ProductCategorySpecifications
                //            //    .Where(a => a.CategorySpecification.SpecificationId == specificationId && a.CategorySpecification.CategoryId == category.Id).ToListAsync();

                //            //foreach (ProductCategorySpecification item in temp)
                //            //{
                //            //    item.CategorySpecification = null;
                //            //    item.CategorySpecificationId = null;
                //            //}

                //            categorySpecifications.Remove(oldcategorySpecifications.FirstOrDefault(c => c.SpecificationId == specificationId));
                //        }
                //    }
                //    dbCategory.CategorySpecifications = categorySpecifications;
                //}



                if (category.SpecificationIds != null && category.SpecificationIds.Count() > 0)
                {
                    //category.CategorySpecifications.RemoveRange(dbCategory.CategorySpecifications);

                    List<CategorySpecification> categorySpecifications = new List<CategorySpecification>();

                    foreach (int specificationId in category.SpecificationIds)
                    {
                        if (!dbCategory.CategorySpecifications.Any(a => a.SpecificationId == specificationId))
                        {
                            if (!await _context.Specifications.AnyAsync(c => c.IsDeleted == false && c.Id == specificationId))
                            {
                                ModelState.AddModelError("SpecificationIds", $"{specificationId} id deyeri yanlisdir");
                                return View(category);
                            }

                            CategorySpecification categorySpecification = new CategorySpecification
                            {
                                SpecificationId = specificationId,
                                CategoryId = category.Id,
                                CreatedAt = DateTime.UtcNow.AddHours(4),
                                CreatedBy = "System"
                            };

                            categorySpecifications.Add(categorySpecification);

                            IEnumerable<Product> temp = await _context.Products
                                .Include(p => p.ProductCategorySpecifications.Where(pc => pc.IsDeleted == false))
                                            .Where(a => a.IsDeleted == false && a.CategoryId == category.Id).ToListAsync();

                            if (temp != null && temp.Count() > 0)
                            {
                                foreach (Product item in temp)
                                {

                                    ProductCategorySpecification tempProduct = new ProductCategorySpecification
                                    {
                                        ProductId = item.Id,
                                        CategorySpecification = categorySpecification,
                                        Value = ""
                                    };

                                    if (item != null) item.ProductCategorySpecifications.Add(tempProduct);
                                }
                            }

                        }

                    }

                    dbCategory.CategorySpecifications.AddRange(categorySpecifications);

                }

            }



            dbCategory.Name = category.Name.Trim();
            dbCategory.UpdatedAt = DateTime.UtcNow.AddHours(4);
            dbCategory.UpdatedBy = "System";
            dbCategory.Icon = category.Icon;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return BadRequest();

            Category category = await _context.Categories
                .Include(c => c.Children.Where(ca => ca.IsDeleted == false)).ThenInclude(ch => ch.Products.Where(p => p.IsDeleted == false))
                .Include(c => c.Products.Where(p => p.IsDeleted == false))
                .FirstOrDefaultAsync(c => c.IsDeleted == false && c.Id == id);

            if (category == null) return NotFound();

            return View(category);
        }

        [HttpGet]
        public async Task<IActionResult> DeleteCategory(int? id)
        {
            if (id == null) return BadRequest();

            Category category = await _context.Categories
                .Include(c => c.Children.Where(ca => ca.IsDeleted == false)).ThenInclude(ch => ch.Products.Where(p => p.IsDeleted == false))
                .Include(c => c.Products.Where(p => p.IsDeleted == false))
                .FirstOrDefaultAsync(c => c.IsDeleted == false && c.Id == id);

            if (category == null) return NotFound();

            if (category.Children != null && category.Children.Count() > 0)
            {
                foreach (Category child in category.Children)
                {
                    child.IsDeleted = true;
                    child.DeletedBy = "System";
                    child.DeletedAt = DateTime.UtcNow.AddHours(4);

                    if (child.Products != null && child.Products.Count() > 0)
                    {
                        foreach (Product product in child.Products)
                        {
                            product.CategoryId = null;
                        }
                    }
                }
            }


            if (category.Products != null && category.Products.Count() > 0)
            {
                foreach (Product product in category.Products)
                {
                    product.CategoryId = null;
                }
            }

            category.IsDeleted = true;
            category.DeletedBy = "System";
            category.DeletedAt = DateTime.UtcNow.AddHours(4);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
