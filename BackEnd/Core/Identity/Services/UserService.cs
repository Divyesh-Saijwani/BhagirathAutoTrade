using Identity.Core.Models;
using Identity.Core.Repositories;
using Identity.Core.Security.Hashing;
using Identity.Core.Security.Tokens;
using Identity.Services.Communication;

namespace Identity.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPasswordHasher _passwordHasher;
        private readonly ITokenHandler _tokenHandler;

        public UserService(IUserRepository userRepository, IUnitOfWork unitOfWork, IPasswordHasher passwordHasher,ITokenHandler tokenHandler)
        {
            _passwordHasher = passwordHasher;
            _unitOfWork = unitOfWork;
            _userRepository = userRepository;
            _tokenHandler = tokenHandler;
        }

        public async Task<CreateUserResponse> CreateUserAsync(User user)
        {
            var existingUser = await _userRepository.FindByEmailAsync(user.Email);
            if(existingUser != null)
            {
                return new CreateUserResponse(false, "Email already in use.", null);
            } 

            user.PasswordHash = _passwordHasher.HashPassword(user.PasswordHash);

            await _userRepository.AddAsync(user);
            await _unitOfWork.CompleteAsync();

            return new CreateUserResponse(true, null, user);
        }

        public async Task<User> FindByEmailAsync(string email)
        {
            return await _userRepository.FindByEmailAsync(email);
        }

        public async Task<string> GetEmailVerificationToken(string email)
        {
            return Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(_tokenHandler.GetSecurityVerificationToken(email).Token));
        }

        public async Task<IEnumerable<User>> GetUsersAsync()
        {
            return await _userRepository.GetUsersAsync();
        }

        public async Task<EmailVerificationResponse> VerifyEmail(string email,string token)
        {
            var user = await _userRepository.FindByEmailAsync(email);

            if(user.EmailConfirmed)
                return new EmailVerificationResponse(true, "Email already verified");


            if (user == null)
            {
                return new EmailVerificationResponse(false, "User not found.");
            }

            if (string.IsNullOrEmpty(token))
            {
                return new EmailVerificationResponse(false, "Invalid or missing token.");
            }

            var verificationResponse = _tokenHandler.VerifySecurityToken(System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(token)), email);

            if(verificationResponse.Item1)
            {
                user.EmailConfirmed = true;
                await _userRepository.UpdateUserDetails(user);
                return new EmailVerificationResponse(verificationResponse.Item1, "Email Verified Successfully");
            }

            return new EmailVerificationResponse(verificationResponse.Item1, verificationResponse.Item2);

        }
    }
}