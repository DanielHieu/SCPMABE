using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ScpmaBe.Repositories.Entities;
using ScpmaBe.Repositories.Interfaces;
using ScpmaBe.Services.Common;
using ScpmaBe.Services.Helpers;
using ScpmaBe.Services.Interfaces;
using ScpmaBe.Services.Models;
using ScpmBe.Services.Exceptions;

namespace ScpmaBe.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly ILogger _logger;
        private readonly ICustomerRepository _customerRepository;
        private readonly IHashHelper _hashHelper;
        private readonly IEmailHelper _emailHelper;
        private readonly AppSettings _appSettings;

        public CustomerService(
            ILogger<CustomerService> logger,
            ICustomerRepository customerRepository, 
            IHashHelper hashHelper, 
            IEmailHelper emailHelper,
            AppSettings appSettings)
        {
            _logger = logger;

            _customerRepository = customerRepository;
            _hashHelper = hashHelper;

            _emailHelper = emailHelper;
            _appSettings = appSettings;
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

            return customer;
        }

        public async Task<List<CustomerResponse>> SearchCustomerAsync(SearchCustomerRequest request)
        {
            var query = _customerRepository.GetAll();

            if (!string.IsNullOrEmpty(request.Keyword))
                query = query.Where(
                        s => s.CustomerId.ToString().Contains(request.Keyword) ||
                            (!string.IsNullOrEmpty(s.FirstName) && s.FirstName.Contains(request.Keyword)) ||
                            (!string.IsNullOrEmpty(s.LastName) && s.LastName.Contains(request.Keyword)) ||
                            (!string.IsNullOrEmpty(s.Phone) && s.Phone.Contains(request.Keyword)));

            var customers = await query.Select(s => new CustomerResponse
            {
                CustomerId = s.CustomerId,
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

            _logger.LogInformation("Register customer: {Customer}", JsonSerializer.Serialize(request));

            // Check if username already exists
            var exstingAcc = await _customerRepository.GetAll().FirstOrDefaultAsync(x => x.Username == request.Username || x.Email == request.Email);

            if (exstingAcc != null) throw AppExceptions.BadRequestUserExists();

            var activationCode = Guid.NewGuid().ToString("N");

            var newAcc = new Customer
            {
                OwnerId = 1,
                Username = request.Username,
                Password = _hashHelper.HashPassword(request.Password),
                FirstName = request.FirstName,
                LastName = request.LastName,
                Phone = request.Phone,
                Email = request.Email,
                IsActive = false,
                ActivationCode = activationCode
            };

            var customer =  await _customerRepository.Insert(newAcc);

            // Gửi mail chúc mừng khách hàng đã tạo tài khoản
            try
            {
                var emailTemplatePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "emails", "AccountCreatedEmail.html");
             
                var htmlContent = await File.ReadAllTextAsync(emailTemplatePath);
                
                var content = htmlContent.Replace("{{UserName}}", customer.Username)
                                         .Replace("{{UserEmail}}", customer.Email)
                                         .Replace("{{CreatedDate}}", DateTime.Now.ToVNTime().ToString("dd/MM/yyyy"))
                                         .Replace("{{ActivationLink}}", $"{_appSettings.LandingPageUrl}/activate/{activationCode}");

                _logger.LogInformation("Sending account created email to {Email}", customer.Email);

                await _emailHelper.SendEmailAsync(customer.Email, "Tài khoản đã được tạo thành công", content, true, null, null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while sending account created email.");
            }

            return customer;
        }

        public async Task<Customer> AuthorizeAsync(string username, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                throw AppExceptions.NotFoundAccount();

            var acc = await _customerRepository.GetAll().SingleOrDefaultAsync(x => x.Username == username);

            if (acc == null) throw AppExceptions.NotFoundAccount();

            if (acc.IsActive == false) throw AppExceptions.AccountNotActivated();

            var passwordHash = _hashHelper.HashPassword(password);
            var isValidPassword = acc.Password == passwordHash;

            if(isValidPassword == false && !string.IsNullOrEmpty(acc.PasswordTemp) && passwordHash == acc.PasswordTemp)
            {
                isValidPassword = true;
                
                acc.PasswordTemp = null;

                await _customerRepository.Update(acc);
            }

            if (!isValidPassword) throw AppExceptions.NotFoundAccount();
            
            return acc;
        }

        public async Task<Customer> UpdateCustomerAsync(UpdateCustomerRequest request)
        {
            var updateAcc = await _customerRepository.GetById(request.CustomerId);

            if (updateAcc == null) throw AppExceptions.NotFoundAccount();

            updateAcc.FirstName = request.FirstName;
            updateAcc.LastName = request.LastName;
            updateAcc.Phone = request.Phone;

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

        public async Task<bool> ApproveCustomerAsync(int customerId)
        {
            var customer = await _customerRepository.GetById(customerId);

            if (customer == null) throw AppExceptions.NotFoundAccount();

            customer.IsActive = true;

            customer.Note = "Tài khoản đã được kích hoạt bởi owner";

            await _customerRepository.Update(customer);

            try
            {
                var emailTemplatePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "emails", "AccountActivatedEmail.html");
                
                var htmlContent = await File.ReadAllTextAsync(emailTemplatePath);

                var content = htmlContent.Replace("{{UserFullName}}", $"{customer.FirstName} {customer.LastName}")
                                         .Replace("{{ActivatedTime}}", DateTime.Now.ToVNTime().ToString("dd/MM/yyyy HH:mm:ss"));

                _logger.LogInformation("Sending activation email to {Email}", customer.Email);

                await _emailHelper.SendEmailAsync(customer.Email, "Tài khoản đã kích hoạt", content, true, null, null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while sending activation email.");
            }

            return true;
        }

        public async Task<bool> DisableCustomerAsync(int customerId, string reason)
        {
            var customer = await _customerRepository.GetById(customerId);
            
            if (customer == null) throw AppExceptions.NotFoundAccount();

            customer.IsActive = false;
            customer.Note = reason;

            await _customerRepository.Update(customer);

            try
            {
                var emailTemplatePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "emails", "AccountDisabledEmail.html");

                var htmlContent = await File.ReadAllTextAsync(emailTemplatePath);

                var content = htmlContent = htmlContent.Replace("{{UserName}}", customer.Username)
                                                 .Replace("{{UserEmail}}", customer.Email)
                                                 .Replace("{{DisabledDate}}", DateTime.Now.ToVNTime().ToString("dd/MM/yyyy"))
                                                 .Replace("{{DisabledReason}}", reason);

                _logger.LogInformation("Sending account disabled to {Email}", customer.Email);

                await _emailHelper.SendEmailAsync(customer.Email, "Tài khoản đã bị tạm dừng", content, true, null, null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while sending approval email.");
            }
           
            return true;
        }

        public async Task<bool> ForgotPasswordAsync(string email)
        {
            try
            {
                var customer = await _customerRepository.GetAll().FirstOrDefaultAsync(x => x.Email == email);

                if(customer != null)
                {
                    var tempPassword = Guid.NewGuid().ToString("N").Substring(0, 8);

                    customer.PasswordTemp = _hashHelper.HashPassword(tempPassword);

                    await _customerRepository.Update(customer);

                    var emailTemplatePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "emails", "ForgotPasswordEmail.html");

                    var htmlContent = await File.ReadAllTextAsync(emailTemplatePath);
                    

                    var content = htmlContent = htmlContent
                                                    .Replace("{{UserFullName}}", $"{customer.FirstName} {customer.LastName}")
                                                    .Replace("{{TempPassword}}", tempPassword);

                    _logger.LogInformation("Quên mật khẩu {Email}", customer.Email);

                    await _emailHelper.SendEmailAsync(customer.Email, "Quên mật khẩu", content, true, null, null);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while sending forgot email.");
            }

            return true;
        }

        public async Task<bool> ChangePasswordAsync(ChangePasswordRequest request)
        {
            if (string.IsNullOrEmpty(request.NewPassword) || string.IsNullOrEmpty(request.ConfirmPassword))
                throw AppExceptions.BadRequest("Các trường mật khẩu không được để trống.");

            if (request.NewPassword != request.ConfirmPassword)
                throw AppExceptions.BadRequest("Mật khẩu không khớp.");

            var customer = await _customerRepository.GetById(request.CustomerId);

            if (customer == null)
                throw AppExceptions.NotFoundAccount();

            customer.Password = _hashHelper.HashPassword(request.NewPassword);

            await _customerRepository.Update(customer);

            try
            {
                var emailTemplatePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "emails", "PasswordChangedEmail.html");

                var htmlContent = await File.ReadAllTextAsync(emailTemplatePath);

                var content = htmlContent.Replace("{{UserFullName}}", $"{customer.FirstName} {customer.LastName}")
                                         .Replace("{{ChangedTime}}", DateTime.Now.ToVNTime().ToString("dd/MM/yyyy HH:mm:ss"));

                _logger.LogInformation("Gửi email thay đổi mật khẩu đến {Email}", customer.Email);

                await _emailHelper.SendEmailAsync(customer.Email, "Mật khẩu của bạn đã được thay đổi", content, true, null, null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Đã xảy ra lỗi khi gửi email thay đổi mật khẩu.");
            }

            return true;
        }

        public async Task<bool> ActivateAccountAsync(string code)
        {
            var customer = await _customerRepository.GetAll().FirstOrDefaultAsync(x => x.ActivationCode == code);

            if (customer == null || customer.IsActive) throw AppExceptions.BadRequest("Tài khoản đã được kích hoạt trước đó.");

            customer.IsActive = true;
            customer.ActivationCode = null;
            customer.Note = "Tài khoản đã được kích hoạt.";

            await _customerRepository.Update(customer);

            try
            {
                var emailTemplatePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "emails", "AccountActivatedEmail.html");

                var htmlContent = await File.ReadAllTextAsync(emailTemplatePath);

                var content = htmlContent.Replace("{{UserFullName}}", $"{customer.FirstName} {customer.LastName}")
                                         .Replace("{{ActivatedTime}}", DateTime.Now.ToVNTime().ToString("dd/MM/yyyy HH:mm:ss"));

                _logger.LogInformation("Sending account activation email to {Email}", customer.Email);

                await _emailHelper.SendEmailAsync(customer.Email, "Tài khoản của bạn đã được kích hoạt", content, true, null, null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while sending account activation email.");
            }

            return true;
        }
    }
}
