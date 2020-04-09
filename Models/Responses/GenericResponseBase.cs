using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Responses
{
    public class GenericResponseBase
    {
        public bool Success { get; set; }
        public string ErrorCode { get; set; }

        public static GenericResponseBase Ok()
        {
            return new GenericResponseBase
            {
                Success = true
            };
        }

        public static GenericResponseBase Error(string errorCode)
        {
            return new GenericResponseBase
            {
                Success = false,
                ErrorCode = errorCode
            };
        }
    }
}
