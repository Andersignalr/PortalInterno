public enum PapelProjeto
{
    Leitor,
    Membro,
    Gestor
}

public class ProjetoMembro
{
    public int ProjetoId { get; set; }
    public Projeto Projeto { get; set; } = null!;

    public string UsuarioId { get; set; } = string.Empty;
    public ApplicationUser Usuario { get; set; } = null!;

    public PapelProjeto Papel { get; set; }
}
