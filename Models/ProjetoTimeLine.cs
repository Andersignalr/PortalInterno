public class ProjetoTimeline
{
    public int Id { get; set; }

    public int ProjetoId { get; set; }
    public Projeto Projeto { get; set; } = null!;

    public string Evento { get; set; } = string.Empty;
    public string UsuarioId { get; set; } = string.Empty;

    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;
}
