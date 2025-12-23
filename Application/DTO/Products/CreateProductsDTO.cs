using Domain;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTO
{
    public class CreateProductsDTO
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public float Invoice { get; set; }
        public List<IFormFile>? images { get; set; }

        public int CategoryId { get; set; }


        public List<ProductImageDto> ExistingImages { get; set; } = new();
    }
    public class ProductImageDto
    {
        public int Id { get; set; }
        public string PicUrl { get; set; }
     
    }
    public class EditProductDto : CreateProductsDTO
    {
        public int Id { get; set; }
        
        public List<ProductImageDto> ExistingImages { get; set; } = new();
    }

    
    
}
