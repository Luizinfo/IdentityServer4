# Evidência do grupo 2 — OAuth2Introspection

Grupo executado por subagent exclusivo em `C:\Projetos\Identity\IdentityModel.AspNetCore.OAuth2Introspection` e validado centralmente pelo orquestrador em `2026-07-20T10:07:07.5664278-03:00`.

## Escopo e contrato público

Projetos ativos: `build/build.csproj`, `src/IdentityModel.AspNetCore.OAuth2Introspection.csproj`, `test/Tests/Tests.csproj` e o novo `test/ConsumerCompileTest/ConsumerCompileTest.csproj`; todos usam somente `net10.0`.

O SDK está fixado em `10.0.301`, com roll-forward desabilitado. A CI usa .NET 10 com `actions/checkout@v7` e `actions/setup-dotnet@v6`; o push remoto legado foi removido. O entrypoint oficial passou a usar somente APIs do SDK, sem Bullseye ou SimpleExec.

O PackageReference `Duende.IdentityModel` foi substituído por `IdentityModel` versão exata `[10.0.0]`; namespaces públicos e internos foram migrados para `IdentityModel` e `IdentityModel.Client`. O `README.md` documenta a quebra binária e de código-fonte nos tipos expostos. `ConsumerCompileTest` integra a solution e compila o contrato público novo, recebendo `IdentityModel 10.0.0` transitivamente pelo projeto sob teste, sem ProjectReference ao repositório IdentityModel.

## Restore, build e testes

| Evidência | Resultado |
| --- | --- |
| `dotnet --version` | exit 0; `10.0.301` |
| primeiro restore sandboxed | exit 1 somente por `NU1301`/SSL ao NuGet.org |
| restore autorizado com `staging/NuGet.Config` e cache isolado | exit 0 |
| `.nupkg.metadata` de `IdentityModel/10.0.0` | source `C:\Projetos\Identity\nuget-packages` |
| `dotnet list` da biblioteca | solicitado `[10.0.0]`, resolvido `10.0.0`, `net10.0` |
| `dotnet list` do ConsumerCompileTest | IdentityModel transitivo `10.0.0`, `net10.0` |
| entrypoint oficial `.\build.cmd build test pack` | exit 0 |
| build | 0 errors; 2 warnings de API de teste ASP.NET obsoleta (`ASPDEPR004`, `ASPDEPR008`) |
| testes | 42 aprovados; 0 falhas; 0 ignorados |
| compilação de consumidor | exit 0 dentro do build da solution |
| `git diff --check` | exit 0 |

`NuGetAudit=false` foi usado apenas como variável do comando de validação offline, sem ser persistido. O pacote público foi restaurado do staging e os demais pacotes públicos do NuGet.org.

## Pack e auditoria

Artefato validado: `C:\Projetos\Identity\nuget-packages\IdentityModel.AspNetCore.OAuth2Introspection.10.0.0.nupkg`.

| Propriedade | Evidência |
| --- | --- |
| PackageId | `IdentityModel.AspNetCore.OAuth2Introspection` |
| Version | `10.0.0` |
| Tamanho | 58291 bytes |
| SHA-256 | `65AAC4781D4DDF2128F50F100235AFDA1CB6D90853C0A37E45EC3097AE53985D` |
| Assets | DLL, PDB e XML somente em `lib/net10.0` |
| Dependency group | somente `net10.0` |
| Dependência interna | `IdentityModel [10.0.0]` |
| Framework reference | `Microsoft.AspNetCore.App`, `net10.0` |

A inspeção de projetos, `project.assets.json`, `.deps.json` e `.nuspec` encontrou zero PackageIds, PackageReferences ou ProjectReferences iniciados por `Duende.`. Ocorrências históricas em documentação e atribuições legais foram preservadas e não são dependências. Licença Apache-2.0, autores, icon e project URL permanecem no pacote.

Todos os arquivos editados/criados estão em UTF-8 sem BOM. O output intermediário do pack oficial foi removido após inspeção; somente o nupkg validado no feed local permanece. Não houve publicação remota nem leitura de credenciais.
