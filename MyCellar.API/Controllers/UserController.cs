using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyCellar.API.Utils;
using MyCellar.Business.Repository;
using MyCellar.Common.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MyCellar.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public UserController(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        [Authorize(Roles = "User")]
        [HttpGet("{id}")]
        public async Task<IActionResult> getCurrentUser(int id)
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            var userId = int.Parse(identity.FindFirst("id").Value);

            if (userId == id)
            {
                try
                {
                    if (!ModelState.IsValid)
                    {
                        return BadRequest(new CustomResponse<object>
                        {
                            Message = Global.ResponseMessages.BadRequest,
                            StatusCode = StatusCodes.Status400BadRequest,
                            Result = ModelState
                        });
                    }


                    return Ok(new CustomResponse<User>
                    {
                        Message = Global.ResponseMessages.Success,
                        StatusCode = StatusCodes.Status200OK,
                        Result = await _userRepository.GetById(userId)
                    });
                }
                catch (SqlException ex)
                {
                    return StatusCode(Error.LogError(ex));
                }
            }
            return NotFound();
        }

        [Authorize(Roles = "User")]
        [HttpPut("{id}")]
        public async Task<IActionResult> updateCurrentUser(int id, [FromBody] User user)
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            var userId = int.Parse(identity.FindFirst("id").Value);

            if (userId == id)
            {
                try
                {
                    if (!ModelState.IsValid)
                    {
                        return BadRequest(new CustomResponse<object>
                        {
                            Message = Global.ResponseMessages.BadRequest,
                            StatusCode = StatusCodes.Status400BadRequest,
                            Result = ModelState
                        });
                    }

                    var userToUpdate = await _userRepository.GetById(userId);
                    if (userToUpdate != null)
                    {
                        // userToUpdate = _mapper.Map(user, userToUpdate);
                        userToUpdate.Email = user.Email;
                        userToUpdate.UserName = user.UserName;

                        return Ok(new CustomResponse<User>
                        {
                            Message = Global.ResponseMessages.Success,
                            StatusCode = StatusCodes.Status200OK,
                            Result = await _userRepository.Update(userToUpdate)
                        });
                    }                    
                }
                catch (SqlException ex)
                {
                    return StatusCode(Error.LogError(ex));
                }
            }
            return NotFound();
        }

        [Authorize(Roles = "User")]
        [HttpGet("current/products")]
        public async Task<IActionResult> GetAllProductsFromCurrentUser()
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            var userId = identity.FindFirst("id").Value;
            if (userId != null)
            {
                try
                {
                    if (!ModelState.IsValid)
                    {
                        return BadRequest(new CustomResponse<object>
                        {
                            Message = Global.ResponseMessages.BadRequest,
                            StatusCode = StatusCodes.Status400BadRequest,
                            Result = ModelState
                        });
                    }
                    return Ok(new CustomResponse<List<Product>>
                    {
                        Message = Global.ResponseMessages.Success,
                        StatusCode = StatusCodes.Status200OK,
                        Result = await _userRepository.GetAllProductsFromCurrentUser(int.Parse(userId))
                    });
                }
                catch (SqlException ex)
                {
                    return StatusCode(Error.LogError(ex));
                }
            }
            return NotFound();
        }

        [Authorize(Roles = "User")]
        [HttpPost("current/products")]
        public async Task<IActionResult> AssignProductToCurrentUser([FromBody] int productId)
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            var userId = identity.FindFirst("id").Value;
            if (userId != null)
            {
                try
                {
                    if (!ModelState.IsValid)
                    {
                        return BadRequest(new CustomResponse<object>
                        {
                            Message = Global.ResponseMessages.BadRequest,
                            StatusCode = StatusCodes.Status400BadRequest,
                            Result = ModelState
                        });
                    }


                    return Ok(new CustomResponse<User>
                    {
                        Message = Global.ResponseMessages.Success,
                        StatusCode = StatusCodes.Status200OK,
                        Result = await _userRepository.AssignOneProductToCurrentUser(int.Parse(userId), productId)
                    });
                }
                catch (SqlException ex)
                {
                    return StatusCode(Error.LogError(ex));
                }
            }
            return NotFound();
        }

        [Authorize(Roles = "User")]
        [HttpDelete("current/products/{productId}")]
        public async Task<IActionResult> DeleteProductToCurrentUser(int productId)
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            var userId = identity.FindFirst("id").Value;
            if (userId != null) { 

                try
                {
                    if (!ModelState.IsValid)
                    {
                        return BadRequest(new CustomResponse<object>
                        {
                            Message = Global.ResponseMessages.BadRequest,
                            StatusCode = StatusCodes.Status400BadRequest,
                            Result = ModelState
                        });
                    }


                    return Ok(new CustomResponse<User>
                    {
                        Message = Global.ResponseMessages.Success,
                        StatusCode = StatusCodes.Status200OK,
                        Result = await _userRepository.DeleteOneProductFromCurrentUser(int.Parse(userId), productId)
                    });
                }
                catch (SqlException ex)
                {
                    return StatusCode(Error.LogError(ex));
                }

            }

            return NotFound();          
        }
    }
}
