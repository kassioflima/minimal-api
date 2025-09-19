namespace mininal_api.Dominio.Servicos;

public class HashServico : IHashServico
{
    public string HashSenha(string senha)
    {
        if (string.IsNullOrEmpty(senha))
            throw new ArgumentNullException(nameof(senha));
            
        return BCrypt.Net.BCrypt.HashPassword(senha, BCrypt.Net.BCrypt.GenerateSalt(12));
    }

    public bool VerificarSenha(string senha, string hash)
    {
        if (string.IsNullOrEmpty(senha) || string.IsNullOrEmpty(hash))
            return false;
            
        try
        {
            return BCrypt.Net.BCrypt.Verify(senha, hash);
        }
        catch
        {
            return false;
        }
    }
}
