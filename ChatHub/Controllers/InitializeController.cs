using ChatHub.BLL.DTOs;
using ChatHub.BLL.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ChatHub.Controllers
{
    [Authorize(Roles =("superadmin"))]
    [Route("api/[controller]")]
    [ApiController]
    public class InitializeController : ControllerBase
    {
        private readonly IInitializeService _initializeService;
        public InitializeController(IInitializeService initializeService)
        {
            _initializeService = initializeService;
        }
        [AllowAnonymous]
        [HttpGet]
        [Route("CheckInitialize")]
        public async Task<Boolean> CheckInitialize()
        {
            var checkInitialization = await _initializeService.CheckInitialize();
            if (checkInitialization.Success)
            {
                return true;
            }
            return false;
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("InitializeSystem")]
        public async Task<IActionResult> Initialize([FromForm] OrganizationDTO organizationDTO)
        {
            var checkInitialization = await _initializeService.CheckInitialize();
            if (checkInitialization.Success)
            {
                var result = await _initializeService.Initialize(organizationDTO);
                if (!result.Success)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, result.Errors);
                }
                else
                {
                    return StatusCode(StatusCodes.Status200OK , "System has bee successfully initialize.");
                }
            }
            else
            {
                return StatusCode(StatusCodes.Status500InternalServerError, checkInitialization.Errors);
            }
        }
        [AllowAnonymous]
        [HttpGet]
        [Route("ResetSystem")]
        public async Task<IActionResult> ResetSystem()
        {
            var result = await _initializeService.ResetSystem();
            if (!result.Success)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, result.Errors);
            }
            return StatusCode(StatusCodes.Status200OK, "System has been successfully reset");
        }
    }
}
