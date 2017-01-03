using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;

namespace DataService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
    [ServiceContract]
    public interface IDataService
    {
        /*[OperationContract]
        [WebInvoke(Method = "GET", UriTemplate = "Test/{value}", ResponseFormat = WebMessageFormat.Json)]
        string GetData(string value);

        [OperationContract]
        ResultType GetDataUsingDataContract(ResultType composite);*/

        /// <summary>
        /// Gets all results and stores them in the database
        /// </summary>
        [OperationContract]
        [WebInvoke(Method = "GET", UriTemplate = "Results", ResponseFormat = WebMessageFormat.Json)]
        ResultType GetResults();

        /// <summary>
        /// Gets information of a number from the database
        /// </summary>
        [OperationContract]
        [WebInvoke(Method = "GET", UriTemplate = "Results/{number}", ResponseFormat = WebMessageFormat.Json)]
        ResultType CheckNumber(string number);
    }


    // Use a data contract as illustrated in the sample below to add composite types to service operations.
    [DataContract]
    public class ResultType
    {
        bool _status = true;
        string _message = "Hello ";

        [DataMember]
        public bool status
        {
            get { return _status; }
            set { _status = value; }
        }

        [DataMember]
        public string message
        {
            get { return _message; }
            set { _message = value; }
        }
    }
}
