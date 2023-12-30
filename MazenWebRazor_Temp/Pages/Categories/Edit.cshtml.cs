using MazenWebRazor_Temp.Data;
using MazenWebRazor_Temp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MazenWebRazor_Temp.Pages.Categories
{
    [BindProperties]
    public class EditModel : PageModel
    {
        private readonly AppDbContext _context;
        public Category? Category { get; set; }
        public EditModel(AppDbContext context)
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
            _context.Categories.Update(Category);
            _context.SaveChanges();
            TempData["success"] = "Category updated successfully";

            return RedirectToPage("Index");
        }
    }
}
