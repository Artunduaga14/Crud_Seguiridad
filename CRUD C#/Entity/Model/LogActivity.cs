using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Model
{
    public class LogActivity
    {
        public int Id { get; set; }
        public string Action { get; set; }
        public string DataPrevious { get; set; }
        public string DataNew { get; set; }
        public DateTime Data { get; set; }
        public bool Active { get; set; }
    }
}
