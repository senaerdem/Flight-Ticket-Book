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
    public class CustomerManager : ICustomerService
    {
        private readonly ICustomerRepository _customerRepository;

        public CustomerManager(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }

        public async Task CreateAsync(Customer customer)
        {
            await _customerRepository.CreateAsync(customer);
        }


        public async Task DeleteAsync(Customer customer)
        {
            await _customerRepository.DeleteAsync(customer);
        }

        public async Task<List<Customer>> GetAllAsync()
        {
            return await _customerRepository.GetAllAsync();
        }

        public async Task<Customer> GetByIdAsync(int id)
        {
            return await _customerRepository.GetByIdAsync(id);
        }

        public async Task<Customer> GetCustomerByUserNameAsync(string userName)
        {
            return await _customerRepository.GetCustomerByUserNameAsync(userName);
        }

        public async Task UpdateAsync(Customer customer)
        {
           await _customerRepository.UpdateAsync(customer);
        }
    }
}
