using Application.DTO;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infra;
using Domain;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Application.Interface
{
    public interface ICategory
    {
        Task<SelectList> GetAll();
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

        public async Task<SelectList> GetAll()
        {
            var listdata =await mydb.Categories.Select(p =>new { p.Id,p.Name}).ToListAsync();
			return new SelectList(listdata,"Id","Name");
		}
    }
}
