using Microsoft.EntityFrameworkCore;
using ScpmaBe.Repositories.Entities;
using ScpmaBe.Repositories.Interfaces;
using ScpmaBe.Services.Common;
using ScpmaBe.Services.Enums;
using ScpmaBe.Services.Helpers;
using ScpmaBe.Services.Interfaces;
using ScpmaBe.Services.Models;
using ScpmBe.Services.Exceptions;

namespace ScpmaBe.Services
{
    public class CarService : ICarService
    {
        private readonly ICarRepository _carRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly IEntryExitLogRepository _entryExitLogRepository;
        private readonly AppSettings _appSettings;

        public CarService(
            ICarRepository carRepository,
            ICustomerRepository customerRepository,
            IEntryExitLogRepository entryExitLogRepository,
            AppSettings appSettings)
        {
            _carRepository = carRepository;
            _customerRepository = customerRepository;
            _entryExitLogRepository = entryExitLogRepository;

            _appSettings = appSettings;
        }

        public async Task<List<Car>> GetPaging(int pageIndex, int pageSize)
        {
            return await _carRepository.GetAll().Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
        }

        public async Task<List<CarAndEntranceResponse>> GetCarsOfCustomerAsync(int customerId)
        {
            var cars = await _carRepository.GetAll()
                                           .Where(x => x.CustomerId == customerId)
                                           .Select(x => new CarAndEntranceResponse
                                           {
                                               CarId = x.CarId,
                                               Thumbnail = x.Thumbnail,
                                               Brand = x.Brand,
                                               LicensePlate = x.LicensePlate,
                                               Model = x.Model,
                                               Color = x.Color,
                                               CustomerId = customerId
                                           })
                                           .ToListAsync();

            if (cars.Count == 0)
                return new List<CarAndEntranceResponse>();

            var licensePlates = cars.Select(x => x.LicensePlate).ToList();

            var entranceResponses = await _entryExitLogRepository
                                            .GetAll()
                                            .Include(x => x.ParkingSpace).ThenInclude(x => x.Floor).ThenInclude(x => x.Area).ThenInclude(x => x.ParkingLot)
                                            .Where(x => x.IsPaid == false && licensePlates.Contains(x.LicensePlate))
                                            .Select(x => new EntranceResponse
                                            {
                                                EntranceId = x.EntryExitLogId,
                                                EntranceDate = x.EntryTime.ToString("dd/MM/yyyy"),
                                                EntranceTime = x.EntryTime.ToString("HH:mm:ss"),
                                                ParkingSpaceName = x.ParkingSpace.ParkingSpaceName,
                                                FloorName = x.ParkingSpace.Floor.FloorName,
                                                AreaName = x.ParkingSpace.Floor.Area.AreaName,
                                                ParkingLotName = x.ParkingSpace.Floor.Area.ParkingLot.ParkingLotName,
                                                Status = ((ParkingSpaceStatus)x.ParkingSpace.Status).ToString(),
                                                LicensePlate = x.LicensePlate
                                            })
                                            .ToListAsync();

            foreach (var car in cars)
            {
                car.Thumbnail = GetCarImageUrl(car.Thumbnail);
                car.Entrance = entranceResponses.FirstOrDefault(e => e.LicensePlate == car.LicensePlate);
            }

            return cars;
        }

