using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

public class HomeController : Controller
{
    private readonly ApplicationDbContext _context;

    public HomeController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index(int page = 1)
    {
        const int pageSize = 5;

        var query = _context.Noticias
        .Include(n => n.Autor)
        .Where(n => n.Publicada || User.IsInRole("Admin") || User.IsInRole("Editor"));


        var total = await query.CountAsync();

        var noticias = await query
            .OrderByDescending(n => n.DataPublicacao)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var model = new FeedNoticiasViewModel
        {
            Noticias = noticias,
            PaginaAtual = page,
            TotalPaginas = (int)Math.Ceiling(total / (double)pageSize)
        };

        return View(model);
    }
}
