using ChatHub.BLL.DTOs;
using ChatHub.BLL.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ChatHub.Controllers
{
    [Authorize(Roles ="superadmin")]
    [Route("api/[controller]")]
    [ApiController]
    public class DepartmentController : ControllerBase
    {
        private readonly IDepartmentService _departmentService;
        public DepartmentController(IDepartmentService departmentService)
        {
            _departmentService = departmentService;
        }

        [HttpPost]
        [Route("CreateDepartment")]
        public async Task<IActionResult> CreateDepartment([FromForm] DepartmentDTO departmentDTO)
        {
            var result = await _departmentService.CreateDepartment(departmentDTO);
            if (!result.Success)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, result.Errors);
            }
            return StatusCode(StatusCodes.Status200OK, result.Data);
        }
        [HttpGet]
        [Route("GetAllDepartments")]
        public async Task<IActionResult> GetDepartments()
        {
            var result = await _departmentService.GetDepartments();
            if (!result.Success)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, result.Errors);
            }
            return StatusCode(StatusCodes.Status200OK, result.Data);
        }
        [HttpGet]
        [Route("GetDepartmentById/{departmentId}")]
        public async Task<IActionResult> GetDepartmentById(string departmentId)
        {
            var result = await _departmentService.GetDepartmentById(departmentId);
            if (!result.Success)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, result.Errors);
            }
            return StatusCode(StatusCodes.Status200OK, result.Data);
        }
        [HttpPatch]
        [Route("UpdateDepartment")]
        public async Task<IActionResult> UpdateDepartment([FromForm] DepartmentDTO departmentDTO)
        {
            var result = await _departmentService.UpdateDepartment(departmentDTO);
            if (!result.Success)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, result.Errors);
            }
            return StatusCode(StatusCodes.Status200OK, result.Data);
        }
    }
}
