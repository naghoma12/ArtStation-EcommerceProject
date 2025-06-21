using ArtStation.Core;
using ArtStation.Core.Entities.Identity;
using ArtStation.Core.Services.Contract;
using ArtStation.Dtos;
using ArtStation.Dtos.AuthDtos;
using ArtStation.Extensions;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ArtStation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {

        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManger;
        private readonly ITokenService _tokenService;
     
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
            ITokenService tokenService,
            
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
            _tokenService = tokenService;
            _smsService = smsService;
            _signInManager = signInManager;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _webHostEnvironment = webHostEnvironment;
            _apiBaseUrl = configuration["ApiBaseUrl"];
            _imagepath = $"{_webHostEnvironment.WebRootPath}";

        }
        

        [HttpPost("send")]
        public IActionResult Send(SendSMSDto dto)
        {
            var result = _smsService.SendVerificationCode(dto.MobileNumber);

            if (!string.IsNullOrEmpty(result.ErrorMessage))
                return BadRequest(result.ErrorMessage);

            return Ok(result);
        }

        //Login EndPoint Domain/Api/Account/login

        //[HttpPost("login")]
        //public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
        //{
        //    var user = await _userManager.FindByEmailAsync(loginDto.EmailOrUserName) ??
        //        await _userManager.FindByNameAsync(loginDto.EmailOrUserName);

        //    if (user == null) return Unauthorized(new
        //    {
        //        Message = "اسم المستخدم أو البريد الالكترونى غير صحيح"
        //    });

        //    var result = await _signInManger.CheckPasswordSignInAsync(user, loginDto.Password, false);
        //    if (!result.Succeeded)
        //        return Unauthorized(new
        //        {

        //            Message = " كلمة المرور غير صحيحة"
        //        });



        //    return Ok(new UserDto()
        //    {
        //        UserName = user.UserName,
        //        Email = user.Email,
        //        Token = await _tokenService.CreateToken(user, _userManager)
        //    });
        //}


        //Register EndPoint Domain/Api/Account/register
        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
        {
            var phoneExsist = await _userManager.FindByPhoneNumberAsync(registerDto.PhoneNumber);
            if (phoneExsist != null)
            {
                return new BadRequestObjectResult(new { Message = "البريد الإلكتروني مستخدم بالفعل" });
            }
            var user = new AppUser()
            {

               FullName = registerDto.FullName,
               PhoneNumber = registerDto.PhoneNumber,
              
            };

            var result = await _userManager.CreateAsync(user);

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





    }
}
