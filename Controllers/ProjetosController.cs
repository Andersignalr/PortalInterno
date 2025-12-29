using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Authorize]
public class ProjetosController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public ProjetosController(
        ApplicationDbContext context,
        UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    // =========================
    // LISTAR
    // =========================
    public async Task<IActionResult> Index()
    {
        var projetos = await _context.Projetos
            .OrderByDescending(p => p.CriadoEm)
            .ToListAsync();

        return View(projetos);
    }

    // =========================
    // CRIAR
    // =========================
    public IActionResult Criar()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Criar(Projeto projeto)
    {
        if (!ModelState.IsValid)
            return View(projeto);

        projeto.CriadoEm = DateTime.UtcNow;

        _context.Projetos.Add(projeto);
        await _context.SaveChangesAsync();

        // 👑 quem cria vira gestor automaticamente
        var userId = _userManager.GetUserId(User);

        _context.ProjetoMembros.Add(new ProjetoMembro
        {
            ProjetoId = projeto.Id,
            UsuarioId = userId!,
            Papel = PapelProjeto.Gestor
        });

        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Detalhes), new { id = projeto.Id });
    }

    // =========================
    // DETALHES (core do sistema)
    // =========================
    public async Task<IActionResult> Detalhes(int id)
    {
        var projeto = await _context.Projetos
            .Include(p => p.Tarefas)
            .Include(p => p.Membros)
                .ThenInclude(m => m.Usuario)
            .Include(p => p.Comentarios)
                .ThenInclude(c => c.Usuario)
            .Include(p => p.Timeline)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (projeto == null)
            return NotFound();

        return View(projeto);
    }

    [HttpPost]
    public async Task<IActionResult> Comentar(int projetoId, string texto)
    {
        var comentario = new ComentarioProjeto
        {
            ProjetoId = projetoId,
            UsuarioId = _userManager.GetUserId(User)!,
            Texto = texto
        };

        _context.Add(comentario);
        await _context.SaveChangesAsync();

        return RedirectToAction("Detalhes", new { id = projetoId });
    }

}
