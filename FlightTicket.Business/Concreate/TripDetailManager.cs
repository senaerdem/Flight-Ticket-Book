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
    public class TripDetailManager : ITripDetailService
    {
        private readonly ITripDetailRepository _tripDetailRepository;
        public TripDetailManager(ITripDetailRepository tripDetailRepository)
        {
            _tripDetailRepository = tripDetailRepository;
        }


        public async Task CreateAsync(TripDetail tripDetail)
        {
           await _tripDetailRepository.CreateAsync(tripDetail);
        }

        public async Task DeleteAsync(TripDetail tripDetail)
        {
            await _tripDetailRepository.DeleteAsync(tripDetail);
        }

        public async Task<List<TripDetail>> GetAllAsync()
        {
           return await _tripDetailRepository.GetAllAsync();
        }

        public async Task<TripDetail> GetByIdAsync(int id)
        {
            return await (_tripDetailRepository.GetByIdAsync(id));
        }

        public async Task UpdateAsync(TripDetail tripDetail)
        {
            await _tripDetailRepository.UpdateAsync(tripDetail);
        }
    }
}
