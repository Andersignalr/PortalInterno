public class Projeto
{
    public int Id { get; set; }

    public string Titulo { get; set; } = string.Empty;

    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;

    // Relacionamentos
    public List<Tarefa> Tarefas { get; set; } = new();
    public List<ApplicationUser> Membros { get; set; } = new();
}
