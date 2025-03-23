using Microsoft.EntityFrameworkCore;
using ScpmaBe.Repositories.Entities;
using ScpmaBe.Repositories.Interfaces;
using ScpmaBe.Services.Interfaces;
using ScpmaBe.Services.Models;
using ScpmBe.Services.Exceptions;

namespace ScpmaBe.Services
{
    public class OwnerService : IOwnerService
    {
        private readonly IOwnerRepository _ownerRepository;
        private readonly IHashHelper _hashHelper;
        public OwnerService(IOwnerRepository ownerRepository, IHashHelper hashHelper)
        {
            _ownerRepository = ownerRepository;
            _hashHelper = hashHelper;
        }

        public async Task<Owner> GetById(int id)
        {
            var owner = await _ownerRepository.GetById(id);

            if (owner == null) throw AppExceptions.NotFoundId();

            return owner;
        }

        public async Task<Owner> AuthorizeAsync(string username, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                throw AppExceptions.BadRequestUsernameIsInvalid();

            var acc = await _ownerRepository.GetAll().SingleOrDefaultAsync(x => x.Username.ToLower() == username.ToLower());

            if (acc == null) 
                throw AppExceptions.NotFoundAccount();

            var passwordHash = _hashHelper.HashPassword(password);

            if (acc.Password != passwordHash) 
                throw AppExceptions.NotFoundAccount();

            return acc;
        }

        public async Task<Owner> UpdateOwnerAsync(UpdateOwnerRequest request)
        {
            var updateAcc = await _ownerRepository.GetById(request.OwnerId);

            if (updateAcc == null) throw AppExceptions.NotFoundAccount();

            updateAcc.OwnerId = request.OwnerId;

            updateAcc.Email = request.Email;
            updateAcc.FirstName = request.FirstName;
            updateAcc.LastName = request.LastName;
            updateAcc.Phone = request.Phone;
            updateAcc.IsActive = request.IsActived;

            await _ownerRepository.Update(updateAcc);

            return updateAcc;
        }
    }
}
