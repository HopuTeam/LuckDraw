using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LuckDraw.Models
{
    public class InitData
    {
        public static void Send(IServiceProvider service)
        {
            using var context = new CoreEntities(service.GetRequiredService<DbContextOptions<CoreEntities>>());
            if (context.Signs.ToList().Count() == 0)
            {
                context.Signs.Add(new Sign()
                {
                    Account = "admin",
                    //Password = "E1ADC3949BA59ABBE56E057F2F883E",// MD5(123456)
                    Password = "123456",
                    Email = "admin@admin.com",
                    Identity = 1,
                });
                context.SaveChanges();
            }
        }
    }
}
