using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ScpmaBe.Repositories.Entities;
using ScpmaBe.Repositories.Interfaces;
using ScpmaBe.Services.Enum;
using ScpmaBe.Services.Enums;
using ScpmaBe.Services.Helpers;
using ScpmaBe.Services.Interfaces;
using ScpmaBe.Services.Models;
using ScpmBe.Services.Exceptions;

namespace ScpmaBe.Services
{
    public class ContractService : IContractService
    {
        private readonly IContractRepository _contractRepository;
        private readonly ICarRepository _carRepository;
        private readonly IParkingSpaceRepository _parkingSpaceRepository;
        private readonly IParkingLotRepository _parkingLotRepository;
        private readonly IPaymentContractRepository _paymentContractRepository;
        private readonly ILogger _logger;

        public ContractService(
            ILogger<ContractService> logger,
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
            _logger = logger;
        }

        public async Task<ContractResponse> GetByIdAsync(int id)
        {
            var now = DateTime.Now.ToVNTime();

            var contract = await _contractRepository.GetAll()
                                                      .Include(x => x.Car)
                                                      .Include(x => x.PaymentContracts)
                                                      .Include(x => x.ParkingSpace).ThenInclude(x => x.Floor).ThenInclude(x => x.Area).ThenInclude(x => x.ParkingLot)
                                                      .Where(x => x.ContractId == id)
                                                      .Select(x => new ContractResponse
                                                      {
                                                          ContractId = x.ContractId,
                                                          StartDate = x.StartDate.ToDateTime(TimeOnly.MinValue),
                                                          EndDate = x.EndDate.ToDateTime(TimeOnly.MaxValue),
                                                          Status = x.Status == (int)ContractStatus.Active && now < x.StartDate.ToDateTime(TimeOnly.MinValue) ? ContractStatus.PendingActivation.ToString() : ((ContractStatus)x.Status).ToString(),
                                                          CreatedDate = x.CreatedDate,
                                                          UpdatedDate = x.UpdatedDate,
                                                          CreatedDateString = x.CreatedDate.ToString("dd/MM/yyyy HH:mm:ss"),
                                                          Note = x.Note,
                                                          ParkingSpaceId = x.ParkingSpaceId,
                                                          ParkingSpaceName = x.ParkingSpace.ParkingSpaceName,
                                                          ParkingLotName = x.ParkingSpace.Floor.Area.ParkingLot.ParkingLotName,
                                                          Lat = x.ParkingSpace.Floor.Area.ParkingLot.Lat ?? 0,
                                                          Long = x.ParkingSpace.Floor.Area.ParkingLot.Long ?? 0,
                                                          ParkingLotAddress = x.ParkingSpace.Floor.Area.ParkingLot.Address,
                                                          AreaName = x.ParkingSpace.Floor.Area.AreaName,
                                                          FloorName = x.ParkingSpace.Floor.FloorName,
                                                          Car = new CarResponse
                                                          {
                                                              CarId = x.Car.CarId,
                                                              Thumbnail = x.Car.Thumbnail,
                                                              Brand = x.Car.Brand,
                                                              LicensePlate = x.Car.LicensePlate,
                                                              Model = x.Car.Model,
                                                              Color = x.Car.Color,
                                                              CustomerName = $"{x.Car.Customer.FirstName} {x.Car.Customer.LastName}",
                                                              CustomerId = x.Car.CustomerId
                                                          },
                                                          TotalAllPayments = x.PaymentContracts.Sum(pc => pc.PaymentAmount)
                                                      }).FirstOrDefaultAsync();

            if (contract == null) throw AppExceptions.NotFoundId();

            return contract;
        }

