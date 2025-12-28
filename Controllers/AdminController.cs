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
    public IActionResult Usuarios()
    {
        var usuarios = _userManager.Users.ToList();
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
        var vm = new AdminDashboardViewModel
        {
            TotalUsuarios = await _userManager.Users.CountAsync(),
            TotalNoticias = await _context.Noticias.CountAsync(),
            NoticiasPublicadas = await _context.Noticias.CountAsync(n => n.Publicada),
            NoticiasRascunho = await _context.Noticias.CountAsync(n => !n.Publicada),

            UltimasNoticias = await _context.Noticias
                .Include(n => n.Autor)
                .OrderByDescending(n => n.DataPublicacao)
                .Take(5)
                .ToListAsync()
        };

        return View(vm);
    }
}
