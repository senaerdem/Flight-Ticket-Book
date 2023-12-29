using FlightTicket.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightTicket.Business.Abstract
{
    public interface IFlightService
    {
        Task<List<Flight>> GetAllAsync();

        Task<Flight> GetByIdAsync(int id);

        Task CreateAsync(Flight Flight);

        Task UpdateAsync(Flight Flight);

        Task DeleteAsync(Flight Flight);
    }
}
