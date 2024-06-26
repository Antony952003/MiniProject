﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MiniProjectAPI.Models;
using MovieBookingAPI.Exceptions;
using MovieBookingAPI.Interfaces;
using MovieBookingAPI.Models;
using MovieBookingAPI.Models.DTOs.Screen;

namespace MovieBookingAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ScreenController : ControllerBase
    {
        private readonly IScreenService _screenService;

        public ScreenController(IScreenService screenService) {
            _screenService = screenService;
        }
        [Authorize(Roles = "Admin")]
        [HttpPost("AddScreenToTheater")]
        [ProducesResponseType(typeof(ScreenOutputDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<ScreenOutputDTO>> AddScreenToTheater(ScreenDTO screenDTO)
        {
            try
            {
                var result = await _screenService.AddScreenToTheater(screenDTO);
                return Ok(result);
            }
            catch (EntityAlreadyExists ex)
            {
                return BadRequest(new ErrorModel(409, ex.Message));
            }
            catch(EntityNotFoundException ex)
            {
                return BadRequest(new ErrorModel(404, ex.Message));
            }
        }
        [Authorize(Roles = "Admin, User")]
        [HttpGet("GetScreenByTheaterName")]
        [ProducesResponseType(typeof(ScreenOutputDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<List<ScreenOutputDTO>>> GetScreensByTheaterName(string theaterName)
        {
            try
            {
                var result = await _screenService.GetScreensByTheaterName(theaterName);
                return Ok(result);
            }
            catch(Exception ex)
            {
                return NotFound(new ErrorModel(404, ex.Message));
            }
        }
        [Authorize(Roles = "Admin, User")]
        [HttpGet("GetScreenByScreenName")]
        [ProducesResponseType(typeof(ScreenOutputDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<ScreenOutputDTO>> GetScreensByScreenName(string screenName)
        {
            try
            {
                var result = await _screenService.GetScreenByScreenName(screenName);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return NotFound(new ErrorModel(404, ex.Message));
            }
        }
        [Authorize(Roles = "Admin, User")]
        [HttpGet("GetScreenById")]
        [ProducesResponseType(typeof(ScreenOutputDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<ScreenOutputDTO>> GetScreensById(int screenid)
        {
            try
            {
                var result = await _screenService.GetScreenByScreenId(screenid);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return NotFound(new ErrorModel(404, ex.Message));
            }
        }




    }
}
