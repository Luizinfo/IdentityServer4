# Staging local isolado

Esta configuração é o staging pré-publicação do change. Ela não contém dados do feed remoto nem credenciais.

Em uma sessão PowerShell, inicialize os diretórios e as variáveis de cache:

```powershell
& .\openspec\changes\upgrade-dotnet-10-identitymodel-packages\staging\Initialize-Staging.ps1
```

Use o arquivo retornado em `NuGetConfig` em todos os restores de validação e envie os nupkgs para o diretório retornado em `PackagesFeed`. O feed compartilhado fica em `C:\Projetos\Identity\nuget-packages`. Exemplo a partir da raiz do IdentityServer4:

```powershell
$staging = '.\openspec\changes\upgrade-dotnet-10-identitymodel-packages\staging'
$packagesFeed = 'C:\Projetos\Identity\nuget-packages'
dotnet restore <projeto-ou-solucao> --configfile "$staging\NuGet.Config" --no-http-cache
dotnet pack <projeto> --no-restore -p:PackageVersion=10.0.0 --output $packagesFeed
```

O `packageSourceMapping` associa exatamente os 11 IDs finais `ZinfoFramework.IdentityModel*` e `ZinfoFramework.IdentityServer4*` a `local-staging`. IDs históricos sem o prefixo não são internos e não permanecem mapeados ao staging. A regra `*` do `nuget.org` permanece disponível para os demais pacotes; a correspondência exata dos IDs internos tem precedência sobre essa regra genérica. O `globalPackagesFolder` e as variáveis `NUGET_HTTP_CACHE_PATH` e `NUGET_PLUGINS_CACHE_PATH` usam os diretórios novos `artifacts/group-5-zinfo-*` e impedem que pacotes do cache usual do usuário ou de validações anteriores mascarem a origem.

O diretório `artifacts` contém apenas os caches isolados e é ignorado pelo Git. Os `.nupkg` permanecem no feed compartilhado `C:\Projetos\Identity\nuget-packages`. O inicializador cria e reutiliza somente `group-5-zinfo-global-packages`, `group-5-zinfo-http-cache` e `group-5-zinfo-plugins-cache`; ele não apaga caches existentes. A publicação no feed remoto não faz parte deste script e continua sujeita ao gate manual do change.
