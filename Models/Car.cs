using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ComponentModel.DataAnnotations;

namespace WpfAppT.Models
{
    public class Car
    {
        [RegularExpression(@"^[АБВГҐДЕЄЖЗИІЇЙКЛМНОПРСТУФХЦЧШЩЬЮЯ]{2}\d{4}[АБВГҐДЕЄЖЗИІЇЙКЛМНОПРСТУФХЦЧШЩЬЮЯ]{2}$",
            ErrorMessage = "Формат номеру: АА1234ВВ")]
        public string LicensePlate { get; set; }

        public int BrandId { get; set; }
        public CarBrand Brand { get; set; }

        [Required]
        public string Model { get; set; }

        public int CustomerId { get; set; }
        public Customer Customer { get; set; }

        public ICollection<Record> Records { get; set; }
    }
}
