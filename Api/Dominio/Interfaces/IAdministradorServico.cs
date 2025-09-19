using mininal_api.Dominio.DTOs;
using mininal_api.Dominio.Entidades;

namespace mininal_api.Dominio.Interfaces;

public interface IAdministradorServico
{
    Administrador? Login(LoginDTO loginDTO);
    Administrador Incluir(Administrador administrador);
    Administrador? BuscaPorId(int id);
    List<Administrador> Todos(int? pagina);
    void Excluir(int id, int usuarioId);
    void Atualizar(Administrador administrador);
}