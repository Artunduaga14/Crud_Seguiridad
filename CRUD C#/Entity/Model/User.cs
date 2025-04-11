using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Model
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public DateTime CreationDate { get; set; } 
        public bool Active { get; set; }
        public int PersonId { get; set; }
        public string CompanyId { get; set; }
        public List<RolUser> RolUsers { get; set; } = new List<RolUser>();
        public Person Person { get; set; }
        public Company Companys { get; set; }
    
    }
}
