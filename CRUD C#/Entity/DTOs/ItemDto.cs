using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.DTOs
{
    public class ItemDto
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
    }
}
