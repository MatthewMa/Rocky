using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Rocky.Models
{
    public class AppType
    {
        [Key]
        public int Id { get; set; }
        [Required, MaxLength(10, ErrorMessage ="The length of the name should be less than 10!"), MinLength(3, ErrorMessage = "The length of the name should more or equal to 3")]
        public string Name { get; set; }
    }
}
