using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityLayer.RequestFeautures
{
    public abstract class RequestParams
    {
        const int maxPageSize = 10;
        public int PageNumber { get; set; } = 1;
        private int _pageSize;

        public int PageSize
        {
            get { return _pageSize; }
            set { _pageSize = value <= maxPageSize ? _pageSize = value : _pageSize = maxPageSize; }
        }
    }
}
