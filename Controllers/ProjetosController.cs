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

    // ======================================================
    // MÉTODO CENTRAL — membro atual no projeto
    // ======================================================
    private async Task<ProjetoMembro?> GetMembroProjeto(int projetoId)
    {
        var userId = _userManager.GetUserId(User);

        return await _context.ProjetoMembros
            .Include(m => m.Projeto)
            .FirstOrDefaultAsync(m =>
                m.ProjetoId == projetoId &&
                m.UsuarioId == userId);
    }

    // ======================================================
    // LISTAR — apenas projetos do usuário
    // ======================================================
    public async Task<IActionResult> Index()
    {
        var userId = _userManager.GetUserId(User);

        var projetos = await _context.ProjetoMembros
            .Where(pm => pm.UsuarioId == userId)
            .Select(pm => pm.Projeto)
            .OrderByDescending(p => p.CriadoEm)
            .ToListAsync();

        return View(projetos);
    }

    // ======================================================
    // CRIAR
    // ======================================================
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

        var userId = _userManager.GetUserId(User)!;

        _context.ProjetoMembros.Add(new ProjetoMembro
        {
            ProjetoId = projeto.Id,
            UsuarioId = userId,
            Papel = PapelProjeto.Gestor
        });

        _context.ProjetoTimeline.Add(new ProjetoTimeline
        {
            ProjetoId = projeto.Id,
            Evento = "Projeto criado",
            UsuarioId = userId
        });

        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Detalhes), new { id = projeto.Id });
    }

    // ======================================================
    // DETALHES — núcleo do sistema
    // ======================================================
    public async Task<IActionResult> Detalhes(int id)
    {
        var membro = await GetMembroProjeto(id);
        if (membro == null)
            return Forbid();

        var projeto = await _context.Projetos
            .Include(p => p.Tarefas)
            .Include(p => p.Membros).ThenInclude(m => m.Usuario)
            .Include(p => p.Comentarios).ThenInclude(c => c.Usuario)
            .Include(p => p.Timeline)
            .FirstAsync(p => p.Id == id);

        ViewBag.MeuPapel = membro.Papel;

        return View(projeto);
    }

    // ======================================================
    // COMENTAR
    // ======================================================
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Comentar(int projetoId, string texto)
    {
        var membro = await GetMembroProjeto(projetoId);
        if (membro == null)
            return Forbid();

        var userId = _userManager.GetUserId(User)!;

        _context.ComentariosProjeto.Add(new ComentarioProjeto
        {
            ProjetoId = projetoId,
            UsuarioId = userId,
            Texto = texto
        });

        _context.ProjetoTimeline.Add(new ProjetoTimeline
        {
            ProjetoId = projetoId,
            Evento = "Comentário adicionado",
            UsuarioId = userId
        });

        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Detalhes), new { id = projetoId });
    }

    // ======================================================
    // ADICIONAR MEMBRO (somente Gestor)
    // ======================================================
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AdicionarMembro(int projetoId, string email)
    {
        var gestor = await GetMembroProjeto(projetoId);
        if (gestor == null || !ProjetoPermissao.PodeEditar(gestor))
            return Forbid();

        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
            return NotFound("Usuário não encontrado.");

        var jaMembro = await _context.ProjetoMembros
            .AnyAsync(pm => pm.ProjetoId == projetoId && pm.UsuarioId == user.Id);

        if (!jaMembro)
        {
            _context.ProjetoMembros.Add(new ProjetoMembro
            {
                ProjetoId = projetoId,
                UsuarioId = user.Id,
                Papel = PapelProjeto.Membro
            });

            _context.ProjetoTimeline.Add(new ProjetoTimeline
            {
                ProjetoId = projetoId,
                Evento = $"Usuário {user.UserName} adicionado ao projeto",
                UsuarioId = gestor.UsuarioId
            });

            await _context.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Detalhes), new { id = projetoId });
    }

    // ======================================================
    // ALTERAR PAPEL (somente Gestor)
    // ======================================================
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AlterarPapel(int projetoId, string usuarioId, PapelProjeto papel)
    {
        var gestor = await GetMembroProjeto(projetoId);
        if (gestor == null || !ProjetoPermissao.PodeEditar(gestor))
            return Forbid();

        var membro = await _context.ProjetoMembros
            .FirstOrDefaultAsync(pm =>
                pm.ProjetoId == projetoId &&
                pm.UsuarioId == usuarioId);

        if (membro == null)
            return NotFound();

        membro.Papel = papel;

        _context.ProjetoTimeline.Add(new ProjetoTimeline
        {
            ProjetoId = projetoId,
            Evento = $"Papel do usuário alterado para {papel}",
            UsuarioId = gestor.UsuarioId
        });

        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Detalhes), new { id = projetoId });
    }

    // ======================================================
    // REMOVER MEMBRO (somente Gestor)
    // ======================================================
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RemoverMembro(int projetoId, string usuarioId)
    {
        var gestor = await GetMembroProjeto(projetoId);
        if (gestor == null || !ProjetoPermissao.PodeEditar(gestor))
            return Forbid();

        var membro = await _context.ProjetoMembros
            .FirstOrDefaultAsync(pm =>
                pm.ProjetoId == projetoId &&
                pm.UsuarioId == usuarioId);

        if (membro == null)
            return NotFound();

        _context.ProjetoMembros.Remove(membro);

        _context.ProjetoTimeline.Add(new ProjetoTimeline
        {
            ProjetoId = projetoId,
            Evento = "Membro removido do projeto",
            UsuarioId = gestor.UsuarioId
        });

        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Detalhes), new { id = projetoId });
    }
}
