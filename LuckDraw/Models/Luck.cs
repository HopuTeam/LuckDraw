using System.ComponentModel.DataAnnotations;

namespace LuckDraw.Models
{
    //选项表
    public class Luck
    {
        [Key]
        public int ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Weigh { get; set; }
        public int SignID { get; set; }
        public int ParentID { get; set; }
    }
}