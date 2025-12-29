public class ProjetoResumoViewModel
{
    public int Id { get; set; }
    public string Titulo { get; set; } = string.Empty;

    public int TotalTarefas { get; set; }
    public int TarefasConcluidas { get; set; }

    public int Progresso =>
        TotalTarefas == 0
            ? 0
            : (int)((double)TarefasConcluidas / TotalTarefas * 100);

    public List<string> Membros { get; set; } = new();
}
