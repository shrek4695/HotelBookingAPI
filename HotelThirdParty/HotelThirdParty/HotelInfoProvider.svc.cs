using Cassandra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace HotelThirdParty
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "HotelInfoProvider" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select HotelInfoProvider.svc or HotelInfoProvider.svc.cs at the Solution Explorer and start debugging.
    public class HotelInfoProvider : IHotelInfoProvider
    {
        public List<RoomsInfo> GetHotelInfo()
        {
            List<RoomsInfo> roomList = new List<RoomsInfo>();
            RoomsInfo room;
            var cluster = Cluster.Builder()
            .AddContactPoints("127.0.0.1")
            .Build();
            var session = cluster.Connect("hotels");
            var result = session.Execute("Select hotel_id,room_id,numberofrooms,price from roomsinfo");
            foreach (Row row in result)
            {
                room = new RoomsInfo();
                room.hotel_id = row.GetValue<int>("hotel_id");
                room.room_id = row.GetValue<int>("room_id");
                room.numberofrooms = row.GetValue<int>("numberofrooms");
                room.price = row.GetValue<int>("price");
                roomList.Add(room);   
            }
            return roomList;
        }

        public string MakeABooking(BookingDetails book)
        {
            try
            {
                var cluster = Cluster.Builder()
                .AddContactPoints("127.0.0.1")
                .Build();
                var session = cluster.Connect("hotels");
                var result = session.Execute("UPDATE hotels.roomsinfo SET numberofrooms =" + (book.numberofroomsavailable - book.numberofroomstobook) + " WHERE room_id =" + book.room_id + " and hotel_id=" + book.hotel_id + "; ");
                return "Success";
            }
            catch(Exception e)
            {
                return e.Message;
            }
        }
    }
}
