using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.DTOs
{
    public class ZoneDto
    {
        public int Id { get; set; }
        public int BranchId { get; set; }
        public bool Active { get; set; }
    }
}
