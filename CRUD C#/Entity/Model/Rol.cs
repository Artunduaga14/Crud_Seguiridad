using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Model
{
    public class Rol
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Code { get; set; }
        public bool Active { get; set; }
        public string Description { get; set; }
        public List<RolFormPermission> RolFormPermissions { get; set; } = new List<RolFormPermission>();
        public List<RolUser> RolUsers { get; set; } = new List<RolUser>();
    }
}
