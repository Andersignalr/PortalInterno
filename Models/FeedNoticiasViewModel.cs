public class FeedNoticiasViewModel
{
    public IEnumerable<Noticia> Noticias { get; set; } = new List<Noticia>();

    public int PaginaAtual { get; set; }
    public int TotalPaginas { get; set; }
}
