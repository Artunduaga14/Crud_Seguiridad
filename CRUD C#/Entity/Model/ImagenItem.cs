using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Model
{
    public class ImagenItem
    {
        public int Id { get; set; }
        public int ItemId { get; set; }
        public string UrlImage { get; set; }
        public DateTime DateRegistry { get; set; }
        public bool Active { get; set; }
        public Item Item { get; set; }
    }
}
