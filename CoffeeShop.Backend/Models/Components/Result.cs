using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CoffeeShop.Backend.Models.Components
{
    public class Result
    {
        public bool IsSuccess { get; set; }
        public string ErrorMessage { get; set; }

        public object Data { get; set; }

        public static Result Success()
        {
            return new Result { IsSuccess = true };
        }
   
        public static Result Success(object data = null)
        {
            return new Result { IsSuccess = true, Data = data };
        }
        public static Result Fail(string errorMessage)
        {
            return new Result
            {
                IsSuccess = false,
                ErrorMessage = errorMessage
            };
        }
    }
}