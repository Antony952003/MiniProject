using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MiniProjectAPI.Models;
using MovieBookingAPI.Interfaces;
using MovieBookingAPI.Models;
using MovieBookingAPI.Models.DTOs.Booking;
using System.Runtime.Intrinsics.X86;

namespace MovieBookingAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingController : ControllerBase
    {
        readonly IBookingService _bookingService;
        public BookingController(IBookingService bookingService) { 
             _bookingService = bookingService;
        }
       // [Authorize(Roles = "User")]
        [HttpPost("AddBooking")]
        [ProducesResponseType(typeof(BookingReturnDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<BookingReturnDTO>> AddBooking(BookingInputDTO bookingInputDTO)
        {
            
            try
            {
                var result = await _bookingService.MakeBooking(bookingInputDTO);
                return Ok(result);
            }
            catch(Exception ex)
            {
                return NotFound(new ErrorModel(404, ex.Message));
            }
        }
        [HttpGet]
        [Route("GetAllBookings")]
        [ProducesResponseType(typeof(BookingReturnDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<List<BookingReturnDTO>>> GetAllBookings()
        {
            try
            {
                var result = await _bookingService.GetAllBookings();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return NotFound(new ErrorModel(404, ex.Message));
            }
        }
        [HttpGet]
        [Route("GetAllUserBookings")]
        [ProducesResponseType(typeof(List<BookingReturnDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<List<BookingReturnDTO>>> GetAllUserBookings()
        {
            try
            {
                var userstring = User.Claims.FirstOrDefault(x => x.Type == "uid").Value;
                var userid = Convert.ToInt32(userstring);
                var result = await _bookingService.GetAllUserBookings(userid);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return NotFound(new ErrorModel(404, ex.Message));
            }
        }
        [HttpGet]
        [Route("GetBookingById")]
        [ProducesResponseType(typeof(BookingReturnDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<BookingReturnDTO>> GetBookingById(int bookingid)
        {
            try
            {
                var result = await _bookingService.GetBookingById(bookingid);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return NotFound(new ErrorModel(404, ex.Message));
            }
        }

    }
}
