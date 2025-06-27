using DataAccess.ResponseModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Repositorys.DTOs.AccountDTO;
using Services.IService;

namespace Lab_Orchid_SE161629_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;

        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpPost("login")]
        [ProducesResponseType(typeof(BaseResponseModel<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponseModel<string>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Login([FromBody] LoginDTO model)
        {
            try
            {
                var token = await _accountService.LoginAsync(model.Email, model.Password);
                return Ok(BaseResponseModel<string>.OkResponseModel(null, additionalData: new { message = "Login successful", tokenAccount = token }));
            }
            catch (UnauthorizedAccessException ex)
            {
                return BadRequest(BaseResponseModel<string>.BadRequestResponseModel(null, additionalData: new { message = ex.Message }));
            }
        }

        [HttpPost("register")]
        [ProducesResponseType(typeof(BaseResponseModel<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponseModel<string>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Register([FromBody] RegisterDTO model)
        {
            try
            {
                if (model.Password != model.ConfirmPassword)
                    return BadRequest(BaseResponseModel<string>.BadRequestResponseModel(null, additionalData: new { message = "Passwords do not match" }));

                await _accountService.RegisterAsync(model.Username, model.Password, model.Email);
                return Ok(BaseResponseModel<string>.OkResponseModel(null, additionalData: new { message = "Registration successful" }));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(BaseResponseModel<string>.BadRequestResponseModel(null, additionalData: new { message = ex.Message }));
            }
        }

        [HttpPost("forgot-password")]
        [ProducesResponseType(typeof(BaseResponseModel<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponseModel<string>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDTO model)
        {
            try
            {
                if (model.NewPassword != model.ConfirmNewPassword)
                    return BadRequest(BaseResponseModel<string>.BadRequestResponseModel(null, additionalData: new { message = "Passwords do not match" }));

                var result = await _accountService.ResetPasswordAsync(model.Email, model.NewPassword);
                if (!result)
                    return BadRequest(BaseResponseModel<string>.BadRequestResponseModel(null, additionalData: new { message = "Email not found" }));

                return Ok(BaseResponseModel<string>.OkResponseModel(null, additionalData: new { message = "Password reset successful" }));
            }
            catch (Exception ex)
            {
                return BadRequest(BaseResponseModel<string>.BadRequestResponseModel(null, additionalData: new { message = ex.Message }));
            }
        }

        [Authorize]
        [HttpPost("change-password")]
        [ProducesResponseType(typeof(BaseResponseModel<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponseModel<string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(BaseResponseModel<string>), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDTO model)
        {
            try
            {
                var result = await _accountService.ChangePasswordAsync(model.CurrentPassword, model.NewPassword);
                if (!result)
                    return BadRequest(BaseResponseModel<string>.BadRequestResponseModel(null, additionalData: new { message = "Current password is incorrect" }));

                return Ok(BaseResponseModel<string>.OkResponseModel(null, additionalData: new { message = "Password changed successfully" }));
            }
            catch (Exception ex)
            {
                return BadRequest(BaseResponseModel<string>.BadRequestResponseModel(null, additionalData: new { message = ex.Message }));
            }
        }

        [Authorize]
        [HttpPost("logout")]
        [ProducesResponseType(typeof(BaseResponseModel<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponseModel<string>), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Logout()
        {
            await _accountService.LogoutAsync();
            return Ok(BaseResponseModel<string>.OkResponseModel(null, additionalData: new { message = "Logged out successfully" }));
        }
    }
}