using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using MyCellar.API.Utils;
using Microsoft.AspNetCore.Authorization;
using MyCellar.Business.Repository;
using MyCellar.Business.Wrappers;
using MyCellar.Common.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace MyCellar.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly IRepository<Category> _categoryRepository;
        private readonly IMapper _mapper;

        public CategoryController(IRepository<Category> categoryRepository, IMapper mapper)
        {
            _categoryRepository = categoryRepository;
            _mapper = mapper;
        }

        [Authorize(Roles = "User, Admin")]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                return Ok(new CustomResponse<List<Category>>
                {
                    Message = Global.ResponseMessages.Success,
                    StatusCode = StatusCodes.Status200OK,
                    Result = await _categoryRepository.GetAll()
                });
            }
            catch (SqlException ex)
            {
                return StatusCode(Error.LogError(ex));
            }
        }

        [Authorize(Roles = "User, Admin")]
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                var c = await _categoryRepository.GetById(id);
                if (c != null)
                {
                    return Ok(new CustomResponse<Category>
                    {
                        Message = Global.ResponseMessages.Success,
                        StatusCode = StatusCodes.Status200OK,
                        Result = c
                    });
                }
                else
                {
                    return NotFound(new CustomResponse<Error>
                    {
                        Message = Global.ResponseMessages.NotFound,
                        StatusCode = StatusCodes.Status404NotFound,
                        Result = new Error { ErrorMessage = Global.ResponseMessages.GenerateInvalid("Category with " + id + " is not found") }
                    });
                }
            }
            catch (SqlException ex)
            {
                return StatusCode(Error.LogError(ex));
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Category category)
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

                Category cToSave = _mapper.Map<Category>(category);

                return Ok(new CustomResponse<Category>
                {
                    Message = Global.ResponseMessages.Success,
                    StatusCode = StatusCodes.Status200OK,
                    Result = await _categoryRepository.Add(cToSave)
                });
            }
            catch (SqlException ex)
            {
                return StatusCode(Error.LogError(ex));
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] Category category)
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

                var cToUpdate = await _categoryRepository.GetById(id);
                if (cToUpdate != null)
                {
                    //cToUpdate = _mapper.Map<Category, Category>(category, cToUpdate);
                    cToUpdate.Title = category.Title;
                    cToUpdate.Description = category.Description;
                    return Ok(new CustomResponse<Category>
                    {
                        Message = Global.ResponseMessages.Success,
                        StatusCode = StatusCodes.Status200OK,
                        Result = await _categoryRepository.Update(cToUpdate)
                    });
                }
                else
                {
                    return NotFound(new CustomResponse<Error>
                    {
                        Message = Global.ResponseMessages.NotFound,
                        StatusCode = StatusCodes.Status404NotFound,
                        Result = new Error { ErrorMessage = Global.ResponseMessages.GenerateInvalid("Category with " + id + " is not found") }
                    });
                }
            }
            catch (SqlException ex)
            {
                return StatusCode(Error.LogError(ex));
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPatch("{id}")]
        public async Task<IActionResult> Patch(int id, [FromBody] JsonPatchDocument<Category> patchCategory)
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

                var cToEdit = await _categoryRepository.GetById(id);
                if (cToEdit != null)
                {
                    patchCategory.ApplyTo(cToEdit);
                    return Ok(new CustomResponse<Category>
                    {
                        Message = Global.ResponseMessages.Success,
                        StatusCode = StatusCodes.Status200OK,
                        Result = await _categoryRepository.Update(cToEdit)
                    });
                }
                else
                {
                    return NotFound(new CustomResponse<Error>
                    {
                        Message = Global.ResponseMessages.NotFound,
                        StatusCode = StatusCodes.Status404NotFound,
                        Result = new Error { ErrorMessage = Global.ResponseMessages.GenerateInvalid("Category with " + id + " is not found") }
                    });
                }
            }
            catch (SqlException ex)
            {
                return StatusCode(Error.LogError(ex));
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var cToDelete = await _categoryRepository.GetById(id);
                if (cToDelete != null)
                {
                    await _categoryRepository.Delete(cToDelete);
                    return Ok(new CustomResponse<string>
                    {
                        Message = Global.ResponseMessages.Success,
                        StatusCode = StatusCodes.Status200OK,
                        Result = "Deleted Successfully !"
                    });
                }
                else
                {
                    return NotFound(new CustomResponse<Error>
                    {
                        Message = Global.ResponseMessages.NotFound,
                        StatusCode = StatusCodes.Status404NotFound,
                        Result = new Error { ErrorMessage = Global.ResponseMessages.GenerateInvalid("Category with " + id + " is not found") }
                    });
                }
            }
            catch (SqlException ex)
            {
                return StatusCode(Error.LogError(ex));
            }
        }

        [Authorize(Roles = "User, Admin")]
        [HttpGet("count")]
        public async Task<IActionResult> Count()
        {
            try
            {
                return Ok(new CustomResponse<int>
                {
                    Message = Global.ResponseMessages.Success,
                    StatusCode = StatusCodes.Status200OK,
                    Result = await _categoryRepository.Count()
                });
            }
            catch (SqlException ex)
            {
                return StatusCode(Error.LogError(ex));
            }
        }

        [Authorize(Roles = "User, Admin")]
        [HttpGet("paginate")]
        public async Task<IActionResult> GetPaginate([FromQuery] int? page, [FromQuery] int pagesize = 10, [FromQuery] string search = "")
        {
            try
            {
                return Ok(new CustomResponse<PageResult<Category>>
                {
                    Message = Global.ResponseMessages.Success,
                    StatusCode = StatusCodes.Status200OK,
                    Result = await _categoryRepository.GetAllPaginate(page, pagesize, search)
                });
            }
            catch (SqlException ex)
            {
                return StatusCode(Error.LogError(ex));
            }
        }
    }
}