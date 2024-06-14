using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using SampleWebApp.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SampleWebApp.Pages.Client
{
    public class IndexModel : PageModel
    {
        private readonly DAL _dal;

        public IndexModel(IConfiguration configuration)
        {
            _dal = new DAL(configuration);
        }

        public List<User> Users { get; set; }

        public async Task OnGetAsync()
        {
            Users = await _dal.GetUsersAsync();
        }
    }
}
