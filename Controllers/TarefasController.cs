using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Authorize]
public class TarefasController : Controller
{
    private readonly ApplicationDbContext _context;

    public TarefasController(ApplicationDbContext context)
    {
        _context = context;
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
}
