using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using mininal_api.Dominio.DTOs;
using mininal_api.Dominio.Entidades;
using mininal_api.Dominio.Enuns;
using mininal_api.Dominio.Interfaces;
using mininal_api.Dominio.ModelViews;
using mininal_api.Dominio.Servicos;
using mininal_api.Infraestrutura.Db;

namespace mininal_api.Extensions;

public static class EndpointsExtensions
{
    public static void MapMinimalApiEndpoints(this WebApplication app)
    {
        #region Home
        app.MapGet("/", () => Results.Json(new Home())).AllowAnonymous().WithTags("Home");
        #endregion

        #region Administradores
        app.MapPost("/administradores/login", async ([FromBody] LoginDTO loginDTO,
            IAdministradorServico administradorServico,
            IJwtServico jwtServico,
            DbContexto contexto,
            ILogger<Program> logger,
            HttpContext httpContext) =>
        {

            logger.LogInformation("Tentativa de login para o email: {Email}", loginDTO.Email);

            var adm = administradorServico.Login(loginDTO);
            if (adm != null)
            {
                string accessToken = jwtServico.GerarAccessToken(adm);
                string refreshToken = jwtServico.GerarRefreshToken();

                // Salvar refresh token no banco
                var refreshTokenEntity = new RefreshToken
                {
                    Token = refreshToken,
                    JwtId = Guid.NewGuid().ToString(),
                    DataCriacao = DateTime.UtcNow,
                    DataExpiracao = DateTime.UtcNow.AddDays(7),
                    AdministradorId = adm.Id
                };

                contexto.RefreshTokens.Add(refreshTokenEntity);
                await contexto.SaveChangesAsync();

                logger.LogInformation("Login bem-sucedido para o usuário: {Email}", adm.Email);

                return Results.Ok(new AdministradorLogado
                {
                    Email = adm.Email,
                    Perfil = adm.Perfil,
                    Token = accessToken,
                    RefreshToken = refreshToken
                });
            }
            else
            {
                logger.LogWarning("Tentativa de login falhou para o email: {Email}", loginDTO.Email);
                return Results.Unauthorized();
            }
        }).AllowAnonymous().WithTags("Administradores");

        app.MapPost("/administradores/refresh", ([FromBody] RefreshTokenDTO refreshTokenDTO,
            IJwtServico jwtServico,
            ILogger<Program> logger) =>
        {

            try
            {
                string novoAccessToken = jwtServico.RenovarAccessToken(refreshTokenDTO.RefreshToken);
                logger.LogInformation("Token renovado com sucesso");
                return Results.Ok(new { AccessToken = novoAccessToken });
            }
            catch (Exception ex)
            {
                logger.LogWarning("Falha ao renovar token: {Message}", ex.Message);
                return Results.BadRequest(new { Message = ex.Message });
            }
        }).AllowAnonymous().WithTags("Administradores");

        app.MapGet("/administradores", ([FromQuery] int? pagina, IAdministradorServico administradorServico) =>
        {
            var adms = new List<AdministradorModelView>();
            var administradores = administradorServico.Todos(pagina);
            foreach (var adm in administradores)
            {
                adms.Add(new AdministradorModelView
                {
                    Id = adm.Id,
                    Email = adm.Email,
                    Perfil = adm.Perfil
                });
            }
            return Results.Ok(adms);
        })
        .RequireAuthorization()
        .RequireAuthorization(new AuthorizeAttribute { Roles = "Adm" })
        .WithTags("Administradores");

        app.MapGet("/administradores/{id}", ([FromRoute] int id, IAdministradorServico administradorServico) =>
        {
            var administrador = administradorServico.BuscaPorId(id);
            if (administrador == null) return Results.NotFound();
            return Results.Ok(new AdministradorModelView
            {
                Id = administrador.Id,
                Email = administrador.Email,
                Perfil = administrador.Perfil
            });
        })
        .RequireAuthorization()
        .RequireAuthorization(new AuthorizeAttribute { Roles = "Adm" })
        .WithTags("Administradores");

        app.MapPost("/administradores", ([FromBody] AdministradorDTO administradorDTO, IAdministradorServico administradorServico) =>
        {
            var administrador = new Administrador
            {
                Email = administradorDTO.Email,
                Senha = administradorDTO.Senha,
                Perfil = administradorDTO.Perfil.ToString() ?? Perfil.Editor.ToString()
            };

            administradorServico.Incluir(administrador);

            return Results.Created($"/administradores/{administrador.Id}", new AdministradorModelView
            {
                Id = administrador.Id,
                Email = administrador.Email,
                Perfil = administrador.Perfil
            });
        })
        .RequireAuthorization()
        .RequireAuthorization(new AuthorizeAttribute { Roles = "Adm" })
        .WithTags("Administradores");

        app.MapPut("/administradores/{id}", ([FromRoute] int id, [FromBody] AdministradorDTO administradorDTO, IAdministradorServico administradorServico, IUsuarioContextoServico usuarioContextoServico) =>
        {
            var usuarioId = usuarioContextoServico.ObterUsuarioId();
            var administrador = new Administrador
            {
                Id = id,
                Email = administradorDTO.Email,
                Senha = administradorDTO.Senha,
                Perfil = administradorDTO.Perfil.ToString() ?? Perfil.Editor.ToString(),
                AtualizadoPor = usuarioId?.ToString() ?? "Sistema"
            };

            administradorServico.Atualizar(administrador);

            return Results.Ok(new AdministradorModelView
            {
                Id = administrador.Id,
                Email = administrador.Email,
                Perfil = administrador.Perfil
            });
        })
        .RequireAuthorization()
        .RequireAuthorization(new AuthorizeAttribute { Roles = "Adm" })
        .WithTags("Administradores");

        app.MapDelete("/administradores/{id}", ([FromRoute] int id, IAdministradorServico administradorServico, IUsuarioContextoServico usuarioContextoServico) =>
        {
            var usuarioId = usuarioContextoServico.ObterUsuarioId();
            if (usuarioId.HasValue)
            {
                administradorServico.Excluir(id, usuarioId.Value);
            }
            else
            {
                return Results.Unauthorized();
            }
            return Results.NoContent();
        })
        .RequireAuthorization()
        .RequireAuthorization(new AuthorizeAttribute { Roles = "Adm" })
        .WithTags("Administradores");

        app.MapGet("/administradores/me", (IUsuarioContextoServico usuarioContextoServico) =>
        {
            if (!usuarioContextoServico.EstaAutenticado())
            {
                return Results.Unauthorized();
            }

            var usuarioId = usuarioContextoServico.ObterUsuarioId();
            var email = usuarioContextoServico.ObterUsuarioEmail();
            var perfil = usuarioContextoServico.ObterUsuarioPerfil();

            return Results.Ok(new UsuarioLogadoModelView
            {
                Id = usuarioId ?? 0,
                Email = email ?? string.Empty,
                Perfil = perfil ?? string.Empty,
                EstaAutenticado = true
            });
        })
        .RequireAuthorization()
        .WithTags("Administradores");
        #endregion

        #region Veiculos
        app.MapPost("/veiculos", ([FromBody] VeiculoDTO veiculoDTO, IVeiculoServico veiculoServico, IUsuarioContextoServico usuarioContextoServico) =>
        {
            var usuarioId = usuarioContextoServico.ObterUsuarioId();

            var veiculo = new Veiculo
            {
                Nome = veiculoDTO.Nome,
                Marca = veiculoDTO.Marca,
                Ano = veiculoDTO.Ano,
                CriadoPor = usuarioId?.ToString() ?? "Sistema",
                AtualizadoPor = usuarioId?.ToString() ?? "Sistema",
            };
            veiculoServico.Incluir(veiculo);

            return Results.Created($"/veiculos/{veiculo.Id}", veiculo);
        })
        .RequireAuthorization()
        .RequireAuthorization(new AuthorizeAttribute { Roles = "Adm,Editor" })
        .WithTags("Veiculos");

        app.MapGet("/veiculos", ([FromQuery] int? pagina, [FromQuery] string? nome, [FromQuery] string? marca, IVeiculoServico veiculoServico) =>
        {
            var resultado = veiculoServico.Todos(pagina, nome, marca);
            return Results.Ok(resultado);
        }).RequireAuthorization().WithTags("Veiculos");

        app.MapGet("/veiculos/{id}", ([FromRoute] int id, IVeiculoServico veiculoServico) =>
        {
            var veiculo = veiculoServico.BuscaPorId(id);
            if (veiculo == null) return Results.NotFound();
            return Results.Ok(veiculo);
        })
        .RequireAuthorization()
        .RequireAuthorization(new AuthorizeAttribute { Roles = "Adm,Editor" })
        .WithTags("Veiculos");

        app.MapPut("/veiculos/{id}", ([FromRoute] int id, VeiculoDTO veiculoDTO, IVeiculoServico veiculoServico, IUsuarioContextoServico usuarioContextoServico) =>
        {
            var usuarioId = usuarioContextoServico.ObterUsuarioId();

            var veiculo = veiculoServico.BuscaPorId(id);
            if (veiculo == null) return Results.NotFound();

            veiculo.Nome = veiculoDTO.Nome;
            veiculo.Marca = veiculoDTO.Marca;
            veiculo.Ano = veiculoDTO.Ano;
            veiculo.CriadoPor = usuarioId?.ToString() ?? "Sistema";
            veiculo.AtualizadoPor = usuarioId?.ToString() ?? "Sistema";

            veiculoServico.Atualizar(veiculo);

            return Results.Ok(veiculo);
        })
        .RequireAuthorization()
        .RequireAuthorization(new AuthorizeAttribute { Roles = "Adm" })
        .WithTags("Veiculos");

        app.MapDelete("/veiculos/{id}", ([FromRoute] int id, IVeiculoServico veiculoServico) =>
        {
            var veiculo = veiculoServico.BuscaPorId(id);
            if (veiculo == null) return Results.NotFound();

            veiculoServico.Apagar(veiculo);

            return Results.NoContent();
        })
        .RequireAuthorization()
        .RequireAuthorization(new AuthorizeAttribute { Roles = "Adm" })
        .WithTags("Veiculos");
        #endregion
    }

    public static string GetClientIPAddress(HttpContext context)
    {
        // Obter IP real considerando proxies
        var forwardedHeader = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
        if (!string.IsNullOrEmpty(forwardedHeader))
        {
            return forwardedHeader.Split(',')[0].Trim();
        }

        var realIPHeader = context.Request.Headers["X-Real-IP"].FirstOrDefault();
        if (!string.IsNullOrEmpty(realIPHeader))
        {
            return realIPHeader;
        }

        return context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
    }
}
