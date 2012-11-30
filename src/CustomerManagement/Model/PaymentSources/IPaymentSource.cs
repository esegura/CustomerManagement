using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CustomerManagement
{
    public interface IPaymentSource
    {
        string PaymentType { get; }
        void Process();
    }
}
