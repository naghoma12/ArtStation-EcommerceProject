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
                    return NotFound(new { Message = ControllerMessages.UserNotFound });
                }

                var userData = _mapper.Map<AppUser, UserProfileDto>(user);
                return Ok(userData);
            }
            catch (Exception)
            {
                return BadRequest(new { message = ControllerMessages.SomethingWrong });
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
                    return NotFound(new { message = ControllerMessages.UserNotFound });
                }

                var phoneExsist = await _userManager.FindByPhoneNumberAsync(updateUserProfile.PhoneNumber);

                if (updateUserProfile.PhoneNumber != user.PhoneNumber && phoneExsist != null)
                {
                    return BadRequest(new { Message = ControllerMessages.PhoneNumberIsTaken });
                }

                if (!string.IsNullOrEmpty(updateUserProfile.Email))
                {
                    var emailExsist = await _userManager.FindByEmailAsync(updateUserProfile.Email);
                    if (updateUserProfile.Email != user.Email && emailExsist != null)
                    {
                        return BadRequest(new { Message = ControllerMessages.EmailAlreadyInUse });
                    }
                }

                var userData = _mapper.Map(updateUserProfile, user);
                var result = await _userManager.UpdateAsync(userData);

                if (result.Succeeded)
                {
                    return Ok(new { message = ControllerMessages.UserDataUpdatedSucessfully });
                }
                else
                {
                    return BadRequest(new { message = ControllerMessages.FailedToUpdateUserData });
                }
            }
            catch (Exception)
            {
                return BadRequest(new { message = ControllerMessages.SomethingWrong });
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
                    return NotFound(new { Message = ControllerMessages.UserNotFound });
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
                    return Ok(new { message = ControllerMessages.PhotoUpdatedSucessfully });
                }
                else
                {
                    return BadRequest(new { Message = ControllerMessages.PhotoUploadedFailed });
                }
            }
            catch (Exception)
            {
                return BadRequest(new { message = ControllerMessages.SomethingWrong });
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
                    return NotFound(new { Message = ControllerMessages.UserNotFound });
                }

               
                user.Country = countryDto.Country;

                var result = await _userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    return Ok(new { message = ControllerMessages.CountryAddedSuccessfully});
                }
                else
                {
                    return BadRequest(new { Message = ControllerMessages.CountryFailedToAdd});
                }
            }
            catch (Exception)
            {
                return BadRequest(new { message = ControllerMessages.SomethingWrong });
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
                    Message = ControllerMessages.AddressAddedSucessfully
                });

            }
            else
                return BadRequest(new
                {
                    Message = ControllerMessages.AddressAddedFailed
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
                        Message = ControllerMessages.AddressEditSuccessfully
                    });

                }
                else
                    return BadRequest(new
                    {
                        Message = ControllerMessages.AddressEditFailed
                    });

            }
            catch (Exception ex)
            {

                return BadRequest(new
                {
                    Message = ControllerMessages.AddressEditFailed
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
                    return NotFound(new { Message = ControllerMessages.AddressNotFound });
                }

                var addressmaped = _mapper.Map<Address, AddressDtoUseId>(address);
                return Ok(addressmaped);
            }
            catch (Exception)
            {
                return BadRequest(new
                {
                    Message = ControllerMessages.AddressNotFound
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
                    return Ok(addressesMapped);

                }
                else
                    return NotFound(new { Message = ControllerMessages.AddressLoadedFailed  });

            }
            catch (Exception)
            {

                return BadRequest(new
                {
                    Message = ControllerMessages.AddressLoadedFailed
                });
            }

        }


    }
}
