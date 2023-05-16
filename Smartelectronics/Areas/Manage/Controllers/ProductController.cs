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
            ViewBag.Brands = await _context.Brands.Where(c => c.IsDeleted == false).ToListAsync();
            ViewBag.Specifications = await _context.Specifications.Where(a => a.IsDeleted == false).ToListAsync();
            ViewBag.Colors = await _context.Colors.Where(c => c.IsDeleted == false).ToListAsync();
            ViewBag.LaonRanges = await _context.LoanRanges.Where(c => c.IsDeleted == false).ToListAsync();
            ViewBag.LoanCompanies = await _context.LoanCompanies.Where(c => c.IsDeleted == false).ToListAsync();

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductVM productVM)
        {
            ViewBag.MainCategories = await _context.Categories.Where(c => c.IsDeleted == false && c.IsMain == false).ToListAsync();
            ViewBag.Brands = await _context.Brands.Where(c => c.IsDeleted == false).ToListAsync();
            ViewBag.ProductCategorySpecifications = await _context.ProductCategorySpecifications.Where(a => a.IsDeleted == false).ToListAsync();
            ViewBag.Colors = await _context.Colors.Where(c => c.IsDeleted == false).ToListAsync();
            ViewBag.LaonRanges = await _context.LoanRanges.Where(c => c.IsDeleted == false).ToListAsync();
            ViewBag.LoanCompanies = await _context.LoanCompanies.Where(c => c.IsDeleted == false).ToListAsync();

            if (!ModelState.IsValid)
            {
                List<ModelStateError> modelErrors = new List<ModelStateError>();

                foreach (var key in ModelState.Keys)
                {
                    if (ModelState[key]?.Errors.Count > 0)
                    {
                        foreach (var error in ModelState[key].Errors)
                        {
                            var errorMessage = error.ErrorMessage;
                            var inputId = key;

                            ModelStateError modelError = new ModelStateError
                            {
                                InputId = inputId,
                                ErrorMessage = errorMessage
                            };

                            modelErrors.Add(modelError);
                        }
                    }
                }

                ViewBag.ModelErrors = modelErrors;
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

            if (productVM.BrandId == null)
            {
                ModelState.AddModelError("BrandId", "Brand Mutleq secilmelidir");
                return View(productVM);
            }

            if (!await _context.Brands.AnyAsync(c => c.IsDeleted == false && c.Id == productVM.BrandId))
            {
                ModelState.AddModelError("BrandId", "Duzgun category secin");
                return View(productVM);
            }

            CategoryBrand categoryBrand;

            if(await _context.CategoryBrands.AnyAsync(c => c.CategoryId == productVM.CategoryId && c.BrandId == productVM.BrandId))
            {
                categoryBrand = await _context.CategoryBrands.FirstOrDefaultAsync(c => c.CategoryId == productVM.CategoryId && c.BrandId == productVM.BrandId);
            }
            else
            {
                categoryBrand = new CategoryBrand
                {
                    CategoryId = productVM.CategoryId,
                    BrandId = productVM.BrandId,
                    IsDeleted = false,
                    CreatedAt = DateTime.UtcNow.AddHours(4),
                    CreatedBy = "System"
                };
            }

            productVM.Product.CategoryBrand = categoryBrand;

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
                        CategorySpecificationId = categorySpecification?.Id,
                        Value = specificationVM.Value,
                        IsDeleted = false,
                        CreatedAt = DateTime.UtcNow.AddHours(4),
                        CreatedBy = "System"

                    };

                    productCategorySpecifications.Add(productCategorySpecification);
                }

                productVM.Product.ProductCategorySpecifications = productCategorySpecifications;

            }

            if (productVM.ColorImageVMs != null && productVM.ColorImageVMs.Count() > 0)
            {
                List<ProductColor> productColors = new List<ProductColor>();

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
                        if (colorImageVM.Files != null && colorImageVM.Files.Count() > 6)
                        {
                            ModelState.AddModelError($"ColorImageVMs[{count}].Files", "Her reng ucun maksimum 6 sekil yukleye bilersiniz");
                            return View(productVM);
                        }

                        

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
                                IsDeleted = false,
                                CreatedAt = DateTime.UtcNow.AddHours(4),
                                CreatedBy = "System"
                            };

                            productColors.Add(productColor);
                        }

                        
                    }
                    else
                    {
                        ModelState.AddModelError($"ColorImageVMs[{count}].Files", "Sekil mutleq secilmelidir");
                        return View(productVM);
                    }
                    count++;
                }

                productVM.Product.ProductColors = productColors;
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
                                LoanTermId = loanTerm.Id,
                                IsDeleted = false,
                                CreatedAt = DateTime.UtcNow.AddHours(4),
                                CreatedBy = "System"
                            };

                            loanTermLoanRanges.Add(termLoanRange);
                        }

                        loanTerm.LoanTermLoanRanges = loanTermLoanRanges;
                    }

                    count++;

                    loanTerms.Add(loanTerm);
                }

                productVM.Product.LoanTerms = loanTerms;

            }

            if (productVM.IFLoanVMs != null && productVM.IFLoanVMs.Count() > 0)
            {
                List <ProductIFLoanRange> ifLoanRanges = new List<ProductIFLoanRange>();

                foreach (IFLoanVM ifloanVM in productVM.IFLoanVMs)
                {
                    int count = 0;

                    if (ifloanVM.LoanRangeId == null)
                    {
                        ModelState.AddModelError("IFLoanRangeIds", $"{ifloanVM.LoanRangeId} id deyeri secilmelidir");
                        return View(productVM);
                    }

                    LoanRange loanRange = await _context.LoanRanges.FirstOrDefaultAsync(c => c.IsDeleted == false && c.Id == ifloanVM.LoanRangeId);

                    if (!await _context.LoanRanges.AnyAsync(c => c.IsDeleted == false && c.Id == ifloanVM.LoanRangeId))
                    {
                        ModelState.AddModelError("IFLoanRangeIds", $"{ifloanVM.LoanRangeId} id deyeri yanlisdir");
                        return View(productVM);
                    }

                    if (ifloanVM.InitialPayment + (ifloanVM.MonthlyPayment * loanRange.Range) != ifloanVM.TotalPayment)
                    {
                        ModelState.AddModelError($"IFLoanVMs[{count}].TotalPayment", "Hesablama yanlisdir");
                        return View(productVM);
                    }

                    ProductIFLoanRange productIFLoanRange = new ProductIFLoanRange
                    {
                        LoanRangeId = ifloanVM.LoanRangeId,
                        InitialPayment = ifloanVM.InitialPayment,
                        MonthlyPayment = ifloanVM.MonthlyPayment,
                        TotalPayment = ifloanVM.TotalPayment,
                        IsDeleted = false,
                        CreatedAt = DateTime.UtcNow.AddHours(4),
                        CreatedBy = "System"
                    };

                    ifLoanRanges.Add(productIFLoanRange);

                    count++;
                }
                productVM.Product.ProductIFLoanRanges = ifLoanRanges;
            }

            if (productVM.LoanVMs != null && productVM.LoanVMs.Count() > 0)
            {
                List<ProductLoanRange> loanRanges = new List<ProductLoanRange>();

                foreach (LoanVM loanVM in productVM.LoanVMs)
                {
                    int count = 0;

                    if (loanVM.LoanRangeId == null)
                    {
                        ModelState.AddModelError("LoanRangeIds", $"{loanVM.LoanRangeId} id deyeri secilmelidir");
                        return View(productVM);
                    }

                    LoanRange loanRange = await _context.LoanRanges.FirstOrDefaultAsync(c => c.IsDeleted == false && c.Id == loanVM.LoanRangeId);

                    if (!await _context.LoanRanges.AnyAsync(c => c.IsDeleted == false && c.Id == loanVM.LoanRangeId))
                    {
                        ModelState.AddModelError("LoanRangeIds", $"{loanVM.LoanRangeId} id deyeri yanlisdir");
                        return View(productVM);
                    }

                    if (loanVM.InterestForVipUsers > 100 || loanVM.InterestForVipUsers < 0)
                    {
                        ModelState.AddModelError("LoanVMs[@count].InterestForVipUsers", "Faiz deyeri yanlisdir");
                        return View(productVM);
                    }

                    if (loanVM.InterestForStandartUsers > 100 || loanVM.InterestForStandartUsers < 0)
                    {
                        ModelState.AddModelError("LoanVMs[@count].InterestForStandartUsers", "Faiz deyeri yanlisdir");
                        return View(productVM);
                    }

                    ProductLoanRange productLoanRange = new ProductLoanRange
                    {
                        LoanRangeId = loanVM.LoanRangeId,
                        InterestForVipUsers = loanVM.InterestForVipUsers,
                        InterestForStandartUsers = loanVM.InterestForStandartUsers,
                        IsDeleted = false,
                        CreatedAt = DateTime.UtcNow.AddHours(4),
                        CreatedBy = "System"
                    };

                    loanRanges.Add(productLoanRange);

                    count++;
                }
                productVM.Product.ProductLoanRanges = loanRanges;
            }

            string seria = _context.Categories.FirstOrDefault(c => c.Id == productVM.Product.CategoryBrand.CategoryId).Name.Substring(0, 2);
            seria += _context.Brands.FirstOrDefault(c => c.Id == productVM.Product.CategoryBrand.BrandId).Name.Substring(0, 2);
            seria = seria.ToLower();

            int code = _context.Products.Where(p => p.Seria == seria).OrderByDescending(p => p.Id).FirstOrDefault() != null ?
                (int)_context.Products.Where(p => p.Seria == seria).OrderByDescending(p => p.Id).FirstOrDefault().Code + 1 : 1;

            productVM.Product.Seria = seria;
            productVM.Product.Code = code;
            productVM.Product.IsDeleted = false;
            productVM.Product.CreatedAt = DateTime.UtcNow.AddHours(4);
            productVM.Product.CreatedBy = "System";

            await _context.Products.AddAsync(productVM.Product);
            await _context.SaveChangesAsync();

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
