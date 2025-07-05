using ArtStation.Core;
using ArtStation.Core.Entities.Identity;
using ArtStation.Core.Services.Contract;
using ArtStation.Dtos.AuthDtos;
using ArtStation.Extensions;

using ArtStation.Core.Resources;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static System.Runtime.InteropServices.JavaScript.JSType;
using ArtStation.Core.Roles;

namespace ArtStation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {

        private readonly UserManager<AppUser> _userManager;
       
        private readonly RoleManager<AppRole> _roleManager;
        private readonly ITokenService _tokenService;
        private readonly IVerificationCodeService _verificationCodeService;
        private readonly SignInManager<AppUser> _signInManager;
       
        private readonly ISMSService _smsService;
        

      
        public AuthController(
            UserManager<AppUser> userManager,
            RoleManager<AppRole> roleManager,
            ITokenService tokenService,
            IVerificationCodeService verificationCodeService,
            SignInManager<AppUser> signInManager,
            ISMSService smsService
         
            )

        {
            _userManager = userManager;
            _roleManager = roleManager;
            _tokenService = tokenService;
           _verificationCodeService = verificationCodeService;
            _smsService = smsService;
            _signInManager = signInManager;
           
        }
        

        [HttpPost("sendRegisterCode")]
        public async Task<IActionResult> SendRegisterCode(SendSMSDto smsdto)
        {
            var phoneExsist = await _userManager.FindByPhoneNumberAsync(smsdto.PhoneNumber);
            if (phoneExsist != null)
            {
                return BadRequest(new
                {
                  message = ControllerMessages.PhoneAlreadyExists,
                    data = (object?)null
                });
               
            }
            else
            {
                var code = _verificationCodeService.GenerateCode(smsdto.PhoneNumber);
                var result = _smsService.SendVerificationCode(smsdto.PhoneNumber, code);
                if (!string.IsNullOrEmpty(result.ErrorMessage))
                    return BadRequest(new
                    {
                        message = ControllerMessages.SendCodeFailed,
                        data = (object?)null
                    });

                return Ok(
                    new
                    {
                        message = ControllerMessages.SendCodeSuccess,
                        data = (object?)null
                    });
            }

           
        }

        //Register EndPoint Domain/Api/Account/register
        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> VerifyCode(RegisterDto registerDto,string? fcmToken)
        {
             try
                {
                    var phoneExsist = await _userManager.FindByPhoneNumberAsync(registerDto.PhoneNumber);
                    if (phoneExsist != null)
                    {
                        return BadRequest(new
                        {
                            message = ControllerMessages.PhoneAlreadyExists,
                            data = (object?)null
                        });
                    }

                    var user = new AppUser()
                    {
                        FullName = registerDto.FullName,
                        PhoneNumber = registerDto.PhoneNumber,
                        UserName = registerDto.PhoneNumber,
                        PhoneNumberConfirmed = true,
                        FCMToken = fcmToken ?? string.Empty
                    };

                    var result = await _userManager.CreateAsync(user);

                    if (!result.Succeeded)
                    {
                        return BadRequest(new
                        {
                            message = string.Join(" | ", result.Errors),
                            data = (object?)null
                        });
                    }

                    var resultRole = await _userManager.AddToRoleAsync(user, Roles.Customer);

                    return Ok(new
                    {
                        message = ControllerMessages.RegisterSuccessfull,
                        data = new UserDto()
                        {
                            UserName = user.FullName,
                            Token = await _tokenService.CreateTokenAsync(user)
                        }
                    });
                }
                catch (Exception ex)
                {
                    return StatusCode(500, new
                    {
                        message = "حدث خطأ غير متوقع أثناء عملية التسجيل.",
                        error = ex.Message,
                        data = (object?)null
                    });
                }

                //if(!_verificationCodeService.ValidateCode(registerDto.PhoneNumber, registerDto.Code) == false)
                //{
                //    var user = new AppUser()
                //    {

                //        FullName = registerDto.FullName,
                //        PhoneNumber = registerDto.PhoneNumber,
                //        UserName = registerDto.PhoneNumber,
                //        PhoneNumberConfirmed = true,

                //    };

                //    var result = await _userManager.CreateAsync(user);
                //    var resultRole = await _userManager.AddToRoleAsync(user, "Customer");
                //    if (!result.Succeeded)
                //    {

                //        return BadRequest(new
                //        {

                //            message = string.Join(" | ", result.Errors)
                //        });


                //    }
                //    return Ok(new UserDto()
                //    {
                //        UserName = user.FullName,
                //        Token = await _tokenService.CreateTokenAsync(user)
                //    });

                //}
                //else
                //{
                //    return BadRequest(new
                //    {

                //        message = ControllerMessages.InvalidVerificationCode
                //    });
                //}




            }