        public async Task<List<ContractResponse>> SearchContractsAsync(SearchContractRequest request)
        {
            var now = DateTime.Now.ToVNTime();

            var query = _contractRepository.GetAll()
                                           .Include(x => x.PaymentContracts)
                                           .Include(x => x.Car)
                                           .Include(x => x.ParkingSpace).ThenInclude(x => x.Floor).ThenInclude(x => x.Area).ThenInclude(x => x.ParkingLot).AsQueryable();

            if (!string.IsNullOrEmpty(request.Keyword))
            {
                query = query.Where(x =>
                                        x.ContractId.ToString().Contains(request.Keyword) ||
                                        x.Car.LicensePlate.Contains(request.Keyword)
                                    );
            }

            if (!string.IsNullOrEmpty(request.LicensePlate))
            {
                query = query.Where(x => request.LicensePlate == x.Car.LicensePlate);                          
            }

            if(request.Status != 0)
            {
                var date = now.ToDateOnly();

                if (request.Status == (int)ContractStatus.Active)
                    query = query.Where(x => x.Status == (int)ContractStatus.Active && date >= x.StartDate && date <= x.EndDate);
                else if(request.Status == (int)ContractStatus.PendingActivation)
                    query = query.Where(x => x.Status == (int)ContractStatus.Active && date < x.StartDate);
                else
                    query = query.Where(x => request.Status == x.Status);
            }

            if(request.ParkingLotId != 0)
            {
                query = query.Where(x => x.ParkingSpace.Floor.Area.ParkingLotId == request.ParkingLotId);
            }

            var result = await query.Select(x => new ContractResponse
                                {
                                    ContractId = x.ContractId,
                                    StartDate = x.StartDate.ToDateTime(TimeOnly.MinValue),
                                    EndDate = x.EndDate.ToDateTime(TimeOnly.MaxValue),
                                    Status = x.Status == (int)ContractStatus.Active && now < x.StartDate.ToDateTime(TimeOnly.MinValue) ? ContractStatus.PendingActivation.ToString() : ((ContractStatus)x.Status).ToString(),
                                    Note = x.Note,
                                    ParkingSpaceId = x.ParkingSpaceId,
                                    ParkingSpaceName = x.ParkingSpace.ParkingSpaceName,
                                    ParkingLotName = x.ParkingSpace.Floor.Area.ParkingLot.ParkingLotName,
                                    Lat = x.ParkingSpace.Floor.Area.ParkingLot.Lat ?? 0,
                                    Long = x.ParkingSpace.Floor.Area.ParkingLot.Long ?? 0,
                                    ParkingLotAddress = x.ParkingSpace.Floor.Area.ParkingLot.Address,
                                    AreaName = x.ParkingSpace.Floor.Area.AreaName,
                                    FloorName = x.ParkingSpace.Floor.FloorName,
                                    Car = new CarResponse
                                    {
                                        CarId = x.Car.CarId,
                                        Thumbnail = x.Car.Thumbnail,
                                        Brand = x.Car.Brand,
                                        LicensePlate = x.Car.LicensePlate,
                                        Model = x.Car.Model,
                                        Color = x.Car.Color,
                                        CustomerName = $"{x.Car.Customer.FirstName} {x.Car.Customer.LastName}",
                                        CustomerId = x.Car.CustomerId
                                    },
                                    TotalAllPayments = x.PaymentContracts.Sum(pc => pc.PaymentAmount),
                                    CreatedDate = x.CreatedDate,
                                    UpdatedDate = x.UpdatedDate,
                                    NeedToProcess = x.PaymentContracts.Any(pc => pc.Status == (int)PaymentContractStatus.Pending) ||
                                                    x.PaymentContracts.Any(pc => pc.Status == (int)PaymentContractStatus.Paid)
                                })
                                .ToListAsync();


            return result;
        }

        public async Task<List<PaymentContractResponse>> GetPaymentContracts(int contractId)
        {
            var result = await _paymentContractRepository.GetAll()
                                                         .Where(x => x.ContractId == contractId)
                                                         .Select(x => new PaymentContractResponse
                                                         {
                                                             PaymentContractId = x.PaymentContractId,
                                                             StartDate = x.StartDate.ToDateTime(TimeOnly.MinValue),
                                                             EndDate = x.EndDate.ToDateTime(TimeOnly.MinValue),
                                                             Status = ((PaymentContractStatus)x.Status).ToString(),
                                                             PricePerMonth = x.PricePerMonth,
                                                             PaymentAmount = x.PaymentAmount,
                                                             PaymentMethod = x.PaymentMethod,
                                                             PaymentDate = x.PaymentDate.HasValue ? x.PaymentDate.Value.ToString("dd/MM/yyyy HH:mm:ss") : "",
                                                             Note = x.Note,
                                                             CreatedDate = x.CreatedDate
                                                         }).ToListAsync();

            return result.OrderByDescending(x => x.CreatedDate).ToList();
        }

