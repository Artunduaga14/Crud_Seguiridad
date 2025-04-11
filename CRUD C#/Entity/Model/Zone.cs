using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Model
{
    public class Zone
    {
        public int Id { get; set; }
        public int BranchId { get; set; }
        public bool Active { get; set; }
        public Branch Branch { get; set; }
        public List<InventaryDetails> InvenraryDetails { get; set; } = new List<InventaryDetails>();
        public List<Item> Items { get; set; } = new List<Item>();
    }
}
