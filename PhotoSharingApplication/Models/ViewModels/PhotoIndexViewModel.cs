using System.Collections.Generic;
using PhotoSharingApplication.Models;

namespace PhotoSharingApplication.Models.ViewModels
{
    public class PhotoIndexViewModel
    {
        public List<Photo> Photos { get; set; } = new List<Photo>();

        public string Query { get; set; }
        public string Sort { get; set; } = "latest";

        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 12;

        public int TotalCount { get; set; }
        public int TotalPages { get; set; }
    }
}
