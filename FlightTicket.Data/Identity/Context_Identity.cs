using FlightTicket.Entity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FlightTicket.Data.Identity
{
    public class Context_Identity : IdentityDbContext<User>
    {
        public Context_Identity(DbContextOptions<Context_Identity> options):base(options)
        {
        }
    }
}
