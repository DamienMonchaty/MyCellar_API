using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MyCellar.API.JwtHelpers;
using MyCellar.API.Messages.Requests;
using MyCellar.API.Utils;
using MyCellar.Business.Repository;
using MyCellar.Common.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using BC = BCrypt.Net.BCrypt;

namespace MyCellar.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ILogger<AuthController> _logger;

        public IConfiguration _configuration { get; }
        private readonly IUserRepository _userRepository;

        public AuthController(ILogger<AuthController> logger, IUserRepository userRepository, IConfiguration configuration)
        {
            _logger = logger;
            _userRepository = userRepository;
            _configuration = configuration;
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login(LoginUser model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var user = await _userRepository.GetByUserName(model.UserName);

                if (user != null || BC.Verify(model.Password, user.Password))
                {
                    user.GenerateToken(_configuration);

                    return Ok(new CustomResponse<User>
                    {
                        Message = Global.ResponseMessages.Success,
                        StatusCode = StatusCodes.Status200OK,
                        Result = user
                    });
                }
                else
                {
                    return Ok(new CustomResponse<Error>
                    {
                        Message = Global.ResponseMessages.Forbidden,
                        StatusCode = StatusCodes.Status403Forbidden,
                        Result = new Error { ErrorMessage = Global.ResponseMessages.GenerateInvalid("username or password") }
                    });
                }
            }
            catch (SqlException ex)
            {
                return StatusCode(Error.LogError(ex));
            }
        }

        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register(User user)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                return Ok(new CustomResponse<User>
                {
                    Message = Global.ResponseMessages.Success,
                    StatusCode = StatusCodes.Status200OK,
                    Result = await _userRepository.Add(user)
                });
            }
            catch (SqlException ex)
            {
                return StatusCode(Error.LogError(ex));
            }
        }

    }
}
