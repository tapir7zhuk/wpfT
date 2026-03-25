using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace WpfAppT.Models
{
    public class Specialist
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public string Specialization { get; set; }

        public int? PhotoId { get; set; }
        public Photo Photo { get; set; }

        public ICollection<Record> Records { get; set; }
    }
}

