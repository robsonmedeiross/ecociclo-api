# Projeto — Cidades ESGInteligentes

## Visão geral

O **Cidades ESGInteligentes** utiliza a API **EcoCiclo**, desenvolvida em ASP.NET Core, para trabalhar com funcionalidades relacionadas a pontos e agendamentos de coleta. Este repositório demonstra containerização com Docker, orquestração local com Docker Compose e integração/entrega contínuas com GitHub Actions e Docker Hub.

**Escopo atual:** não há hospedagem em nuvem. Os ambientes **staging** e **produção** são identificados por branches e tags de imagens Docker; a publicação no Docker Hub não significa que a aplicação esteja executando em um servidor.

### Organização do projeto

```text
EcoCicloSolution/
├── .github/workflows/ci-cd.yml  # Pipeline GitHub Actions
├── EcoCiclo.API/                 # API ASP.NET Core e Dockerfile
│   └── Dockerfile
├── EcoCiclo.Domain/              # Domínio e contratos
├── EcoCiclo.Infrastructure/      # Persistência e acesso a dados
├── EcoCiclo.Tests/              # Testes automatizados
├── EcoCicloSolution.slnx        # Solução .NET
├── docker-compose.yml           # API + SQL Server (execução local)
├── .dockerignore
└── README.md
```

> A estrutura acima representa a organização esperada após adicionar os arquivos Docker e CI/CD. Confirme os caminhos no repositório antes da entrega.

## Como executar localmente com Docker

### Pré-requisitos

- Docker Desktop com Docker Compose habilitado;
- Git;
- Opcional: .NET SDK 8 para executar testes e migrations diretamente.

### 1. Clonar o repositório

```bash
git clone https://github.com/SEU_USUARIO/ecociclo-api.git
cd ecociclo-api
```

Substitua `SEU_USUARIO` pelo usuário real do GitHub.

### 2. Configurar as variáveis locais

Crie um arquivo `.env` na raiz do repositório:

```dotenv
SQL_PASSWORD=SUBSTITUA_POR_UMA_SENHA_FORTE
JWT_KEY=SUBSTITUA_POR_UMA_CHAVE_ALEATORIA_DE_32_CARACTERES_OU_MAIS
```

O `.env` deve estar no `.gitignore` e **não deve ser enviado ao GitHub**. O exemplo pressupõe que a API leia `Jwt:Key` da configuração e que o Compose utilize essas variáveis.

### 3. Iniciar API e SQL Server

Na pasta que contém `docker-compose.yml`, execute:

```bash
docker compose up --build -d
```

Verifique o estado dos containers e os logs:

```bash
docker compose ps
docker compose logs -f api
```

Com o mapeamento `8080:8080` e o ambiente `Development`, acesse:

- API: `http://localhost:8080`
- Swagger: `http://localhost:8080/swagger`
- Health check: `http://localhost:8080/health` **somente se o endpoint tiver sido implementado**.

A rota `/` não precisa existir; um HTTP 404 nessa rota não significa necessariamente falha da API.

**Banco de dados:** se o banco estiver vazio, aplique as migrations do Entity Framework Core antes de testar endpoints que dependem de tabelas. Execute-as usando uma conexão local ao SQL Server publicado pelo Compose e confira os nomes reais dos projetos e da connection string.

Para parar os containers:

```bash
docker compose down
```

Para remover também o volume do banco (**apaga os dados locais**):

```bash
docker compose down -v
```

### 4. Testar a construção da imagem separadamente

```bash
docker build -f EcoCiclo.API/Dockerfile -t ecociclo-api:local .
docker images
```

O comando deve ser executado na raiz da solução, pois o Dockerfile copia arquivos de vários projetos.

## Pipeline CI/CD

O pipeline está definido em `.github/workflows/ci-cd.yml` e é executado pelo **GitHub Actions**. O **Docker Hub** recebe as imagens geradas automaticamente após as validações.

### Estratégia de branches e ambientes

