using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfAppT.Models
{
        public class Photo
        {
            public int Id { get; set; }
            public string FilePath { get; set; }
            public DateTime UploadedAt { get; set; } = DateTime.Now;
        }
    
}