        public async Task<CarAndEntranceResponse> GetById(int id)
        {
            var existCar = await _carRepository.GetById(id);

            if (existCar == null) throw AppExceptions.NotFoundCar();

            return new CarAndEntranceResponse
            {
                CarId = existCar.CarId,
                Thumbnail = GetCarImageUrl(existCar.Thumbnail),
                CustomerId = existCar.CustomerId,
                Brand = existCar.Brand,
                Color = existCar.Color,
                LicensePlate = existCar.LicensePlate,
                Model = existCar.Model,
                Entrance = await _entryExitLogRepository
                                    .GetAll()
                                    .Include(x => x.ParkingSpace).ThenInclude(x => x.Floor).ThenInclude(x => x.Area).ThenInclude(x => x.ParkingLot)
                                    .Where(x => x.IsPaid == false && x.LicensePlate == existCar.LicensePlate)
                                    .Select(x => new EntranceResponse
                                    {
                                        EntranceId = x.EntryExitLogId,
                                        EntranceDate = x.EntryTime.ToString("dd/MM/yyyy"),
                                        EntranceTime = x.EntryTime.ToString("HH:mm:ss"),
                                        ParkingSpaceName = x.ParkingSpace.ParkingSpaceName,
                                        FloorName = x.ParkingSpace.Floor.FloorName,
                                        AreaName = x.ParkingSpace.Floor.Area.AreaName,
                                        ParkingLotName = x.ParkingSpace.Floor.Area.ParkingLot.ParkingLotName,
                                        Status = ((ParkingSpaceStatus)x.ParkingSpace.Status).ToString(),
                                        LicensePlate = x.LicensePlate
                                    })
                                    .FirstOrDefaultAsync()
            };
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
                Thumbnail = x.Thumbnail,
                CustomerId = x.CustomerId,
                Brand = x.Brand,
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
                Thumbnail = request.Thumbnail,
                Model = request.Model,
                Color = request.Color,
                Brand = request.Brand,
                RegistedDate = DateTime.Now.ToVNTime(),
                Status = true
            };

            var added = await _carRepository.Insert(newCar);

            return new Car
            {
                CarId = added.CarId,
                Brand = added.Brand,
                Color = added.Color,
                CustomerId = added.CustomerId,
                Model = added.Model,
                LicensePlate = added.LicensePlate,
                Thumbnail = GetCarImageUrl(added.Thumbnail),
            };
        }

        public async Task<Car> UpdateCarAsync(UpdateCarRequest request)
        {
            var updateCar = await _carRepository.GetById(request.CarId);
            if (updateCar == null) throw AppExceptions.NotFoundCar();

            var existingLiPLa = await _carRepository
                                        .GetAll()
                                        .FirstOrDefaultAsync(c =>
                                            c.CarId != request.CarId &&
                                            c.LicensePlate == request.LicensePlate);

            if (existingLiPLa != null) throw AppExceptions.BadRequestLicensePlateExists();

            if(request.LicensePlate != updateCar.LicensePlate)
            {
                var existInEntrance = await _entryExitLogRepository.GetAll().Where(
                                x => x.LicensePlate == request.LicensePlate ||
                                     x.LicensePlate == updateCar.LicensePlate).AnyAsync();

                if(existInEntrance)
                    throw AppExceptions.BadRequest("Xe đã có trong danh sách vào bãi, không thể thay đổi biển số xe");
            }

            updateCar.LicensePlate = request.LicensePlate;
            updateCar.Model = request.Model;
            updateCar.Color = request.Color;
            updateCar.Thumbnail = request.Thumbnail;
            updateCar.Brand = request.Brand;

            await _carRepository.Update(updateCar);

            return new Car
            {
                CarId = updateCar.CarId,
                Brand = updateCar.Brand,
                Color = updateCar.Color,
                CustomerId = updateCar.CustomerId,
                Model = updateCar.Model,
                LicensePlate = updateCar.LicensePlate,
                Thumbnail = GetCarImageUrl(updateCar.Thumbnail),
            };
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

        const string ImageFolder = "files/cars/";

        public string GetImageFolder()
        {
            return ImageFolder;
        }

        private string GetCarImageUrl(string thumbnail)
        {
            if (string.IsNullOrEmpty(thumbnail)) return "";

            if (thumbnail.StartsWith("http://") || thumbnail.StartsWith("https://"))
                return thumbnail;

            // Assuming _appSettings.ApplicationUrl is the base URL of your application
            return $"{_appSettings.ApplicationUrl}/{ImageFolder}/{thumbnail}";
        }
    }
}
