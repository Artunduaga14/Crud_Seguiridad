using Data.Repostories.Global;
using Entity.Context;
using Entity.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data
{
    public class RolData : GenericData<Rol>
    {

        public RolData(ApplicationDbContext context, ILogger<Rol> logger):base(context, logger)
        {
        }



    }
}
