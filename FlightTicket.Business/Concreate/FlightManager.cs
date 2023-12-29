using FlightTicket.Business.Abstract;
using FlightTicket.Data.Abstract;
using FlightTicket.Data.Concreate.EfCore;
using FlightTicket.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightTicket.Business.Concreate
{
    public class FlightManager : IFlightService
    {
        private readonly IFlightRepository _flightRepository;
        public FlightManager(IFlightRepository flighRepository)
        {
            _flightRepository = flighRepository;
        }


        public async Task CreateAsync(Flight Flight)
        {
           await _flightRepository.CreateAsync(Flight);
        }

        public async Task DeleteAsync(Flight Flight)
        {
            await _flightRepository.DeleteAsync(Flight);
        }

        public async Task<List<Flight>> GetAllAsync()
        {
           return await _flightRepository.GetAllAsync();
        }

        public async Task<Flight> GetByIdAsync(int id)
        {
            return await (_flightRepository.GetByIdAsync(id));
        }

        public async Task UpdateAsync(Flight Flight)
        {
            await _flightRepository.UpdateAsync(Flight);
        }
    }
}
