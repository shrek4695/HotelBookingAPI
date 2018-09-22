using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TaviscaServiceLayer.Models
{
    public class RoomsDetails
    {
        public int hotel_id { get; set; }
        public int room_id { get; set; }
        public int price { get; set; }
        public int numberofrooms { get; set; }
    }
}