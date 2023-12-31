using FlightTicket.Data.Abstract;
using FlightTicket.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightTicket.Data.Concreate.EfCore
{
    public class DriverRepository : GenericRepository<Driver> , IDriverRepository
    {
        public Context_FlightTicket Context { get
            {
                return _context as Context_FlightTicket;
            } 
        }
        public DriverRepository(Context_FlightTicket Context) : base(Context)
        {
        }



    }
}
