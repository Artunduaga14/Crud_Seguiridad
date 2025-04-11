using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Repostories.Interfaces
{
    public interface IQueryLinq<T>
    {
        Task<IEnumerable<T>> GetAllAsyncLinq();
    }
}
