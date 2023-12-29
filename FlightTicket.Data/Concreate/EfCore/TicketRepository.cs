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
    public class TicketRepository : GenericRepository<Ticket>, ITicketRepository
    {
        public Context_FlightTicket Context
        {
            get
            {
                return _context as Context_FlightTicket;
            }
        }

        public TicketRepository(Context_FlightTicket Context) : base(Context)
        {
        }

        public async Task<Ticket> GetTicketWithTrip(int id)
        {
            return await Context.Tickets
                .Where(e => e.Id == id)
                .Include(e => e.Trip)
                .ThenInclude(e => e.MidLine)
                .ThenInclude(e => e.Line)
                .Include(e => e.Trip.TripDetail)
                .FirstOrDefaultAsync();
        }

        public async Task<List<Ticket>> GetTicketsByCustomerIdAsync(int id)
        {
            return await Context.Tickets
                .Where(e => e.CustomerId == id)
                 .Include(e => e.Trip)
                .ThenInclude(e => e.MidLine)
                .ThenInclude(e => e.Line)
                .Include(e => e.Trip.TripDetail)
                .ToListAsync();
        }
        public async Task<List<Ticket>> GetTicketsByUserNameAsync(string userName)
        {
            return await Context.Tickets
                .Where(e => e.UserName == userName)
                 .Include(e => e.Trip)
                .ThenInclude(e => e.MidLine)
                .ThenInclude(e => e.Line)
                .Include(e => e.Trip.TripDetail)
                .ToListAsync();
        }

        public async Task<List<Ticket>> GetTicketsByPnrAsync(string pnr)
        {
            return await Context.Tickets
                .Where(e => e.PnrNo == pnr)
                .Include(e => e.Customer)
                 .Include(e => e.Trip)
                .ThenInclude(e => e.MidLine)
                .ThenInclude(e => e.Line)
                .Include(e => e.Trip.TripDetail)
                .ToListAsync();
        }
    }
}
