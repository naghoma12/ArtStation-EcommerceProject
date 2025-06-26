using ArtStation.Core;
using ArtStation.Core.Entities.Identity;
using ArtStation.Core.Repository.Contract;
using ArtStation.Core.Roles;
using ArtStation.Dtos.AuthDtos;
using ArtStation.Dtos.UserDtos;
using ArtStation.Extensions;
using ArtStation.Helper;
using ArtStation.Resources;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Globalization;
using System.Reflection;
using System.Runtime.Serialization;

namespace ArtStation.Controllers
{
    [Authorize(Roles = Roles.Customer)]
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IAddressRepository _addressRepository;
        private readonly UserManager<AppUser> _userManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public UserController(IAddressRepository addressRepository,UserManager<AppUser> userManager, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _addressRepository = addressRepository;
            _userManager = userManager;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        [Authorize(Roles = Roles.Customer)]
        [HttpGet("GetUser")]
        public async Task<ActionResult<UserProfileDto>> GetCurrentUser()
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user is null)
                {
                    return NotFound(new { Message = ControllerMessages.UserNotFound ,
                    data=(object?)null});
                }

                var userData = _mapper.Map<AppUser, UserProfileDto>(user);
                userData.Photo= userData.Photo!=null ? "Images/Users/" + userData.Photo : null;
                return Ok(new { message=ControllerMessages.GetUserDataSuccess,data= userData });
            }
            catch (Exception)
            {
                return BadRequest(new { message = ControllerMessages.SomethingWrong, data = (object?)null  });
            }
        }


        [Authorize]
        [HttpPut("UpdateUser")]
        public async Task<ActionResult> UpdateUser(UpdateUserProfileDto updateUserProfile)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return NotFound(new { message = ControllerMessages.UserNotFound , data = (object?)null });
                }

                var phoneExsist = await _userManager.FindByPhoneNumberAsync(updateUserProfile.PhoneNumber);

                if (updateUserProfile.PhoneNumber != user.PhoneNumber && phoneExsist != null)
                {
                    return BadRequest(new { Message = ControllerMessages.PhoneNumberIsTaken, data = (object?)null  });
                }

                if (!string.IsNullOrEmpty(updateUserProfile.Email))
                {
                    var emailExsist = await _userManager.FindByEmailAsync(updateUserProfile.Email);
                    if (updateUserProfile.Email != user.Email && emailExsist != null)
                    {
                        return BadRequest(new { Message = ControllerMessages.EmailAlreadyInUse , data = (object?)null });
                    }
                }

                var userData = _mapper.Map(updateUserProfile, user);
                var result = await _userManager.UpdateAsync(userData);

                if (result.Succeeded)
                {
                    return Ok(new { message = ControllerMessages.UserDataUpdatedSucessfully
                    ,
                        data = (object?)null
                    });
                }
                else
                {
                    return BadRequest(new { message = ControllerMessages.FailedToUpdateUserData,
                        data = (object?)null
                    });
                }
            }
            catch (Exception)
            {
                return BadRequest(new { message = ControllerMessages.SomethingWrong,
                    data = (object?)null
                });
            }
        }


        [Authorize]
        [HttpPatch("UpdateProfilePhoto")]
        public async Task<ActionResult> AddProfilePhoto(UserProfilePhotoDto userProfilePhotoDto)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return NotFound(new { Message = ControllerMessages.UserNotFound,
                        data = (object?)null
                    });
                }

                string photoName;

                // Delete old photo if exists
                if (!string.IsNullOrEmpty(user.Image))
                {
                    HandlerPhoto.DeletePhoto("Users", user.Image);
                }

                photoName = HandlerPhoto.UploadPhoto(userProfilePhotoDto.Photo, "Users");
                user.Image = photoName;

                var result = await _userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    return Ok(new { message = ControllerMessages.PhotoUpdatedSucessfully,
                        data = (object?)null
                    });
                }
                else
                {
                    return BadRequest(new { Message = ControllerMessages.PhotoUploadedFailed,
                        data = (object?)null
                    });
                }
            }
            catch (Exception)
            {
                return BadRequest(new { message = ControllerMessages.SomethingWrong,
                    data = (object?)null
                });
            }
        }

        [Authorize]
        [HttpPatch("Country")]
        public async Task<ActionResult> ChooseCountry(CountryDto countryDto)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return NotFound(new { Message = ControllerMessages.UserNotFound,
                        data = (object?)null
                    });
                }

               
                user.Country = countryDto.Country;

                var result = await _userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    return Ok(new { message = ControllerMessages.CountryAddedSuccessfully,
                        data = (object?)null
                    });
                }
                else
                {
                    return BadRequest(new { Message = ControllerMessages.CountryFailedToAdd,
                        data = (object?)null
                    });
                }
            }
            catch (Exception)
            {
                return BadRequest(new { message = ControllerMessages.SomethingWrong,
                    data = (object?)null
                });
            }
        }

        [Authorize]
        [HttpPost("AddAddress")]
        public async Task<ActionResult> AddAddress(AddressDto address)
        {

            var addressResult = await _userManager.AddAddressUser(_unitOfWork, address, User);
            if (addressResult > 0)
            {
                return Ok(new
                {
                    Message = ControllerMessages.AddressAddedSucessfully,
                    data = (object?)null
                });

            }
            else
                return BadRequest(new
                {
                    Message = ControllerMessages.AddressAddedFailed,
                    data = (object?)null
                });


        }

        [Authorize]
        [HttpPut("EditAddress")]
        public async Task<ActionResult> EditAddress(AddressDtoUseId addressDto)

        {

            try
            {
                var user = await _userManager.GetUserAsync(User);
                var useraddress = await _unitOfWork.Repository<Address>().GetByIdAsync(addressDto.Id);

                _mapper.Map(addressDto, useraddress);
                _unitOfWork.Repository<Address>().Update(useraddress);
                var count = await _unitOfWork.Complet();
                if (count > 0)
                {
                    return Ok(new
                    {
                        Message = ControllerMessages.AddressEditSuccessfully,
                        data = (object?)null
                    });

                }
                else
                    return BadRequest(new
                    {
                        Message = ControllerMessages.AddressEditFailed,
                        data = (object?)null
                    });

            }
            catch (Exception ex)
            {

                return BadRequest(new
                {
                    Message = ControllerMessages.AddressEditFailed,
                    data = (object?)null
                });

            }


        }

        [Authorize]
        [HttpGet("GetAddress")]
        public async Task<ActionResult<AddressDtoUseId>> GetAddress(int id)
        {
            try
            {
                var address = await _unitOfWork.Repository<Address>().GetByIdAsync(id);
                if (address == null)
                {
                    return NotFound(new { Message = ControllerMessages.AddressNotFound,
                        data = (object?)null
                    });
                }

                var addressmaped = _mapper.Map<Address, AddressDtoUseId>(address);
                return Ok(new { message =ControllerMessages.GetAddressSucessfull, data = addressmaped });
            }
            catch (Exception)
            {
                return BadRequest(new
                {
                    Message = ControllerMessages.AddressNotFound,
                    data = (object?)null
                });


            }

        }

        [Authorize]
        [HttpGet("GetUserAddressess")]
        public async Task<ActionResult<AddressDtoUseId>> GetUserAddressess()
        {
            var user = await _userManager.GetUserAsync(User);

            try
            {

                var addresses = await _addressRepository.GetAllUserAddress(user.Id);
                var addressesMapped = _mapper.Map<IEnumerable<Address>, IEnumerable<AddressDtoUseId>>(addresses);

                if (addressesMapped.Count() != 0)
                {
                    return Ok(new { message = ControllerMessages.GetAddressSucessfull, data = addressesMapped });

                }
                else
                    return NotFound(new { Message = ControllerMessages.AddressLoadedFailed,
                        data = (object?)null
                    });

            }
            catch (Exception)
            {

                return BadRequest(new
                {
                    Message = ControllerMessages.AddressLoadedFailed,
                    data = (object?)null
                });
            }

        }


    }
}
