using System.ComponentModel.DataAnnotations;

public class Noticia
{
    public int Id { get; set; }

    [Required]
    public string Titulo { get; set; } = string.Empty;

    [Required]
    public string Conteudo { get; set; } = string.Empty;

    // definido no backend
    public string AutorId { get; set; } = string.Empty;

    public ApplicationUser? Autor { get; set; }

    public DateTime DataPublicacao { get; set; }
    public bool Publicada { get; set; }
}
