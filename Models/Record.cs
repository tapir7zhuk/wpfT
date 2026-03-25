using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfAppT.Models
{
    public class Record
    {
        public int Id { get; set; }

        public int SpecialistId { get; set; }
        public Specialist Specialist { get; set; }

        public int CustomerId { get; set; }
        public Customer Customer { get; set; }

        public string LicensePlate { get; set; }
        public Car Car { get; set; }

        public string Reason { get; set; }
        public string MasterDescription { get; set; }
        public DateTime DateAdded { get; set; } = DateTime.Now;
        public DateTime? DateCompleted { get; set; }
        public bool IsCompleted { get; set; } = false;

        public int? PhotoId { get; set; }
        public Photo Photo { get; set; }
    }
}

