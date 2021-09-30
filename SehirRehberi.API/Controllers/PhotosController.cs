using AutoMapper;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SehirRehberi.API.Data;
using SehirRehberi.API.Dtos;
using SehirRehberi.API.Helpers;
using SehirRehberi.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SehirRehberi.API.Controllers
{
    [Produces("application/json")]

    [Authorize]
    [Route("cities/photos")]

    public class PhotosController : Controller
    {
        private IAppRepository _appRepository;
        IMapper _mapper;
        IOptions<CloudinarySettings> _cloudinaryConfig;
        private Cloudinary _cloudinary;
        public PhotosController(IAppRepository appRepository, IMapper mapper, IOptions<CloudinarySettings> cloudinaryConfig)
        {
            _appRepository = appRepository;
            _mapper = mapper;
            _cloudinaryConfig = cloudinaryConfig;

            Account account = new Account(
                _cloudinaryConfig.Value.CloudName,
                _cloudinaryConfig.Value.ApiKey,
                _cloudinaryConfig.Value.ApiSecret);

            _cloudinary = new Cloudinary(account);
        }

        //./cities/photos/cityId?={{cityId}}
        [HttpPost]
        public ActionResult AddPhotoForCity(int cityId, [FromForm] PhotoForCreationDto photoForCreationDto)
        {
            var city = _appRepository.GetCityById(cityId);

            if (city == null)
            {
                return BadRequest("Could not find the city");
            }

            var currentUserId = 0;
            System.Security.Claims.ClaimsPrincipal currentUser = HttpContext.User;
            if (currentUser.HasClaim(c => c.Type == ClaimTypes.NameIdentifier))
            {
                currentUserId = int.Parse(currentUser.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value);
            }


            //var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);


            if (currentUserId != city.UserId)
            {
                return Unauthorized();
            }

            var file = photoForCreationDto.File;
            var uploadResult = new ImageUploadResult();
            if (file.Length > 0)
            {
                using (var stream = file.OpenReadStream())
                {
                    var uploadParams = new ImageUploadParams
                    {
                        File = new FileDescription(file.Name, stream)
                    };

                    uploadResult = _cloudinary.Upload(uploadParams);
                }

            }

            photoForCreationDto.Url = uploadResult.Uri.ToString();
            photoForCreationDto.PublicId = uploadResult.PublicId;

            var photo = _mapper.Map<Photo>(photoForCreationDto);
            photo.City = city;
            photo.CityId = cityId;
            photo.Description = city.Description;
            if (!city.Photos.Any(p => p.IsMain))
            {
                photo.IsMain = true;
            }
            city.Photos.Add(photo);
            if (_appRepository.SaveAll())
            {
                var photoToReturn = _mapper.Map<PhotoForReturnDto>(photo);
                return CreatedAtRoute("GetPhoto", new { id = photo.Id }, photoToReturn);
            }
            return BadRequest("Could not add the photo");

        }



        //./cities/photos/{Id}
        [HttpGet("{Id}", Name = "GetPhoto")]
        public ActionResult GetPhoto(int Id)
        {
            var photoFromDb = _appRepository.GetPhoto(Id);
            var photo = _mapper.Map<PhotoForReturnDto>(photoFromDb);
            return Ok(photo);

        }

    }
}
