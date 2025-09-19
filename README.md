# Minimal API - Sistema de Gestão de Veículos

## 🚀 Funcionalidades Implementadas

### ✅ **Funcionalidades Principais**
- **Autenticação JWT**: Sistema completo com access tokens e refresh tokens
- **Obtenção do Usuário Logado**: Endpoint `/administradores/me` para informações do usuário autenticado
- **Criptografia de Senha**: Hash BCrypt para segurança
- **Validação de Dados**: FluentValidation para todos os DTOs
- **Tratamento de Exceções**: Middleware global de tratamento
- **Logging Estruturado**: Serilog com arquivos e console
- **Auditoria Automática**: Campos automáticos de criação/atualização com usuário logado
- **Soft Delete**: Exclusão lógica com histórico
- **Health Checks**: Monitoramento de saúde da aplicação
- **Paginação Avançada**: Com metadados e navegação
- **Filtros de Busca**: Busca por nome e marca de veículos
- **Refresh Token**: Sistema completo de renovação de tokens
- **Docker**: Containerização da aplicação

### ✅ **Funcionalidades Removidas**
- **Rate Limiting**: Removido conforme solicitado
- **Cache Redis**: Removido da configuração atual

## 🏗️ Arquitetura

```
Api/
├── Dominio/
│   ├── Entidades/          # Modelos de dados (BaseEntity, Administrador, Veiculo, RefreshToken)
│   ├── DTOs/              # Objetos de transferência (LoginDTO, AdministradorDTO, etc.)
│   ├── Interfaces/        # Contratos dos serviços
│   ├── ModelViews/        # Modelos de visualização (AdministradorLogado, UsuarioLogadoModelView)
│   ├── Servicos/          # Lógica de negócio (JWT, Hash, UsuarioContexto, etc.)
│   └── Validadores/       # Validações FluentValidation
├── Extensions/
│   └── EndpointsExtensions.cs  # Mapeamento de endpoints organizados
├── Infraestrutura/
│   ├── Db/                # Contexto do Entity Framework
│   └── Migrations/        # Migrações do banco de dados
├── Middleware/             # Middlewares customizados
└── Program.cs             # Configuração moderna da aplicação (.NET 9)
```

## 🛠️ Tecnologias Utilizadas

- **.NET 9.0** - Framework principal
- **Entity Framework Core 9.0** - ORM para banco de dados
- **MySQL** - Banco de dados principal
- **JWT Bearer** - Autenticação e autorização
- **BCrypt** - Hash de senhas
- **FluentValidation** - Validação de dados
- **Serilog** - Logging estruturado
- **Swagger/OpenAPI** - Documentação da API
- **Docker** - Containerização

## 📋 Pré-requisitos

- .NET 9.0 SDK
- MySQL 8.0+
- Docker (opcional)
- Visual Studio 2022 ou VS Code

## 🚀 Como Executar

### 1. Clone o repositório
```bash
git clone <url-do-repositorio>
cd minimal-api
```

### 2. Configure o banco de dados
```bash
# Execute as migrações
cd Api
dotnet ef database update
```

### 3. Configure as variáveis de ambiente
Edite o arquivo `Api/appsettings.json` com suas configurações de banco.

### 4. Execute a aplicação
```bash
cd Api
dotnet run
```

### 5. Acesse a documentação
- **Swagger UI**: http://localhost:5000/swagger
- **Health Check**: http://localhost:5000/health
- **API Home**: http://localhost:5000/

## 🔐 Autenticação

### Login
```http
POST /administradores/login
Content-Type: application/json

{
  "email": "administrador@teste.com",
  "senha": "123456"
}
```

**Resposta:**
```json
{
  "email": "administrador@teste.com",
  "perfil": "Adm",
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "refreshToken": "base64_refresh_token..."
}
```

### Refresh Token
```http
POST /administradores/refresh
Content-Type: application/json

{
  "refreshToken": "base64_refresh_token_aqui"
}
```

### Obter Usuário Logado
```http
GET /administradores/me
Authorization: Bearer <access_token>
```

**Resposta:**
```json
{
  "id": 1,
  "email": "administrador@teste.com",
  "perfil": "Adm",
  "estaAutenticado": true
}
```

## 📊 Endpoints Principais

### Administradores
- `POST /administradores/login` - Login (público)
- `POST /administradores/refresh` - Renovar token (público)
- `GET /administradores/me` - Usuário logado (autenticado)
- `GET /administradores` - Lista todos (requer role Adm)
- `GET /administradores/{id}` - Busca por ID (requer role Adm)
- `POST /administradores` - Cria novo (requer role Adm)
- `PUT /administradores/{id}` - Atualiza (requer role Adm)
- `DELETE /administradores/{id}` - Remove (requer role Adm)

