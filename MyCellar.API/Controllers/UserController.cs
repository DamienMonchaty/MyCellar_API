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
        [HttpGet("{id}/products")]
        public async Task<IActionResult> GetAllProductsFromCurrentUser(int id)
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
                    return Ok(new CustomResponse<List<Product>>
                    {
                        Message = Global.ResponseMessages.Success,
                        StatusCode = StatusCodes.Status200OK,
                        Result = await _userRepository.GetAllProductsFromCurrentUser(userId)
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
        [HttpPost("{id}/products")]
        public async Task<IActionResult> AssignProductToCurrentUser(int id, [FromBody] int idProduct)
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
                        Result = await _userRepository.AssignOneProductToCurrentUser(userId, idProduct)
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
        [HttpDelete("{id}/products/{productId}")]
        public async Task<IActionResult> DeleteProductToCurrentUser(int id, int productId)
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            var userId = int.Parse(identity.FindFirst("id").Value);

            if(userId == id) {

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
                        Result = await _userRepository.DeleteOneProductFromCurrentUser(userId, productId)
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