| Evento | Branch de destino | Testes e build | Entrega |
|---|---|---|---|
| Push | `develop` | Sim | Publica `ecociclo-api:staging` |
| Push | `main` | Sim | Publica `ecociclo-api:latest` |
| Pull Request | `develop` | Sim | Sem publicação |
| Pull Request | `main` | Sim | Sem publicação |

- **Staging:** branch `develop`, imagem `SEU_USUARIO_DOCKER/ecociclo-api:staging`.
- **Produção:** branch `main`, imagem `SEU_USUARIO_DOCKER/ecociclo-api:latest`.
- Também são publicadas tags rastreáveis por commit: `staging-<SHA>` e `prod-<SHA>`.

### Etapas automatizadas

1. **Checkout:** baixa o código do repositório no runner do GitHub.
2. **Configuração do .NET 8:** instala o SDK necessário.
3. **Restore:** restaura as dependências NuGet.
4. **Build:** compila a solução em modo Release.
5. **Testes:** executa os testes existentes em `EcoCiclo.Tests`.
6. **Build Docker:** verifica se o Dockerfile gera uma imagem válida, sem publicá-la.
7. **Entrega staging ou produção:** em pushes, autentica no Docker Hub, reconstrói a imagem e a publica com as tags do ambiente correspondente.

**Regra de segurança:** os jobs são encadeados por `needs`. Se testes ou build falharem, a publicação é bloqueada. Pull Requests executam validações, mas não publicam imagens.

### Secrets necessários

Configure em **GitHub → Settings → Secrets and variables → Actions**:

| Secret | Finalidade |
|---|---|
| `DOCKERHUB_USERNAME` | Usuário do Docker Hub |
| `DOCKERHUB_TOKEN` | Token Docker Hub com permissão de leitura e escrita |

Nunca coloque o token diretamente no YAML ou no README.

### Como acompanhar

1. Abra a aba **Actions** do repositório.
2. Selecione **EcoCiclo - CI/CD**.
3. Abra a execução do push ou Pull Request.
4. Confira os jobs **Build e Testes**, **Build Docker** e **Deploy Staging** ou **Deploy Production**.
5. Acesse o repositório no Docker Hub e confira as tags publicadas.

### Fluxo de promoção

```text
Pull Request para develop
          ↓
     Testes + Build
          ↓
    Merge em develop
          ↓
  Publicação :staging
          ↓
 Pull Request develop → main
          ↓
     Testes + Build
          ↓
      Merge em main
          ↓
  Publicação :latest
```

> **Limitação do escopo:** os jobs chamados `Deploy Staging` e `Deploy Production` fazem **push de imagens ao Docker Hub**. Não há deploy de containers em infraestrutura online. Para executar cada imagem, é necessário usar Docker localmente ou configurar posteriormente um servidor.

## Containerização

### Dockerfile

Arquivo: `EcoCiclo.API/Dockerfile`.

```dockerfile
# Estágio de compilação
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["EcoCiclo.API/EcoCiclo.API.csproj", "EcoCiclo.API/"]
COPY ["EcoCiclo.Domain/EcoCiclo.Domain.csproj", "EcoCiclo.Domain/"]
COPY ["EcoCiclo.Infrastructure/EcoCiclo.Infrastructure.csproj", "EcoCiclo.Infrastructure/"]
RUN dotnet restore "EcoCiclo.API/EcoCiclo.API.csproj"

COPY . .
RUN dotnet publish "EcoCiclo.API/EcoCiclo.API.csproj" -c Release -o /app/publish --no-restore

# Estágio de execução
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
ENTRYPOINT ["dotnet", "EcoCiclo.API.dll"]
```

> Este é o Dockerfile padronizado proposto para o pipeline. Se o Dockerfile efetivamente versionado no GitHub estiver diferente, atualize este trecho para reproduzir o arquivo real.

### Estratégias adotadas

- **Multi-stage build:** SDK para compilar; runtime ASP.NET Core para executar, reduzindo o tamanho da imagem final.
- **Cache de dependências:** cópia dos arquivos `.csproj` antes do restante do código, permitindo reutilizar a camada do `restore` quando possível.
- **Porta padronizada:** API exposta internamente na porta `8080`.
- **`.dockerignore`:** exclui `bin`, `obj`, `.git`, `.env` e outros arquivos desnecessários do contexto de build.
- **Configuração externa:** senhas, connection strings e chave JWT devem ser fornecidas por variáveis de ambiente, sem serem incluídas na imagem.
- **Tags por ambiente e commit:** permitem distinguir staging e produção e rastrear versões publicadas.

