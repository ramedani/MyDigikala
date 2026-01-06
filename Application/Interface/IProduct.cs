using Application.DTO;
using Application.Helper;
using Domain;
using Infra;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Application.Interface
{
    public interface IProduct
    {
        Task<List<ProductListDto>> GetAll();
        Task<EditProductDto> GetForEdit(int id);
        Task<int> Create(CreateProductDto dto);
        Task Update(EditProductDto dto); // 

        Task Delete(int id);
        Task DeleteImage(int imageId);
        Task DeleteGroup(List<int> ids);
        Task ToggleStatusGroup(List<int> ids);
        Task UpdatePriceGroup(List<int> ids, double percentage);
        Task<Application.DTO.ProductDetailViewModelDto> GetProductDetailAsync(int id, string? userId = null);
        Task<(int likes, int dislikes)> GetCommentCountsAsync(int commentId);
        Task<bool> ToggleVoteAsync(int commentId, int userId, bool isLike);
        Task<Tuple<List<ProductCardDto>, int>> GetProductsForIndex(int pageId = 1, string sort = "newest");
        Task IncrementProductVisitAsync(int productId);
        Task<ProductDashboardStatsDto> GetDashboardStatsAsync();
        Task<Product?> GetMostViewedProduct();
        Task<List<Category>> GetCategoriesAsync();
        Task <List<ProductListDto>> GetLastProducts(int take);
        Task<List<ProductListDto>> GetAmazingProducts(int Take);

    }


    public class ProductService : IProduct
    {
        private readonly AppDbContext _mydb;
        private readonly IWebHostEnvironment _env; 
        private readonly IWishlist _iwishlist;
        private readonly IFileSecurityHelper _fileHelper;

        public ProductService(AppDbContext mydb, IWebHostEnvironment env, IWishlist iwishlist,IFileSecurityHelper fileHelper)
        {
            _mydb = mydb;
            _env = env;
            _iwishlist = iwishlist;
            _fileHelper = fileHelper;
        }

        public async Task<List<ProductListDto>> GetAll()
        {
            return await _mydb.Products
                .Include(p => p.brand)
                .Include(p => p.cat)
                .Include(p => p.images)
                .Select(p => new ProductListDto
                {
                    Id = p.Id,
                    Title = p.Title,
                    Price = p.Price,
                    Invoice = p.Invoice,
                    IsActive = p.IsActive,
                    VisitCount = p.VisitCount,
                    CategoryName = p.cat != null ? p.cat.Name : "-",
                    PrimaryImage = p.images.FirstOrDefault(i => i.IsPrimary).PicUrl ??
                                   p.images.FirstOrDefault().PicUrl 
                })
                .OrderByDescending(p => p.Id)
                .ToListAsync();
        }
        public async Task<ProductDashboardStatsDto> GetDashboardStatsAsync()
        {
            var total = await _mydb.Products.CountAsync();
            var active = await _mydb.Products.CountAsync(n => n.IsActive);

            return new ProductDashboardStatsDto
            {
                TotalProduct = total,
                PublishedCount = active,
                DraftCount = total - active,

            };
        }
        public async Task<Product?> GetMostViewedProduct()
        {
            return _mydb.Products
                .OrderByDescending(p => p.VisitCount)
                .FirstOrDefault();
        }
        public async Task<int> Create(CreateProductDto dto)
        {
            // 1. مپ کردن اطلاعات ساده
            var product = new Product
            {
                Title = dto.Title,
                EnTitle = dto.EnTitle,
                Description = dto.Description,
                Slogan = dto.Slogan,
                Price = dto.Price,
                PurchasePrice = dto.PurchasePrice,
                Discount = dto.Discount,
                DiscountPercentage = dto.DiscountPercentage,
                BrandId = dto.BrandId,
                CategoryId = dto.CategoryId,
                Country = dto.Country,
                Weight = dto.Weight,
                Invoice = dto.Invoice,
                PrimaryImageId = dto.PrimaryImageId,
                IsActive = dto.IsActive,
                AmazingOffers = dto.AmazingOffers,
                CreateTime = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            
            _mydb.Products.Add(product);
            await _mydb.SaveChangesAsync();
            
            if (dto.NewImages != null && dto.NewImages.Count > 0)
            {
                foreach (var file in dto.NewImages)
                {
                    if (file.Length > 0)
                    {
                        try
                        {

                            var fileName = await _fileHelper.SecureUploadAsync(file, _env.WebRootPath, "ProductImage");

                            bool isFirst = !_mydb.ProductImages.Any(x => x.ProductId == product.Id);

                            _mydb.ProductImages.Add(new ProductImage
                            {
                                ProductId = product.Id,
                                PicUrl = fileName,
                                IsPrimary = isFirst
                            });
                        }
                        catch (Exception ex)
                        {

                            // Log.Error(ex, "خطا در آپلود امن فایل");
                        }
                    }
                }

                await _mydb.SaveChangesAsync();
            }

            return product.Id;
        }

        public async Task DeleteImage(int imageId)
        {
            var img = await _mydb.ProductImages.FindAsync(imageId);
            if (img != null)
            {

                var path = Path.Combine(_env.WebRootPath, "ProductImage", img.PicUrl);
                if (System.IO.File.Exists(path)) System.IO.File.Delete(path);

                _mydb.ProductImages.Remove(img);
                await _mydb.SaveChangesAsync();
            }
        }

        public async Task<EditProductDto> GetForEdit(int id)
        {
            var p = await _mydb.Products
                .Include(x => x.images)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (p == null) return null;

            return new EditProductDto
            {
                Id = p.Id,
                Title = p.Title,
                EnTitle = p.EnTitle,
                Description = p.Description,
                Slogan = p.Slogan,
                Price = p.Price,
                PurchasePrice = p.PurchasePrice,
                Discount = p.Discount,
                DiscountPercentage = p.DiscountPercentage,
                BrandId = p.BrandId,
                CategoryId = p.CategoryId,
                Country = p.Country,
                Weight = p.Weight,
                Invoice = p.Invoice,
                IsActive = p.IsActive,
                AmazingOffers = p.AmazingOffers,
                
                ExistingImages = p.images?.Select(i => new ProductImageDto
                {
                    Id = i.Id,
                    PicUrl = i.PicUrl,
                    IsPrimary = i.IsPrimary
                }).ToList() ?? new List<ProductImageDto>(),

                PrimaryImageId = p.images?.FirstOrDefault(i => i.IsPrimary)?.Id
            };
        }

        public async Task Update(EditProductDto dto)
        {
            var product = await _mydb.Products
                .Include(p => p.images)
                .FirstOrDefaultAsync(p => p.Id == dto.Id);

            if (product == null) return;
            
            product.Title = dto.Title;
            product.EnTitle = dto.EnTitle;
            product.Description = dto.Description;
            product.Slogan = dto.Slogan;
            product.Price = dto.Price;
            product.PurchasePrice = dto.PurchasePrice;
            product.Discount = dto.Discount;
            product.DiscountPercentage = dto.DiscountPercentage;
            product.BrandId = dto.BrandId;
            product.CategoryId = dto.CategoryId;
            product.Country = dto.Country;
            product.Weight = dto.Weight;
            product.Invoice = dto.Invoice;
            product.IsActive = dto.IsActive;
            product.AmazingOffers = dto.AmazingOffers;
            product.UpdatedAt = DateTime.Now;

            if (dto.NewImages != null && dto.NewImages.Count > 0)
            {
                foreach (var file in dto.NewImages)
                {
                    try
                    {
                        var fileName = await  _fileHelper.SecureUploadAsync(file, _env.WebRootPath, "ProductImage");
                        _mydb.ProductImages.Add(new ProductImage
                        {
                            ProductId = product.Id,
                            PicUrl = fileName
                        });
                    }
                    catch (Exception ex)
                    {
                        // لاگ کردن خطا
                    }
                }
            }
            
            await _mydb.SaveChangesAsync();
            
            if (dto.PrimaryImageId.HasValue)
            {

                var allImages = await _mydb.ProductImages.Where(x => x.ProductId == product.Id).ToListAsync();
                foreach (var img in allImages)
                {
                    img.IsPrimary = (img.Id == dto.PrimaryImageId.Value);
                }

                await _mydb.SaveChangesAsync();
            }
        }

        public async Task Delete(int id)
        {
            var product = await _mydb.Products.FindAsync(id);
            if (product != null)
            {

                _mydb.Products.Remove(product);
                await _mydb.SaveChangesAsync();
            }
        }

        public async Task DeleteGroup(List<int> ids)
        {
            var products = await _mydb.Products.Where(p => ids.Contains(p.Id)).ToListAsync();
            _mydb.Products.RemoveRange(products);
            await _mydb.SaveChangesAsync();
        }

        public async Task ToggleStatusGroup(List<int> ids)
        {
            var products = await _mydb.Products.Where(p => ids.Contains(p.Id)).ToListAsync();
            foreach (var p in products)
            {
                p.IsActive = !p.IsActive;
            }

            await _mydb.SaveChangesAsync();
        }

        public async Task UpdatePriceGroup(List<int> ids, double percentage)
        {
            var products = await _mydb.Products.Where(p => ids.Contains(p.Id)).ToListAsync();
            foreach (var item in products)
            {

                decimal currentPrice = item.Price ;
                
                decimal changeAmount = currentPrice * ((decimal)percentage / 100);
                
                decimal newPrice = currentPrice + changeAmount;
                
                newPrice = Math.Round(newPrice); 


                item.Price = newPrice;
            }

            await _mydb.SaveChangesAsync();
        }



     
       public async Task<Application.DTO.ProductDetailViewModelDto> GetProductDetailAsync(int id, string? userId = null)
{
    var product = await _mydb.Products
        .Include(p => p.images)
        .Include(p => p.cat)
        .Include(p => p.brand)
        .Include(p => p.Msg4Products.Where(c => c.IsConfirmed)) // فقط نظرات تایید شده
        // .ThenInclude(m => m.User)
        .FirstOrDefaultAsync(p => p.Id == id);

    if (product == null) return null;


    bool isLiked = false;
    if (!string.IsNullOrEmpty(userId))
    {
        isLiked = await _iwishlist.IsProductInWishlistAsync(userId, id);
    }

    var approvedComments = product.Msg4Products.ToList();
    var ratedComments = approvedComments.Where(c => c.FiveStar != null && c.FiveStar > 0).ToList();
    double averageRating = ratedComments.Any() ? ratedComments.Average(c => c.FiveStar.Value) : 0;


    List<ProductListDto> relatedProductsDto = new List<ProductListDto>();
    if (product.cat != null)
    {
 
        var relatedEntities = await _mydb.Products
            .Include(p => p.images)
            .Where(p => p.cat.Id == product.cat.Id && p.Id != product.Id) // خودش را نشان ندهد
            .OrderByDescending(p => p.Id)
            .Take(10)
            .ToListAsync();

        
        relatedProductsDto = relatedEntities.Select(p => new ProductListDto
        {
            Id = p.Id,
            Title = p.Title,
            Price = p.Price,
            EnTitle = p.EnTitle,
            Invoice = p.Invoice,
            Discount = p.Discount, 
            IsActive = p.IsActive,

            PrimaryImage = p.images.FirstOrDefault(i => i.IsPrimary)?.PicUrl 
                           ?? p.images.FirstOrDefault()?.PicUrl 
                           ?? "placeholder.png"
        }).ToList();
    }

    var model = new Application.DTO.ProductDetailViewModelDto
    {
        Id = product.Id,
        Title = product.Title,
        EnTitle = product.EnTitle,
        Description = product.Description,
        Price = product.Price,
        Slogan = product.Slogan, 
        Discount = product.Discount,
        DiscountPercentage = product.DiscountPercentage,
        Invoice = product.Invoice, 
        BrandNameF = product.brand?.Name ?? "-",
        CategoryName = product.cat?.Name ?? "-",
        IsActive = product.IsActive,
        Quantity = product.count ?? 1,
        IsInWishlist = isLiked,
        Country = product.Country,
        Weight = product.Weight,
        
        Images = product.images.Select(i => i.PicUrl).ToList(),

        AverageRating = Math.Round(averageRating, 1),
        ApprovedCommentsCount = approvedComments.Count,
        RatedCommentsCount = ratedComments.Count,
        CommentCount = approvedComments.Count,

        Comments = approvedComments.Select(c => new Application.DTO.ProductCommentForViewDto()
        {
            Id = c.Id,
            // UserName = c.User?.Username ?? "کاربر ناشناس",
            Content = c.Content,
            Like = c.Like ?? 0,
            Dislike = c.Dislike ?? 0,
            Title = c.Title,
            IsRecommended = c.IsRecommended ?? true,
            Name = c.Name,
            
            // Date = c.CreateDate // اگر تاریخ دارید اضافه کنید
        }).ToList(),

        RelatedProducts = relatedProductsDto
    };

    return model;
}


        public async Task<(int likes, int dislikes)> GetCommentCountsAsync(int commentId)
        {
            var c = await _mydb.Msg4Products
                .Where(x => x.Id == commentId)
                .Select(x => new { Like = x.Like ?? 0, Dislike = x.Dislike ?? 0 })
                .FirstOrDefaultAsync();

            return (c?.Like ?? 0, c?.Dislike ?? 0);
        }


        public async Task<bool> ToggleVoteAsync(int commentId, int userId, bool isLike)
        {

            var comment = await _mydb.Msg4Products.FindAsync(commentId);
            if (comment == null) return false;


            var existingVote = await _mydb.CommentVotes
               .FirstOrDefaultAsync(v => v.CommentId == commentId );

            if (existingVote != null)
            {

                if (existingVote.IsLike == isLike)
                {

                    _mydb.CommentVotes.Remove(existingVote);

                    if (isLike) comment.Like = Math.Max(0, (comment.Like ?? 0) - 1);
                    else comment.Dislike = Math.Max(0, (comment.Dislike ?? 0) - 1);
                }
                else
                {
                    
                    existingVote.IsLike = isLike; 

                    if (isLike)
                    {

                        comment.Dislike = Math.Max(0, (comment.Dislike ?? 0) - 1);
                        comment.Like = (comment.Like ?? 0) + 1;
                    }
                    else
                    {

                        comment.Dislike = (comment.Dislike ?? 0) + 1;
                        comment.Like = Math.Max(0, (comment.Like ?? 0) - 1);
                    }
                }
            }
            else
            {

                var newVote = new CommentVote
                {
                    CommentId = commentId,
                   // UserId = userId,
                    IsLike = isLike
                };
                _mydb.CommentVotes.Add(newVote);

                if (isLike) comment.Like = (comment.Like ?? 0) + 1;
                else comment.Dislike = (comment.Dislike ?? 0) + 1;
            }

            await _mydb.SaveChangesAsync();
            return true;
        }

        public async Task<Tuple<List<ProductCardDto>, int>> GetProductsForIndex(int pageId = 1, string sort = "newest")
        {
            int take = 12; 
            int skip = (pageId - 1) * take;

     
            var query = _mydb.Products
                .AsNoTracking() 
                .Where(p => p.IsActive); 
            
            switch (sort)
            {
                case "cheap":
                    query = query.OrderBy(p => p.Price);
                    break;
                case "expensive":
                    query = query.OrderByDescending(p => p.Price);
                    break;
                default: // newest
                    query = query.OrderByDescending(p => p.Id); 
                    break;
            }
            
            int totalCount = await query.CountAsync();
            
            var products = await query
                .Skip(skip)
                .Take(take)

                .Select(p => new ProductCardDto
                {
                    Id = p.Id,
                    Title = p.Title,
                    Price = p.Price,
                    CreateDate = p.CreateTime ?? DateTime.Now, 
 
                    CategoryName = p.cat != null ? p.cat.Name : "بدون دسته‌بندی",
                    ImageUrl = p.images.Where(i => i.IsPrimary).Select(i => i.PicUrl).FirstOrDefault()
                               ?? p.images.Select(i => i.PicUrl).FirstOrDefault(),


                })

                .ToListAsync();

            return Tuple.Create(products, totalCount);
        }


        public async Task IncrementProductVisitAsync(int productId)
        {
            var product = await _mydb.Products.FindAsync(productId);
            if (product != null)
            {
                product.VisitCount += 1; 
                await _mydb.SaveChangesAsync();
            }
        }
        public async Task<List<Category>> GetCategoriesAsync()
        {
            return await _mydb.Categories.ToListAsync();
        }
        public async Task<List<ProductListDto>> GetLastProducts(int take)
        {
            var products = await _mydb.Products
                .AsNoTracking()
                .Where(p => p.IsActive)
                .OrderByDescending(p => p.Id)
                .Take(take)
                .Select(p => new ProductListDto
                {
                    Id = p.Id,
                    Title = p.Title,
                    Price = p.Price,
                    Discount = p.Discount,  
                    Invoice = p.Invoice,    
                })
                .ToListAsync();
            
            foreach (var item in products)
            {
                var imgUrl = await _mydb.ProductImages
                    .AsNoTracking()
                    .Where(img => img.ProductId == item.Id)
                    .Select(img => img.PicUrl)
                    .FirstOrDefaultAsync();

                item.PrimaryImage = imgUrl ?? "default.png";
            }

            return products;
        }
        public async Task<List<ProductListDto>> GetAmazingProducts(int Take)
        {

            var products = await _mydb.Products
                .AsNoTracking()
                .Where(p => p.IsActive && p.AmazingOffers == true) 
                .OrderByDescending(p => p.Id)
                
                .Select(p => new ProductListDto
                {
                    Id = p.Id,
                    Title = p.Title,
                    Price = p.Price,
                    Discount = p.Discount
                })
                .ToListAsync();


            foreach (var item in products)
            {
                var imgUrl = await _mydb.ProductImages
                    .AsNoTracking()
                    .Where(img => img.ProductId == item.Id)
                    .Select(img => img.PicUrl)
                    .FirstOrDefaultAsync();

                item.PrimaryImage = imgUrl ?? "default.png"; 
            }

            return products;
        }
        
        

    }
}
    
    
