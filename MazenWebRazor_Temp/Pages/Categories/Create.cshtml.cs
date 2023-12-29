using MazenWebRazor_Temp.Data;
using MazenWebRazor_Temp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MazenWebRazor_Temp.Pages.Categories
{
    public class CreateModel : PageModel
    {
        private readonly AppDbContext _context;
        public Category? Category { get; set; }

        public CreateModel(AppDbContext context)
        {
            _context = context;
        }
        public void OnGet()
        {

        }
        public IActionResult OnPost(Category category)
        {
            _context.Categories.Add(category);
            _context.SaveChanges();

            return RedirectToPage("Index");
        }
    }
}
