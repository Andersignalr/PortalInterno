public class Projeto
{
    public int Id { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;

    public List<Tarefa> Tarefas { get; set; } = new();

    public List<ProjetoMembro> Membros { get; set; } = new();

    // 👇 ADICIONAR
    public List<ComentarioProjeto> Comentarios { get; set; } = new();

    // 👇 ADICIONAR
    public List<ProjetoTimeline> Timeline { get; set; } = new();
}

