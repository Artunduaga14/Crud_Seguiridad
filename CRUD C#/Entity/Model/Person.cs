using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Model
{
    public class Person
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }
        public int NumberIdentification { get; set; }
        public string Phone { get; set; }
        public bool Active { get; set; }
        //public int UserId { get; set; }
        //public User User { get; set; }
    }
}
