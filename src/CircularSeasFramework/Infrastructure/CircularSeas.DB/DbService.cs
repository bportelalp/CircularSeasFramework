using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CircularSeas.DB.Context;

namespace CircularSeas.DB
{
    public class DbService
    {
        public DbService(CircularSeas.DB.Context.CircularSeasContext dbContext)
        {
            DbContext = dbContext;
        }

        public CircularSeasContext DbContext { get; }


    }
}
