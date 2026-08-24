# Evidência do grupo 4 — AccessTokenValidation

Grupo executado por subagent exclusivo em `C:\Projetos\Identity\IdentityModel.AspNetCore.AccessTokenValidation` e validado centralmente pelo orquestrador em `2026-07-20T10:56:28.4990167-03:00`.

## Escopo e dependências

Projetos ativos: biblioteca e testes na solution, além do build auxiliar usado pelos scripts oficiais. Todos usam somente `net10.0`; o SDK está fixado em `10.0.301`, com roll-forward desabilitado.

Os testes referenciam `IdentityModel.AspNetCore.OAuth2Introspection [10.0.0]`, resolvido do staging, e recebem `IdentityModel 10.0.0` apenas transitivamente. A biblioteca pública não contém dependência direta de IdentityModel nem OAuth2Introspection; seu `.nuspec` tem dependency group `net10.0` vazio e framework reference `Microsoft.AspNetCore.App`.

O entrypoint oficial aceita `ACCESSTOKENVALIDATION_NUGET_CONFIG`: scripts restauram o projeto de build com essa config e executam o runner com `--no-restore`; o runner restaura a solution com a mesma config e usa `--no-restore` em build, test e pack. O target Pack fixa `MinVerVersionOverride=10.0.0`, sem depender de tag ou variável de versão externa.

## Restore, build e testes

| Evidência | Resultado |
| --- | --- |
| `dotnet --version` | exit 0; `10.0.301` |
| restore com `staging/NuGet.Config` | exit 0 |
| `.nupkg.metadata` de OAuth2Introspection e IdentityModel | source `C:\Projetos\Identity\nuget-packages` |
| `dotnet list` da biblioteca | apenas MinVer privado; nenhuma dependência pública IdentityModel |
| `dotnet list` dos testes | OAuth2Introspection `[10.0.0]`; IdentityModel `10.0.0` transitivo |
| `.\build.cmd build test pack` com config do staging | exit 0 |
| build | 0 warnings; 0 errors |
| testes | 10 aprovados; 0 falhas; 0 ignorados |
| pack | exit 0; versão determinística `10.0.0` |
| `git diff --check` | exit 0 |

O restore sandboxed inicial falhou somente por TLS/`NU1301` ao NuGet.org. A repetição autorizada baixou dependências públicas; os IDs internos continuaram restritos ao feed local pelo source mapping.

## Pacote e auditoria

Artefato: `C:\Projetos\Identity\nuget-packages\IdentityModel.AspNetCore.AccessTokenValidation.10.0.0.nupkg`.

| Propriedade | Evidência |
| --- | --- |
| PackageId | `IdentityModel.AspNetCore.AccessTokenValidation` |
| Version | `10.0.0` |
| Tamanho | 11831 bytes |
| SHA-256 | `811C052744B797FDA448B0DADB88D42154A17ECA2A33E3A568BF44F22A463D60` |
| Assets | DLL e XML somente em `lib/net10.0` |
| Dependency group | somente `net10.0`, vazio |
| Framework reference | `Microsoft.AspNetCore.App` |
| Avisos legais | `LICENSE` e `README.md` incluídos |

Projetos, assets, `.deps.json`, assemblies resolvidos, `.nuspec` e nupkg possuem zero dependências cujo PackageId começa por `Duende.`. As alterações de teste modernizam TestServer/Host, assertions assíncronas e o construtor de AuthenticationHandler sem mudar os fluxos validados.

Todos os 12 arquivos modificados/novos estão em UTF-8 sem BOM. Não houve publicação remota nem leitura de credenciais.
