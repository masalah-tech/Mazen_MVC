using MazenWebRazor_Temp.Data;
using MazenWebRazor_Temp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MazenWebRazor_Temp.Pages.Categories
{
    [BindProperties]
    public class DeleteModel : PageModel
    {
        private readonly AppDbContext _context;
        public Category? Category { get; set; }

        public DeleteModel(AppDbContext context)
        {
            _context = context;
        }
        public void OnGet(int id)
        {
            Category =
                _context.Categories
                .Find(id);
        }
        public IActionResult OnPost()
        {
            _context.Categories.Remove(Category);
            _context.SaveChanges();
            TempData["success"] = "Category deleted successfully";

            return RedirectToPage("Index");
        }
    }
}
