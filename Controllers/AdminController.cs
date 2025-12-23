using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public AdminController(
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    // Lista de usuários
    public IActionResult Usuarios()
    {
        var usuarios = _userManager.Users.ToList();
        return View(usuarios);
    }

    // Tela de edição de cargos
    public async Task<IActionResult> EditarCargos(string id)
    {
        if (id == null) return NotFound();

        var usuario = await _userManager.FindByIdAsync(id);
        if (usuario == null) return NotFound();

        var rolesUsuario = await _userManager.GetRolesAsync(usuario);
        var todasRoles = _roleManager.Roles.Select(r => r.Name).ToList();

        var model = new EditarCargosViewModel
        {
            UserId = usuario.Id,
            Email = usuario.Email!,
            Roles = todasRoles,
            RolesSelecionadas = rolesUsuario.ToList()
        };

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> EditarCargos(EditarCargosViewModel model)
    {
        var usuario = await _userManager.FindByIdAsync(model.UserId);
        if (usuario == null) return NotFound();

        var rolesAtuais = await _userManager.GetRolesAsync(usuario);

        await _userManager.RemoveFromRolesAsync(usuario, rolesAtuais);
        await _userManager.AddToRolesAsync(usuario, model.RolesSelecionadas);

        return RedirectToAction(nameof(Usuarios));
    }
}
