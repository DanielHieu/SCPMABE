using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ScpmaBe.Repositories.Entities;
using ScpmaBe.Repositories.Interfaces;
using ScpmaBe.Services.Enum;
using ScpmaBe.Services.Interfaces;
using ScpmaBe.Services.Models;
using ScpmBe.Services.Exceptions;

namespace ScpmaBe.Services
{
    public class StaffService : IStaffService
    {
        private readonly ILogger _logger;
        private readonly IHashHelper _hashHelper;
        private readonly IStaffRepository _staffRepository;
        private readonly ITaskEachRepository _taskEachRepository;

        public StaffService(
            ILogger<StaffService> logger,
            IHashHelper hashHelper,
            IStaffRepository staffRepository,
            ITaskEachRepository taskEachRepository)
        {
            _logger = logger;
            _hashHelper = hashHelper;

            _staffRepository = staffRepository;
            _taskEachRepository = taskEachRepository;
        }

        public async Task<StaffResponse> GetById(int id)
        {
            var staff = await _staffRepository.GetById(id);

            if (staff == null) throw AppExceptions.NotFoundAccount();

            return new StaffResponse
            {
                StaffId = staff.StaffId,
                Email = staff.Email,
                FirstName = staff.FirstName,
                LastName = staff.LastName,
                FullName = $"{staff.FirstName} {staff.LastName}",
                Phone = staff.Phone,
                Username = staff.Username,
                IsActive = staff.IsActive
            };
        }

        public async Task<List<StaffResponse>> SearchStaffAsync(SearchStaffRequest request)
        {
            var query = _staffRepository.GetAll();

            if (!string.IsNullOrEmpty(request.Keyword))
                query = query.Where(
                            s => s.StaffId.ToString().Contains(request.Keyword) ||
                            (!string.IsNullOrEmpty(s.FirstName) && s.FirstName.Contains(request.Keyword)) ||
                            (!string.IsNullOrEmpty(s.LastName) && s.LastName.Contains(request.Keyword)) ||
                            (!string.IsNullOrEmpty(s.Phone) && s.Phone.Contains(request.Keyword)));

            var staffs = await query.Select(s => new StaffResponse
            {
                StaffId = s.StaffId,
                FirstName = s.FirstName,
                LastName = s.LastName,
                Phone = s.Phone,
                Email = s.Email,
                Username = s.Username,
                IsActive = s.IsActive,
                FullName = $"{s.FirstName} {s.LastName}"
            }).ToListAsync();

            return staffs;
        }

        public async Task<Staff> AddStaffAsync(AddStaffRequest request)
        {
            if (string.IsNullOrEmpty(request.Username) || request.Username.Length < 4)
                throw AppExceptions.BadRequestUsernameIsInvalid();

            // Check if username already exists
            var exstingAcc = await _staffRepository.GetAll().FirstOrDefaultAsync(x => x.Username == request.Username);
            if (exstingAcc != null) throw AppExceptions.BadRequestUserExists();

            var newStaffAcc = new Staff
            {
                OwnerId = 1,
                Username = request.Username,
                Password = _hashHelper.HashPassword(request.Password),
                FirstName = request.FirstName,
                LastName = request.LastName,
                Phone = request.Phone,
                Email = request.Email,
                IsActive = request.IsActive
            };

            return await _staffRepository.Insert(newStaffAcc);
        }

        public async Task<Staff> AuthorizeAsync(string username, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                throw AppExceptions.NotFoundAccount();

            var staffAcc = await _staffRepository.GetAll().SingleOrDefaultAsync(x => x.Username == username);

            if (staffAcc == null) throw AppExceptions.NotFoundAccount();

            if(staffAcc.IsActive == false) throw AppExceptions.AccountNotActivated();

            var passwordHash = _hashHelper.HashPassword(password);

            if (staffAcc.Password != passwordHash) throw AppExceptions.NotFoundAccount();

            return staffAcc;
        }

        public async Task<Staff> UpdateStaffAsync(UpdateStaffRequest request)
        {
            var updateAcc = await _staffRepository.GetById(request.StaffId);

            if (updateAcc == null) 
                throw AppExceptions.NotFoundAccount();

            updateAcc.Email = request.Email;
            updateAcc.FirstName = request.FirstName;
            updateAcc.LastName = request.LastName;
            updateAcc.Phone = request.Phone;
            updateAcc.IsActive = request.IsActive;

            await _staffRepository.Update(updateAcc);

            return updateAcc;
        }

