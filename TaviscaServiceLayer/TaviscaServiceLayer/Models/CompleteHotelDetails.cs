using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TaviscaServiceLayer.Models
{
    public class CompleteHotelDetails
    {
        public int hotel_id { get; set; }
        public string name { get; set; }
        public int rating { get; set; }
        public string city { get; set; }
        public string location { get; set; }
        public string landmark { get; set; }
        public string state { get; set; }
        public string amenities { get; set; }
        public string policies { get; set; }
        public int pincode { get; set; }
        public string image { get; set; }

        public List<rooms> roomsinfo { get; set; }
    }
}