# Evidência do grupo 0 — coordenação multi-repositório

Inventário realizado em `2026-07-19T16:17:41.5398712-03:00`, sem troca de branch, commit, reset ou alteração nos quatro repositórios externos.

## Branch, commit e worktree iniciais

| Repositório | Branch | Commit inicial | Worktree inicial |
| --- | --- | --- | --- |
| `C:\Projetos\Identity\IdentityServer4` | `master` | `5bcc2abbcf6c47200c4eb3b77e756f1bc0e04358` | `?? .codex/`; `?? openspec/` |
| `C:\Projetos\Identity\IdentityModel` | `main` | `5f76150f8ee305b3a5d51ca1af692f8475a2d745` | modificados: `.github/workflows/ci.yml`, `.github/workflows/codeql.yml`, `build/build.csproj`, `global.json`, `src/IdentityModel.csproj`, `test/TrimmableAnalysis/TrimmableAnalysis.csproj`, `test/UnitTests/UnitTests.csproj`; não rastreados: `.codex/`, `openspec/` |
| `C:\Projetos\Identity\IdentityModel.AspNetCore.AccessTokenValidation` | `main` | `92176574a104e042940a2043913e49c853d1ba2c` | limpo |
| `C:\Projetos\Identity\IdentityModel.AspNetCore.OAuth2Introspection` | `main` | `90899c4d01418ef427bf3087b1942643da1d9ca7` | limpo |
| `C:\Projetos\Identity\IdentityModel.OidcClient` | `main` | `2b0c7e980bd78965f164614675a7239f878b7c80` | limpo |

Comandos de evidência: `git -C <raiz> branch --show-current`, `git -C <raiz> rev-parse HEAD` e `git -C <raiz> status --short --branch`.

## Autorização de escrita

Verificação repetida em `2026-07-20T08:50:50.5659756-03:00`. Um arquivo-probe foi criado por `apply_patch`, confirmado com `Test-Path` e removido por `apply_patch` em cada raiz. Nenhum probe permaneceu nos worktrees.

| Raiz | Situação verificada |
| --- | --- |
| `C:\Projetos\Identity\IdentityServer4` | escrita autorizada e probe confirmado |
| `C:\Projetos\Identity\IdentityModel` | escrita autorizada e probe confirmado |
| `C:\Projetos\Identity\IdentityModel.AspNetCore.AccessTokenValidation` | escrita autorizada e probe confirmado |
| `C:\Projetos\Identity\IdentityModel.AspNetCore.OAuth2Introspection` | escrita autorizada e probe confirmado |
| `C:\Projetos\Identity\IdentityModel.OidcClient` | escrita autorizada e probe confirmado |

A task 0.2 está concluída. Cada grupo posterior deve receber um novo subagent, uma única raiz de edição, tarefas exatas, pré-requisitos, arquivos relevantes, critérios de aceite e evidências obrigatórias, com proibição de avançar para outros grupos.

## Matriz registrada

| Repositório | Projeto que produz o pacote | PackageId | Versão-alvo determinística |
| --- | --- | --- | --- |
| IdentityModel | `src/IdentityModel.csproj` | `IdentityModel` | `10.0.0` |
| AccessTokenValidation | `src/IdentityModel.AspNetCore.AccessTokenValidation.csproj` | `IdentityModel.AspNetCore.AccessTokenValidation` (implícito pelo nome do projeto) | `10.0.0` |
| OAuth2Introspection | `src/IdentityModel.AspNetCore.OAuth2Introspection.csproj` | `IdentityModel.AspNetCore.OAuth2Introspection` | `10.0.0` |
| OidcClient | `src/OidcClient/OidcClient.csproj` | `IdentityModel.OidcClient` | `10.0.0` |
| OidcClient | `src/DPoP/DPoP.csproj` | `IdentityModel.OidcClient.DPoP` | `10.0.0` |
| OidcClient | `src/IdentityTokenValidator/IdentityTokenValidator.csproj` | `IdentityModel.OidcClient.IdentityTokenValidator` | `10.0.0` |
| IdentityServer4 | `src/IdentityServer4/src/IdentityServer4.csproj` | `IdentityServer4` | `10.0.0` |
| IdentityServer4 | `src/Storage/src/IdentityServer4.Storage.csproj` | `IdentityServer4.Storage` | `10.0.0` |
| IdentityServer4 | `src/EntityFramework.Storage/src/IdentityServer4.EntityFramework.Storage.csproj` | `IdentityServer4.EntityFramework.Storage` | `10.0.0` |
| IdentityServer4 | `src/EntityFramework/src/IdentityServer4.EntityFramework.csproj` | `IdentityServer4.EntityFramework` | `10.0.0` |
| IdentityServer4 | `src/AspNetIdentity/src/IdentityServer4.AspNetIdentity.csproj` | `IdentityServer4.AspNetIdentity` | `10.0.0` |

