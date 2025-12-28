public class AdminDashboardViewModel
{
    public int TotalUsuarios { get; set; }
    public int TotalNoticias { get; set; }
    public int NoticiasPublicadas { get; set; }
    public int NoticiasRascunho { get; set; }

    public List<Noticia> UltimasNoticias { get; set; } = new();
}
