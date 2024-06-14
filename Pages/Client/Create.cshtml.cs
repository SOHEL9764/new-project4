using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using SampleWebApp.Model;
using System.Threading.Tasks;

namespace SampleWebApp.Pages.Client
{
    public class CreateModel : PageModel
    {
        private readonly DAL _dal;

        public CreateModel(IConfiguration configuration)
        {
            _dal = new DAL(configuration);
        }

        [BindProperty]
        public User User { get; set; }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            await _dal.AddUserAsync(User);
            return RedirectToPage("./Index");
        }
    }
}
