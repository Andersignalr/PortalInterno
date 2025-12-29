using System.ComponentModel.DataAnnotations;

public class Tarefa
{
    public int Id { get; set; }

    public string Titulo { get; set; } = string.Empty;
    public string? Descricao { get; set; }

    public PrioridadeTarefa Prioridade { get; set; }

    public DateTime Prazo { get; set; }

    public bool Finalizada { get; set; }

    public DateTime CriadaEm { get; set; } = DateTime.UtcNow;

    public int? ProjetoId { get; set; }
    public Projeto? Projeto { get; set; }

}


public enum PrioridadeTarefa
{
    Baixa = 0,   // cinza
    Media = 1,   // azul
    Alta = 2     // vermelho
}
