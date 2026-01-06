using Application.DTO;
using Domain;
using Infra;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interface
{
    public interface ICreateUser
    {
        Task<int> Creat(Create_a_UserDTO dto);
    }

    public class letsCreat : ICreateUser
    {
        private AppDbContext mydb;
        public letsCreat(AppDbContext _mydb)
        {
            mydb = _mydb;
        }

        public async Task<int> Creat(Create_a_UserDTO dto)
        {
            var result = mydb.registers.ToList();
            if (result == null)
            {

                var user = new register
                {
                    Password = dto.Password,
                    Email = dto.Email,
                    Phone = dto.Phone,
                    MeliCode = dto.MeliCode,
                    Address = dto.Address
                };
                mydb.Add(user);
                await mydb.SaveChangesAsync();
                return user.Id;
                
            }
         
        }

      
    }

}
