using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly ApplicationDbContext _context;

    public AdminController(
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager,
        ApplicationDbContext context)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _context = context;
    }

    // ✅ GET — lista de usuários
    public async Task<IActionResult> Usuarios()
    {
        var usuarios = _userManager.Users.ToList();

        var userRoles = new Dictionary<string, IList<string>>();

        foreach (var user in usuarios)
        {
            userRoles[user.Id] = await _userManager.GetRolesAsync(user);
        }

        ViewBag.UserRoles = userRoles;

        return View(usuarios);
    }


    // ✅ GET — tela de edição de cargos
    public async Task<IActionResult> EditarCargos(string id)
    {
        if (string.IsNullOrEmpty(id))
            return NotFound();

        var usuario = await _userManager.FindByIdAsync(id);
        if (usuario == null)
            return NotFound();

        var rolesUsuario = await _userManager.GetRolesAsync(usuario);
        var todasRoles = await _roleManager.Roles
            .Select(r => r.Name!)
            .ToListAsync();

        var model = new EditarCargosViewModel
        {
            UserId = usuario.Id,
            Email = usuario.Email!,
            Roles = todasRoles,
            RolesSelecionadas = rolesUsuario.ToList()
        };

        return View(model);
    }

    // ✅ POST — salvar cargos
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditarCargos(EditarCargosViewModel model)
    {
        var usuario = await _userManager.FindByIdAsync(model.UserId);
        if (usuario == null)
            return NotFound();

        var rolesAtuais = await _userManager.GetRolesAsync(usuario);

        await _userManager.RemoveFromRolesAsync(usuario, rolesAtuais);
        await _userManager.AddToRolesAsync(usuario, model.RolesSelecionadas);

        return RedirectToAction(nameof(Usuarios));
    }

    // ✅ GET — dashboard
    public async Task<IActionResult> Dashboard()
    {
        var hoje = DateTime.UtcNow;
        var ultimos7 = hoje.AddDays(-7);
        var anteriores7 = hoje.AddDays(-14);

        // ===============================
        // BUSCA PROJETOS (SEM FILTRO)
        // ===============================
        var projetos = await _context.Projetos
            .Include(p => p.Tarefas)
            .Include(p => p.Membros)
            .Select(p => new ProjetoResumoViewModel
            {
                Id = p.Id,
                Titulo = p.Titulo,
                TotalTarefas = p.Tarefas.Count,
                TarefasConcluidas = p.Tarefas.Count(t => t.Finalizada),
                Membros = p.Membros.Select(m => m.UserName!).ToList()
            })
            .ToListAsync(); // 👈 materializa aqui

        // ===============================
        // VIEWMODEL
        // ===============================
        var vm = new AdminDashboardViewModel
        {
            TotalUsuarios = await _userManager.Users.CountAsync(),
            TotalNoticias = await _context.Noticias.CountAsync(),
            NoticiasPublicadas = await _context.Noticias.CountAsync(n => n.Publicada),
            NoticiasRascunho = await _context.Noticias.CountAsync(n => !n.Publicada),

            NoticiasUltimos7Dias = await _context.Noticias
                .CountAsync(n => n.DataPublicacao >= ultimos7),

            Noticias7DiasAnteriores = await _context.Noticias
                .CountAsync(n =>
                    n.DataPublicacao >= anteriores7 &&
                    n.DataPublicacao < ultimos7),

            UltimasNoticias = await _context.Noticias
                .Include(n => n.Autor)
                .OrderByDescending(n => n.DataPublicacao)
                .Take(5)
                .ToListAsync(),

            TarefasRecentes = await _context.Tarefas
                .OrderBy(t => t.Finalizada)
                .ThenBy(t => t.Prazo)
                .Take(5)
                .ToListAsync(),

            // 👇 FILTRO AGORA É EM MEMÓRIA
            ProjetosEmAndamento = projetos
                .Where(p => p.Progresso < 100)
                //.Take(6)
                .ToList()
        };

        return View(vm);
    }




    // =========================
    // RESETAR SENHA (ADMIN)
    // =========================
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> ResetarSenha(string id)
    {
        if (string.IsNullOrEmpty(id))
            return NotFound();

        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
            return NotFound();

        return View(user);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ResetarSenha(string id, string novaSenha)
    {
        if (string.IsNullOrWhiteSpace(novaSenha))
        {
            ModelState.AddModelError("", "Informe a nova senha");
            return View();
        }

        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
            return NotFound();

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        var result = await _userManager.ResetPasswordAsync(user, token, novaSenha);

        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
                ModelState.AddModelError("", error.Description);

            return View(user);
        }

        return RedirectToAction(nameof(Usuarios));
    }

}
