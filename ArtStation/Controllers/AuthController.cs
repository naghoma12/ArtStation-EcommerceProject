using ArtStation.Core;
using ArtStation.Core.Entities.Identity;
using ArtStation.Core.Services.Contract;
using ArtStation.Dtos.AuthDtos;
using ArtStation.Extensions;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ArtStation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {

        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManger;
        private readonly RoleManager<AppRole> _roleManager;
        private readonly ITokenService _tokenService;
        private readonly IVerificationCodeService _verificationCodeService;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ISMSService _smsService;
        

        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly string _apiBaseUrl;
        public readonly string _imagepath;
        public AuthController(
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManger,
            RoleManager<AppRole> roleManager,
            ITokenService tokenService,
            IVerificationCodeService verificationCodeService,
            SignInManager<AppUser> signInManager,
            IUnitOfWork unitOfWork,
         ISMSService smsService,
            IMapper mapper,
            IWebHostEnvironment webHostEnvironment,
            IConfiguration configuration
            )

        {
            _userManager = userManager;
            _signInManger = signInManger;
            _roleManager = roleManager;
            _tokenService = tokenService;
           _verificationCodeService = verificationCodeService;
            _smsService = smsService;
            _signInManager = signInManager;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _webHostEnvironment = webHostEnvironment;
            _apiBaseUrl = configuration["ApiBaseUrl"];
            _imagepath = $"{_webHostEnvironment.WebRootPath}";

        }
        

        [HttpPost("sendRegisterCode")]
        public async Task<IActionResult> SendRegisterCode(SendSMSDto smsdto)
        {
            var phoneExsist = await _userManager.FindByPhoneNumberAsync(smsdto.PhoneNumber);
            if (phoneExsist != null)
            {
                return new BadRequestObjectResult(new { Message = "this phone number is exist already" });
            }
            else
            {
                var code = _verificationCodeService.GenerateCode(smsdto.PhoneNumber);
                var result = _smsService.SendVerificationCode(smsdto.PhoneNumber, code);
                if (!string.IsNullOrEmpty(result.ErrorMessage))
                    return BadRequest(new { Message = "Error In Sending Code" });

                return Ok(new { Message = " Sending Code Done" });
            }

           
        }

        //Register EndPoint Domain/Api/Account/register
        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> VerifyCode(RegisterDto registerDto)
        {
            var phoneExsist = await _userManager.FindByPhoneNumberAsync(registerDto.PhoneNumber);
            if (phoneExsist != null)
            {
                return new BadRequestObjectResult(new { Message = "this phone number is exist already" });
            }

            if(!_verificationCodeService.ValidateCode(registerDto.PhoneNumber, registerDto.Code) == false)
            {
                var user = new AppUser()
                {

                    FullName = registerDto.FullName,
                    PhoneNumber = registerDto.PhoneNumber,
                    UserName = registerDto.PhoneNumber,
                    PhoneNumberConfirmed = true,

                };

                var result = await _userManager.CreateAsync(user);
                var resultRole = await _userManager.AddToRoleAsync(user, "Customer");
                if (!result.Succeeded)
                {

                    return BadRequest(new { message = result.Errors });


                }
                return Ok(new UserDto()
                {
                    UserName = user.FullName.Split(' ')[0],
                    Token = await _tokenService.CreateTokenAsync(user)
                });
              
            }
            else
            {
                return BadRequest(new { message = "Invalid verification code." });
            }
     
        }

        [HttpPost("sendLoginCode")]
        public async Task<IActionResult> SendLoginCode(SendSMSDto smsdto)
        {
            var phoneExsist = await _userManager.FindByPhoneNumberAsync(smsdto.PhoneNumber);
            if (phoneExsist == null)
            {
                return  BadRequest(new { Message = "this phone number is Not exist " });
            }
            else
            {
                var code = _verificationCodeService.GenerateCode(smsdto.PhoneNumber);
                var result = _smsService.SendVerificationCode(smsdto.PhoneNumber, code);
                if (!string.IsNullOrEmpty(result.ErrorMessage))
                    return BadRequest(new { Message = "Error In Sending Code" });

                return Ok(new { Message = " Sending Code Done" });
            }


        }
        //Login EndPoint Domain/Api/Account/login

        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
        {
            var user = await _userManager.FindByPhoneNumberAsync(loginDto.PhoneNumber);
            if (user == null)
            {

                return BadRequest(new { Message = "this phone number is Not exist " });
            }
            if (user == null)
                return Unauthorized("Invalid phone number.");

            if (!user.PhoneNumberConfirmed)
                return Unauthorized("Phone number not verified.");
            
            if (!_verificationCodeService.ValidateCode(loginDto.PhoneNumber, loginDto.Code))
                return Unauthorized("Invalid or expired code.");

            await _signInManager.SignInAsync(user, isPersistent: false);

            return Ok(new UserDto()
            {
                UserName = user.FullName.Split(' ')[0],
                Token = await _tokenService.CreateTokenAsync(user)
            });
        }




        [HttpPost("resendCode")]
        public async Task<IActionResult> ResendCode(SendSMSDto smsdto)
        {
           
                var code = _verificationCodeService.GenerateCode(smsdto.PhoneNumber);
                var result = _smsService.SendVerificationCode(smsdto.PhoneNumber, code);
                if (!string.IsNullOrEmpty(result.ErrorMessage))
                    return BadRequest(new { Message = "Error In Sending Code" });

                return Ok(new { Message = " Sending Code Done" });
            


        }




    }
}
