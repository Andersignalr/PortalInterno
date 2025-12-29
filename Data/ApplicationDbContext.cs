using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

public class ApplicationDbContext
    : IdentityDbContext<ApplicationUser, IdentityRole, string>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }

    public DbSet<Noticia> Noticias { get; set; }
    public DbSet<Tarefa> Tarefas { get; set; }
    public DbSet<Projeto> Projetos { get; set; }
    public DbSet<ProjetoMembro> ProjetoMembros { get; set; }
    public DbSet<ComentarioProjeto> ComentariosProjeto { get; set; }
    public DbSet<ProjetoTimeline> ProjetoTimeline { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<ProjetoMembro>()
            .HasKey(pm => new { pm.ProjetoId, pm.UsuarioId });
    }

}

