namespace mininal_api.Dominio.Servicos;

public interface IHashServico
{
    string HashSenha(string senha);
    bool VerificarSenha(string senha, string hash);
}
