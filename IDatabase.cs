using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Web.ModelBinding;

namespace DatabaseWebService
{
    [ServiceContract]
    public interface IDatabase
    {

        [OperationContract]
        [WebGet(UriTemplate = "/GetDestination", RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        List<Destination> GetDestination();

        [OperationContract]
        [WebGet(UriTemplate = "/GetDestination/{From}/{To}", RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        Dictionary<string, List<string>> GetDateOfDestination(string from, string to);

        [OperationContract]
        [WebGet(UriTemplate = "/GetConfirmation/{From}/{To}/{Datetime}", RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        Response GetConfirmation(string from, string to, string datetime);

    }



    [DataContract]
    public class Destination
    {
        string Route_from;
        string Route_to;

        [DataMember]
        public string route_from
        {
            get { return Route_from; }
            set { Route_from = value; }
        }

        [DataMember]
        public string route_to
        {
            get { return Route_to; }
            set { Route_to = value; }
        }
    }

    [DataContract]
    public class Date
    {
        int Id;
        DateTime DateDestination;
        int IdDestination;

        [DataMember]
        public int id
        {
            get { return Id; }
            set { Id = value; }
        }

        [DataMember]
        public DateTime dateDestination
        {
            get { return DateDestination; }
            set { DateDestination = value; }
        }

        [DataMember]
        public int idDestination
        {
            get { return IdDestination; }
            set { IdDestination = value; }
        }
    }

    [DataContract]
    public class Response
    {
        int ResponseCode;
        string ResponseMessage;

        [DataMember]
        public int responseCode
        {
            get { return ResponseCode; }
            set { ResponseCode = value; }
        }

        [DataMember]
        public string responseMessage
        {
            get { return ResponseMessage; }
            set { ResponseMessage = value; }
        }
    }


    
}
