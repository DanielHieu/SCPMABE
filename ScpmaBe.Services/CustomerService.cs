using Microsoft.EntityFrameworkCore;
using ScpmaBe.Repositories.Entities;
using ScpmaBe.Repositories.Interfaces;
using ScpmaBe.Services.Interfaces;
using ScpmaBe.Services.Models;
using ScpmBe.Services.Exceptions;

namespace ScpmaBe.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IOwnerRepository _ownerRepository;
        private readonly IHashHelper _hashHelper;

        public CustomerService(ICustomerRepository customerRepository, IHashHelper hashHelper, IOwnerRepository ownerRepository)
        {
            _customerRepository = customerRepository;
            _hashHelper = hashHelper;
            _ownerRepository = ownerRepository;
        }

        public async Task<List<Customer>> GetPaging(int pageIndex, int pageSize)
        {
            return await _customerRepository.GetAll().Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
        }

        public async Task<List<Customer>> GetCustomersOfOwnerAsync(int ownerId)
        {
            return await _customerRepository.GetAll()
                                            .Where(x => x.OwnerId == ownerId)
                                            .ToListAsync();
        }

        public async Task<Customer> GetById(int id)
        {
            var customer = await _customerRepository.GetById(id);
            if (customer == null) throw AppExceptions.NotFoundAccount();

            return customer;
        }

        public async Task<List<Customer>> SearchCustomerAsync(SearchCustomerRequest request)
        {
            var query = await _customerRepository.GetAll();

            if (!string.IsNullOrEmpty(request.Keyword))
                query = query.Where(
                        s => s.CustomerId.ToString().Contains(request.Keyword.Contains()) ||
                            (!string.IsNullOrEmpty(s.FirstName) && s.FirstName.Contains(request.Keyword)) ||
                            (!string.IsNullOrEmpty(s.LastName) && s.LastName.Contains(request.Keyword)) ||
                            (!string.IsNullOrEmpty(s.Phone) && s.Phone.Contains(request.Keyword)));

            var customers = query.Select(s => new Customer
            {
                CustomerId = s.CustomerId,
                OwnerId = s.OwnerId,
                FirstName = s.FirstName,
                LastName = s.LastName,
                Phone = s.Phone,
                Email = s.Email,
                Username = s.Username,
                IsActive = s.IsActive
            }).ToListAsync();

            return customers;
        }

        public async Task<Customer> RegisterCustomerAsync(RegisterCustomerRequest request)
        {
            if (string.IsNullOrEmpty(request.Username) || request.Username.Length < 4)
                throw AppExceptions.BadRequestUsernameIsInvalid();

            var ownerExists = await _ownerRepository.ExistsByIdAsync(request.OwnerId);

            if (!ownerExists)
            {
                throw AppExceptions.NotFoundOnwerId(); // Return fail if OwnerId not existed.
            }

            // Check if username already exists
            var exstingAcc = await _customerRepository.GetAll().FirstOrDefaultAsync(x => x.Username == request.Username);

            if (exstingAcc != null) throw AppExceptions.BadRequestUserExists();

            var newAcc = new Customer
            {
                OwnerId = request.OwnerId,
                Username = request.Username,
                Password = _hashHelper.HashPassword(request.Password),
                FirstName = request.FirstName,
                LastName = request.LastName,
                Phone = request.Phone,
                Email = request.Email,
                IsActive = request.IsActive
            };

            return await _customerRepository.Insert(newAcc);
        }

        public async Task<Customer> AuthorizeAsync(string username, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                throw AppExceptions.NotFoundAccount();

            var acc = await _customerRepository.GetAll().SingleOrDefaultAsync(x => x.Username == username);

            if (acc == null) throw AppExceptions.NotFoundAccount();

            var passwordHash = _hashHelper.HashPassword(password);

            if (acc.Password != passwordHash) throw AppExceptions.NotFoundAccount();

            return acc;
        }

        public async Task<Customer> UpdateCustomerAsync(UpdateCustomerRequest request)
        {
            var updateAcc = await _customerRepository.GetById(request.CustomerId);

            if (updateAcc == null) throw AppExceptions.NotFoundAccount();

            updateAcc.CustomerId = request.CustomerId;
            updateAcc.Email = request.Email;
            updateAcc.FirstName = request.FirstName;
            updateAcc.LastName = request.LastName;
            updateAcc.Phone = request.Phone;
            updateAcc.IsActive = request.IsActive;

            await _customerRepository.Update(updateAcc);

            return updateAcc;
        }

        public async Task<bool> DeleteCustomerAsync(int id)
        {
            try
            {
                var acc = await _customerRepository.GetById(id);

                if (acc == null) throw AppExceptions.NotFoundAccount();

                await _customerRepository.Delete(id);

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
