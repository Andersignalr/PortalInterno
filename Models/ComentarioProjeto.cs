public class ComentarioProjeto
{
    public int Id { get; set; }

    public int ProjetoId { get; set; }
    public Projeto Projeto { get; set; } = null!;

    public string UsuarioId { get; set; } = string.Empty;
    public ApplicationUser Usuario { get; set; } = null!;

    public string Texto { get; set; } = string.Empty;

    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;
}
