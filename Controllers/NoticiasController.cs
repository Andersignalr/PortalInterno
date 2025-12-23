using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

public class NoticiasController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public NoticiasController(
        ApplicationDbContext context,
        UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    // LISTAGEM (Feed)
    public async Task<IActionResult> Index()
    {
        var noticias = await _context.Noticias
            .Where(n => n.Publicada)
            .Include(n => n.Autor)
            .OrderByDescending(n => n.DataPublicacao)
            .ToListAsync();

        return View(noticias);
    }


    // DETALHES
    public async Task<IActionResult> Detalhes(int id)
    {
        var noticia = await _context.Noticias
            .Include(n => n.Autor)
            .FirstOrDefaultAsync(n => n.Id == id);

        if (noticia == null) return NotFound();

        return View(noticia);
    }

    // CREATE
    [Authorize(Roles = "Admin,Editor")]
    public IActionResult Criar()
    {
        var noticia = new Noticia
        {
            DataPublicacao = DateTime.UtcNow
        };

        return View(noticia);
    }


    [HttpPost]
    [Authorize(Roles = "Admin,Editor")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Criar(Noticia noticia)
    {
        if (!ModelState.IsValid)
            return View(noticia);

        noticia.AutorId = _userManager.GetUserId(User)!;
        noticia.DataPublicacao = DateTime.UtcNow;

        _context.Noticias.Add(noticia);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }



    // EDIT
    [Authorize(Roles = "Admin,Editor")]
    public async Task<IActionResult> Editar(int id)
    {
        var noticia = await _context.Noticias.FindAsync(id);
        if (noticia == null) return NotFound();

        return View(noticia);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Editar(int id, Noticia model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var noticiaDb = await _context.Noticias.FindAsync(id);
        if (noticiaDb == null)
            return NotFound();

        // Atualiza apenas os campos editáveis
        noticiaDb.Titulo = model.Titulo;
        noticiaDb.Conteudo = model.Conteudo;
        noticiaDb.Publicada = model.Publicada;

        // NÃO mexe no AutorId
        // noticiaDb.AutorId permanece intacto

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }


    // DELETE
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Excluir(int id)
    {
        var noticia = await _context.Noticias.FindAsync(id);
        if (noticia == null) return NotFound();

        return View(noticia);
    }

    [HttpPost, ActionName("Excluir")]
    [Authorize(Roles = "Admin")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ExcluirConfirmado(int id)
    {
        var noticia = await _context.Noticias.FindAsync(id);

        _context.Noticias.Remove(noticia!);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = "Admin,Editor")]
    public async Task<IActionResult> Gerenciar()
    {
        IQueryable<Noticia> query = _context.Noticias
            .Include(n => n.Autor);

        // Editor vê só as próprias
        if (User.IsInRole("Editor") && !User.IsInRole("Admin"))
        {
            var userId = _userManager.GetUserId(User);
            query = query.Where(n => n.AutorId == userId);
        }

        var noticias = await query
            .OrderByDescending(n => n.DataPublicacao)
            .ToListAsync();

        return View(noticias);
    }

}
