using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace HotelThirdParty
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IHotelInfoProvider" in both code and config file together.
    [ServiceContract]
    public interface IHotelInfoProvider
    {
        [OperationContract]
        [WebGet(UriTemplate = "/HotelService", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        
        List<RoomsInfo> GetHotelInfo();

        [OperationContract]
        [WebInvoke(UriTemplate = "/BookingService", Method = "PUT", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]

        string MakeABooking(BookingDetails book);

    }
}