        public async Task<bool> DeleteStaffAsync(int id)
        {
            try
            {
                var acc = await _staffRepository.GetById(id);

                if (acc == null) 
                    throw AppExceptions.NotFoundAccount();

                await _staffRepository.Delete(id);

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<bool> ResetPassword(int staffId)
        {
            var staff = await _staffRepository.GetById(staffId);

            if (staff == null) throw AppExceptions.NotFoundAccount();

            try
            {
                var newPassword = Guid.NewGuid().ToString("N").Substring(0, 8);
                
                staff.Password = _hashHelper.HashPassword(newPassword);
                
                await _staffRepository.Update(staff);

                string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "emails", "StaffResetPasswordEmail.html");

                string htmlContent = File.ReadAllText(filePath);

                htmlContent = htmlContent.Replace("{{EmployeeName}}", $"{staff.FirstName} {staff.LastName}")
                                         .Replace("{{EmployeeEmail}}", staff.Email)
                                         .Replace("{{EmployeeUsername}}", staff.Username)
                                         .Replace("{{NewPassword}}", staff.Password);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while resetting password for staff ID: {StaffId}", staffId);

                return false;
            }

            return true;
        }

        public async Task<List<StaffResponse>> GetAll()
        {
            var staffs = await _staffRepository.GetAll()
                                        .OrderBy(x => x.LastName)
                                        .Select(x=> new StaffResponse
                                        {
                                            StaffId = x.StaffId,
                                            FirstName = x.FirstName,
                                            LastName = x.LastName,
                                            FullName = $"{x.FirstName} {x.LastName}",
                                            Phone = x.Phone,
                                            Email = x.Email,
                                            Username = x.Username,
                                        })
                                        .ToListAsync();

            return staffs;
        }

        public async Task<List<TaskEachResponse>> GetTasks(int id)
        {
            var tasks = await _taskEachRepository.GetAll()
                                                 .Where(x => x.AssignedToId == id)
                                                 .Select(x=> new TaskEachResponse
                                                 {
                                                     TaskEachId = x.TaskEachId,
                                                     Title = x.Title,
                                                     Description = x.Description,
                                                     StartDate = x.StartDate.ToString("dd/MM/yyyy"),
                                                     EndDate = x.EndDate.ToString("dd/MM/yyyy"),
                                                     Priority = ((TaskEachPriority)x.Priority).ToString(),
                                                     Status = ((TaskEachStatus)x.Status).ToString(),
                                                     AssignedToId = x.AssignedToId,
                                                     AssigneeName = "",
                                                 })
                                                 .ToListAsync();

            return tasks;
        }

        public async Task<bool> ChangePasswordAsync(StaffChangePasswordRequest request)
        {
            if (string.IsNullOrEmpty(request.NewPassword) || string.IsNullOrEmpty(request.ConfirmPassword))
                throw AppExceptions.BadRequest("Các trường mật khẩu không được để trống.");

            if (request.NewPassword != request.ConfirmPassword)
                throw AppExceptions.BadRequest("Mật khẩu không khớp.");

            var staff = await _staffRepository.GetById(request.StaffId);

            if (staff == null)
                throw AppExceptions.NotFoundAccount();

            staff.Password = _hashHelper.HashPassword(request.NewPassword);

            await _staffRepository.Update(staff);

            return true;
        }

        public async Task<List<TaskEachResponse>> GetScheduleAsync(ScheduleRquest request)
        {
            var tasks = await _taskEachRepository.GetAll()
                .Where(x => x.AssignedToId == request.StaffId
                    && x.StartDate.Date >= request.StartDate.Date
                    && x.EndDate.Date <= request.EndDate.Date)
                .Select(x => new TaskEachResponse
                {
                    TaskEachId = x.TaskEachId,
                    Title = x.Title,
                    Description = x.Description,
                    StartDate = x.StartDate.ToString("yyyy-MM-dd"),
                    EndDate = x.EndDate.ToString("yyyy-MM-dd"),
                    Priority = ((TaskEachPriority)x.Priority).ToString(),
                    Status = ((TaskEachStatus)x.Status).ToString(),
                })
                .ToListAsync();

            return tasks;
        }
    }
}
