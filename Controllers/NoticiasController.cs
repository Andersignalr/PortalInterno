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

    // =========================
    // FEED PÚBLICO
    // =========================
    public async Task<IActionResult> Index()
    {
        var noticias = await _context.Noticias
            .Where(n => n.Publicada)
            .Include(n => n.Autor)
            .OrderByDescending(n => n.DataPublicacao)
            .ToListAsync();

        return View(noticias);
    }

    // =========================
    // DETALHES
    // =========================
    public async Task<IActionResult> Detalhes(int id)
    {
        var noticia = await _context.Noticias
            .Include(n => n.Autor)
            .FirstOrDefaultAsync(n => n.Id == id);

        if (noticia == null)
            return NotFound();

        return View(noticia);
    }

    // =========================
    // CRIAR
    // =========================
    [Authorize(Roles = "Admin,Editor,Autor")]
    public IActionResult Criar()
    {
        return View(new Noticia());
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Editor,Autor")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Criar(Noticia noticia)
    {
        if (!ModelState.IsValid)
            return View(noticia);

        var userId = _userManager.GetUserId(User)!;

        noticia.AutorId = userId;
        noticia.DataPublicacao = DateTime.UtcNow;

        // 🔒 REGRA DE NEGÓCIO
        if (!User.IsInRole("Admin") && !User.IsInRole("Editor"))
        {
            // Autor nunca publica direto
            noticia.Publicada = false;
        }

        _context.Noticias.Add(noticia);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    // =========================
    // EDITAR
    // =========================
    [Authorize(Roles = "Admin,Editor")]
    public async Task<IActionResult> Editar(int id)
    {
        var noticia = await _context.Noticias.FindAsync(id);
        if (noticia == null)
            return NotFound();

        return View(noticia);
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Editor")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Editar(int id, Noticia model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var noticiaDb = await _context.Noticias.FindAsync(id);
        if (noticiaDb == null)
            return NotFound();

        noticiaDb.Titulo = model.Titulo;
        noticiaDb.Conteudo = model.Conteudo;

        // Só Admin/Editor podem alterar publicação
        noticiaDb.Publicada = model.Publicada;

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    // =========================
    // EXCLUIR
    // =========================
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Excluir(int id)
    {
        var noticia = await _context.Noticias.FindAsync(id);
        if (noticia == null)
            return NotFound();

        return View(noticia);
    }

    [HttpPost, ActionName("Excluir")]
    [Authorize(Roles = "Admin")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ExcluirConfirmado(int id)
    {
        var noticia = await _context.Noticias.FindAsync(id);
        if (noticia == null)
            return NotFound();

        _context.Noticias.Remove(noticia);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    // =========================
    // GERENCIAR
    // =========================
    [Authorize(Roles = "Admin,Editor")]
    public async Task<IActionResult> Gerenciar()
    {
        IQueryable<Noticia> query = _context.Noticias
            .Include(n => n.Autor);

        // Editor vê apenas as próprias
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
