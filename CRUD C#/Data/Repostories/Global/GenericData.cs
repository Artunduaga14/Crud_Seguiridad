using Data.Repostories.Interfaces;
using Entity.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Repostories.Global
{
    public class GenericData<T> : ICrudBase<T> where T : class
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<T> _logger;
        public GenericData(ApplicationDbContext context, ILogger<T> logger)
        {
            _context = context;
            _logger = logger;
        }

        // SELECT ALL
        public virtual async Task<IEnumerable<T>> GetAllAsyncLinq()
        {
            try
            {
                return await _context.Set<T>().ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex, $"No se pudo obetner los datos del objeto ");
                throw;
            }
        }
    }
}
