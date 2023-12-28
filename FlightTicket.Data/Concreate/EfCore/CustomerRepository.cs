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
    public class CustomerRepository: GenericRepository<Customer>, ICustomerRepository
    {
        public CustomerRepository(Context_FlightTicket busTicket) : base(busTicket)
        {
        }

        public Context_FlightTicket Context {
            get
            {
                return _context as Context_FlightTicket;
            }
        }

        public async Task<Customer> GetCustomerByUserNameAsync(string userName)
        {
            var customer = await Context.Customers.Where(e => e.UserName == userName).FirstOrDefaultAsync();

            return customer;
        }

    }
}
