using System.Collections.Generic;
using PhotoSharingApplication.Models;

namespace PhotoSharingApplication.Models.ViewModels
{
    public class PhotoDisplayViewModel
    {
        public Photo Photo { get; set; }
        public List<Comment> Comments { get; set; } = new List<Comment>();
        public Comment NewComment { get; set; } = new Comment();
    }
}
