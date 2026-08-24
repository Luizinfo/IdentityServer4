# Grupo 7 - Exemplos em escopo

Data: 2026-08-18

## Estado inicial preservado

- Antes das edições, `git status --short --untracked-files=all` mostrou alterações existentes do Grupo 6 em arquivos centrais de build, CI, pacote e testes, além dos arquivos OpenSpec ainda não rastreados.
- `git diff --stat` inicial registrou 53 arquivos modificados, 283 inserções e 243 remoções. Essas alterações não foram revertidas.

## Alterações aplicadas

- Quickstarts ativos em `samples/Quickstarts/*` retargetados para `net10.0`.
- Clientes ativos em `samples/Clients/src/*` retargetados para `net10.0`, exceto `WindowsConsoleSystemBrowser`, que usa APIs Windows e foi retargetado para `net10.0-windows`.
- `samples/Quickstarts/Directory.Build.targets` atualizado para versões .NET 10 e PackageIds `ZinfoFramework.*` `10.0.0`.
- `samples/Clients/Directory.Build.targets` atualizado para versões .NET 10 e PackageIds `ZinfoFramework.IdentityModel*` `10.0.0`.
- `ResourceBasedApi` mantém referências explícitas a:
  - `ZinfoFramework.IdentityModel.AspNetCore.AccessTokenValidation`
  - `ZinfoFramework.IdentityModel.AspNetCore.OAuth2Introspection`
- `ResourceBasedApi` preserva:
  - `Selector.ForwardReferenceToken("introspection")`
  - `services.AddScopeTransformation()`
- `ConsoleCode` e `WindowsConsoleSystemBrowser` consomem `ZinfoFramework.IdentityModel.OidcClient` `10.0.0`.
- `samples/Clients/src/MvcAutomaticTokenManagement/MvcAutomaticTokenManagement.csproj` foi removido de `samples/Clients/Clients.sln`.
- `samples/Clients/old` e `samples/KeyManagement` não foram migrados, restaurados nem compilados como evidência de sucesso.

## Restore e build

Cache NuGet isolado usado: `.nuget-cache-group7`.

Comandos executados:

```text
dotnet restore samples\Clients\Clients.sln --packages .nuget-cache-group7 --configfile NuGet.config
dotnet build samples\Clients\Clients.sln --no-restore --packages .nuget-cache-group7 -warnaserror:0
dotnet build samples\Clients\src\APIs\ResourceBasedApi\ResourceBasedApi.csproj --no-restore --packages .nuget-cache-group7 -warnaserror:0
dotnet build samples\Clients\src\ConsoleCode\ConsoleCode.csproj --no-restore --packages .nuget-cache-group7 -warnaserror:0
dotnet build samples\Clients\src\WindowsConsoleSystemBrowser\WindowsConsoleSystemBrowser.csproj --no-restore --packages .nuget-cache-group7 -warnaserror:0
dotnet build samples\Quickstarts\1_ClientCredentials\Quickstart.sln --no-restore --packages .nuget-cache-group7 -warnaserror:0
dotnet build samples\Quickstarts\2_InteractiveAspNetCore\Quickstart.sln --no-restore --packages .nuget-cache-group7 -warnaserror:0
dotnet build samples\Quickstarts\3_AspNetCoreAndApis\Quickstart.sln --no-restore --packages .nuget-cache-group7 -warnaserror:0
dotnet build samples\Quickstarts\4_JavaScriptClient\Quickstart.sln --no-restore --packages .nuget-cache-group7 -warnaserror:0
dotnet build samples\Quickstarts\5_EntityFramework\Quickstart.sln --no-restore --packages .nuget-cache-group7 -warnaserror:0
dotnet build samples\Quickstarts\6_AspNetIdentity\Quickstart.sln --no-restore --packages .nuget-cache-group7 -warnaserror:0
```

Resultados:

- `samples/Clients/Clients.sln`: 23 projetos compilados, 1 aviso, 0 erros.
- `ResourceBasedApi`: compilado, 3 avisos, 0 erros.
- `ConsoleCode`: compilado, 3 avisos, 0 erros.
- `WindowsConsoleSystemBrowser`: compilado, 0 avisos, 0 erros.
- `Quickstart 1_ClientCredentials`: compilado, 0 avisos, 0 erros.
- `Quickstart 2_InteractiveAspNetCore`: compilado, 5 avisos, 0 erros.
- `Quickstart 3_AspNetCoreAndApis`: compilado, 5 avisos, 0 erros.
- `Quickstart 4_JavaScriptClient`: compilado, 5 avisos, 0 erros.
- `Quickstart 5_EntityFramework`: compilado, 5 avisos, 0 erros.
- `Quickstart 6_AspNetIdentity`: compilado, 7 avisos, 0 erros.

Avisos remanescentes:

- `ASPDEPR008`/`ASPDEPR004` em hosts sample baseados em `IWebHost`/`WebHostBuilder`.
- `ASP0019` em filtros sample que usam `Response.Headers.Add`.
- `CS0618` em `UseDatabaseErrorPage` no quickstart AspNetIdentity.
- `SYSLIB0057` em carregamento de certificado no sample mTLS.
- `NU1903` para `SQLitePCLRaw.lib.e_sqlite3` transitivo no quickstart AspNetIdentity.

## Auditoria de pacote, origem e Duende

Auditoria textual em `.csproj` e `project.assets.json` ativos:

- Nenhum `.csproj` ativo em `samples/Quickstarts` ou `samples/Clients/src` resolve `netcoreapp*`, `netstandard*` ou `net4x`.
- Nenhum `.csproj` ativo em escopo usa PackageIds históricos `IdentityModel*` ou `IdentityServer4*`.
- `rg -n 'Duende\.' samples\Quickstarts samples\Clients\src -g 'project.assets.json' -g '*.csproj' ...` retornou nenhuma ocorrência para exemplos ativos.

Contagem em `project.assets.json` ativos:

- `zinfoframework.identitymodel/10.0.0`: 34 arquivos.
- `zinfoframework.identitymodel.aspnetcore.accesstokenvalidation/10.0.0`: 1 arquivo.
- `zinfoframework.identitymodel.aspnetcore.oauth2introspection/10.0.0`: 1 arquivo.
- `zinfoframework.identitymodel.oidcclient/10.0.0`: 2 arquivos.
- `zinfoframework.identityserver4/10.0.0`: 6 arquivos.
- `zinfoframework.identityserver4.aspnetidentity/10.0.0`: 1 arquivo.
- `zinfoframework.identityserver4.entityframework/10.0.0`: 1 arquivo.

Origem no cache isolado `.nuget-cache-group7`:

- `ZinfoFramework.IdentityModel` `10.0.0`: `https://api.nuget.org/v3/index.json`
- `ZinfoFramework.IdentityModel.AspNetCore.AccessTokenValidation` `10.0.0`: `https://api.nuget.org/v3/index.json`
- `ZinfoFramework.IdentityModel.AspNetCore.OAuth2Introspection` `10.0.0`: `https://api.nuget.org/v3/index.json`
- `ZinfoFramework.IdentityModel.OidcClient` `10.0.0`: `https://api.nuget.org/v3/index.json`
- `ZinfoFramework.IdentityServer4*` `10.0.0`: `C:\Projetos\Identity\IdentityServer4\nuget`, conforme pacote provisório local ainda não promovido neste grupo.

## Status das tasks

- 7.1 concluída.
- 7.2 concluída.
- 7.3 concluída.
- 7.4 concluída.
