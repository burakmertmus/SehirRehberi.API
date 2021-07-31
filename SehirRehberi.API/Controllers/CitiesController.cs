using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SehirRehberi.API.Data;
using SehirRehberi.API.Dtos;
using SehirRehberi.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SehirRehberi.API.Controllers
{
    [Produces("application/json")]
    [ApiController]
    [Route("cities")]
    public class CitiesController : Controller
    {
        private IAppRepository _appRepository;
        private IMapper _mapper;
        public CitiesController(IAppRepository appRepository, IMapper mapper)
        {
            _appRepository = appRepository;
            _mapper = mapper;
        }

        [HttpGet]
        public ActionResult GetCities()
        {

            var cities = _appRepository.GetCities();
            //AutoMapper Olmasaydı
            //     .Select(c =>
            //     new CityForListDto { Description = c.Description, Name = c.Name, Id = c.Id, PhotoUrl = c.Photos.FirstOrDefault(p => p.IsMain == true).Url }).ToList();
            var citiesToReturn = _mapper.Map<List<CityForListDto>>(cities);
            return Ok(citiesToReturn);
        }

        [HttpPost]
        [Route("add")]
        public ActionResult Add([FromBody] City city)
        {
            
            //TimeZoneInfo cstZone = TimeZoneInfo.FindSystemTimeZoneById("GMT Standard Time");
            //city.DateAdded=TimeZoneInfo.ConvertTimeFromUtc(city.DateAdded.Date, cstZone);


            _appRepository.Add(city);
            _appRepository.SaveAll();
            return Ok(city);
        }


        [HttpGet]
        [Route("detail")]
        public ActionResult GetCityById(int id)
        {

            var city = _appRepository.GetCityById(id);
            var cityToReturn = _mapper.Map<CityForDetailDto>(city);
            return Ok(cityToReturn);
        }


        [HttpGet("cityId")]
        [Route("CityPhotos")]
        public ActionResult GetPhotosById(int cityId)
        {
            var photos = _appRepository.GetPhotosByCity(cityId);
            return Ok(photos);
        }




    }
}
