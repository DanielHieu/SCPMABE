using Microsoft.EntityFrameworkCore;
using ScpmaBe.Repositories.Entities;
using ScpmaBe.Repositories.Interfaces;
using ScpmaBe.Services.Interfaces;
using ScpmaBe.Services.Models;
using ScpmBe.Services.Exceptions;


namespace ScpmaBe.Services
{
    public class ContractService : IContractService
    {
        private IContractRepository _contractRepository;
        private ICarRepository _carRepository;
        private IParkingSpaceRepository _parkingSpaceRepository;

        public ContractService(IContractRepository contractRepository, ICarRepository carRepository, IParkingSpaceRepository parkingSpaceRepository)
        {
            _contractRepository = contractRepository;
            _carRepository = carRepository;
            _parkingSpaceRepository = parkingSpaceRepository;
        }

        public async Task<List<Contract>> GetPaging(int pageIndex, int pageSize)
        {
            return await _contractRepository.GetAll().Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
        }

        public async Task<Contract> GetByIdAsync(int id)
        {
            var existId = await _contractRepository.GetById(id);
            if (existId == null) throw AppExceptions.NotFoundId();

            return existId;
        }

        public async Task<List<Contract>> GetContractsOfCustomerAsync(int customerId)
        {
            return await _contractRepository.GetAll()
                                            .Where(x => x.Car.CustomerId == customerId)
                                            .ToListAsync();
        }

        public async Task<List<Contract>> GetContractsOfOwnerAsync(int ownerId)
        {
            return await _contractRepository.GetAll()
                                            .Where(x => x.Car.Customer.OwnerId == ownerId)
                                            .ToListAsync();
        }

        public async Task<List<Contract>> SearchContractAsync(SearchContractRequest request)
        {
            var searchTask = await _contractRepository.GetAll()
                                                      .Include(x => x.Car)
                                                      .Where(x =>
                                                        !string.IsNullOrEmpty(request.Keyword) && 
                                                        (x.ContractId.ToString().Contains(request.Keyword) || x.Car.LicensePlate.Contains(request.Keyword))
                                                      )
                                                      .Where(x => !string.IsNullOrEmpty(request.LicencePlate) && request.LicencePlate == x.Car.LicensePlate)
                                                      .Where(x => request.Status.HasValue && request.Status.Value == x.Status)
                                                      .Select(x => new Contract
                                                      {
                                                          ContractId = x.ContractId,
                                                          CarId = x.CarId,
                                                          ParkingSpaceId = x.ParkingSpaceId,
                                                          StartDate = x.StartDate,
                                                          EndDate = x.EndDate,
                                                          Status = x.Status,
                                                          CreatedDate = x.CreatedDate,
                                                          UpdatedDate = x.UpdatedDate,
                                                          Note = x.Note
                                                      })
                                                      .ToListAsync();

            return searchTask;
        }

        public async Task<Contract> AddContractAsync(AddContractRequest request)
        {
            var existCar = await _carRepository.CarIdExsistAsync(request.CarId);
            if (!existCar) throw AppExceptions.NotFoundId();

            var existParkingSpace = await _parkingSpaceRepository.ParkingSpaceIdExsistAsync(request.ParkingSpaceId);
            if (!existParkingSpace) throw AppExceptions.NotFoundId();

            var newContract = new Contract()
            {
                CarId = request.CarId,
                ParkingSpaceId = request.ParkingSpaceId,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                Status = request.Status,
                CreatedDate = DateTime.Now,
                UpdatedDate = DateTime.Now,
                Note = request.Note
            };

            return await _contractRepository.Insert(newContract);

        }

        public async Task<Contract> UpdateContractAsync(UpdateContractRequest request)
        {
            var existCar = await _carRepository.CarIdExsistAsync(request.CarId);
            if (!existCar) throw AppExceptions.NotFoundId();

            var existParkingSpace = await _parkingSpaceRepository.ParkingSpaceIdExsistAsync(request.ParkingSpaceId);
            if (!existParkingSpace) throw AppExceptions.NotFoundId();

            var updateContract = await _contractRepository.GetById(request.ContractId);
            if (updateContract == null) throw AppExceptions.NotFoundId();

            updateContract.ContractId = request.ContractId;
            updateContract.ParkingSpaceId = request.ParkingSpaceId;
            updateContract.StartDate = request.StartDate;
            updateContract.EndDate = request.EndDate;
            updateContract.Status = request.Status;
            updateContract.CreatedDate = request.CreatedDate;
            updateContract.UpdatedDate = DateTime.Now;
            updateContract.Note = request.Note;

            await _contractRepository.Update(updateContract);
            return updateContract;
        }

        public async Task<bool> DeleteContractAsync(int id)
        {
            try
            {
                var car = await _contractRepository.GetById(id);
                if (car == null) throw AppExceptions.NotFoundContract();

                await _contractRepository.Delete(id);
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
