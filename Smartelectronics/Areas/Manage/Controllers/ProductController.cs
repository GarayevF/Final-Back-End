using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Smartelectronics.Areas.Manage.ViewModels.ProductViewModels;
using Smartelectronics.DataAccessLayer;
using Smartelectronics.Extensions;
using Smartelectronics.Helpers;
using Smartelectronics.Models;
using Smartelectronics.ViewModels;
using System.Data;
using System.Drawing;

namespace Smartelectronics.Areas.Manage.Controllers
{
    [Area("Manage")]
    [Authorize(Roles = "SuperAdmin, Admin")]
    public class ProductController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public ProductController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
        public IActionResult Index(int pageIndex = 1)
        {
            IQueryable<Product> queries = _context.Products
                .Include(p => p.ProductColors.Where(a => a.IsDeleted == false))
                .Include(p => p.Category)
                .Where(p => p.IsDeleted == false);

            return View(PageNatedList<Product>.Create(queries, pageIndex, 3, 5));
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

            if (productVM.Product.CategoryId == null)
            {
                ModelState.AddModelError("Product.CategoryId", "Category Mutleq secilmelidir");
                return View(productVM);
            }

            if (!await _context.Categories.AnyAsync(c => c.IsDeleted == false && c.Id == productVM.Product.CategoryId))
            {
                ModelState.AddModelError("Product.CategoryId", "Duzgun category secin");
                return View(productVM);
            }

            if (productVM.Product.BrandId == null)
            {
                ModelState.AddModelError("Product.BrandId", "Brand Mutleq secilmelidir");
                return View(productVM);
            }

            if (!await _context.Brands.AnyAsync(c => c.IsDeleted == false && c.Id == productVM.Product.BrandId))
            {
                ModelState.AddModelError("Product.BrandId", "Duzgun category secin");
                return View(productVM);
            }

            if(productVM.Product.DiscountedPrice > productVM.Product.Price)
            {
                ModelState.AddModelError("Product.DiscountedPrice", "Endirimli qiymət əsl qiymətdən yuxarı ola bilməz");
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
                        .FirstOrDefaultAsync(c => c.CategoryId == productVM.Product.CategoryId && c.SpecificationId == specificationVM.SpecificationId);

                    ProductCategorySpecification productCategorySpecification = new ProductCategorySpecification
                    {
                        ProductId = productVM.Product.Id,
                        CategorySpecificationId = categorySpecification?.Id,
                        Value = specificationVM.Value.Trim(),
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

                    ProductIFLoanRange productIFLoanRange = new ProductIFLoanRange
                    {
                        LoanRangeId = ifloanVM.LoanRangeId,
                        InitialPayment = ifloanVM.InitialPayment,
                        MonthlyPayment = ifloanVM.MonthlyPayment,
                        TotalPayment = ifloanVM.InitialPayment + (ifloanVM.MonthlyPayment * loanRange.Range),
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

            string seria = _context.Categories.FirstOrDefault(c => c.Id == productVM.Product.CategoryId).Name.Substring(0, 2);
            seria += _context.Brands.FirstOrDefault(c => c.Id == productVM.Product.BrandId).Name.Substring(0, 2);
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

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Update(int? id)
        {
            ViewBag.MainCategories = await _context.Categories.Where(c => c.IsDeleted == false && c.IsMain == false).ToListAsync();
            ViewBag.Brands = await _context.Brands.Where(c => c.IsDeleted == false).ToListAsync();
            ViewBag.ProductCategorySpecifications = await _context.ProductCategorySpecifications.Where(a => a.IsDeleted == false).ToListAsync();
            ViewBag.Colors = await _context.Colors.Where(c => c.IsDeleted == false).ToListAsync();
            ViewBag.LaonRanges = await _context.LoanRanges.Where(c => c.IsDeleted == false).ToListAsync();
            ViewBag.LoanCompanies = await _context.LoanCompanies.Where(c => c.IsDeleted == false).ToListAsync();
            ViewBag.ProductColors = await _context.ProductColors.Where(p => p.IsDeleted == false).ToListAsync();

            if (id == null) return BadRequest();

            Product product = await _context.Products
                .Include(cb => cb.Category).ThenInclude(ct => ct.Parent).Where(b => b.IsDeleted == false)
                .Include(cb => cb.Brand)
                .Include(p => p.ProductColors.Where(a => a.IsDeleted == false && a.ProductId == id))
                 .ThenInclude(pa => pa.Color).Where(a => a.IsDeleted == false)
                .Include(p => p.LoanTerms.Where(p => p.IsDeleted == false))
                .ThenInclude(lt => lt.LoanTermLoanRanges).ThenInclude(ltlr => ltlr.LoanRange)
                .Include(p => p.LoanTerms).ThenInclude(lt => lt.LoanCompany).Where(p => p.IsDeleted == false)
                .Include(p => p.ProductCategorySpecifications.Where(p => p.IsDeleted == false && p.ProductId == id))
                .ThenInclude(pcs => pcs.CategorySpecification).ThenInclude(cs => cs.Specification).ThenInclude(s => s.SpecificationGroup)
                .Include(p => p.ProductCategorySpecifications).ThenInclude(pcs => pcs.CategorySpecification)
                .ThenInclude(cs => cs.Category)
                .Include(p => p.ProductLoanRanges.Where(pl => pl.IsDeleted == false)).ThenInclude(plr => plr.LoanRange)
                .Include(p => p.ProductIFLoanRanges.Where(pl => pl.IsDeleted == false)).ThenInclude(plr => plr.LoanRange)
                .FirstOrDefaultAsync(p => p.Id == id && p.IsDeleted == false);

            if (product == null) return NotFound();

            List<ProductCategorySpecification> productCategorySpecifications = await _context.ProductCategorySpecifications
            .Include(pcs => pcs.CategorySpecification).ThenInclude(cs => cs.Specification).ThenInclude(s => s.SpecificationGroup)
            .Where(pcs => pcs.ProductId == id).ToListAsync();

            List<SpecificationVM> specifications = productCategorySpecifications
                .Select(spec => new SpecificationVM
                {
                    SpecificationId = spec.CategorySpecification.Specification.Id,
                    Name = spec.CategorySpecification.Specification.Name,
                    Value = spec.Value
                }).ToList();

            List<LoanTermVM> loanTermVMs = product.LoanTerms.Where(p => p.IsDeleted == false)
                .Select(loan => new LoanTermVM
                {
                    LoanCompanyId = loan.LoanCompanyId,
                    LoanRangeIds = loan.LoanTermLoanRanges.Select(a => a.LoanRange.Id).ToList(),
                    Title = loan.Title
                }).ToList();

            List<IFLoanVM> iFLoanVMs = product.ProductIFLoanRanges.Where(p => p.IsDeleted == false)
                .Select(loan => new IFLoanVM
                {
                    InitialPayment = loan.InitialPayment,
                    MonthlyPayment = loan.MonthlyPayment,
                    LoanRangeId = loan.LoanRangeId,
                }).ToList();

            List<LoanVM> loanVMs = product.ProductLoanRanges.Where(p => p.IsDeleted == false)
                .Select(loan => new LoanVM
                {
                    LoanRangeId = loan.LoanRangeId,
                    InterestForStandartUsers = loan.InterestForStandartUsers,
                    InterestForVipUsers = loan.InterestForVipUsers
                }).ToList();

            List<ColorImageVM> colorImageVMs = product.ProductColors.Where(p => p.IsDeleted == false)
                .Select(color => new ColorImageVM
                {
                    ColorId = (int)color.ColorId,
                    Name = _context.Colors.FirstOrDefault(c => c.IsDeleted == false && c.Id == color.ColorId).Code
                }).ToList();

            ProductVM productVM = new ProductVM
            {
                Product = product,
                SpecificationVMs = specifications,
                LoanTermVMs = loanTermVMs,
                IFLoanVMs = iFLoanVMs,
                LoanVMs = loanVMs,
                ColorIds = product.ProductColors.Where(p => p.IsDeleted == false).Select(p => p.Color.Id).ToList(),
                IFLoanRangeIds = product.ProductIFLoanRanges.Where(p => p.IsDeleted == false).Select(p => p.LoanRangeId).ToList(),
                LoanRangeIds = product.ProductLoanRanges.Where(p => p.IsDeleted == false).Select(p => p.LoanRangeId).ToList(),
                LoanCompanyIds = product.LoanTerms.Select(a => a.LoanCompanyId).ToList(),
                ColorImageVMs = colorImageVMs
            };

            return View(productVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int? id, ProductVM productVM)
        {
            ViewBag.MainCategories = await _context.Categories.Where(c => c.IsDeleted == false && c.IsMain == false).ToListAsync();
            ViewBag.Brands = await _context.Brands.Where(c => c.IsDeleted == false).ToListAsync();
            ViewBag.ProductCategorySpecifications = await _context.ProductCategorySpecifications.Where(a => a.IsDeleted == false).ToListAsync();
            ViewBag.Colors = await _context.Colors.Where(c => c.IsDeleted == false).ToListAsync();
            ViewBag.LaonRanges = await _context.LoanRanges.Where(c => c.IsDeleted == false).ToListAsync();
            ViewBag.LoanCompanies = await _context.LoanCompanies.Where(c => c.IsDeleted == false).ToListAsync();
            ViewBag.ProductColors = await _context.ProductColors.Where(p => p.IsDeleted == false).ToListAsync();

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

            if (id == null) return BadRequest();

            if (id != productVM.Product.Id) return BadRequest();

            Product dbproduct = await _context.Products
                .Include(cb => cb.Category).ThenInclude(ct => ct.Parent).Where(b => b.IsDeleted == false)
                .Include(cb => cb.Brand)
                .Include(p => p.ProductColors.Where(a => a.IsDeleted == false && a.ProductId == id))
                 .ThenInclude(pa => pa.Color).Where(a => a.IsDeleted == false)
                .Include(p => p.LoanTerms.Where(p => p.IsDeleted == false))
                .ThenInclude(lt => lt.LoanTermLoanRanges).ThenInclude(ltlr => ltlr.LoanRange)
                .Include(p => p.LoanTerms).ThenInclude(lt => lt.LoanCompany).Where(p => p.IsDeleted == false)
                .Include(p => p.ProductCategorySpecifications.Where(p => p.IsDeleted == false && p.ProductId == id))
                .ThenInclude(pcs => pcs.CategorySpecification).ThenInclude(cs => cs.Specification).ThenInclude(s => s.SpecificationGroup)
                .Include(p => p.ProductCategorySpecifications).ThenInclude(pcs => pcs.CategorySpecification)
                .ThenInclude(cs => cs.Category)
                .Include(p => p.ProductLoanRanges.Where(pl => pl.IsDeleted == false)).ThenInclude(plr => plr.LoanRange)
                .Include(p => p.ProductIFLoanRanges.Where(pl => pl.IsDeleted == false)).ThenInclude(plr => plr.LoanRange)
                .FirstOrDefaultAsync(p => p.Id == id && p.IsDeleted == false);

            if (dbproduct == null) return NotFound();

            if (productVM.Product.CategoryId == null)
            {
                ModelState.AddModelError("Product.CategoryId", "Category Mutleq secilmelidir");
                return View(productVM);
            }

            if (!await _context.Categories.AnyAsync(c => c.IsDeleted == false && c.Id == productVM.Product.CategoryId))
            {
                ModelState.AddModelError("Product.CategoryId", "Duzgun category secin");
                return View(productVM);
            }

            if (productVM.Product.BrandId == null)
            {
                ModelState.AddModelError("Product.BrandId", "Brand Mutleq secilmelidir");
                return View(productVM);
            }

            if (!await _context.Brands.AnyAsync(c => c.IsDeleted == false && c.Id == productVM.Product.BrandId))
            {
                ModelState.AddModelError("Product.BrandId", "Duzgun brand secin");
                return View(productVM);
            }

            if (productVM.Product.DiscountedPrice > productVM.Product.Price)
            {
                ModelState.AddModelError("Product.DiscountedPrice", "Endirimli qiymət əsl qiymətdən yuxarı ola bilməz");
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
                        .FirstOrDefaultAsync(c => c.CategoryId == productVM.Product.CategoryId && c.SpecificationId == specificationVM.SpecificationId);

                    ////kohneni product id ile tap isdeleted true ele

                    if(dbproduct.ProductCategorySpecifications
                        .FirstOrDefault(p => p.CategorySpecification.Id == categorySpecification.Id).Value != specificationVM.Value.Trim())
                    {
                        

                        dbproduct.ProductCategorySpecifications
                        .FirstOrDefault(p => p.CategorySpecification.Id == categorySpecification.Id).DeletedAt = DateTime.UtcNow.AddHours(4);

                        dbproduct.ProductCategorySpecifications
                        .FirstOrDefault(p => p.CategorySpecification.Id == categorySpecification.Id).DeletedBy = "System";

                        dbproduct.ProductCategorySpecifications
                        .FirstOrDefault(p => p.CategorySpecification.Id == categorySpecification.Id).IsDeleted = true;

                        ProductCategorySpecification productCategorySpecification = new ProductCategorySpecification
                        {
                            ProductId = productVM.Product.Id,
                            CategorySpecificationId = categorySpecification?.Id,
                            Value = specificationVM.Value.Trim(),
                            IsDeleted = false,
                            CreatedAt = DateTime.UtcNow.AddHours(4),
                            CreatedBy = "System"

                        };

                        productCategorySpecifications.Add(productCategorySpecification);
                    }
                    else
                    {
                        productCategorySpecifications.Add(dbproduct.ProductCategorySpecifications
                        .FirstOrDefault(p => p.CategorySpecification.Id == categorySpecification.Id));
                    }
                    
                }

                dbproduct.ProductCategorySpecifications = productCategorySpecifications;

            }

            if (productVM.ColorImageVMs != null && productVM.ColorImageVMs.Count() > 0)
            {
                List<ProductColor> productColors = new List<ProductColor>();

                foreach (ColorImageVM colorImageVM in productVM.ColorImageVMs)
                {
                    int count = 0;

                    int canUpload = 6 - (dbproduct.ProductColors.DistinctBy(p => p.ColorId).Where(p => p.IsDeleted == false) != null ? dbproduct.ProductColors.DistinctBy(p => p.ColorId).Where(p => p.IsDeleted == false).Count() : 0);

                    if (colorImageVM.Files != null && canUpload < colorImageVM.Files.Count())
                    {
                        ModelState.AddModelError("Files", $"Maksimum {canUpload} qeder fayl upload edebilersiniz");
                        return View(dbproduct);
                    }

                    if (!await _context.Colors.AnyAsync(c => c.IsDeleted == false && c.Id == colorImageVM.ColorId))
                    {
                        ModelState.AddModelError("ColorIds", $"{colorImageVM.ColorId} id deyeri yanlisdir");
                        return View(productVM);
                    }

                    if (colorImageVM.Files != null && colorImageVM.Files.Count() > 0)
                    {
                        if (colorImageVM.Files != null && colorImageVM.Files.Count() > canUpload)
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
                    else if (dbproduct.ProductColors.DistinctBy(p => p.ColorId) == null ||
                             dbproduct.ProductColors.DistinctBy(p => p.ColorId).Count(p => p.IsDeleted == false) <= 0)
                    {
                        ModelState.AddModelError($"ColorImageVMs[{count}].Files", "Sekil mutleq secilmelidir");
                        return View(productVM);
                    }
                    count++;
                }

                dbproduct.ProductColors.AddRange(productColors);
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

                    /////kohnenin isdeleted true
                    ///
                    
                    dbproduct.LoanTerms.FirstOrDefault(p => p.IsDeleted == false && p.LoanCompany.Id == loanTermVM.LoanCompanyId).DeletedAt = DateTime.UtcNow.AddHours(4);
                    dbproduct.LoanTerms.FirstOrDefault(p => p.IsDeleted == false && p.LoanCompany.Id == loanTermVM.LoanCompanyId).DeletedBy = "System";
                    dbproduct.LoanTerms.FirstOrDefault(p => p.IsDeleted == false && p.LoanCompany.Id == loanTermVM.LoanCompanyId).IsDeleted = true;

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

                            /////kohnenin isdeleted true

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

                dbproduct.LoanTerms = loanTerms;

            }

            if (productVM.IFLoanVMs != null && productVM.IFLoanVMs.Count() > 0)
            {
                List<ProductIFLoanRange> ifLoanRanges = new List<ProductIFLoanRange>();

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

                    /////kohnenin isdeleted true
                    ///
                    dbproduct.ProductIFLoanRanges.FirstOrDefault(p => p.IsDeleted == false && p.LoanRange.Id == ifloanVM.LoanRangeId).DeletedAt = DateTime.UtcNow.AddHours(4);
                    dbproduct.ProductIFLoanRanges.FirstOrDefault(p => p.IsDeleted == false && p.LoanRange.Id == ifloanVM.LoanRangeId).DeletedBy = "System";
                    dbproduct.ProductIFLoanRanges.FirstOrDefault(p => p.IsDeleted == false && p.LoanRange.Id == ifloanVM.LoanRangeId).IsDeleted = true;

                    ProductIFLoanRange productIFLoanRange = new ProductIFLoanRange
                    {
                        LoanRangeId = ifloanVM.LoanRangeId,
                        InitialPayment = ifloanVM.InitialPayment,
                        MonthlyPayment = ifloanVM.MonthlyPayment,
                        TotalPayment = ifloanVM.InitialPayment + (ifloanVM.MonthlyPayment * loanRange.Range),
                        IsDeleted = false,
                        CreatedAt = DateTime.UtcNow.AddHours(4),
                        CreatedBy = "System"
                    };

                    ifLoanRanges.Add(productIFLoanRange);

                    count++;
                }
                dbproduct.ProductIFLoanRanges = ifLoanRanges;
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

                    /////kohnenin isdeleted true
                    ///
                    dbproduct.ProductLoanRanges.FirstOrDefault(p => p.IsDeleted == false && p.LoanRange.Id == loanVM.LoanRangeId).DeletedAt = DateTime.UtcNow.AddHours(4);
                    dbproduct.ProductLoanRanges.FirstOrDefault(p => p.IsDeleted == false && p.LoanRange.Id == loanVM.LoanRangeId).DeletedBy = "System";
                    dbproduct.ProductLoanRanges.FirstOrDefault(p => p.IsDeleted == false && p.LoanRange.Id == loanVM.LoanRangeId).IsDeleted = true;

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
                dbproduct.ProductLoanRanges = loanRanges;
            }

            dbproduct.Title = productVM.Product.Title;
            dbproduct.Price = productVM.Product.Price;
            dbproduct.DiscountedPrice = productVM.Product.DiscountedPrice;
            dbproduct.Count = productVM.Product.Count;
            dbproduct.IsMostViewed = productVM.Product.IsMostViewed;
            dbproduct.IsNewArrival = productVM.Product.IsNewArrival;

            dbproduct.UpdatedBy = "System";
            dbproduct.UpdatedAt = DateTime.UtcNow.AddHours(4);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));

        }

        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return BadRequest();

            if (!await _context.Products.AnyAsync(p => p.Id == id && p.IsDeleted == false)) return BadRequest();

            Product dbproduct = await _context.Products
                .FirstOrDefaultAsync(p => p.Id == id && p.IsDeleted == false);

            if (dbproduct == null)
            {
                return NotFound();
            }

            dbproduct.IsDeleted = true;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
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

        public async Task<IActionResult> DeleteImage(int? id, int? imageId)
        {
            if (id == null) return BadRequest();

            if (imageId == null) return BadRequest();

            Product product = await _context.Products
                .Include(p => p.ProductColors.Where(pi => pi.IsDeleted == false))
                .FirstOrDefaultAsync(p => p.IsDeleted == false && p.Id == id);

            if (product == null) return NotFound();

            if (!product.ProductColors.Any(pi => pi.Id == imageId)) return BadRequest();

            if (product.ProductColors.Count <= 1)
            {
                return BadRequest();
            }

            
            product.ProductColors.FirstOrDefault(p => p.IsDeleted == false && p.Id == imageId).DeletedBy = "System";
            product.ProductColors.FirstOrDefault(p => p.IsDeleted == false && p.Id == imageId).DeletedAt = DateTime.UtcNow.AddHours(4);
            product.ProductColors.FirstOrDefault(p => p.IsDeleted == false && p.Id == imageId).IsDeleted = true;

            await _context.SaveChangesAsync();

            FileHelper.DeleteFile(product.ProductColors?.FirstOrDefault(p => p.Id == imageId).Image, _env, "assets", "images", "products");

            return PartialView("_ProductImagePartial", product.ProductColors.Where(pi => pi.IsDeleted == false).ToList());
        }
    }
}
