using DataAccess;
using System;

namespace DataService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Service1.svc or Service1.svc.cs at the Solution Explorer and start debugging.
    public class DataService : IDataService
    {
        /*public string GetData(string value)
        {
            return string.Format("You entered: {0}", value);
        }

        public ResultType GetDataUsingDataContract(ResultType composite)
        {
            if (composite == null)
            {
                throw new ArgumentNullException("composite");
            }
            if (composite.Status)
            {
                composite.Message += "Suffix";
            }
            return composite;
        }*/

        /// <summary>
        /// Converts a string array to base result type
        /// </summary>
        private ResultType ConvertToResultType(string[] array)
        {
            return new ResultType() { status = Convert.ToBoolean(array[0]), message = array[1] };
        }

        public ResultType GetResults()
        {
            var dataAccess = new DataReader();
            var result = dataAccess.GetResults();
            return ConvertToResultType(result);
        }

        public ResultType CheckNumber(string number)
        {
            var dataAccess = new DataReader();
            var result = dataAccess.CheckNumber(number);
            return ConvertToResultType(result);
        }
    }
}
