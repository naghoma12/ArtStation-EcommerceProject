using ArtStation.Core;
using ArtStation.Core.Entities.Identity;
using ArtStation.Core.Repository.Contract;
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

        [Authorize]
        [HttpGet("GetUser")]
        public async Task<ActionResult<UserProfileDto>> GetCurrentUser()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user is null)
            {
                return NotFound(new
                {
                    Message = ControllerMessages.UserNotFound
                });
            }

           
            return Ok(new UserProfileDto()

            {
                Fname = user.FullName?.Split(' ')[0] ??null,
                LName = user.FullName?.Split(' ')[1]?? null,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Gender = EnumHelper.GetEnumMemberValue(user.Gender),
                Nationality = user?.Nationality,
                Birthday = user?.BirthDay?.ToString("yyyy-MM-dd"),
                Photo = user?.Image,
                Country = user?.Country

            });
        }


        [Authorize]
        [HttpPut("UpdateUser")]
        public async Task<ActionResult> UpdateUser(UpdateUserProfileDto updateUserProfile)
        {

            try
            {
                var user = await _userManager.GetUserAsync(User);

                var phoneExsist = await _userManager.FindByPhoneNumberAsync(updateUserProfile.PhoneNumber);

                if (updateUserProfile.PhoneNumber != user.PhoneNumber)
                {
                    if (phoneExsist != null)
                    {
                        return BadRequest(new { Message = ControllerMessages.PhoneNumberIsTaken });
                    }
                }
                if (updateUserProfile.Email !=null)
                {
                    var emailExsist = await _userManager.FindByEmailAsync(updateUserProfile.Email);
                    if (updateUserProfile.Email != user.Email)
                    {
                        if (emailExsist != null)
                        {

                            return BadRequest(new { Message = ControllerMessages.EmailAlreadyInUse });
                        }


                    }
                }
                if (user == null)
                {
                    return NotFound(new { message = ControllerMessages.UserNotFound });
                }
                user.FullName = updateUserProfile.Fname + ' ' + updateUserProfile.LName;
                user.Email = updateUserProfile.Email;
                user.PhoneNumber = updateUserProfile.PhoneNumber;
                user.UserName = updateUserProfile.PhoneNumber;
                user.BirthDay = Common.ConvertBirthday(updateUserProfile?.Birthday);
                user.Nationality = string.IsNullOrWhiteSpace(updateUserProfile?.Nationality) ? null: updateUserProfile?.Nationality;
                Enum.TryParse<Gender>(updateUserProfile?.Gender?.ToLower(), true, out var genderEnum);
                user.Gender = updateUserProfile.Gender ==null ? null:genderEnum;

                var result = await _userManager.UpdateAsync(user);
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
        [HttpPut("UpdateProfilePhoto")]
        public async Task<ActionResult> AddProfilePhoto(UserProfilePhotoDto userProfilePhotoDto)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound(new { Message = ControllerMessages.UserNotFound });
            }

            var photoName = "";

            // Check if the user already has an existing photo and delete it
            if (!string.IsNullOrEmpty(user.Image))
            {
                HandlerPhoto.DeletePhoto("Users", user.Image);
                photoName = HandlerPhoto.UploadPhoto(userProfilePhotoDto.Photo, "Users");
            }
            else
            {
                photoName = HandlerPhoto.UploadPhoto(userProfilePhotoDto.Photo, "Users");
            }
            // Update the user's photo path
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
