using FlightTicket.Data.Abstract;
using FlightTicket.Entity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightTicket.Data.Concreate.EfCore
{
    public class MidlineRepository : GenericRepository<MidLine> , IMidlineRepository
    {
        public MidlineRepository(Context_FlightTicket busTicket) : base(busTicket)
        {

        }
        public Context_FlightTicket Context
        {
            get { return _context as Context_FlightTicket; }
        }
    }
}
