using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LuckDraw.Models
{
    //日志表
    public class Log
    {
        public int ID{get;set;}
        public int LuckID{get;set;}
        public int DrawID{get;set;}
        public int? Number{get;set;}
        public DateTime? EntrTime { get; set; }
    }
}
