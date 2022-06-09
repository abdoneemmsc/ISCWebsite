using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CMS.Web.ViewModels
{
    public class HomePageViewModel
    {
        public HomePageViewModel()
        {
            Sliders = new List<PostViewModel>();
            Services = new List<PostViewModel>();
            Products = new List<PostViewModel>();
            News = new List<PostViewModel>();
            OurClients = new List<PostViewModel>();
            Features = new List<PostViewModel>();
        }
        public List<PostViewModel> Sliders { get; set; }
        public PostViewModel About { get; set; }
        public List<PostViewModel> Services { get; set; }
        public List<PostViewModel> Products { get; set; }
        public List<PostViewModel> News { get; set; }
        public List<PostViewModel> OurClients { get; set; }
        public List<PostViewModel> Features { get; set; }
        public string MapUrl { get; set; }

    }
}
