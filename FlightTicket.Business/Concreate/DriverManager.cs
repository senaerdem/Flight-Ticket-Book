using FlightTicket.Business.Abstract;
using FlightTicket.Data.Abstract;
using FlightTicket.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightTicket.Business.Concreate
{
    public class DriverManager : IDriverService
    {
        private readonly IDriverRepository _driverRepository;
        public DriverManager(IDriverRepository driverRepository)
        {
            _driverRepository = driverRepository;
        }


        public async Task CreateAsync(Driver Driver)
        {
           await _driverRepository.CreateAsync(Driver);
        }

        public async Task DeleteAsync(Driver Driver)
        {
            await _driverRepository.DeleteAsync(Driver);
        }

        public async Task<List<Driver>> GetAllAsync()
        {
           return await _driverRepository.GetAllAsync();
        }

        public async Task<Driver> GetByIdAsync(int id)
        {
            return await (_driverRepository.GetByIdAsync(id));
        }

        public async Task UpdateAsync(Driver Driver)
        {
            await _driverRepository.UpdateAsync(Driver);
        }
    }
}
