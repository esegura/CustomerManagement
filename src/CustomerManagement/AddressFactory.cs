using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CustomerManagement.Model.AddressType;
using System.Reflection;
using CustomerManagement.Model;
/*
namespace CustomerManagement
{
    internal class AddressFactory
    {
        internal static Address Create(Address.CountryCode country)
        {
            Address result = null;

            switch (country)
            {
                case Address.CountryCode.US:
                    result = new USAddress();
                    break;
                case Address.CountryCode.UK:
                    result = new UKAddress();
                    break;
                case Address.CountryCode.CA:
                    result = new CAAddress();
                    break;
                case Address.CountryCode.AU:
                    result = new AUAddress();
                    break;
                default:
                    throw new ArgumentOutOfRangeException("country");
            }

            result.Country = country;
            return result;
        }

        // Needed for serialization
        internal static System.Type[] GetAddressTypes()
        {
            return typeof(AddressFactory).Module.FindTypes((t,o) => t.IsSubclassOf(typeof(Address)), null);
        }



    }
}
*/