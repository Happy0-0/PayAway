using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PayAway.WebAPI
{
    public class Enums
    {
        public enum ORDER_STATUS
        {
            Undefined = 0,
            New = 1,
            Updated = 2,
            SMS_Sent = 3,
            Paid = 4
        }
    }
}
