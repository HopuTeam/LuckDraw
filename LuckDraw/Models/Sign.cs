using System.ComponentModel.DataAnnotations;

namespace LuckDraw.Models
{
    public class Sign
    {
        [Key]
        public int ID { get; set; }
        public string Account { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public bool Status { get; set; }
        public int Identity { get; set; }
    }
}