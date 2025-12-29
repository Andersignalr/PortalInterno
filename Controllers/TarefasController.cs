using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Authorize]
public class TarefasController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;


    public TarefasController(
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
        var tarefas = await _context.Tarefas
            .OrderBy(t => t.Finalizada)
            .ThenBy(t => t.Prazo)
            .ToListAsync();

        return View(tarefas);
    }

    // =========================
    // CRIAR
    // =========================
    public IActionResult Criar()
    {
        return View(new Tarefa
        {
            Prazo = DateTime.Today.AddDays(1)
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Criar(Tarefa tarefa)
    {
        if (!ModelState.IsValid)
            return View(tarefa);

        tarefa.Prazo = DateTime.SpecifyKind(tarefa.Prazo, DateTimeKind.Utc);
        tarefa.CriadaEm = DateTime.UtcNow;

        _context.Tarefas.Add(tarefa);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }


    // =========================
    // EDITAR
    // =========================
    public async Task<IActionResult> Editar(int id)
    {
        var tarefa = await _context.Tarefas.FindAsync(id);
        if (tarefa == null)
            return NotFound();

        return View(tarefa);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Editar(int id, Tarefa model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var tarefa = await _context.Tarefas.FindAsync(id);
        if (tarefa == null)
            return NotFound();

        tarefa.Titulo = model.Titulo;
        tarefa.Descricao = model.Descricao;
        tarefa.Prioridade = model.Prioridade;
        tarefa.Prazo = DateTime.SpecifyKind(model.Prazo, DateTimeKind.Utc);


        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    // =========================
    // FINALIZAR
    // =========================
    [HttpPost]
    public async Task<IActionResult> Finalizar(int id)
    {
        var tarefa = await _context.Tarefas.FindAsync(id);
        if (tarefa == null)
            return NotFound();

        tarefa.Finalizada = true;
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    // =========================
    // EXCLUIR
    // =========================
    public async Task<IActionResult> Excluir(int id)
    {
        var tarefa = await _context.Tarefas.FindAsync(id);
        if (tarefa == null)
            return NotFound();

        return View(tarefa);
    }

    [HttpPost, ActionName("Excluir")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ExcluirConfirmado(int id)
    {
        var tarefa = await _context.Tarefas.FindAsync(id);
        if (tarefa == null)
            return NotFound();

        _context.Tarefas.Remove(tarefa);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }
    public async Task<IActionResult> CriarNoProjeto(int projetoId)
    {
        var userId = _userManager.GetUserId(User);

        var membro = await _context.ProjetoMembros
            .FirstOrDefaultAsync(m =>
                m.ProjetoId == projetoId &&
                m.UsuarioId == userId);

        if (membro == null || !ProjetoPermissao.PodeCriarTarefa(membro))
            return Forbid();

        return View(new Tarefa
        {
            ProjetoId = projetoId,
            Prazo = DateTime.UtcNow.AddDays(1)
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CriarNoProjeto(Tarefa tarefa)
    {
        var userId = _userManager.GetUserId(User);

        var membro = await _context.ProjetoMembros
            .FirstOrDefaultAsync(m =>
                m.ProjetoId == tarefa.ProjetoId &&
                m.UsuarioId == userId);

        if (membro == null || !ProjetoPermissao.PodeCriarTarefa(membro))
            return Forbid();

        if (!ModelState.IsValid)
            return View(tarefa);

        tarefa.Prazo = DateTime.SpecifyKind(tarefa.Prazo, DateTimeKind.Utc);
        tarefa.CriadaEm = DateTime.UtcNow;

        _context.Tarefas.Add(tarefa);

        if (tarefa.ProjetoId == null)
            return BadRequest("Tarefa não pertence a um projeto.");

        _context.ProjetoTimeline.Add(new ProjetoTimeline
        {
            ProjetoId = tarefa.ProjetoId.Value, // 👈 aqui
            UsuarioId = userId!,
            Evento = $"Criou a tarefa: {tarefa.Titulo}"
        });


        await _context.SaveChangesAsync();

        return RedirectToAction(
            "Detalhes",
            "Projetos",
            new { id = tarefa.ProjetoId });
    }



}
