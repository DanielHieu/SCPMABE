using Microsoft.EntityFrameworkCore;
using ScpmaBe.Repositories.Entities;
using ScpmaBe.Repositories.Interfaces;
using ScpmaBe.Services.Enum;
using ScpmaBe.Services.Enums;
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
        private IParkingLotRepository _parkingLotRepository;    
        private IPaymentContractRepository _paymentContractRepository;
        public ContractService(
            IContractRepository contractRepository,
            ICarRepository carRepository,
            IParkingSpaceRepository parkingSpaceRepository,
            IParkingLotRepository parkingLotRepository,
            IPaymentContractRepository paymentContractRepository)
        {
            _contractRepository = contractRepository;
            _carRepository = carRepository;
            _parkingSpaceRepository = parkingSpaceRepository;
            _parkingLotRepository = parkingLotRepository;
            _paymentContractRepository = paymentContractRepository;
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

        public async Task<List<ContractResponse>> GetContractsOfCustomerAsync(int customerId)
        {
            return await _contractRepository.GetAll()
                                            .Include(x => x.PaymentContracts)
                                            .Include(x => x.Car).ThenInclude(Car => Car.Customer)
                                            .Where(x => x.Car.CustomerId == customerId)
                                            .Select(x => new ContractResponse
                                            {
                                                ContractId = x.ContractId,
                                                StartDate = x.StartDate.ToDateTime(TimeOnly.MinValue),
                                                EndDate = x.EndDate.ToDateTime(TimeOnly.MinValue),
                                                Status = ((ContractStatus)x.Status).ToString(),
                                                Note = x.Note,
                                                ParkingSpaceId = x.ParkingSpaceId,
                                                ParkingSpaceName = x.ParkingSpace.ParkingSpaceName,
                                                ParkingLotName = $"PL{x.ParkingSpace.Floor.Area.ParkingLotId:0#}",
                                                Lat = x.ParkingSpace.Floor.Area.ParkingLot.Lat ?? 0,
                                                Long = x.ParkingSpace.Floor.Area.ParkingLot.Long ?? 0,
                                                ParkingLotAddress = x.ParkingSpace.Floor.Area.ParkingLot.Address,
                                                Car = new CarResponse
                                                {
                                                    CarId = x.Car.CarId,
                                                    LicensePlate = x.Car.LicensePlate,
                                                    Model = x.Car.Model,
                                                    Color = x.Car.Color,
                                                    CustomerName = $"{x.Car.Customer.FirstName} {x.Car.Customer.LastName}",
                                                    CustomerId = x.Car.CustomerId
                                                },
                                                PaymentContract = x.PaymentContracts != null ? x.PaymentContracts.OrderByDescending(x => x.CreatedDate)
                                                                                                 .Select(x => new PaymentContractResponse
                                                                                                 {
                                                                                                     PaymentContractId = x.PaymentContractId,
                                                                                                     PaymentAmount = x.PaymentAmount,
                                                                                                     Status = ((PaymentContractStatus)x.Status).ToString(),
                                                                                                     StartDate = x.StartDate.ToDateTime(TimeOnly.MinValue),
                                                                                                     EndDate = x.EndDate.ToDateTime(TimeOnly.MinValue),
                                                                                                     Note = x.Note
                                                                                                 }).FirstOrDefault() : null
                                            })
                                            .ToListAsync();
        }

        public async Task<List<ContractResponse>> SearchContractAsync(SearchContractRequest request)
        {
            var searchTask = await _contractRepository.GetAll()
                                                      .Include(x => x.PaymentContracts)
                                                      .Include(x => x.Car)
                                                      .Where(x =>
                                                        string.IsNullOrEmpty(request.Keyword) ||
                                                        x.ContractId.ToString().Contains(request.Keyword) ||
                                                        x.Car.LicensePlate.Contains(request.Keyword)
                                                      )
                                                      .Where(x => string.IsNullOrEmpty(request.LicensePlate) || request.LicensePlate == x.Car.LicensePlate)
                                                      .Where(x => request.Status.HasValue == false || request.Status.Value == x.Status)
                                                      .Select(x => new ContractResponse
                                                      {
                                                          ContractId = x.ContractId,
                                                          StartDate = x.StartDate.ToDateTime(TimeOnly.MinValue),
                                                          EndDate = x.EndDate.ToDateTime(TimeOnly.MinValue),
                                                          Status = ((ContractStatus)x.Status).ToString(),
                                                          Note = x.Note,
                                                          ParkingSpaceId = x.ParkingSpaceId,
                                                          ParkingSpaceName = x.ParkingSpace.ParkingSpaceName,
                                                          ParkingLotName = $"PL{x.ParkingSpace.Floor.Area.ParkingLotId:0#}",
                                                          Lat = x.ParkingSpace.Floor.Area.ParkingLot.Lat ?? 0,
                                                          Long = x.ParkingSpace.Floor.Area.ParkingLot.Long ?? 0,
                                                          ParkingLotAddress = x.ParkingSpace.Floor.Area.ParkingLot.Address,
                                                          Car = new CarResponse
                                                          {
                                                              CarId = x.Car.CarId,
                                                              LicensePlate = x.Car.LicensePlate,
                                                              Model = x.Car.Model,
                                                              Color = x.Car.Color,
                                                              CustomerName = $"{x.Car.Customer.FirstName} {x.Car.Customer.LastName}",
                                                              CustomerId = x.Car.CustomerId
                                                          },
                                                          PaymentContract = x.PaymentContracts != null ? x.PaymentContracts.OrderByDescending(x => x.CreatedDate)
                                                                                                 .Select(x => new PaymentContractResponse
                                                                                                 {
                                                                                                     PaymentContractId = x.PaymentContractId,
                                                                                                     PaymentAmount = x.PaymentAmount,
                                                                                                     Status = ((PaymentContractStatus)x.Status).ToString(),
                                                                                                     StartDate = x.StartDate.ToDateTime(TimeOnly.MinValue),
                                                                                                     EndDate = x.EndDate.ToDateTime(TimeOnly.MinValue),
                                                                                                     Note = x.Note
                                                                                                 }).FirstOrDefault() : null
                                                      })
                                                      .ToListAsync();

            return searchTask;
        }

        public async Task<Contract> AddContractAsync(AddContractRequest request)
        {
            var existCar = await _carRepository.CarIdExsistAsync(request.CarId);
            if (!existCar) throw AppExceptions.NotFoundId();

            var parkingLot = await _parkingLotRepository.GetById(request.ParkingLotId);

            if (parkingLot == null) throw AppExceptions.NotFoundParkingLot();

            var parkingSpaceId = await FindParkingSpace(request.ParkingLotId, request.StartDate, request.EndDate);

            if(parkingSpaceId == 0) throw AppExceptions.NotFoundParkingSpace();

            var newContract = new Contract()
            {
                CarId = request.CarId,
                ParkingSpaceId = parkingSpaceId,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                Status = (int)ContractStatus.Inactive,
                CreatedDate = DateTime.Now,
                UpdatedDate = DateTime.Now,
                Note = request.Note,
                PaymentContracts = new List<PaymentContract>()
                {
                    new PaymentContract()
                    {
                        StartDate = request.StartDate,
                        EndDate = request.EndDate,
                        Status = (int)PaymentContractStatus.Pending,
                        CreatedDate = DateTime.Now,
                        UpdatedDate = DateTime.Now,
                        PricePerMonth = parkingLot.PricePerMonth,
                        PaymentAmount = parkingLot.PricePerMonth * CalculateMonthsBetween(request.StartDate, request.EndDate),
                        PaymentMethod = "Cash",
                        Note = request.Note
                    }
                }
            };

            return await _contractRepository.Insert(newContract);
        }

        private async Task<int> FindParkingSpace(int parkingLotId, DateOnly startDate, DateOnly endDate)
        {
            // Get all active contracts for the specified parking lot
            var activeContracts = await _contractRepository
                                                .GetAll()
                                                .Where(c => c.Status != (int)ContractStatus.Expired &&
                                                            c.ParkingSpace.Floor.Area.ParkingLotId == parkingLotId)
                                                .Select(c => new
                                                {
                                                    c.ParkingSpaceId,
                                                    c.Status,
                                                    c.StartDate,
                                                    c.EndDate,
                                                })
                                                .ToListAsync();

            // Find parking spaces in the given parking lot that are available for contract rental
            var contractParkingSpaces = await _parkingSpaceRepository
                                                        .GetAll()
                                                        .Where(ps => ps.Floor.Area.ParkingLotId == parkingLotId &&
                                                                     ps.Floor.Area.RentalType == (int)RentalType.Contract)
                                                        .Select(x => x.ParkingSpaceId)
                                                        .ToListAsync();

            // Find a parking space that doesn't have overlapping contracts for the date range
            foreach (var parkingSpaceId in contractParkingSpaces)
            {
                // Check if this parking space has any overlapping contracts
                var overlappingContracts = activeContracts
                    .Where(c => c.ParkingSpaceId == parkingSpaceId)
                    .Where(c =>
                        // Contract starts during our period
                        (c.StartDate >= startDate && c.StartDate <= endDate) ||
                        // Contract ends during our period
                        (c.EndDate >= startDate && c.EndDate <= endDate) ||
                        // Contract completely encompasses our period
                        (c.StartDate <= startDate && c.EndDate >= endDate) ||
                        // Our period completely encompasses the contract
                        (startDate <= c.StartDate && endDate >= c.EndDate))
                    .Any();

                if (!overlappingContracts)
                {
                    return parkingSpaceId;
                }
            }

            // No available parking space found
            return 0;
        }

        private decimal CalculateMonthsBetween(DateOnly startDate, DateOnly endDate)
        {
            // Calculate total months between dates (including partial months)
            int months = (endDate.Year - startDate.Year) * 12 + endDate.Month - startDate.Month;

            // Add partial month based on days
            decimal daysInMonth = DateTime.DaysInMonth(endDate.Year, endDate.Month);
            decimal partialMonth = (endDate.Day - startDate.Day) / daysInMonth;

            return months + partialMonth;
        }

        public async Task<Contract> UpdateContractAsync(UpdateContractRequest request)
        {
            var existCar = await _carRepository.CarIdExsistAsync(request.CarId);
            if (!existCar) throw AppExceptions.NotFoundId();

            var updateContract = await _contractRepository.GetById(request.ContractId);
            if (updateContract == null) throw AppExceptions.NotFoundId();

            updateContract.StartDate = request.StartDate;
            updateContract.EndDate = request.EndDate;
            updateContract.Status = request.Status;

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

        public async Task<bool> Reject(int contractId)
        {
            var paymentContract = await _paymentContractRepository
                                    .GetAll()
                                    .Where(x => x.ContractId == contractId)
                                    .OrderByDescending((x => x.CreatedDate))
                                    .FirstOrDefaultAsync();

            if (paymentContract == null) return false;

            if (paymentContract.Status == (int)PaymentContractStatus.Completed)
            {
                throw AppExceptions.PaymentAlreadyCompleted();
            }

            if (paymentContract.Status == (int)PaymentContractStatus.Pending)
            {
                paymentContract.Status = (int)PaymentContractStatus.Rejected;
                paymentContract.UpdatedDate = DateTime.Now;

                await _paymentContractRepository.Update(paymentContract);
            }

            return true;
        }

        public async Task<bool> Approve(int contractId)
        {
            var paymentContract = await _paymentContractRepository
                                    .GetAll()
                                    .Where(x => x.ContractId == contractId)
                                    .OrderByDescending((x => x.CreatedDate))
                                    .FirstOrDefaultAsync();
            
            if (paymentContract == null) return false;
            if (paymentContract.Status == (int)PaymentContractStatus.Completed)
            {
                throw AppExceptions.PaymentAlreadyCompleted();
            }

            if (paymentContract.Status == (int)PaymentContractStatus.Pending)
            {
                paymentContract.Status = (int)PaymentContractStatus.Approved;
                paymentContract.UpdatedDate = DateTime.Now;

                await _paymentContractRepository.Update(paymentContract);
            }

            return false;
        }

        public async Task<bool> Pay(int contractId)
        {
            var paymentContract = await _paymentContractRepository
                                    .GetAll()
                                    .Where(x=>x.ContractId == contractId)
                                    .OrderByDescending((x=>x.CreatedDate))
                                    .FirstOrDefaultAsync();

            if(paymentContract == null) return false;

            if(paymentContract.Status == (int)PaymentContractStatus.Completed)
            {
                throw AppExceptions.PaymentAlreadyCompleted();
            }

            if(paymentContract.Status == (int)PaymentContractStatus.Approved)
            {
                paymentContract.Status = (int)PaymentContractStatus.Paid;
                paymentContract.PaymentDate = DateTime.Now;
                paymentContract.UpdatedDate = DateTime.Now;

                await _paymentContractRepository.Update(paymentContract);
            }

            return true;
        }

        public async Task<bool> Complete(int contractId)
        {
            var paymentContract = await _paymentContractRepository
                                    .GetAll()
                                    .Where(x => x.ContractId == contractId)
                                    .OrderByDescending((x => x.CreatedDate))
                                    .FirstOrDefaultAsync();

            if (paymentContract == null) return false;

            if (paymentContract.Status == (int)PaymentContractStatus.Completed)
            {
                throw AppExceptions.PaymentAlreadyCompleted();
            }

            if (paymentContract.Status == (int)PaymentContractStatus.Paid)
            {
                paymentContract.Status = (int)PaymentContractStatus.Completed;
                paymentContract.UpdatedDate = DateTime.Now;

                var contract = await _contractRepository.GetById(contractId);

                contract.UpdatedDate = DateTime.Now;
                contract.Status = (int)ContractStatus.Active;
                contract.EndDate = paymentContract.EndDate;

                await _contractRepository.Update(contract);
            }

            return true;
        }
    }
}
