using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TaviscaServiceLayer.Models
{
    public class BookingDetails
    {
        public string UserEmailId { get; set; }
        public string UserName { get; set; }
        public int hotel_id { get; set; }
        public int room_id { get; set; }
        public int numberofroomsavailable { get; set; }
        public int numberofroomstobook { get; set; }
    }
}