using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Model
{
    public class Company
    {
        public int Id { get; set; }
        public string Name { get; set;}
        public string Address { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Logo { get; set; }
        public bool Active { get; set; }
        public DateTime DataRegistry { get; set; }
        public List<User> Users { get; set; } = new List<User>();
        public List<Branch> Branches { get; set; } = new List<Branch>();
    }
}
