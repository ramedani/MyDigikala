using Application.DTO;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infra;
using Domain;

namespace Application.Interface
{
    public interface ICategory
    {
        Task<List<CreateCategoryDTO>> GetAll();
        Task<CreateCategoryDTO> Get(int id);
        Task<int> Creat(CreateCategoryDTO dto);
    }

    public class CategoryService : ICategory
    {
        private AppDbContext mydb;
        public CategoryService(AppDbContext _mydb)
        {
            mydb= _mydb;
        }
        public async Task<int> Creat(CreateCategoryDTO dto)
        {
            var Cat=new Category(){
                Name =dto.Name,
                Picture =dto.Picture,
            };
            mydb.Add(Cat);
            await mydb.SaveChangesAsync();
            return Cat.Id;

        }

        public Task<CreateCategoryDTO> Get(int id)
        {
            throw new NotImplementedException();
        }

        public Task<List<CreateCategoryDTO>> GetAll()
        {
            throw new NotImplementedException();
        }
    }
}
