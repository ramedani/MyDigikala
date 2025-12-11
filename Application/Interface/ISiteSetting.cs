using Application.DTO;
using Domain;
using Infra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Application.Interface
{
     public interface ISiteSetting
    {
        SiteSettingDTO Get();
        Task<int> CreatOrUpdate(SiteSettingDTO dto);
    }

    public class SiteSettingService : ISiteSetting
    {
        private AppDbContext mydb;
        public SiteSettingService(AppDbContext _mydb)
        {
            mydb = _mydb;
        }
        public async Task<int> CreatOrUpdate(SiteSettingDTO dto)
        {
            var result = mydb.SiteSettings.FirstOrDefault();
            if (result == null)
            {
                var Setting = new SiteSetting()
                {
                   Title= dto.Title,
                   Description= dto.Description,
                   KeyWord= dto.KeyWord,
                   Tel=dto.Tel,
                   icon=dto.icon
                };

                mydb.Add(Setting);
                await mydb.SaveChangesAsync();
                return Setting.Id;
            }
            else
            {
                result.Title = dto.Title;
                result.Description = dto.Description;
                result.KeyWord = dto.KeyWord;
                await mydb.SaveChangesAsync();
                return result.Id;

            }
        }

        public SiteSettingDTO Get()
        {
           var info= new SiteSettingDTO();
            var result = mydb.SiteSettings.FirstOrDefault();
            if(result != null)
            {
                info.Title = result.Title;
                info.Description = result.Description;
                info.KeyWord = result.KeyWord;
                info.Tel = result.Tel;

            }

            return info;
        }
    }


}
