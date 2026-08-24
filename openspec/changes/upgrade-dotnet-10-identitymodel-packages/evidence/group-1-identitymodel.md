# Evidência do grupo 1 — IdentityModel

Grupo executado por subagent exclusivo no repositório `C:\Projetos\Identity\IdentityModel` e validado centralmente pelo orquestrador em `2026-07-20T09:22:55.9921976-03:00`. Nenhum outro grupo foi iniciado durante sua execução.

## Escopo e alterações

Projetos ativos inventariados:

| Projeto | Papel | TFM validado |
| --- | --- | --- |
| `build/build.csproj` | entrypoint dos scripts oficiais | `net10.0` |
| `src/IdentityModel.csproj` | biblioteca e pacote público | `net10.0` |
| `test/UnitTests/UnitTests.csproj` | suíte da solution principal | `net10.0` |
| `test/TrimmableAnalysis/TrimmableAnalysis.csproj` | análise chamada pelo build oficial | `net10.0` |

`samples/HttpClientFactorySample/WebApplication1/HttpClientFactory.csproj` permanece fora do escopo: possui solution própria e não integra a solution principal, o entrypoint oficial ou a CI.

Arquivos do resultado: `.github/workflows/ci.yml`, `.github/workflows/codeql.yml`, `build/build.csproj`, `build/Program.cs`, `global.json`, `src/IdentityModel.csproj`, `test/TrimmableAnalysis/TrimmableAnalysis.csproj` e `test/UnitTests/UnitTests.csproj`. As alterações locais preexistentes nesses arquivos foram preservadas e aproveitadas; não houve reset, checkout destrutivo ou descarte da worktree.

O SDK está fixado em `10.0.301`, com `rollForward: disable`. A CI usa esse SDK com `actions/checkout@v7`, `actions/setup-dotnet@v6` e `github/codeql-action@v4`. Essas majors foram conferidas nas fontes oficiais atuais em `https://github.com/actions/checkout`, `https://github.com/actions/setup-dotnet` e `https://github.com/github/codeql-action/releases`.

## Build, testes e trimming

| Comando | Resultado verificável |
| --- | --- |
| `dotnet --version` | exit 0; `10.0.301` selecionado pelo `global.json` |
| `.\build.cmd build test pack` | primeira tentativa sandboxed: exit 1 apenas por `NU1301`/SSL ao NuGet.org; repetição autorizada com rede: exit 0 |
| build Release do entrypoint oficial | 0 warnings; 0 errors |
| testes do entrypoint oficial | 296 aprovados; 0 falhas; 0 ignorados |
| `dotnet publish test\TrimmableAnalysis\TrimmableAnalysis.csproj -c Release -r win-x64 --no-restore --nologo '-p:IntermediateOutputPath=bin\orchestrator-intermediate\' '-p:OutputPath=bin\orchestrator-output\' '-p:PublishDir=bin\orchestrator-publish\'` | exit 0; trimming concluído sem warnings |
| `git diff --check` | exit 0 |

A primeira execução do trimming encontrou apenas uma ACL ambiental em `obj` criado pelo restore com acesso ampliado. A validação foi repetida sem remoção de arquivos, usando intermediários isolados dentro do repositório, e passou. O SDK efetivamente selecionado foi .NET 10; outros SDKs instalados na máquina não foram selecionados devido ao `global.json` exato e ao roll-forward desabilitado.

## Pack e inspeção

Comando final de staging:

```powershell
dotnet pack .\src\IdentityModel.csproj -c Release -o 'C:\Projetos\Identity\nuget-packages' --no-build --nologo -p:MinVerVersionOverride=10.0.0
```

Resultado: exit 0 e pacote `C:\Projetos\Identity\nuget-packages\IdentityModel.10.0.0.nupkg`.

| Propriedade | Evidência |
| --- | --- |
| PackageId | `IdentityModel` |
| Version | `10.0.0` |
| Tamanho | 173826 bytes |
| SHA-256 | `6F10EF774043A8B090EAF19D47797CD1006DDAA3BAE049B23F0529D00BE4EA64` |
| Dependency group | somente `net10.0`, vazio |
| Assets compilados | `lib/net10.0/IdentityModel.dll`, `.pdb` e `.xml` |
| Outros TFMs ou pastas públicas | nenhuma pasta `ref/`, `runtimes/` ou `build/`; nenhum outro TFM |
| Avisos legais | licença Apache-2.0, `README.md`, `icon.jpg` e atribuições preservados |

## Auditoria de dependências

`dotnet list <projeto> package --include-transitive --no-restore` passou para build, biblioteca, UnitTests e TrimmableAnalysis, todos sob `net10.0`. A inspeção dos quatro `project.assets.json`, dos projetos e do `.nuspec` confirmou zero PackageIds, `PackageReference` ou `ProjectReference` iniciados por `Duende.`. Ocorrências textuais históricas em atribuições legais não são dependências e foram preservadas.

Os arquivos editados foram validados como UTF-8 sem BOM. Nenhum pacote foi publicado em feed remoto e nenhuma credencial foi lida ou exibida.
