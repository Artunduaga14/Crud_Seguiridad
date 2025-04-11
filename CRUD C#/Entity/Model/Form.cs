using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Model
{
    public class Form
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool Active { get;set; }
        public List<FormModule> FormModules { get; set; } = new List<FormModule>();
        public List<RolFormPermission> RolFormPermissions { get; set; } = new List<RolFormPermission>();
    }
}
