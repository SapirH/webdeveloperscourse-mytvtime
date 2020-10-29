using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MyTvTime.NamespaceAnnotations
{
	public class MinimumAgeAttribute : ValidationAttribute
	{
        int _minimumAge;
        string _errorMessage;

        public MinimumAgeAttribute(int minimumAge, string ErrorMessage)
        {
            _minimumAge = minimumAge;
            _errorMessage = ErrorMessage;
        }

        public override bool IsValid(object value)
        {
            DateTime date;
            if (DateTime.TryParse(value.ToString(), out date))
            {
                return date.AddYears(_minimumAge) < DateTime.Now;
            }

            return false;
        }

        public override string FormatErrorMessage(string name)
        {
            return _errorMessage;
        }

    }
}
