namespace mininal_api.Dominio.DTOs;

public class RefreshTokenDTO
{
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
}
