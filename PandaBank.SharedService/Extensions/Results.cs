using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PandaBank.SharedService.Extensions
{ 
    public class Results<DTO>
    {
        public DTO Data { get; set; }

        public List<string> Errors { get; set; }
        public bool IsError() => Errors != null && Errors.Any();
        public PagingInfo PageInfo { get; set; }

    }

    public class PagingParameters
    {
        public string OrderBy { get; set; } = "Id";
        public bool Asc { get; set; } = false;
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 30;
        public int Top { get; set; } = 0;
    }

    public class PagingInfo
    {
        public int Total { get; set; }
        public int TotalPages { get; set; }
        public int PageSize { get; set; }
        public int CurrentPage { get; set; }
    }

}
