using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PersonnelManagement.Models.DTOs;
using PersonnelManagement.Models.Entities;
using PersonnelManagement.Repositories.Interfaces;
using PersonnelManagement.Security;
using System.Net.WebSockets;

namespace PersonnelManagement.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class PersonnelController : ControllerBase
    {
        private readonly ICccdRepository _cccdRepository;
        private readonly ILogger<PersonnelController> _logger;
        public PersonnelController(ICccdRepository cccdRepository, ILogger<PersonnelController> logger)
        {
            _cccdRepository = cccdRepository;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult GetPersonnelList()
        {
            try
            {
                var personnelList = _cccdRepository.GetAllPersonnel();
                if (personnelList == null)
                {
                    return NotFound(new
                    {
                        Message = "Not found list personnel in system."
                    });
                }
                return Ok(new
                {
                    Message = "Get list personnel successfully.",
                    Data = personnelList
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting personnel list");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error occurred while getting personnel list");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPersonnelById(string id)
        {
            try
            {
                var personnel = await _cccdRepository.GetPersonnelById(id);
                if (personnel == null)
                {
                    return NotFound(new {
                        Message = $"Not found personnel id: {id}"
                    });
                }
                return Ok(new
                {
                    Message = "Get personnel by id successfully",
                    Data = personnel
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting personnel by ID");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error occurred while getting personnel by ID");
            }
        }

        [HttpPost]
        [RequirePermission("ADD_PERSONNEl")]
        public async Task<IActionResult> CreatePersonnel(IFormFile cccd, Guid rolePersonnelId, string phoneNumber)
        {
            try
            {
                var personnel = await _cccdRepository.AddPersonnel(cccd, rolePersonnelId, phoneNumber);
                return personnel ? Ok(new
                {
                    Message = "Personnel created successfully"
                }) : NotFound(new
                {
                    Message = "Failed to personnel created"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating personnel");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error occurred while creating personnel");
            }
        }

        [AllowAnonymous]
        [HttpPost("check-authen-image")]
        public async Task<IActionResult> CheckAuthenByImage(IFormFile cccd)
        {
            try
            {
                var isAuthenticated = await _cccdRepository.CheckAuthenByImage(cccd);
                string isMessaged = isAuthenticated?.Status == Status.CheckIn ? "Check-in" : "Check-out";

                if (isAuthenticated != null)
                {
                    return Ok(new
                    {
                        Message = $"{isMessaged} successful.",
                        Data = isAuthenticated
                    });
                }

                return Unauthorized(new
                {
                    Message = "Un-Authorized",
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while checking authentication by image");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error occurred while checking authentication by image");
            }
        }

        [AllowAnonymous]
        [HttpPost("check-in-out")]
        public async Task<IActionResult> CheckInOut(string numberId)
        {
            try
            {
                if (string.IsNullOrEmpty(numberId))
                {
                    return BadRequest(new { Message = "Number ID is required" });
                }

                var result = await _cccdRepository.ProcessCheckInOut(numberId);
                if (result == null)
                {
                    return NotFound(new { Message = "Personnel not found" });
                }

                string statusMessage = result.Status == Status.CheckIn ? "Check-in" : "Check-out";

                return Ok(new
                {
                    Message = $"{statusMessage} successful",
                    Data = result
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during check-in/check-out process");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error occurred during check-in/check-out process");
            }
        }

        [HttpGet("history")]
        public async Task<IActionResult> GetPersonnelHistory()
        {
            try
            {
                var history = await _cccdRepository.GetPersonnelHistory();
                return Ok(new
                {
                    Message = "Get personnel history successfully",
                    Data = history
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting personnel history");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error occurred while getting personnel history");
            }
        }

        [HttpPut("{personnelId}")]
        [RequirePermission("EDIT_PERSONNEl")]
        public async Task<IActionResult> UpdatePersonnel(string personnelId, [FromBody] UpdatePersonnel personnel)
        {
            try
            {
                var personnelRes = await _cccdRepository.UpdatePersonnel(personnelId, personnel);
                if (personnelRes == null)
                {
                    return NotFound(new
                    {
                        Message = "Not found personnel in system."
                    });
                }
                return Ok(new
                {
                    Message = "Update personnel successfully.",
                    Data = personnelRes
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating personnel");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpDelete("{personnelId}")]
        [RequirePermission("DELETE_PERSONNEl")]
        public async Task<IActionResult> DeletePersonnel(string personnelId)
        {
            try
            {
                var personnelRes = await _cccdRepository.DeletePersonnel(personnelId);
                if (personnelRes == null)
                {
                    return NotFound(new
                    {
                        Message = $"Personnel {personnelId} not found in system."
                    });
                }
                return Ok(new
                {
                    Message = "Delete personnel successfully.",
                    Data = personnelRes
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting personnel");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpGet("roles")]
        public async Task<IActionResult> GetRolePersonnels()
        {
            try
            {
                var roles = await _cccdRepository.GetAllRolePersonnel();
                if (roles == null || !roles.Any())
                {
                    return NotFound(new { Message = "No RolePersonnels found." });
                }
                return Ok(new { Message = "Get RolePersonnels successfully.", Data = roles });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting RolePersonnels");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error occurred while getting RolePersonnels");
            }
        }

        [HttpGet("roles/{roleId}")]
        public async Task<IActionResult> GetRolePersonnelById(Guid roleId)
        {
            try
            {
                var role = await _cccdRepository.GetRolePersonnelById(roleId);
                if (role == null)
                {
                    return NotFound(new { Message = "RolePersonnel not found." });
                }
                return Ok(new { Message = "Get RolePersonnel successfully.", Data = role });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting RolePersonnel by ID");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error occurred while getting RolePersonnel by ID");
            }
        }

        [HttpPost("roles")]
        [RequirePermission("ADD_PERSONNEl_GROUP")]
        public async Task<IActionResult> AddRolePersonnel(string roleName)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(roleName))
                {
                    return BadRequest(new { Message = "Role name is required." });
                }

                var role = await _cccdRepository.AddRolePersonnel(roleName);
                return Ok(new { Message = "RolePersonnel created successfully.", Data = role });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating RolePersonnel");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error occurred while creating RolePersonnel");
            }
        }

        [HttpPut("roles/{roleId}")]
        [RequirePermission("EDIT_PERSONNEl_GROUP")]
        public async Task<IActionResult> UpdateRolePersonnel(Guid roleId, string roleName)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(roleName))
                {
                    return BadRequest(new { Message = "Role name is required." });
                }

                var role = await _cccdRepository.UpdateRolePersonnel(roleId, roleName);
                return Ok(new { Message = "RolePersonnel updated successfully.", Data = role });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating RolePersonnel");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error occurred while updating RolePersonnel");
            }
        }

        [HttpDelete("roles/{roleId}")]
        [RequirePermission("DELETE_PERSONNEl_GROUP")]
        public async Task<IActionResult> DeleteRolePersonnel(Guid roleId)
        {
            try
            {
                var result = await _cccdRepository.DeleteRolePersonnel(roleId);
                if (!result)
                {
                    return NotFound(new { Message = "RolePersonnel not found." });
                }
                return Ok(new { Message = "RolePersonnel deleted successfully." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting RolePersonnel");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error occurred while deleting RolePersonnel");
            }
        }

        [HttpPost("{personnelId}/assign-role/{roleId}")]
        [RequirePermission("EDIT_PERSONNEl")]
        public async Task<IActionResult> AssignRolePersonnelToPersonnel(Guid personnelId, Guid roleId)
        {
            try
            {
                var result = await _cccdRepository.AssignRolePersonnelToPersonnel(personnelId, roleId);
                if (!result)
                {
                    return NotFound(new { Message = "Personnel or RolePersonnel not found." });
                }
                return Ok(new { Message = "RolePersonnel assigned to Personnel successfully." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while assigning RolePersonnel to Personnel");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error occurred while assigning RolePersonnel to Personnel");
            }
        }
    }
}