        public async Task<Contract> AddContractAsync(AddContractRequest request)
        {
            var existCar = await _carRepository.CarIdExsistAsync(request.CarId);
            if (!existCar) throw AppExceptions.NotFoundId();

            var parkingLot = await _parkingLotRepository.GetById(request.ParkingLotId);

            if (parkingLot == null) throw AppExceptions.NotFoundParkingLot();

            var parkingSpaceId = await FindParkingSpace(request.ParkingLotId, request.StartDate, request.EndDate);

            if (parkingSpaceId == 0) throw AppExceptions.NotFoundParkingSpace();

            var newContract = new Contract()
            {
                CarId = request.CarId,
                ParkingSpaceId = parkingSpaceId,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                Status = (int)ContractStatus.Inactive,
                CreatedDate = DateTime.Now.ToVNTime(),
                UpdatedDate = DateTime.Now.ToVNTime(),
                Note = request.Note,
                PaymentContracts = new List<PaymentContract>()
                {
                    new PaymentContract()
                    {
                        StartDate = request.StartDate,
                        EndDate = request.EndDate,
                        Status = (int)PaymentContractStatus.Pending,
                        CreatedDate = DateTime.Now.ToVNTime(),
                        UpdatedDate = DateTime.Now.ToVNTime(),
                        PricePerMonth = parkingLot.PricePerMonth,
                        PaymentAmount = parkingLot.PricePerMonth * CalculateMonthsBetween(request.StartDate, request.EndDate),
                        PaymentMethod = "",
                        Note = request.Note
                    }
                }
            };

            return await _contractRepository.Insert(newContract);
        }

