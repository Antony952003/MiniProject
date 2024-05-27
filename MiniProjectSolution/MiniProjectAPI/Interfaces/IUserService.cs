using MovieBookingAPI.Models.DTOs.Login;
using MovieBookingAPI.Models.DTOs.Register;

namespace MovieBookingAPI.Interfaces
{
    public interface IUserService
    {
            public Task<LoginReturnDTO> Login(UserLoginDTO loginDTO);
            public Task<RegisterOutputDTO> Register(RegisterInputDTO employeeDTO);
           // public Task<ReturnActivatedUserDTO> UpdateStatus(ActivateUserDTO activateuserDTO);
        }
    }
