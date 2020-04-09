using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Models.Responses
{
    public class GenericResponse<T> : GenericResponseBase
    {
        internal GenericResponse()
        {
        }
        public T Data { get; set; }

        public static GenericResponse<T> Ok(T data)
        {
            return new GenericResponse<T>
            {
                Data = data,
                Success = true
            };
        }
        public static GenericResponse<T> Error(string errorCode)
        {
            return new GenericResponse<T>
            {
                ErrorCode = errorCode,
                Success = false
            };
        }
    }

    public static class NewResponse
    {
        public static GenericResponse<T> Ok<T>(T data)
        {
            return new GenericResponse<T>
            {
                Data = data,
                Success = true
            };
        }

        public static GenericResponse<T> Error<T>(string errorCode)
        {
            return new GenericResponse<T>
            {
                ErrorCode = errorCode,
                Success = false
            };
        }
    }
}