### Veículos
- `GET /veiculos` - Lista com paginação e filtros (autenticado)
- `GET /veiculos/{id}` - Busca por ID (requer role Adm/Editor)
- `POST /veiculos` - Cria novo (requer role Adm/Editor)
- `PUT /veiculos/{id}` - Atualiza (requer role Adm)
- `DELETE /veiculos/{id}` - Remove (requer role Adm)

### Sistema
- `GET /` - Home da API (público)
- `GET /health` - Health check (público)

## 🔍 Filtros e Paginação

### Busca de Veículos
```http
GET /veiculos?pagina=1&nome=civic&marca=honda
```

### Resposta Paginada
```json
{
  "itens": [
    {
      "id": 1,
      "nome": "Civic",
      "marca": "Honda",
      "ano": 2023,
      "dataCriacao": "2024-01-01T00:00:00Z",
      "criadoPor": "administrador@teste.com"
    }
  ],
  "paginaAtual": 1,
  "itensPorPagina": 10,
  "totalItens": 25,
  "totalPaginas": 3
}
```

## 🧪 Testes

### Executar Testes
```bash
cd Test
dotnet test
```

### Executar Testes com Cobertura
```bash
dotnet test --collect:"XPlat Code Coverage"
```

## 📝 Logs

Os logs são salvos em:
- **Console**: Durante desenvolvimento
- **Arquivos**: `Api/logs/log-YYYY-MM-DD.txt` (rotacionados diariamente)

### Níveis de Log
- **Information**: Operações normais
- **Warning**: Situações que merecem atenção
- **Error**: Erros que não impedem a execução

## 🔧 Configurações

### JWT
- **Access Token**: Expira em 50 minutos
- **Refresh Token**: Expira em 7 dias
- **Algoritmo**: HMAC SHA256

### Banco de Dados
- **MySQL 8.0+**: Banco principal
- **Migrações**: Automáticas via Entity Framework
- **Auditoria**: Campos automáticos em todas as entidades

## 🔒 Segurança

- **Senhas**: Hash BCrypt com salt 12
- **JWT**: Tokens seguros com validação robusta
- **CORS**: Configurado para desenvolvimento
- **Validação**: FluentValidation em todos os endpoints
- **Auditoria**: Rastreamento de quem criou/modificou registros

## 📈 Monitoramento

- **Health Checks**: `/health` endpoint
- **Logs Estruturados**: Serilog com contexto
- **Auditoria**: Rastreamento automático de mudanças
- **Exception Handling**: Middleware global de tratamento

## 🐳 Docker

### Executar com Docker
```bash
# Build da imagem
docker build -t minimal-api .

# Executar container
docker run -p 5000:80 minimal-api
```

### Docker Compose
```bash
docker-compose up -d
```

## 🔄 Atualizações Recentes

### Migração para .NET 9.0
- ✅ Projeto atualizado para .NET 9.0
- ✅ Pacotes NuGet atualizados para versões compatíveis
- ✅ Estrutura moderna com Program.cs (sem Startup.cs)
- ✅ Endpoints organizados em Extensions

### Funcionalidades Implementadas
- ✅ **Obtenção do Usuário Logado**: Sistema completo implementado
- ✅ **Auditoria Automática**: Campos de criação/atualização com usuário logado
- ✅ **JWT Robusto**: Validação melhorada e logs detalhados
- ✅ **Estrutura Moderna**: Migração do Startup.cs para Program.cs

### Funcionalidades Removidas
- ❌ **Rate Limiting**: Removido conforme solicitado
- ❌ **Cache Redis**: Removido da configuração atual

## 🎯 Próximos Passos

- [ ] Implementar testes de integração
- [ ] Adicionar documentação OpenAPI mais detalhada
- [ ] Implementar cache Redis (se necessário)
- [ ] Adicionar métricas de performance
- [ ] Implementar rate limiting (se necessário)

## 🤝 Contribuição

1. Fork o projeto
2. Crie uma branch para sua feature (`git checkout -b feature/nova-feature`)
3. Commit suas mudanças (`git commit -am 'Adiciona nova feature'`)
4. Push para a branch (`git push origin feature/nova-feature`)
5. Abra um Pull Request

## 📄 Licença

Este projeto está sob a licença MIT. Veja o arquivo `LICENSE` para mais detalhes.

## 🆘 Suporte

Para dúvidas ou problemas:
- Abra uma issue no GitHub
- Consulte a documentação Swagger em `/swagger`
- Verifique os logs da aplicação em `Api/logs/`
- Execute o health check em `/health`

## 📞 Contato

- **Desenvolvedor**: [Seu Nome]
- **Email**: [seu.email@exemplo.com]
- **GitHub**: [@seu-usuario]

---

**Desenvolvido com ❤️ usando .NET 9.0 e Minimal APIs**

*Última atualização: Janeiro 2025*