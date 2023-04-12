using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityLayer.Exceptions
{
    public sealed class PriceOutRangeBadRequestException : BadRequestException
    {
        public PriceOutRangeBadRequestException() : base("Max price should be less than 1000 and greater then 10")
        {

        }
    }
}
