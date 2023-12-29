using FlightTicket.Entity;

namespace FlightTicket.Web.Models
{
    public class CombinedTripsModel
    {
        public List<Trip> CombinedTrips { get; set; }
        public decimal TotalPrice { get; set; }
        public string CombinedTripIds { get; set; }
    }
}
