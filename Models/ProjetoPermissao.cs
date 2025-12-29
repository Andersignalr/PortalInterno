public static class ProjetoPermissao
{
    public static bool PodeEditar(ProjetoMembro membro) =>
        membro.Papel == PapelProjeto.Gestor;

    public static bool PodeCriarTarefa(ProjetoMembro membro) =>
        membro.Papel >= PapelProjeto.Membro;
}