### Docker Compose

O arquivo `docker-compose.yml` orquestra **API + SQL Server** para execução local. A API se conecta ao banco usando o nome do serviço `sqlserver` na rede Docker; o banco mantém seus dados em um volume persistente. O Compose não é usado para hospedar a aplicação na nuvem.

## Prints do funcionamento

Crie uma pasta `docs/prints/` e salve nela as capturas abaixo. Substitua os exemplos pelos arquivos reais antes da entrega.

### 1. Containers locais em execução

![Docker Compose com API e SQL Server em execução](docs/prints/01-docker-compose.png)

**Evidência:** resultado de `docker compose ps`, mostrando os containers ativos.

### 2. API funcionando localmente

![Swagger ou endpoint da API funcionando](docs/prints/02-api-local.png)

**Evidência:** Swagger em `http://localhost:8080/swagger` ou resposta de endpoint existente.

### 3. Pipeline de staging

![Pipeline de staging concluído no GitHub Actions](docs/prints/03-pipeline-staging.png)

**Evidência:** execução após push na `develop`, com testes, build e publicação concluídos.

### 4. Pipeline de produção

![Pipeline de produção concluído no GitHub Actions](docs/prints/04-pipeline-producao.png)

**Evidência:** execução após push na `main`, com testes, build e publicação concluídos.

### 5. Imagens publicadas no Docker Hub

![Tags staging e latest no Docker Hub](docs/prints/05-docker-hub.png)

**Evidência:** repositório Docker Hub exibindo `staging`, `latest` e tags de commits.

### 6. Pull Request validado

![Pull Request com verificações de CI aprovadas](docs/prints/06-pull-request.png)

**Evidência:** checks de testes e build Docker aprovados antes do merge.

**Links para preencher:**

- Repositório GitHub: `https://github.com/SEU_USUARIO/ecociclo-api`
- Histórico de pipelines: `https://github.com/SEU_USUARIO/ecociclo-api/actions`
- Repositório Docker Hub: `https://hub.docker.com/r/SEU_USUARIO_DOCKER/ecociclo-api`

> Os caminhos de imagens acima são **espaços reservados**. Adicione os prints reais à pasta `docs/prints/`; não apresente essas evidências como existentes antes de adicioná-las.

## Tecnologias utilizadas

| Tecnologia | Utilização |
|---|---|
| C# | Linguagem da aplicação |
| .NET 8 / ASP.NET Core Web API | API HTTP |
| Entity Framework Core | Persistência e migrations |
| SQL Server | Banco de dados relacional |
| JWT Bearer | Autenticação baseada em token |
| Swagger / OpenAPI | Documentação e testes dos endpoints |
| Docker | Construção e execução de imagens |
| Docker Compose | Orquestração local de API e banco |
| Docker Hub | Registro e distribuição das imagens |
| Git / GitHub | Versionamento e Pull Requests |
| GitHub Actions | Pipeline CI/CD automatizado |

## Checklist de entrega (obrigatório)

Marque cada item após verificar a evidência correspondente.

- [x] Projeto compactado em `.ZIP` com estrutura organizada, sem `bin/`, `obj/`, `.vs/`, `.env` ou credenciais.
- [x] Dockerfile funcional, com build validado.
- [x] `docker-compose.yml` funcional, com API e SQL Server.
- [x] Pipeline com etapas de build, testes e entrega automatizada de imagens.
- [x] README.md com instruções e prints reais.
- [x] Documentação técnica com evidências em PDF ou PPT.
- [x] Publicação de imagens de staging (`develop`) e produção (`main`) no Docker Hub.
- [ ] deploy real em dois ambientes executando a aplicação (não implementado neste escopo sem servidor).

## Observações finais

A automação demonstra integração contínua, validação automatizada, construção de containers e entrega contínua de artefatos Docker para duas branches.
