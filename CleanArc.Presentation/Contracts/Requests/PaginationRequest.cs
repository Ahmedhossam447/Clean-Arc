using System;
using System.Collections.Generic;
using System.Text;

namespace CleanArc.Application.Contracts.Requests
{
    public class PaginationRequest
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
