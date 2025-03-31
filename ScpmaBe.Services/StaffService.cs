using Microsoft.EntityFrameworkCore;
using ScpmaBe.Repositories.Entities;
using ScpmaBe.Repositories.Interfaces;
using ScpmaBe.Services.Interfaces;
using ScpmaBe.Services.Models;
using ScpmBe.Services.Exceptions;

namespace ScpmaBe.Services
{
    public class StaffService : IStaffService
    {
        private readonly IStaffRepository _staffRepository;
        private readonly IOwnerRepository _ownerRepository;
        private readonly IHashHelper _hashHelper;

        public StaffService(IStaffRepository staffRepository, IHashHelper hashHelper, IOwnerRepository ownerRepository)
        {
            _staffRepository = staffRepository;
            _hashHelper = hashHelper;
            _ownerRepository = ownerRepository;
        }

        public async Task<List<Staff>> GetPaging(int pageIndex, int pageSize)
        {
            return await _staffRepository.GetAll().Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
        }

        public async Task<Staff> GetById(int id)
        {
            var staff = await _staffRepository.GetById(id);

            if (staff == null) throw AppExceptions.NotFoundAccount();

            return staff;
        }

        public async Task<List<Staff>> SearchStaffAsync(SearchStaffRequest request)
        {
            var query = _staffRepository.GetAll();

            if (!string.IsNullOrEmpty(request.Keyword))
                query = query.Where(
                            s => s.StaffId.ToString().Contains(request.Keyword) ||
                            (!string.IsNullOrEmpty(s.FirstName) && s.FirstName.Contains(request.Keyword)) ||
                            (!string.IsNullOrEmpty(s.LastName) && s.LastName.Contains(request.Keyword)) ||
                            (!string.IsNullOrEmpty(s.Phone) && s.Phone.Contains(request.Keyword)));

            var staffs = await query.Select(s => new Staff
            {
                StaffId = s.StaffId,
                OwnerId = s.OwnerId,
                FirstName = s.FirstName,
                LastName = s.LastName,
                Phone = s.Phone,
                Email = s.Email,
                Username = s.Username,
                IsActive = s.IsActive
            }).ToListAsync();

            return staffs;
        }

        public async Task<Staff> RegisterStaffAsync(RegisterStaffRequest request)
        {
            if (string.IsNullOrEmpty(request.Username) || request.Username.Length < 4)
                throw AppExceptions.BadRequestUsernameIsInvalid();

            var ownerExists = await _ownerRepository.ExistsByIdAsync(request.OwnerId);
            if (!ownerExists) throw AppExceptions.NotFoundAccountId();

            // Check if username already exists
            var exstingAcc = await _staffRepository.GetAll().FirstOrDefaultAsync(x => x.Username == request.Username);
            if (exstingAcc != null) throw AppExceptions.BadRequestUserExists();

            var newStaffAcc = new Staff
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

            return await _staffRepository.Insert(newStaffAcc);
        }

        public async Task<Staff> AuthorizeAsync(string username, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                throw AppExceptions.NotFoundAccount();

            var staffAcc = await _staffRepository.GetAll().SingleOrDefaultAsync(x => x.Username == username);

            if (staffAcc == null) throw AppExceptions.NotFoundAccount();

            var passwordHash = _hashHelper.HashPassword(password);

            if (staffAcc.Password != passwordHash) throw AppExceptions.NotFoundAccount();

            return staffAcc;
        }

        public async Task<Staff> UpdateStaffAsync(UpdateStaffRequest request)
        {
            var ownerExists = await _ownerRepository.ExistsByIdAsync(request.OwnerId);

            if (!ownerExists)
            {
                throw AppExceptions.NotFoundAccountId();
            }

            var updateAcc = await _staffRepository.GetById(request.StaffAccountId);

            if (updateAcc == null) throw AppExceptions.NotFoundAccount();

            updateAcc.StaffId = request.StaffAccountId;
            updateAcc.OwnerId = request.OwnerId;
            updateAcc.Email = request.Email;
            updateAcc.FirstName = request.FirstName;
            updateAcc.LastName = request.LastName;
            updateAcc.Phone = request.Phone;
            updateAcc.IsActive = request.IsActived;

            await _staffRepository.Update(updateAcc);
            return updateAcc;
        }

        public async Task<bool> DeleteStaffAsync(int id)
        {
            try
            {
                var acc = await _staffRepository.GetById(id);
                if (acc == null) throw AppExceptions.NotFoundAccount();

                await _staffRepository.Delete(id);
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
