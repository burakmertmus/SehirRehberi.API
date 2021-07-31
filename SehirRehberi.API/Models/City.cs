﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SehirRehberi.API.Models
{
    public class City
    {
        public City()
        {
            Photos = new List<Photo>();
            TimeZoneInfo GMT = TimeZoneInfo.FindSystemTimeZoneById("GMT Standard Time");
            DateAdded = TimeZoneInfo.ConvertTimeToUtc(DateAdded, GMT);
        }
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime DateAdded { get; set; }
        //public int PhotoId { get; set; }
        public List<Photo> Photos { get; set; }
        public User User { get; set; }
    }
}
