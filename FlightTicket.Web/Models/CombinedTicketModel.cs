using FlightTicket.Entity;

namespace FlightTicket.Web.Models
{
    public class CombinedTicketModel
    {
        public List<Ticket> Tickets { get; set; } = new List<Ticket>();
    }
}
