using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhotoAlbum.Shared.DTOs
{
    public class PhotoDTO
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public DateTime UploadTime { get; set; }
        public string Owner { get; set; }
    }
}
