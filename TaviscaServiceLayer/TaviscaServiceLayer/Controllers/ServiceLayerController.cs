using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using TaviscaServiceLayer.Models;

namespace TaviscaServiceLayer.Controllers
{
    public class ServiceLayerController : ApiController
    {
        public async System.Threading.Tasks.Task<List<RoomsDetails>> GetRoomsListFromThirdParty()
        {
            Logger log = Logger.getInstance();
            log.LogMessage("Getting Rooms List from Third Party");
            List<RoomsDetails> RoomsInfoFromThirdParty = new List<RoomsDetails>();
            var client = new HttpClient();
            var response = await client.GetAsync("http://localhost:49800/HotelInfoProvider.svc/HotelService");

            if (response.StatusCode == HttpStatusCode.OK)
                RoomsInfoFromThirdParty = await response.Content.ReadAsAsync<List<RoomsDetails>>();

            return RoomsInfoFromThirdParty;

        }
        public List<HotelsDetails> GetHotelsInfoFromJsonFile()
        {
            Logger log = Logger.getInstance();
            log.LogMessage("Getting Hotels List from Third Party");
            List<HotelsDetails> HotelsInfoFromJson = new List<HotelsDetails>();
            using (StreamReader r = new StreamReader("C://HotelDetails.json"))
            {
                string json = r.ReadToEnd();
                HotelsInfoFromJson = JsonConvert.DeserializeObject<List<HotelsDetails>>(json);
            }
            return HotelsInfoFromJson;
        }
        [HttpGet]
        [Route("GetHotels")]
        public async System.Threading.Tasks.Task<List<CompleteHotelDetails>> GetCompleteHotelDetails()
        {
            Logger log = Logger.getInstance();
            log.LogMessage("Merging the Hotel details");
            List<HotelsDetails> HotelsInfoFromJson = GetHotelsInfoFromJsonFile();
            List<RoomsDetails> RoomsInfoFromThirdParty = await GetRoomsListFromThirdParty();
            List<CompleteHotelDetails>  CompleteHotelDetails = new List<Models.CompleteHotelDetails>();
            CompleteHotelDetails hotelobject;
            rooms roomobject;
            int counter = 0;
            while (counter<HotelsInfoFromJson.Count)
            {
                hotelobject = new CompleteHotelDetails();
                hotelobject.hotel_id = HotelsInfoFromJson[counter].hotel_id;
                hotelobject.name = HotelsInfoFromJson[counter].name;
                hotelobject.rating = HotelsInfoFromJson[counter].rating;
                hotelobject.city = HotelsInfoFromJson[counter].city;
                hotelobject.location = HotelsInfoFromJson[counter].location;
                hotelobject.landmark = HotelsInfoFromJson[counter].landmark;
                hotelobject.state = HotelsInfoFromJson[counter].state;
                hotelobject.amenities = HotelsInfoFromJson[counter].amenities;
                hotelobject.policies = HotelsInfoFromJson[counter].policies;
                hotelobject.pincode = HotelsInfoFromJson[counter].pincode;
                hotelobject.image = HotelsInfoFromJson[counter].image;
                int roomcount = 0;
                List<rooms> roomsdetails = new List<rooms>();
                while (roomcount<RoomsInfoFromThirdParty.Count)
                {
                    if (RoomsInfoFromThirdParty[roomcount].hotel_id==HotelsInfoFromJson[counter].hotel_id)
                    {
                        roomobject = new rooms();
                        roomobject.room_id = RoomsInfoFromThirdParty[roomcount].room_id;
                        roomobject.price = RoomsInfoFromThirdParty[roomcount].price;
                        roomobject.numberofrooms = RoomsInfoFromThirdParty[roomcount].numberofrooms;
                        roomsdetails.Add(roomobject);
                    }
                    roomcount++;
                }
                hotelobject.roomsinfo = roomsdetails;
                counter++;
                CompleteHotelDetails.Add(hotelobject);
            }
            return CompleteHotelDetails;
        }
        [HttpPut]
        [Route("Book")]
        public async Task<string> Put([FromBody] BookingDetails bookdetails)
        {
            Logger log = Logger.getInstance();
            
            try
            {
                log.LogMessage("Make a Booking");
                List<RoomsDetails> RoomsInfoFromThirdParty = await GetRoomsListFromThirdParty();
                var abc = RoomsInfoFromThirdParty.Find(i => i.hotel_id == bookdetails.hotel_id && i.room_id == bookdetails.room_id);
                bookdetails.numberofroomsavailable = abc.numberofrooms;
                if (bookdetails.numberofroomsavailable - bookdetails.numberofroomstobook < 0)
                    throw new Exception("Rooms not available");
                using (SqlConnection conn = new SqlConnection())
                {
                    var Token = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
                    conn.ConnectionString = "Data Source=TAVDESK091;Initial Catalog=TaviscaHotels; User Id=sa;Password=test123!@#";
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("insert into HotelBookingDetails(UserEmailId,UserName,hotel_id,room_id,numberofroomsbooked) values(@useremail,@username,@hotelid,@roomid,@numberofrooms)", conn);
                    cmd.Parameters.AddWithValue("@useremail", bookdetails.UserEmailId);
                    cmd.Parameters.AddWithValue("@username", bookdetails.UserName);
                    cmd.Parameters.AddWithValue("@hotelid", bookdetails.hotel_id);
                    cmd.Parameters.AddWithValue("@roomid", bookdetails.room_id);
                    cmd.Parameters.AddWithValue("@numberofrooms", bookdetails.numberofroomstobook);
                    cmd.ExecuteNonQuery();
                    conn.Close();
                    log.LogMessage("Booking Made Successful");
                    
                }

                string url = "http://localhost:49800/HotelInfoProvider.svc/BookingService";
                HttpClient httpClient = new HttpClient();
                HttpResponseMessage response = await httpClient.PutAsJsonAsync(url, bookdetails);
                return "Booking Made Successfully";
            }
            catch (Exception)
            {
                log.LogMessage("Booking Failed");
                return "Booking Failed";
            }
                      
        }
    }
}
