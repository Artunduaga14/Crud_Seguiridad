using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Model
{
    public class Item
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string CodeQr { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool Active { get; set; }
        public DateTime CreatedAt { get; set; }
        public int CategoryId { get; set; }
        public int ZoneId { get; set; }
        public Zone Zone { get; set; }
        public Category Category { get; set; }
        public List<ImagenItem> ImagenItems { get; set; }
    }
}
