using Application.DTO;
using Domain;
using Infra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interface
{
    public interface ICategory
    {
        Task<List<CreateCategoryDto>> GetAll();
        Task<CreateCategoryDto> Get(int id);
        Task<int> Create(CreateCategoryDto dto);
    }

    public class CategoryService : ICategory
    {
        private AppDbContext mydb;

        public CategoryService(AppDbContext _mydb)
        {
            mydb = _mydb;
        }

        public async Task<int> Create(CreateCategoryDto dto)
        {
           var cat=new Category(){ 
           Name = dto.Name,
           Picture=dto.Picture,
           };

            mydb.Add(cat);
           await mydb.SaveChangesAsync();

           return cat.Id;
        }

        public Task<CreateCategoryDto> Get(int id)
        {
            throw new NotImplementedException();
        }

        public Task<List<CreateCategoryDto>> GetAll()
        {
            throw new NotImplementedException();
        }
    }
}
