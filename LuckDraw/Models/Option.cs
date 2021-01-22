using System.ComponentModel.DataAnnotations;

namespace LuckDraw.Models
{
    //方式表
    public class Option
    {
        [Key]
        public int ID { get; set; }
        public string Name { get; set; }
    }
}