        private async Task<int> FindParkingSpace(int parkingLotId, DateOnly startDate, DateOnly endDate)
        {
            _logger.LogInformation($"Finding parking space for parking lot {parkingLotId} from {startDate} to {endDate}");

            // Get all active contracts for the specified parking lot
            var activeContracts = await _contractRepository
                                                .GetAll()
                                                .Where(c => (c.Status != (int)ContractStatus.Canceled && c.Status != (int)ContractStatus.Expired) &&
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

            updateContract.UpdatedDate = DateTime.Now.ToVNTime();
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

        public async Task<bool> RejectPaymentContract(int paymentContractId, string reason)
        {
            var paymentContract = await _paymentContractRepository.GetById(paymentContractId);

            if (paymentContract == null) return false;

            if (paymentContract.Status == (int)PaymentContractStatus.Completed)
            {
                throw AppExceptions.PaymentAlreadyCompleted();
            }

            if (paymentContract.Status == (int)PaymentContractStatus.Pending)
            {
                paymentContract.Note = reason;
                paymentContract.Status = (int)PaymentContractStatus.Rejected;
                paymentContract.UpdatedDate = DateTime.Now.ToVNTime();

                await _paymentContractRepository.Update(paymentContract);
            }

            return true;
        }

        public async Task<bool> ApprovePaymentContract(int paymentContractId)
        {
            var paymentContract = await _paymentContractRepository.GetById(paymentContractId);

            if (paymentContract == null) return false;
            if (paymentContract.Status == (int)PaymentContractStatus.Completed)
            {
                throw AppExceptions.PaymentAlreadyCompleted();
            }

            if (paymentContract.Status == (int)PaymentContractStatus.Pending)
            {
                paymentContract.Status = (int)PaymentContractStatus.Approved;
                paymentContract.UpdatedDate = DateTime.Now.ToVNTime();

                await _paymentContractRepository.Update(paymentContract);
            }

            return false;
        }

        public async Task<bool> PayPaymentContract(int paymentContractId)
        {
            var paymentContract = await _paymentContractRepository.GetById(paymentContractId);

            if (paymentContract == null) return false;

            if (paymentContract.Status == (int)PaymentContractStatus.Completed)
            {
                throw AppExceptions.PaymentAlreadyCompleted();
            }

            if (paymentContract.Status == (int)PaymentContractStatus.Approved)
            {
                paymentContract.Status = (int)PaymentContractStatus.Paid;
                paymentContract.PaymentDate = DateTime.Now.ToVNTime();
                paymentContract.UpdatedDate = DateTime.Now.ToVNTime();

                await _paymentContractRepository.Update(paymentContract);

                return true;
            }

            return false;
        }

        public async Task<bool> AcceptPaymentContract(int paymentContractId)
        {
            var paymentContract = await _paymentContractRepository.GetById(paymentContractId);

            if (paymentContract == null) return false;

            if (paymentContract.Status == (int)PaymentContractStatus.Completed)
            {
                throw AppExceptions.PaymentAlreadyCompleted();
            }

            if (paymentContract.Status == (int)PaymentContractStatus.Paid)
            {
                paymentContract.Status = (int)PaymentContractStatus.Completed;
                paymentContract.UpdatedDate = DateTime.Now.ToVNTime();

                var contract = await _contractRepository.GetById(paymentContract.ContractId);

                contract.UpdatedDate = DateTime.Now.ToVNTime();

                contract.Status = (int)ContractStatus.Active;
                contract.EndDate = paymentContract.EndDate;

                var parkingSpace = await _parkingSpaceRepository.GetById(contract.ParkingSpaceId);

                // Hợp đồng mới thì mới cập nhật status cho parking space
                if (parkingSpace.Status == (int)ParkingSpaceStatus.Available)
                {
                    parkingSpace.Status = (int)ParkingSpaceStatus.Reserved;
                }

                await _contractRepository.Update(contract);
            }

            return true;
        }

        public async Task<List<ContractResponse>> GetPendingContracts(GetContractsRequest request)
        {
            var paymentContracts = await _paymentContractRepository.GetAll()
                                                   .Include(x => x.Contract)
                                                   .Include(x => x.Contract.Car)
                                                   .Include(x => x.Contract.ParkingSpace)
                                                   .Include(x => x.Contract.ParkingSpace.Floor)
                                                   .Include(x => x.Contract.ParkingSpace.Floor.Area)
                                                   .Include(x => x.Contract.ParkingSpace.Floor.Area.ParkingLot)
                                                   .Include(x => x.Contract.Car.Customer)
                                                   .Where(x => x.Contract.Car.CustomerId == request.CustomerId)
                                                   .OrderByDescending(x => x.CreatedDate)
                                                   .GroupBy(x => x.ContractId)
                                                   .Where(x => x.FirstOrDefault(pc => pc.Status == (int)PaymentContractStatus.Pending) != null)
                                                   .Select(x => x.FirstOrDefault(pc => pc.Status == (int)PaymentContractStatus.Pending))
                                                   .ToListAsync(); // Switch to client evaluation

            return paymentContracts.Select(x => new ContractResponse
            {

                ContractId = x.ContractId,
                StartDate = x.StartDate.ToDateTime(TimeOnly.MinValue),
                EndDate = x.EndDate.ToDateTime(TimeOnly.MaxValue),
                Status = ((PaymentContractStatus)x.Status).ToString(),
                Note = x.Note,
                ParkingSpaceId = x.Contract.ParkingSpaceId,
                ParkingSpaceName = x.Contract.ParkingSpace.ParkingSpaceName,
                Lat = x.Contract.ParkingSpace.Floor.Area.ParkingLot.Lat ?? 0,
                Long = x.Contract.ParkingSpace.Floor.Area.ParkingLot.Long ?? 0,
                ParkingLotId = x.Contract.ParkingSpace.Floor.Area.ParkingLotId,
                ParkingLotName = x.Contract.ParkingSpace.Floor.Area.ParkingLot.ParkingLotName,
                ParkingLotAddress = x.Contract.ParkingSpace.Floor.Area.ParkingLot.Address,
                AreaName = x.Contract.ParkingSpace.Floor.Area.AreaName,
                FloorName = x.Contract.ParkingSpace.Floor.FloorName,
                Car = new CarResponse
                {
                    CarId = x.Contract.Car.CarId,
                    Thumbnail = x.Contract.Car.Thumbnail,
                    LicensePlate = x.Contract.Car.LicensePlate,
                    Brand = x.Contract.Car.Brand,
                    Model = x.Contract.Car.Model,
                    Color = x.Contract.Car.Color,
                    CustomerName = $"{x.Contract.Car.Customer.FirstName} {x.Contract.Car.Customer.LastName}",
                    CustomerId = x.Contract.Car.CustomerId
                },
                PricePerMonth = x.PricePerMonth,
                PaymentContractId = x.PaymentContractId,
                TotalAmount = x.PaymentAmount
            }).ToList();
        }

        public async Task<List<ContractResponse>> GetApprovedContracts(GetContractsRequest request)
        {
            var paymentContracts = await _paymentContractRepository.GetAll()
                                                   .Include(x => x.Contract)
                                                   .Include(x => x.Contract.Car)
                                                   .Include(x => x.Contract.ParkingSpace)
                                                   .Include(x => x.Contract.ParkingSpace.Floor)
                                                   .Include(x => x.Contract.ParkingSpace.Floor.Area)
                                                   .Include(x => x.Contract.ParkingSpace.Floor.Area.ParkingLot)
                                                   .Include(x => x.Contract.Car.Customer)
                                                   .Where(x => x.Contract.Car.CustomerId == request.CustomerId)
                                                   .OrderByDescending(x => x.CreatedDate)
                                                   .GroupBy(x => x.ContractId)
                                                   .Where(x => x.FirstOrDefault(pc => pc.Status == (int)PaymentContractStatus.Approved) != null)
                                                   .Select(x => x.FirstOrDefault(pc => pc.Status == (int)PaymentContractStatus.Approved))
                                                   .ToListAsync(); // Switch to client evaluation

            return paymentContracts.Select(x => new ContractResponse
            {
                ContractId = x.ContractId,
                StartDate = x.StartDate.ToDateTime(TimeOnly.MinValue),
                EndDate = x.EndDate.ToDateTime(TimeOnly.MaxValue),
                Status = ((PaymentContractStatus)x.Status).ToString(),
                Note = x.Note,
                ParkingSpaceId = x.Contract.ParkingSpaceId,
                ParkingSpaceName = x.Contract.ParkingSpace.ParkingSpaceName,
                Lat = x.Contract.ParkingSpace.Floor.Area.ParkingLot.Lat ?? 0,
                Long = x.Contract.ParkingSpace.Floor.Area.ParkingLot.Long ?? 0,
                ParkingLotId = x.Contract.ParkingSpace.Floor.Area.ParkingLotId,
                ParkingLotName = x.Contract.ParkingSpace.Floor.Area.ParkingLot.ParkingLotName,
                ParkingLotAddress = x.Contract.ParkingSpace.Floor.Area.ParkingLot.Address,
                AreaName = x.Contract.ParkingSpace.Floor.Area.AreaName,
                FloorName = x.Contract.ParkingSpace.Floor.FloorName,
                Car = new CarResponse
                {
                    CarId = x.Contract.Car.CarId,
                    Thumbnail = x.Contract.Car.Thumbnail,
                    LicensePlate = x.Contract.Car.LicensePlate,
                    Brand = x.Contract.Car.Brand,
                    Model = x.Contract.Car.Model,
                    Color = x.Contract.Car.Color,
                    CustomerName = $"{x.Contract.Car.Customer.FirstName} {x.Contract.Car.Customer.LastName}",
                    CustomerId = x.Contract.Car.CustomerId
                },
                PricePerMonth = x.PricePerMonth,
                PaymentContractId = x.PaymentContractId,
                TotalAmount = x.PaymentAmount
            }).ToList();
        }

        public async Task<List<ContractResponse>> GetActivatedContracts(GetContractsRequest request)
        {
            var now = DateTime.Now.ToVNTime();

            return await _contractRepository.GetAll()
                                            .Include(x => x.PaymentContracts)
                                            .Include(x => x.Car).ThenInclude(x => x.Customer)
                                            .Include(x => x.ParkingSpace).ThenInclude(x => x.Floor).ThenInclude(x => x.Area).ThenInclude(x => x.ParkingLot)
                                            .Where(x => x.Status == (int)ContractStatus.Active && (request.CustomerId == 0 || x.Car.CustomerId == request.CustomerId))
                                            .Select(x => new ContractResponse
                                            {
                                                ContractId = x.ContractId,
                                                StartDate = x.StartDate.ToDateTime(TimeOnly.MinValue),
                                                EndDate = x.EndDate.ToDateTime(TimeOnly.MaxValue),
                                                Status = now < x.StartDate.ToDateTime(TimeOnly.MinValue) ? ContractStatus.PendingActivation.ToString() : ((ContractStatus)x.Status).ToString(),
                                                Note = x.Note,
                                                ParkingSpaceId = x.ParkingSpaceId,
                                                ParkingSpaceName = x.ParkingSpace.ParkingSpaceName,
                                                Lat = x.ParkingSpace.Floor.Area.ParkingLot.Lat ?? 0,
                                                Long = x.ParkingSpace.Floor.Area.ParkingLot.Long ?? 0,
                                                ParkingLotId = x.ParkingSpace.Floor.Area.ParkingLotId,
                                                ParkingLotName = x.ParkingSpace.Floor.Area.ParkingLot.ParkingLotName,
                                                ParkingLotAddress = x.ParkingSpace.Floor.Area.ParkingLot.Address,
                                                AreaName = x.ParkingSpace.Floor.Area.AreaName,
                                                FloorName = x.ParkingSpace.Floor.FloorName,
                                                Car = new CarResponse
                                                {
                                                    CarId = x.Car.CarId,
                                                    LicensePlate = x.Car.LicensePlate,
                                                    Brand = x.Car.Brand,
                                                    Model = x.Car.Model,
                                                    Color = x.Car.Color,
                                                    CustomerName = $"{x.Car.Customer.FirstName} {x.Car.Customer.LastName}",
                                                    CustomerId = x.Car.CustomerId
                                                },
                                                TotalAllPayments = x.PaymentContracts.Where(pc => pc.Status == (int)PaymentContractStatus.Completed).Sum(pc => pc.PaymentAmount)
                                            })
                                            .ToListAsync();
        }

        public async Task<List<ContractResponse>> GetRejectedContracts(GetContractsRequest request)
        {
            var paymentContracts = await _paymentContractRepository.GetAll()
                                                   .Include(x => x.Contract)
                                                   .Include(x => x.Contract.Car)
                                                   .Include(x => x.Contract.ParkingSpace)
                                                   .Include(x => x.Contract.ParkingSpace.Floor)
                                                   .Include(x => x.Contract.ParkingSpace.Floor.Area)
                                                   .Include(x => x.Contract.ParkingSpace.Floor.Area.ParkingLot)
                                                   .Include(x => x.Contract.Car.Customer)
                                                   .Where(x => request.CustomerId == 0 || x.Contract.Car.CustomerId == request.CustomerId)
                                                   .OrderByDescending(x => x.CreatedDate)
                                                   .GroupBy(x => x.ContractId)
                                                   .Where(x => x.FirstOrDefault(pc => pc.Status == (int)PaymentContractStatus.Rejected) != null)
                                                   .Select(x => x.FirstOrDefault(pc => pc.Status == (int)PaymentContractStatus.Rejected))
                                                   .ToListAsync(); // Switch to client evaluation

            return paymentContracts.Select(x => new ContractResponse
            {
                ContractId = x.ContractId,
                StartDate = x.StartDate.ToDateTime(TimeOnly.MinValue),
                EndDate = x.EndDate.ToDateTime(TimeOnly.MaxValue),
                Status = ((PaymentContractStatus)x.Status).ToString(),
                Note = x.Note,
                ParkingSpaceId = x.Contract.ParkingSpaceId,
                ParkingSpaceName = x.Contract.ParkingSpace.ParkingSpaceName,
                Lat = x.Contract.ParkingSpace.Floor.Area.ParkingLot.Lat ?? 0,
                Long = x.Contract.ParkingSpace.Floor.Area.ParkingLot.Long ?? 0,
                ParkingLotId = x.Contract.ParkingSpace.Floor.Area.ParkingLotId,
                ParkingLotName = x.Contract.ParkingSpace.Floor.Area.ParkingLot.ParkingLotName,
                ParkingLotAddress = x.Contract.ParkingSpace.Floor.Area.ParkingLot.Address,
                AreaName = x.Contract.ParkingSpace.Floor.Area.AreaName,
                FloorName = x.Contract.ParkingSpace.Floor.FloorName,
                Car = new CarResponse
                {
                    CarId = x.Contract.Car.CarId,
                    Thumbnail = x.Contract.Car.Thumbnail,
                    LicensePlate = x.Contract.Car.LicensePlate,
                    Brand = x.Contract.Car.Brand,
                    Model = x.Contract.Car.Model,
                    Color = x.Contract.Car.Color,
                    CustomerName = $"{x.Contract.Car.Customer.FirstName} {x.Contract.Car.Customer.LastName}",
                    CustomerId = x.Contract.Car.CustomerId
                },
                PricePerMonth = x.PricePerMonth,
                PaymentContractId = x.PaymentContractId,
                TotalAmount = x.PaymentAmount
            }).ToList();
        }

        public async Task<List<ContractResponse>> GetPaidContracts(GetContractsRequest request)
        {
            var paymentContracts = await _paymentContractRepository.GetAll()
                                                   .Include(x => x.Contract)
                                                   .Include(x => x.Contract.Car)
                                                   .Include(x => x.Contract.ParkingSpace)
                                                   .Include(x => x.Contract.ParkingSpace.Floor)
                                                   .Include(x => x.Contract.ParkingSpace.Floor.Area)
                                                   .Include(x => x.Contract.ParkingSpace.Floor.Area.ParkingLot)
                                                   .Include(x => x.Contract.Car.Customer)
                                                   .Where(x => request.CustomerId == 0 || x.Contract.Car.CustomerId == request.CustomerId)
                                                   .OrderByDescending(x => x.CreatedDate)
                                                   .GroupBy(x => x.ContractId)
                                                   .Where(x => x.FirstOrDefault(pc => pc.Status == (int)PaymentContractStatus.Paid) != null)
                                                   .Select(x => x.FirstOrDefault(pc => pc.Status == (int)PaymentContractStatus.Paid))
                                                   .ToListAsync(); // Switch to client evaluation

            return paymentContracts.Select(x => new ContractResponse
            {
                ContractId = x.ContractId,
                StartDate = x.StartDate.ToDateTime(TimeOnly.MinValue),
                EndDate = x.EndDate.ToDateTime(TimeOnly.MaxValue),
                Status = ((PaymentContractStatus)x.Status).ToString(),
                Note = x.Note,
                ParkingSpaceId = x.Contract.ParkingSpaceId,
                ParkingSpaceName = x.Contract.ParkingSpace.ParkingSpaceName,
                Lat = x.Contract.ParkingSpace.Floor.Area.ParkingLot.Lat ?? 0,
                Long = x.Contract.ParkingSpace.Floor.Area.ParkingLot.Long ?? 0,
                ParkingLotId = x.Contract.ParkingSpace.Floor.Area.ParkingLotId,
                ParkingLotName = x.Contract.ParkingSpace.Floor.Area.ParkingLot.ParkingLotName,
                ParkingLotAddress = x.Contract.ParkingSpace.Floor.Area.ParkingLot.Address,
                AreaName = x.Contract.ParkingSpace.Floor.Area.AreaName,
                FloorName = x.Contract.ParkingSpace.Floor.FloorName,
                PricePerMonth = x.PricePerMonth,
                PaymentContractId = x.PaymentContractId,
                TotalAmount = x.PaymentAmount,
                Car = new CarResponse
                {
                    CarId = x.Contract.Car.CarId,
                    Thumbnail = x.Contract.Car.Thumbnail,
                    LicensePlate = x.Contract.Car.LicensePlate,
                    Brand = x.Contract.Car.Brand,
                    Model = x.Contract.Car.Model,
                    Color = x.Contract.Car.Color,
                    CustomerName = $"{x.Contract.Car.Customer.FirstName} {x.Contract.Car.Customer.LastName}",
                    CustomerId = x.Contract.Car.CustomerId
                }
            }).ToList();
        }

        public async Task<bool> Renew(RenewRequest request)
        {
            var contract = await _contractRepository.GetById(request.ContractId);

            if (contract == null) throw AppExceptions.NotFoundId();

            var pricePerMonth = await _parkingSpaceRepository
                                            .GetAll()
                                            .Include(x => x.Floor).ThenInclude(x => x.Area).ThenInclude(x => x.ParkingLot)
                                            .Where(x => x.ParkingSpaceId == contract.ParkingSpaceId)
                                            .Select(x => x.Floor.Area.ParkingLot.PricePerMonth)
                                            .FirstOrDefaultAsync();

            var startDate = contract.EndDate.AddDays(1);
            var endDate = contract.EndDate.AddDays(1).AddMonths(request.NumberMonth);

            var paymentContract = new PaymentContract
            {
                PricePerMonth = pricePerMonth,
                StartDate = startDate,
                EndDate = endDate,
                ContractId = contract.ContractId,
                Status = (int)PaymentContractStatus.Pending,
                CreatedDate = DateTime.Now.ToVNTime(),
                UpdatedDate = DateTime.Now.ToVNTime(),
                PaymentAmount = pricePerMonth * CalculateMonthsBetween(startDate, endDate),
                PaymentMethod = "",
                Note = "Gia hạn hợp đồng"
            };

            await _paymentContractRepository.Insert(paymentContract);

            return true;
        }

        public async Task<bool> PayPaymentContract(int paymentContractId, string paymentMethod)
        {
            var paymentContract = await _paymentContractRepository.GetById(paymentContractId);

            paymentContract.PaymentDate = DateTime.Now.ToVNTime();
            paymentContract.PaymentMethod = paymentMethod;
            paymentContract.Status = (int)PaymentContractStatus.Paid;
            paymentContract.UpdatedDate = DateTime.Now.ToVNTime();

            await _paymentContractRepository.Update(paymentContract);

            return await AcceptPaymentContract(paymentContractId);
        }

        public async Task<PaymentContract> GetPaymentContract(int paymentContractId)
        {
            return await _paymentContractRepository.GetById(paymentContractId);
        }

        public async Task<List<ContractNeedToProcessResponse>> GetNeedToProcess()
        {
            return await _paymentContractRepository.GetAll()
                                                   .Include(x => x.Contract).ThenInclude(x => x.Car).ThenInclude(x => x.Customer)
                                                   .Include(x => x.Contract).ThenInclude(x => x.ParkingSpace).ThenInclude(x => x.Floor).ThenInclude(x => x.Area)
                                                   .Where(x => x.Status == (int)PaymentContractStatus.Pending || x.Status == (int)PaymentContractStatus.Paid)
                                                    .Select(x => new ContractNeedToProcessResponse
                                                    {
                                                        Id = x.ContractId,
                                                        CustomerName = $"{x.Contract.Car.Customer.FirstName} {x.Contract.Car.Customer.LastName}",
                                                        ParkingLotName = x.Contract.ParkingSpace.Floor.Area.ParkingLot.ParkingLotName
                                                    })
                                                    .ToListAsync();

        }

        public async Task<List<ContractFutureExpiredResponse>> GetFutureExpired()
        {
            var contracts = await _contractRepository.GetAll()
                                                   .Include(x => x.Car).ThenInclude(x => x.Customer)
                                                   .Include(x => x.ParkingSpace).ThenInclude(x => x.Floor).ThenInclude(x => x.Area)
                                                   .Where(x => x.Status == (int)ContractStatus.Active)
                                                   .Select(x => new ContractFutureExpiredResponse
                                                   {
                                                        Id = x.ContractId,
                                                        CustomerName = $"{x.Car.Customer.FirstName} {x.Car.Customer.LastName}",
                                                        ParkingLotName = x.ParkingSpace.Floor.Area.ParkingLot.ParkingLotName,
                                                        ExpiredDate = x.EndDate,
                                                   })
                                                   .ToListAsync();

            contracts.ForEach(x => x.RemainingDays = (x.ExpiredDate.ToDateTime(TimeOnly.MinValue) - DateTime.Now.ToVNTime()).Days);
            
            return contracts;
        }

        public async Task<List<PaymentContractResponse>> GetPaymentHistories(int customerId)
        {
            return await _paymentContractRepository
                            .GetAll()
                            .Include(x => x.Contract).ThenInclude(x => x.Car)
                            .Where(x => x.Contract.Car.CustomerId == customerId && (x.Status == (int)PaymentContractStatus.Paid || x.Status == (int)PaymentContractStatus.Completed))
                            .Select(x => new PaymentContractResponse
                            {
                                PaymentContractId = x.PaymentContractId,
                                StartDate = x.StartDate.ToDateTime(TimeOnly.MinValue),
                                EndDate = x.EndDate.ToDateTime(TimeOnly.MinValue),
                                Status = ((PaymentContractStatus)x.Status).ToString(),
                                PricePerMonth = x.PricePerMonth,
                                PaymentAmount = x.PaymentAmount,
                                PaymentMethod = x.PaymentMethod,
                                PaymentDate = x.PaymentDate.HasValue ? x.PaymentDate.Value.ToString("dd/MM/yyyy HH:mm:ss") : "",
                                Note = x.Note,
                                CreatedDate = x.CreatedDate,
                                ContractId = x.ContractId
                            }).ToListAsync();
        }
    }
}
