using Microsoft.EntityFrameworkCore;
using ScpmaBe.Repositories.Entities;
using ScpmaBe.Repositories.Interfaces;
using ScpmaBe.Services.Interfaces;
using ScpmaBe.Services.Models;
using ScpmBe.Services.Exceptions;

namespace ScpmaBe.Services
{
    public class CarService : ICarService
    {
        private readonly ICarRepository _carRepository;
        private readonly ICustomerRepository _customerRepository;

        public CarService(ICarRepository carRepository, ICustomerRepository customerRepository)
        {
            _carRepository = carRepository;
            _customerRepository = customerRepository;
        }

        public async Task<List<Car>> GetPaging(int pageIndex, int pageSize)
        {
            return await _carRepository.GetAll().Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
        }

        public async Task<List<Car>> GetCarsOfCustomerAsync(int customerId)
        {
            return await _carRepository.GetAll()
                                 .Where(x => x.CustomerId == customerId)
                                 .ToListAsync();
        }

        public async Task<List<Car>> GetCarsOfOwnerAsync(int ownerId)
        {
            return await _carRepository.GetAll()
                                       .Where(x => x.Customer.OwnerId == ownerId)
                                       .ToListAsync();
        }

        public async Task<Car> GetById(int id)
        {
            var existCar = await _carRepository.GetById(id);
            if (existCar == null) throw AppExceptions.NotFoundCar();

            return existCar;
        }

        public async Task<List<Car>> SearchCarAsync(SearchCarRequest request)
        {
            var query = _carRepository.GetAll();

            if (!string.IsNullOrEmpty(request.Keyword))
            {
                query = query.Where(x => x.CarId.ToString().Contains(request.Keyword) ||
                                         !string.IsNullOrEmpty(x.LicensePlate) && x.LicensePlate.Contains(request.Keyword));
            }

            var cars = await query.Select(x => new Car
            {
                CarId = x.CarId,
                CustomerId = x.CustomerId,
                Model = x.Model,
                Color = x.Color,
                LicensePlate = x.LicensePlate,
                RegistedDate = x.RegistedDate,
                Status = x.Status
            }).ToListAsync();

            return cars;
        }

        public async Task<Car> AddCarAsync(AddCarRequest request)
        {
            var existingLiPLa = await _carRepository.GetAll().FirstOrDefaultAsync(c => c.LicensePlate == request.LicensePlate);
            if (existingLiPLa != null) throw AppExceptions.BadRequestLicensePlateExists();

            //check if customerid exist
            var existingCustomer = await _customerRepository.CustomerIdExsistAsync(request.CustomerId);
            if (!existingCustomer) throw AppExceptions.NotFoundCustomerId();

            var newCar = new Car
            {
                CustomerId = request.CustomerId,
                LicensePlate = request.LicensePlate,
                Model = request.Model,
                Color = request.Color,
                RegistedDate = DateTime.Now,
                Status = request.Status
            };

            return await _carRepository.Insert(newCar);
        }

        public async Task<Car> UpdateCarAsync(UpdateCarRequest request)
        {
            var updateCar = await _carRepository.GetById(request.CarId);
            if (updateCar == null) throw AppExceptions.NotFoundCar();

            //check if customerid exist
            var existingCustomer = await _customerRepository.CustomerIdExsistAsync(request.CustomerId);
            if (!existingCustomer) throw AppExceptions.NotFoundCustomerId();

            var existingLiPLa = await _carRepository.GetAll().FirstOrDefaultAsync(c => c.LicensePlate == request.LicensePlate);
            if (existingLiPLa != null) throw AppExceptions.BadRequestLicensePlateExists();

            updateCar.CarId = request.CarId;
            updateCar.CustomerId = request.CustomerId;
            updateCar.LicensePlate = request.LicensePlate;
            updateCar.Model = request.Model;
            updateCar.Color = request.Color;
            updateCar.RegistedDate = request.RegistedDate;
            updateCar.Status = request.Status;

            await _carRepository.Update(updateCar);
            return updateCar;
        }

        public async Task<bool> DeleteCarAsync(int id)
        {
            try
            {
                var car = await _carRepository.GetById(id);
                if (car == null) throw AppExceptions.NotFoundCar();

                await _carRepository.Delete(id);
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
