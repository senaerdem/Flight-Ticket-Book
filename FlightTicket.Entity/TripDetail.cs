using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightTicket.Entity
{
    public class TripDetail
    {
        public int Id { get; set; }

        public int FlightId { get; set; } //FK
        public Flight? Flight { get; set; } //NavProp

        public int DriverId { get; set; } //FK
        public Driver Driver { get; set; } = null!; //NavProp

        public int? CompanyId { get; set; } //FK
        public Company Company { get; set; } = null!; //NavProp

        public List<Trip> Trips { get; set; } = null!;
    }
}
