using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace DataService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
    [ServiceContract]
    public interface IDataService
    {
        [OperationContract]
        [WebInvoke(Method = "GET", UriTemplate = "Test/{value}", ResponseFormat = WebMessageFormat.Json)]
        string GetData(string value);

        [OperationContract]
        CompositeType GetDataUsingDataContract(CompositeType composite);

        /// <summary>
        /// Gets all results and stores them in the database
        /// </summary>
        [OperationContract]
        [WebInvoke(Method = "GET", UriTemplate = "Results", ResponseFormat = WebMessageFormat.Json)]
        string[] GetResults();

        /// <summary>
        /// Gets information of a number from the database
        /// </summary>
        [OperationContract]
        [WebInvoke(Method = "GET", UriTemplate = "Results/{number}", ResponseFormat = WebMessageFormat.Json)]
        string[] CheckNumber(string number);
    }


    // Use a data contract as illustrated in the sample below to add composite types to service operations.
    [DataContract]
    public class CompositeType
    {
        bool boolValue = true;
        string stringValue = "Hello ";

        [DataMember]
        public bool BoolValue
        {
            get { return boolValue; }
            set { boolValue = value; }
        }

        [DataMember]
        public string StringValue
        {
            get { return stringValue; }
            set { stringValue = value; }
        }
    }
}
