using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
using PhotoAlbum.Shared.DTOs;

namespace PhotoAlbum.Shared.Model
{
    public class Photo
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public string Name { get; set; }
        public DateTime UploadTime { get; set; }
        public string Owner { get; set; }
        public byte[] Image { get; set; }

        public PhotoDTO ToDTO()
        {
            return new PhotoDTO { ID = ID, Name = Name, UploadTime = UploadTime, Owner = Owner };
        }
    }
}
