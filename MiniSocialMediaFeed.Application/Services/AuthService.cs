namespace MiniSocialMediaFeed.Application.Services
{
    using AutoMapper;
    using Microsoft.AspNetCore.Cryptography.KeyDerivation;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Microsoft.IdentityModel.Tokens;
    using MiniSocialMediaFeed.Application.Dtos.RequestDto;
    using MiniSocialMediaFeed.Application.Dtos.ResponseDto;
    using MiniSocialMediaFeed.Application.Interfaces;
    using MiniSocialMediaFeed.Domain.Entities;
    using System.IdentityModel.Tokens.Jwt;
    using System.Net;
    using System.Security.Claims;
    using System.Security.Cryptography;
    using System.Text;

    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly AppSettings _appSettings;
        private readonly ILogger<AuthService> _logger;

        public AuthService(IUnitOfWork unitOfWork, IMapper mapper, IOptions<AppSettings> appSettings, ILogger<AuthService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _appSettings = appSettings.Value;
            _logger = logger;
        }

        public async Task<ResponseDto<UserDto>> RegisterUserAsync(RegisterUserReqDto registerUserDto)
        {
            _logger.LogInformation("Registering user with username: {Username}", registerUserDto.Username);
            var existingUser = await _unitOfWork.Users.GetAsync(u => u.Username == registerUserDto.Username);
            if (existingUser.Any())
            {
                _logger.LogWarning("Username {Username} already exists.", registerUserDto.Username);
                return ResponseDto<UserDto>.Fail("Username already exists.", (int)HttpStatusCode.Conflict);
            }

            var user = new User
            {
                Username = registerUserDto.Username,
                PasswordHash = HashPassword(registerUserDto.Password)
            };

            await _unitOfWork.Users.AddAsync(user);
            await _unitOfWork.SaveAsync();

            var userDto = _mapper.Map<UserDto>(user);
            _logger.LogInformation("User {Username} registered successfully.", registerUserDto.Username);
            return ResponseDto<UserDto>.Success("User registered successfully.", userDto, (int)HttpStatusCode.Created);
        }

        public async Task<ResponseDto<string>> LoginUserAsync(LoginUserReqDto loginUserDto)
        {
            _logger.LogInformation("User login attempt for username: {Username}", loginUserDto.Username);
            var user = (await _unitOfWork.Users.GetAsync(u => u.Username == loginUserDto.Username)).FirstOrDefault();
            if (user == null || !VerifyPassword(loginUserDto.Password, user.PasswordHash))
            {
                _logger.LogWarning("Invalid login attempt for username: {Username}", loginUserDto.Username);
                return ResponseDto<string>.Fail("Invalid username or password.", (int)HttpStatusCode.Unauthorized);
            }

            var token = GenerateJwtToken(user);
            _logger.LogInformation("User {Username} logged in successfully.", loginUserDto.Username);
            return ResponseDto<string>.Success("Login successful.", token);
        }

        private string HashPassword(string password)
        {
            var salt = new byte[128 / 8];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            var hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 10000,
                numBytesRequested: 256 / 8));

            return $"{Convert.ToBase64String(salt)}.{hashed}";
        }

        private bool VerifyPassword(string password, string storedHash)
        {
            var parts = storedHash.Split('.');
            if (parts.Length != 2)
            {
                throw new FormatException("Unexpected hash format.");
            }

            var salt = Convert.FromBase64String(parts[0]);
            var storedPasswordHash = parts[1];

            var hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 10000,
                numBytesRequested: 256 / 8));

            return hashed == storedPasswordHash;
        }

        private string GenerateJwtToken(User user)
        {
            var key = new byte[32];
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(key);
            }

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
            };

            var symmetricSecurityKey = new SymmetricSecurityKey(key);
            var creds = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _appSettings.Jwt.Issuer,
                audience: _appSettings.Jwt.Audience,
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