        [HttpPost("sendLoginCode")]
        public async Task<IActionResult> SendLoginCode(SendSMSDto smsdto)
        {
            var phoneExsist = await _userManager.FindByPhoneNumberAsync(smsdto.PhoneNumber);
            if (phoneExsist == null)
            {
                return  BadRequest(new
                {
                    
                    message = ControllerMessages.PhoneNotFound,
                    data = (object?)null
                });
            }
            else
            {
                var code = _verificationCodeService.GenerateCode(smsdto.PhoneNumber);
                var result = _smsService.SendVerificationCode(smsdto.PhoneNumber, code);
                if (!string.IsNullOrEmpty(result.ErrorMessage))
                    return BadRequest(new
                    {
                     
                        message = ControllerMessages.SendCodeFailed,
                        data = (object?)null
                    });
                return Ok(
                    new
                    {
                      
                        message = ControllerMessages.SendCodeSuccess,
                        data = (object?)null
                    });
            }


        }
        //Login EndPoint Domain/Api/Account/login

        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto loginDto, string? fcmToken)
        {
            try
            {
                var user = await _userManager.FindByPhoneNumberAsync(loginDto.PhoneNumber);

                if (user == null)
                {
                    return BadRequest(new
                    {
                        message = ControllerMessages.PhoneNotFound,
                        data = (object?)null
                    });
                }

                await _signInManager.SignInAsync(user, isPersistent: false);

                user.FCMToken = fcmToken ?? string.Empty;
                var updateResult = await _userManager.UpdateAsync(user);
                if (!updateResult.Succeeded)
                {
                    return BadRequest(new
                    {
                        message = "حدث خطأ غير متوقع أثناء تسجيل الدخول.",
                        data = (object?)null
                    });
                }
                return Ok(new
                {
                    message = ControllerMessages.LoginSuccessfull,
                    data = new UserDto()
                    {
                        UserName = user.FullName,
                        Token = await _tokenService.CreateTokenAsync(user)
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "حدث خطأ غير متوقع أثناء تسجيل الدخول.",
                    error = ex.Message,
                    data = (object?)null
                });
            }

            //if (!user.PhoneNumberConfirmed)
            //    return Unauthorized("Phone number not verified.");

            //if (!_verificationCodeService.ValidateCode(loginDto.PhoneNumber, loginDto.Code))
            //    return BadRequest(new
            //    {

            //        message = ControllerMessages.InvalidVerificationCode
            //    });

            //await _signInManager.SignInAsync(user, isPersistent: false);

            //return Ok(new UserDto()
            //{
            //    UserName = user.FullName,

            //    Token = await _tokenService.CreateTokenAsync(user)
            //});
        }




        [HttpPost("resendCode")]
        public async Task<IActionResult> ResendCode(SendSMSDto smsdto)
        {
           
                var code = _verificationCodeService.GenerateCode(smsdto.PhoneNumber);
                var result = _smsService.SendVerificationCode(smsdto.PhoneNumber, code);
                if (!string.IsNullOrEmpty(result.ErrorMessage))
                    return BadRequest(new
                    {
                      
                        message = ControllerMessages.SendCodeFailed,
                        data = (object?)null
                    });

            return Ok(
                new
                {
                   
                    message = ControllerMessages.SendCodeSuccess,
                    data = (object?)null
                });



        }




    }
}
