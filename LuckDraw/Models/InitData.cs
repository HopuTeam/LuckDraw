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
                // 用户信息
                context.Signs.Add(new Sign()
                {
                    Account = "admin",
                    Password = "E1ADC3949BA59ABBE56E057F2F883E",// MD5(123456)
                    Status = true,
                    Email = "admin@admin.com",
                    Identity = 1,
                });
                // 抽奖项信息
                context.Lucks.Add(new Luck()
                {
                    Name = "测试抽奖组1",
                    Description = "测试抽奖组描述",
                    Weigh = 0,
                    SignID = 1,
                    ParentID = 0,
                });
                context.Lucks.Add(new Luck()
                {
                    Name = "测试抽奖项1",
                    Description = "测试抽奖项描述",
                    Weigh = 1,
                    SignID = 1,
                    ParentID = 1,
                });
                context.Lucks.Add(new Luck()
                {
                    Name = "测试抽奖项2",
                    Description = "测试抽奖项描述",
                    Weigh = 1,
                    SignID = 1,
                    ParentID = 1,
                });
                // 抽奖项目信息
                context.Draws.Add(new Draw()
                {
                    Name = "测试抽奖后排除",
                    SignID = 1,
                    OptionID = 1,
                });
                context.Draws.Add(new Draw()
                {
                    Name = "测试可重复抽奖",
                    SignID = 1,
                    OptionID = 2,
                });
                // 选项信息
                context.Options.Add(new Option()
                {
                    Name = "抽奖后排除",
                });
                context.Options.Add(new Option()
                {
                    Name = "可重复抽奖",
                });
                // 幸运抽奖表信息
                context.LuckDraws.Add(new LuckDraw()
                {
                    LuckID = 2,
                    DrawID = 1,
                    Number = 0,
                    EntryTime = null,
                });
                context.LuckDraws.Add(new LuckDraw()
                {
                    LuckID = 3,
                    DrawID = 1,
                    Number = 0,
                    EntryTime = null,
                });
                context.LuckDraws.Add(new LuckDraw()
                {
                    LuckID = 2,
                    DrawID = 2,
                    Number = 0,
                    EntryTime = null,
                });
                context.LuckDraws.Add(new LuckDraw()
                {
                    LuckID = 3,
                    DrawID = 2,
                    Number = 0,
                    EntryTime = null,
                });
                context.SaveChanges();
            }
        }
    }
}