Os IDs foram conferidos diretamente nos projetos. Este grupo apenas registra a versão-alvo; a substituição de MinVer e a configuração efetiva de `PackageVersion=10.0.0` pertencem aos grupos de implementação e pack.

## Feed remoto e credencial de CI

O usuário definiu em 2026-07-20:

- nome da origem: `nuget`;
- site do provedor: `https://nuget.org`;
- endpoint V3 (service index): `https://api.nuget.org/v3/index.json`;
- variável protegida de CI/ambiente: `NUGET_KEY`;
- usuário associado: não aplicável para publicação por API key.

Somente o nome da variável foi registrado. O valor da credencial não foi persistido em arquivo, argumento literal ou log. Essa definição conclui a task 0.4.

No gate de publicação, a presença de `NUGET_KEY` foi verificada sem revelar o valor. A variável não estava disponível nos escopos Process, User ou Machine do Windows; portanto, nenhuma chamada de publicação foi iniciada.

## Staging local

O diretório `staging` contém:

- `NuGet.Config`, com fonte local, NuGet.org e mapeamento exato dos 11 IDs internos;
- `Initialize-Staging.ps1`, que cria o feed compartilhado e caches isolados sem publicar pacotes;
- `.gitignore`, que impede o versionamento dos nupkgs e caches gerados;
- `README.md`, com o fluxo de inicialização, restore e pack.

Por solicitação do usuário, o feed compartilhado foi criado em `C:\Projetos\Identity\nuget-packages`. O inicializador retornou esse caminho em `PackagesFeed`, e `NuGet.Config` o usa como source `local-staging`. Os caches globais, HTTP e de plugins continuam isolados sob `staging\artifacts`.

Essa preparação é independente do endpoint remoto e conclui a task 0.5. A validação integrada dos pacotes continua pertencendo aos grupos 1–5.

## Reorquestração após adoção do prefixo ZinfoFramework

Em `2026-07-20T18:41:56-03:00`, o usuário determinou que cada novo grupo de tasks seja executado por um subagent novo do modelo `gpt-5.6-sol`, com raciocínio `low`, recebendo antecipadamente todo o contexto necessário. A matriz histórica acima permanece como evidência dos artefatos anteriores, mas foi substituída para publicação pela matriz `ZinfoFramework.*` registrada em `design.md`.

Para manter uma única raiz de código por subagent, as tasks transversais 5.5 a 5.13 foram decompostas operacionalmente sem mudar seus critérios:

1. grupo 5A — `C:\Projetos\Identity\IdentityModel`;
2. grupo 5B — `C:\Projetos\Identity\IdentityModel.AspNetCore.OAuth2Introspection`, após o pacote-base 5A;
3. grupo 5C — `C:\Projetos\Identity\IdentityModel.OidcClient`, após o pacote-base 5A;
4. grupo 5D — `C:\Projetos\Identity\IdentityModel.AspNetCore.AccessTokenValidation`, após o pacote OAuth2Introspection 5B;
5. grupo 5E — integração dos seis nupkgs, auditoria, testes pós-pack, conformidade e gates 5.14–5.15;
6. grupos 6, 7, 8 e 9 — um subagent novo para cada seção, respeitando os gates anteriores.

Cada briefing deve conter: raiz autorizada; tasks e PackageIds exatos; URLs e metadados; atribuição upstream; pré-requisitos; comandos de build/test/pack; evidências obrigatórias; UTF-8 sem BOM; preservação do worktree; proibição de commit/push/publicação não autorizada; e proibição de ler ou registrar `NUGET_KEY`. Somente o orquestrador altera tasks/evidências coordenadoras e executa gates remotos.

O grupo 5A foi iniciado como `/root/grupo_5a_identitymodel_rebrand`. Os grupos dependentes permanecem bloqueados até a inspeção central do nupkg e das evidências retornadas.
