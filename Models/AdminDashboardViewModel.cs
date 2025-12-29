public class AdminDashboardViewModel
{
    public int TotalUsuarios { get; set; }
    public int TotalNoticias { get; set; }
    public int NoticiasPublicadas { get; set; }
    public int NoticiasRascunho { get; set; }
    public int NoticiasUltimos7Dias { get; set; }
    public int Noticias7DiasAnteriores { get; set; }

    public int DiferencaNoticias => NoticiasUltimos7Dias - Noticias7DiasAnteriores;



    public List<Noticia> UltimasNoticias { get; set; } = new();

    public List<Tarefa> TarefasRecentes { get; set; } = new();

    public List<ProjetoResumoViewModel> ProjetosEmAndamento { get; set; } = new();


}
