using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ComponentModel.DataAnnotations;

namespace WpfAppT.Models
{
    public class Customer
    {
        public int Id { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [RegularExpression(@"^\+380\d{9}$", ErrorMessage = "Формат: +380XXXXXXXXX")]
        public string PhoneNumber { get; set; }

        public ICollection<Car> Cars { get; set; }
        public ICollection<Record> Records { get; set; }
    }
}

