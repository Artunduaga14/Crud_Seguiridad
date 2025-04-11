using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Model
{
    public class Branch
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Address { get; set; }
        public int Phone { get; set; }
        public string Email { get; set; }
        public int Incharge { get; set; }
        public bool Active { get; set; }
        public string LocationFurrow { get; set; }
        public int CompanyId { get; set; }
        public Company company { get; set; }
        public List<Zone> Zones { get; set; } = new List<Zone>();
    }
}
