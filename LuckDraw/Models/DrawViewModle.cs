using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LuckDraw.Models
{
    public class DrawViewModle
    {
        /// <summary>
        /// 项目名称
        /// </summary>
        public string DrawName{ get; set; }

        /// <summary>
        /// Option类型名称
        /// </summary>
        public string OptionName { get; set; }

        /// <summary>
        /// Option ID,用来判断是重复抽奖还是不重复
        /// </summary>
        public int OptionID { get; set; }

        /// <summary>
        /// 用来查询属于这个项目的奖项
        /// </summary>
        public int LuckdrawDrawID { get; set; }

        /// <summary>
        /// 项目包含的奖项数量
        /// </summary>
        public int LuckCount { get; set; }
    }

    public class PlayViweModel
    {
        public string Drawname { get; set; }
        public int Drawid { get; set; }
    }
}
