using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityLayer.Entities.DTOs
{
    public class BookDTO
    {
        [Required,MinLength(2,ErrorMessage ="Min 2 char"),MaxLength(10,ErrorMessage ="Max 10 char")]
        public string BookName { get; set; }
        [Required]
        public float BookPrice { get; set; }
    }
}
