using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseContext.Models.Enums
{
    public enum PaymentStatus : int
    {
        [Description("Not defined")]
        Undefined = 0,
        [Description("Success")]
        Success = 1,
        [Description("Failed")]
        Failed = 2
    }
}
