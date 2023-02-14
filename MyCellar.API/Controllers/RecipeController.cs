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
    public class RecipeController : ControllerBase
    {
        private readonly IRecipeRepository _recipeRepository;
        private readonly IRepository<Product> _productRepository;
        private readonly IMapper _mapper;

        public RecipeController(IRecipeRepository recipeRepository, IRepository<Product> productRepository, IMapper mapper)
        {
            _recipeRepository = recipeRepository;
            _productRepository = productRepository;
            _mapper = mapper;
        }

        [Authorize(Roles = "User, Admin")]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                return Ok(new CustomResponse<List<Recipe>>
                {
                    Message = Global.ResponseMessages.Success,
                    StatusCode = StatusCodes.Status200OK,
                    Result = await _recipeRepository.GetAll()
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
                var a = await _recipeRepository.GetById(id);
                if (a != null)
                {
                    return Ok(new CustomResponse<Recipe>
                    {
                        Message = Global.ResponseMessages.Success,
                        StatusCode = StatusCodes.Status200OK,
                        Result = a
                    });
                }
                else
                {
                    return NotFound(new CustomResponse<Error>
                    {
                        Message = Global.ResponseMessages.NotFound,
                        StatusCode = StatusCodes.Status404NotFound,
                        Result = new Error { ErrorMessage = Global.ResponseMessages.GenerateInvalid("Recipe with " + id + " is not found") }
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
        public async Task<IActionResult> Post([FromBody] Recipe recipe)
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

                Recipe rToSave = _mapper.Map<Recipe>(recipe);

                return Ok(new CustomResponse<Recipe>
                {
                    Message = Global.ResponseMessages.Success,
                    StatusCode = StatusCodes.Status200OK,
                    Result = await _recipeRepository.Add(rToSave)
                });
            }
            catch (SqlException ex)
            {
                return StatusCode(Error.LogError(ex));
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] Recipe recipe)
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

                var rToUpdate = await _recipeRepository.GetById(id);
                if (rToUpdate != null)
                {
                    // rToUpdate = _mapper.Map(recipe, rToUpdate);
                    rToUpdate.Title = recipe.Title;
                    rToUpdate.Description = recipe.Description;
                    rToUpdate.Duration = recipe.Duration;
                    rToUpdate.ImgUrl = recipe.ImgUrl;
                    rToUpdate.Difficulty = recipe.Difficulty;
                    rToUpdate.Caloric = recipe.Caloric;
                    return Ok(new CustomResponse<Recipe>
                    {
                        Message = Global.ResponseMessages.Success,
                        StatusCode = StatusCodes.Status200OK,
                        Result = await _recipeRepository.Update(rToUpdate)
                    });
                }
                else
                {
                    return NotFound(new CustomResponse<Error>
                    {
                        Message = Global.ResponseMessages.NotFound,
                        StatusCode = StatusCodes.Status404NotFound,
                        Result = new Error { ErrorMessage = Global.ResponseMessages.GenerateInvalid("Recipe with " + id + " is not found") }
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
        public async Task<IActionResult> Patch(int id, [FromBody] JsonPatchDocument<Recipe> patchRecipe)
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

                var rToEdit = await _recipeRepository.GetById(id);
                if (rToEdit != null)
                {
                    patchRecipe.ApplyTo(rToEdit);
                    return Ok(new CustomResponse<Recipe>
                    {
                        Message = Global.ResponseMessages.Success,
                        StatusCode = StatusCodes.Status200OK,
                        Result = await _recipeRepository.Update(rToEdit)
                    });
                }
                else
                {
                    return NotFound(new CustomResponse<Error>
                    {
                        Message = Global.ResponseMessages.NotFound,
                        StatusCode = StatusCodes.Status404NotFound,
                        Result = new Error { ErrorMessage = Global.ResponseMessages.GenerateInvalid("Recipe with " + id + " is not found") }
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
                var rToDelete = await _recipeRepository.GetById(id);
                if (rToDelete != null)
                {
                    await _recipeRepository.Delete(rToDelete);
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
                        Result = new Error { ErrorMessage = Global.ResponseMessages.GenerateInvalid("Recipe with " + id + " is not found") }
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
                    Result = await _recipeRepository.Count()
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
                return Ok(new CustomResponse<PageResult<Recipe>>
                {
                    Message = Global.ResponseMessages.Success,
                    StatusCode = StatusCodes.Status200OK,
                    Result = await _recipeRepository.GetAllPaginate(page, pagesize, search)
                });
            }
            catch (SqlException ex)
            {
                return StatusCode(Error.LogError(ex));
            }
        }

        [Authorize(Roles = "User, Admin")]
        [HttpGet("byProducts")]
        public async Task<IActionResult> GetRecipesByProducts([FromQuery] int[] ids)
        {
            try
            {
                return Ok(new CustomResponse<List<Recipe>>
                {
                    Message = Global.ResponseMessages.Success,
                    StatusCode = StatusCodes.Status200OK,
                    Result = await _recipeRepository.GetAllRecipesByProducts(ids)
                });
            }
            catch (SqlException ex)
            {
                return StatusCode(Error.LogError(ex));
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("{id}/products")]
        public async Task<IActionResult> GetAllProductsFromOneRecipe(int id)
        {
            try
            {
                var a = await _recipeRepository.GetAllProductsFromOneRecipe(id);
                if (a != null)
                {
                    return Ok(new CustomResponse<List<Product>>
                    {
                        Message = Global.ResponseMessages.Success,
                        StatusCode = StatusCodes.Status200OK,
                        Result = a
                    });
                }
                else
                {
                    return NotFound(new CustomResponse<Error>
                    {
                        Message = Global.ResponseMessages.NotFound,
                        StatusCode = StatusCodes.Status404NotFound,
                        Result = new Error { ErrorMessage = Global.ResponseMessages.GenerateInvalid("Recipe with " + id + " is not found") }
                    });
                }
            }
            catch (SqlException ex)
            {
                return StatusCode(Error.LogError(ex));
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("{recipeId}/products")]
        public async Task<IActionResult> AssignProductToOneRecipe(int recipeId, [FromBody] int productId)
        {
            try
            {
                var a = await _recipeRepository.GetById(recipeId);
                if (a != null)
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
                    return Ok(new CustomResponse<Recipe>
                    {
                        Message = Global.ResponseMessages.Success,
                        StatusCode = StatusCodes.Status200OK,
                        Result = await _recipeRepository.AssignOneProductToOneRecipe(recipeId, productId)
                    });
                }
                else
                {
                    return NotFound(new CustomResponse<Error>
                    {
                        Message = Global.ResponseMessages.NotFound,
                        StatusCode = StatusCodes.Status404NotFound,
                        Result = new Error { ErrorMessage = Global.ResponseMessages.GenerateInvalid("Recipe with " + recipeId + " is not found") }
                    });
                }
            }
            catch (SqlException ex)
            {
                return StatusCode(Error.LogError(ex));
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{recipeId}/products/{productId}")]
        public async Task<IActionResult> DeleteProductToOneRecipe(int recipeId, int productId)
        {
            try
            {
                var a = await _recipeRepository.GetById(recipeId);
                if (a != null)
                {
                    var b = await _productRepository.GetById(productId);
                    if (b != null)
                    {
                        return Ok(new CustomResponse<Recipe>
                        {
                            Message = Global.ResponseMessages.Success,
                            StatusCode = StatusCodes.Status200OK,
                            Result = await _recipeRepository.DeleteOneProductToOneRecipe(recipeId, productId)
                        });
                    }
                    else
                    {
                        return NotFound(new CustomResponse<Error>
                        {
                            Message = Global.ResponseMessages.NotFound,
                            StatusCode = StatusCodes.Status404NotFound,
                            Result = new Error { ErrorMessage = Global.ResponseMessages.GenerateInvalid("Product with " + productId + " is not found") }
                        });
                    }
                }
                else
                {
                    return NotFound(new CustomResponse<Error>
                    {
                        Message = Global.ResponseMessages.NotFound,
                        StatusCode = StatusCodes.Status404NotFound,
                        Result = new Error { ErrorMessage = Global.ResponseMessages.GenerateInvalid("Recipe with " + recipeId + " is not found") }
                    });
                }
            }
            catch (SqlException ex)
            {
                return StatusCode(Error.LogError(ex));
            }
        }
    }
}
