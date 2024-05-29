using MiniProjectAPI.Models;
using MovieBookingAPI.Exceptions;
using MovieBookingAPI.Interfaces;
using MovieBookingAPI.Models.DTOs.Login;
using MovieBookingAPI.Models.DTOs.Register;
using System.Security.Cryptography;
using System.Text;

namespace MovieBookingAPI.Services
{
    public class UserService : IUserService
    {
        private readonly IRepository<int, User> _userRepo;
        private readonly IRepository<int, UserDetail> _userDetailsRepo;
        private readonly ITokenService _tokenService;

        public UserService(IRepository<int, UserDetail> userDetailsRepo, IRepository<int, User> userRepo, ITokenService tokenService)
        {
            _userDetailsRepo = userDetailsRepo;
            _userRepo = userRepo;
            _tokenService = tokenService;
        }
        /// <summary>
        /// Handles user login with the provided credentials.
        /// </summary>
        /// <param name="logininputDTO">DTO containing user login information.</param>
        /// <returns>A DTO containing login details and token.</returns>
        /// <exception cref="UnauthorizedUserException">Thrown when the user credentials are invalid.</exception>

        public async Task<LoginReturnDTO> Login(UserLoginDTO logininputDTO)
        {

                var userDB = await _userDetailsRepo.Get(logininputDTO.UserId);
                if (userDB == null)
                {
                    throw new Exception("Incorrect UserId or Password");
                }
                HMACSHA512 hMACSHA = new HMACSHA512(userDB.PasswordHashKey);
                var encryptedPass = hMACSHA.ComputeHash(Encoding.UTF8.GetBytes(logininputDTO.Password));
                bool isPasswordSame = ComparePassword(encryptedPass, userDB.Password);
                if (isPasswordSame)
                {
                    var user = await _userRepo.Get(logininputDTO.UserId);
                    LoginReturnDTO loginReturnDTO = MapEmployeeToLoginReturn(user);
                    return loginReturnDTO;
                }
            throw new UnauthorizedUserException("Invalid username or password");
        }

        private LoginReturnDTO MapEmployeeToLoginReturn(User user)
        {
            LoginReturnDTO returnDTO = new LoginReturnDTO();
            returnDTO.UserId = user.Id;
            returnDTO.Role = user.Role ?? "User";
            returnDTO.Token = _tokenService.GenerateToken(user);
            return returnDTO;
        }
        /// <summary>
        /// Compares the encrypted password with the stored password.
        /// </summary>
        /// <param name="encrypterPass">Encrypted password to compare.</param>
        /// <param name="password">Stored password for comparison.</param>
        /// <returns>True if passwords match; otherwise, false.</returns>

        private bool ComparePassword(byte[] encrypterPass, byte[] password)
        {
            for (int i = 0; i < encrypterPass.Length; i++)
            {
                if (encrypterPass[i] != password[i])
                {
                    return false;
                }
            }
            return true;
        }
        /// <summary>
        /// Registers a new user with the provided information.
        /// </summary>
        /// <param name="userregisterDTO">DTO containing user registration information.</param>
        /// <returns>A DTO containing the registered user details.</returns>
        /// <exception cref="UnableToRegisterException">Thrown when registration fails.</exception>

        public async Task<RegisterOutputDTO> Register(RegisterInputDTO userregisterDTO)
        {
            User user = null;
            UserDetail userdetail = null;
            RegisterOutputDTO outputDTO = null;
            try
            {
                user = MapInputwithUser(userregisterDTO);
                userdetail = MapUserToUserDetail(userregisterDTO);
                user = await _userRepo.Add(user);
                if(userdetail != null)
                {
                    userdetail.UserId = user.Id;
                    userdetail = await _userDetailsRepo.Add(userdetail);
                    var result = MapUserToRegisterOutputDTO(user);
                    return result;
                }
                throw new PasswordDoesntMatchException();
            }
            catch (Exception ex)
            {

            }
            if (userdetail != null && user == null)
                await RevertUserDetailInsert(userdetail);
            if (userdetail == null && user != null)
                await RevertUserInsert(user);
            throw new UnableToRegisterException("Not able to register at this moment Or Check if the both passwords match");

        }
        /// <summary>
        /// Reverts the insertion of user details in case of registration failure.
        /// </summary>
        /// <param name="userdetail">User details to revert.</param>

        private async Task RevertUserDetailInsert(UserDetail userdetail)
        {
            await _userDetailsRepo.Delete(userdetail.UserId);
        }
        /// <summary>
        /// Reverts the insertion of user data in case of registration failure.
        /// </summary>
        /// <param name="user">User data to revert.</param>

        private async Task RevertUserInsert(User user)
        {
            await _userRepo.Delete(user.Id);
        }
        /// <summary>
        /// Maps a user entity to a DTO for registration output.
        /// </summary>
        /// <param name="user">User entity to map.</param>
        /// <returns>A DTO containing the mapped user data.</returns>

        private RegisterOutputDTO MapUserToRegisterOutputDTO(User user)
        {
            RegisterOutputDTO outputDTO = new RegisterOutputDTO()
            {
                Id = user.Id,
                Name = user.Name,
                DateOfBirth = user.DateOfBirth,
                Phone = user.Phone,
                Image = user.Image,
                Role = user.Role,
            };
            return outputDTO;
        }
        /// <summary>
        /// Maps user input data to a user entity for registration.
        /// </summary>
        /// <param name="userregisterDTO">DTO containing user registration information.</param>
        /// <returns>A mapped user entity.</returns>

        private UserDetail? MapUserToUserDetail(RegisterInputDTO userregisterDTO)
        {
            UserDetail userdetail = new UserDetail();
            if (userregisterDTO.Password != userregisterDTO.ConfirmPassword)
            {
                userdetail = null;
                return userdetail;
            }
            userdetail.UserId = userregisterDTO.Id;
            HMACSHA512 hMACSHA = new HMACSHA512();
            userdetail.PasswordHashKey = hMACSHA.Key;
            userdetail.Password = hMACSHA.ComputeHash(Encoding.UTF8.GetBytes(userregisterDTO.Password));
            return userdetail;
        }
        /// <summary>
        /// Maps user input data to user details entity for password encryption.
        /// </summary>
        /// <param name="userregisterDTO">DTO containing user registration information.</param>
        /// <returns>A mapped user details entity.</returns>

        private User? MapInputwithUser(RegisterInputDTO userregisterDTO)
        {
            User user = new User()
            {
                Name = userregisterDTO.Name,
                Email = userregisterDTO.Email,
                DateOfBirth = userregisterDTO.DateOfBirth,
                Phone = userregisterDTO.Phone,
                Image = userregisterDTO.Image,
                Role = userregisterDTO.Role,
            };
            return user;
        }
    }
}
