using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Smartelectronics.Areas.Manage.ViewModels.ProductViewModels;
using Smartelectronics.DataAccessLayer;
using Smartelectronics.Extensions;
using Smartelectronics.Models;
using System.Data;

namespace Smartelectronics.Areas.Manage.Controllers
{
    [Area("Manage")]
    public class ProductController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public ProductController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            ViewBag.MainCategories = await _context.Categories.Where(c => c.IsDeleted == false && c.IsMain == false).ToListAsync();
            ViewBag.Specifications = await _context.Specifications.Where(a => a.IsDeleted == false).ToListAsync();
            ViewBag.Colors = await _context.Colors.Where(c => c.IsDeleted == false).ToListAsync();
            ViewBag.LaonRanges = await _context.LoanRanges.Where(c => c.IsDeleted == false).ToListAsync();
            ViewBag.LoanCompanies = await _context.LoanCompanies.Where(c => c.IsDeleted == false).ToListAsync();

            return View();
        }

        [HttpPost]
        public IActionResult GetMesssage([FromBody] List<UserModel> listofusers)
        {
            List<UserModel> test = listofusers;
            return View();
        }

        public class UserModel
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductVM productVM)
        {
            ViewBag.MainCategories = await _context.Categories.Where(c => c.IsDeleted == false && c.IsMain == false).ToListAsync();
            ViewBag.ProductCategorySpecifications = await _context.ProductCategorySpecifications.Where(a => a.IsDeleted == false).ToListAsync();
            ViewBag.Colors = await _context.Colors.Where(c => c.IsDeleted == false).ToListAsync();
            ViewBag.LaonRanges = await _context.LoanRanges.Where(c => c.IsDeleted == false).ToListAsync();
            ViewBag.LoanCompanies = await _context.LoanCompanies.Where(c => c.IsDeleted == false).ToListAsync();

            if (!ModelState.IsValid)
            {
                ViewBag.ErrorMessages = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();
                return View(productVM);
            }

            if (productVM.CategoryId == null)
            {
                ModelState.AddModelError("CategoryId", "Category Mutleq secilmelidir");
                return View(productVM);
            }

            if (!await _context.Categories.AnyAsync(c => c.IsDeleted == false && c.Id == productVM.CategoryId))
            {
                ModelState.AddModelError("CategoryId", "Duzgun category secin");
                return View(productVM);
            }

            if (productVM.SpecificationVMs != null && productVM.SpecificationVMs.Count() > 0)
            {
                List<ProductCategorySpecification> productCategorySpecifications = new List<ProductCategorySpecification>();

                foreach (SpecificationVM specificationVM in productVM.SpecificationVMs)
                {
                    if (!await _context.Specifications.AnyAsync(c => c.IsDeleted == false && specificationVM.SpecificationId == c.Id))
                    {
                        ModelState.AddModelError($"Specifications[{specificationVM.SpecificationId}]", $"{specificationVM.SpecificationId} id deyeri yanlisdir");
                        return View(productVM);
                    }

                    Specification specification = await _context.Specifications.Where(s => s.IsDeleted == false)
                        .Include(s => s.SpecificationGroup).FirstOrDefaultAsync(a => a.Id == specificationVM.SpecificationId);

                    CategorySpecification categorySpecification = await _context.CategorySpecifications.Where(c => c.IsDeleted == false)
                        .FirstOrDefaultAsync(c => c.CategoryId == productVM.CategoryId && c.SpecificationId == specificationVM.SpecificationId);

                    //CategorySpecification categorySpecification = new CategorySpecification
                    //{
                    //    //Specification = specification,
                    //    SpecificationId = specificationVM.SpecificationId,
                    //    CategoryId = productVM.CategoryId,
                    //    CreatedAt = DateTime.UtcNow.AddHours(4),
                    //    CreatedBy = "System"

                    //};

                    ProductCategorySpecification productCategorySpecification = new ProductCategorySpecification
                    {
                        ProductId = productVM.Product.Id,
                        CategorySpecificationId = categorySpecification.Id,
                        Value = specificationVM.Value,

                    };

                    productCategorySpecifications.Add(productCategorySpecification);
                }

                productVM.Product.ProductCategorySpecifications = productCategorySpecifications;

            }

            if (productVM.ColorImageVMs != null && productVM.ColorImageVMs.Count() > 0)
            {

                foreach (ColorImageVM colorImageVM in productVM.ColorImageVMs)
                {
                    int count = 0;

                    if (!await _context.Colors.AnyAsync(c => c.IsDeleted == false && c.Id == colorImageVM.ColorId))
                    {
                        ModelState.AddModelError("ColorIds", $"{colorImageVM.ColorId} id deyeri yanlisdir");
                        return View(productVM);
                    }

                    if (colorImageVM.Files != null && colorImageVM.Files.Count() > 0)
                    {
                        List<ProductColor> productColors = new List<ProductColor>();

                        foreach (IFormFile file in colorImageVM.Files)
                        {
                            if (file.CheckFileContenttype("image/jpeg"))
                            {
                                ModelState.AddModelError($"ColorImageVMs[${count}].Files", $"{file.FileName} adli fayl novu duzgun deyil");
                                return View(productVM);
                            }

                            if (file.CheckFileLength(1024))
                            {
                                ModelState.AddModelError($"ColorImageVMs[${count}].Files", $"{file.FileName} adli fayl hecmi coxdur");
                                return View(productVM);
                            }

                            ProductColor productColor = new ProductColor
                            {
                                Image = await file.CreateFileAsync(_env, "assets", "images", "products"),
                                ColorId = colorImageVM.ColorId,
                                CreatedAt = DateTime.UtcNow.AddHours(4),
                                CreatedBy = "System"
                            };

                            productColors.Add(productColor);
                        }

                        productVM.Product.ProductColors = productColors;
                    }
                    else
                    {
                        ModelState.AddModelError($"ColorImageVMs[{count}].Files", "Sekil mutleq secilmelidir");
                        return View(productVM);
                    }
                    count++;
                }
            }

            if (productVM.LoanTermVMs != null && productVM.LoanTermVMs.Count() > 0)
            {
                List<LoanTerm> loanTerms = new List<LoanTerm>();

                foreach (LoanTermVM loanTermVM in productVM.LoanTermVMs)
                {

                    int count = 0;

                    if (loanTermVM.LoanCompanyId == null)
                    {
                        ModelState.AddModelError("LoanCompanyIds", $"{loanTermVM.LoanCompanyId} id deyeri secilmelidir");
                        return View(productVM);
                    }

                    LoanCompany loanCompany = await _context.LoanCompanies.FirstOrDefaultAsync(c => c.IsDeleted == false && c.Id == loanTermVM.LoanCompanyId);

                    if (!await _context.LoanCompanies.AnyAsync(c => c.IsDeleted == false && c.Id == loanTermVM.LoanCompanyId))
                    {
                        ModelState.AddModelError("LoanCompanyIds", $"{loanTermVM.LoanCompanyId} id deyeri yanlisdir");
                        return View(productVM);
                    }

                    LoanTerm loanTerm = new LoanTerm
                    {
                        ProductId = productVM.Product.Id,
                        LoanCompanyId = loanCompany.Id,
                        Title = loanTermVM.Title,
                        CreatedAt = DateTime.UtcNow.AddHours(4),
                        CreatedBy = "system",
                    };

                    if (loanTermVM.LoanRangeIds != null && loanTermVM.LoanRangeIds.Count() > 0)
                    {
                        List<LoanRange> loanRanges = new List<LoanRange>();
                        List<LoanTermLoanRange> loanTermLoanRanges = new List<LoanTermLoanRange>();

                        foreach (int loanRangeId in loanTermVM.LoanRangeIds)
                        {

                            if (loanRangeId == null)
                            {
                                ModelState.AddModelError($"LoanTermVMs[{count}].loanCompany", "id deyeri yanlisdir");
                                return View(productVM);
                            }

                            if (!await _context.LoanRanges.AnyAsync(c => c.IsDeleted == false && c.Id == loanRangeId))
                            {
                                ModelState.AddModelError($"LoanTermVMs[{count}].loanCompany", "id deyeri yanlisdir");
                                return View(productVM);
                            }

                            LoanRange loanRange = await _context.LoanRanges.FirstOrDefaultAsync(lt => lt.Id == loanRangeId && lt.IsDeleted == false);

                            LoanTermLoanRange termLoanRange = new LoanTermLoanRange
                            {
                                LoanRangeId = loanRangeId,
                                LoanTermId = loanTerm.Id
                            };

                            //loanRanges.Add(loanRange);
                            loanTermLoanRanges.Add(termLoanRange);
                        }

                        loanTerm.LoanTermLoanRanges = loanTermLoanRanges;
                    }

                    count++;

                    loanTerms.Add(loanTerm);
                }

                productVM.Product.LoanTerms = loanTerms;

            }

            //if (product.IFLoanRangeIds != null && product.IFLoanRangeIds.Count() > 0)
            //{
            //    List<ProductIFLoanRange> productIFLoanRanges = new List<ProductIFLoanRange>();

            //    foreach (int loanRangeId in product.IFLoanRangeIds)
            //    {
            //        if (!await _context.LoanRanges.AnyAsync(c => c.IsDeleted == false && c.Id == loanRangeId))
            //        {
            //            ModelState.AddModelError("IFLoanRangeIds", $"{loanRangeId} id deyeri yanlisdir");
            //            return View(product);
            //        }

            //        ProductIFLoanRange productLoanRange = new ProductIFLoanRange
            //        {
            //            LoanRangeId = loanRangeId,
            //            ProductId = product.Id,
            //            CreatedAt = DateTime.UtcNow.AddHours(4),
            //            CreatedBy = "System"
            //        };

            //        productIFLoanRanges.Add(productLoanRange);
            //    }

            //    product.ProductIFLoanRanges = productIFLoanRanges;

            //}

            //if (product.LoanRangeIds != null && product.LoanRangeIds.Count() > 0)
            //{
            //    List<ProductLoanRange> productLoanRanges = new List<ProductLoanRange>();

            //    foreach (int loanRangeId in product.LoanRangeIds)
            //    {
            //        if (!await _context.LoanRanges.AnyAsync(c => c.IsDeleted == false && c.Id == loanRangeId))
            //        {
            //            ModelState.AddModelError("LoanRangeIds", $"{loanRangeId} id deyeri yanlisdir");
            //            return View(product);
            //        }

            //        ProductLoanRange productLoanRange = new ProductLoanRange
            //        {
            //            LoanRangeId = loanRangeId,
            //            ProductId = product.Id,
            //            CreatedAt = DateTime.UtcNow.AddHours(4),
            //            CreatedBy = "System"
            //        };

            //        productLoanRanges.Add(productLoanRange);
            //    }

            //    product.ProductLoanRanges = productLoanRanges;

            //}

            //string seria = _context.Categories.FirstOrDefault(c => c.Id == product.CategoryBrand.CategoryId).Name.Substring(0, 2);
            //seria += _context.Categories.FirstOrDefault(c => c.Id == product.CategoryBrand.CategoryId).Name.Substring(0, 2);
            //seria = seria.ToLower();

            //int code = _context.Products.Where(p => p.Seria == seria).OrderByDescending(p => p.Id).FirstOrDefault() != null ?
            //    (int)_context.Products.Where(p => p.Seria == seria).OrderByDescending(p => p.Id).FirstOrDefault().Code + 1 : 1;

            //product.Seria = seria;
            //product.Code = code;
            //productVM.Product.CreatedAt = DateTime.UtcNow.AddHours(4);
            //productVM.Product.CreatedBy = "System";

            //await _context.Products.AddAsync(productVM.Product);
            //await _context.SaveChangesAsync();

            return View();
        }

        public async Task<JsonResult> GetSpecifications(int? id)
        {
            List<Specification> specifications = await _context.CategorySpecifications
                .Where(cs => cs.IsDeleted == false && cs.CategoryId == id)
                .Include(cs => cs.Specification)
                .Select(cs => cs.Specification)
                .ToListAsync();

            return Json(specifications);
        }

        public async Task<JsonResult> GetLoanRanges()
        {
            List<LoanRange> loanRanges = await _context.LoanRanges
                .Where(cs => cs.IsDeleted == false).ToListAsync();

            return Json(loanRanges);
        }


    }
}
