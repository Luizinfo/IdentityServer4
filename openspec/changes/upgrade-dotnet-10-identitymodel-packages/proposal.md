## Why

O IdentityServer4 e parte do ecossistema IdentityModel ainda usam runtimes e dependências sem suporte. A atualização coordenada para .NET 10 permite restaurar uma base suportada e validar os forks locais como um conjunto, sem introduzir dependências NuGet da Duende. Os seis pacotes IdentityModel `10.0.0` já foram publicados; os cinco pacotes IdentityServer4 ainda estão somente locais. Antes da publicação destes últimos, a dependência EF Core foi ajustada para `9.0.9`; por isso, três pacotes IdentityServer4 permanecem em `10.0.0` e os dois pacotes EntityFramework serão publicados em `10.0.1`. Como os PackageIds originais já pertencem aos mantenedores anteriores no NuGet.org, a continuação independente usa o prefixo próprio `ZinfoFramework.` e documenta claramente sua proveniência.

## What Changes

- Coordenar, em um único change, alterações nos cinco repositórios locais envolvidos.
- Atualizar os projetos ativos, builds, testes e exemplos em escopo para .NET 10.
- **BREAKING** Publicar os cinco pacotes IdentityServer4 restantes com PackageIds prefixados por `ZinfoFramework.` e somente assets `net10.0`: três na versão `10.0.0` e `ZinfoFramework.IdentityServer4.EntityFramework.Storage` e `ZinfoFramework.IdentityServer4.EntityFramework` na versão `10.0.1`. Os seis pacotes IdentityModel já publicados permanecem em `10.0.0`.
- **BREAKING** Migrar referências NuGet internas e consumidores dos IDs históricos para os novos IDs `ZinfoFramework.IdentityModel*` e `ZinfoFramework.IdentityServer4*`, preservando namespaces e nomes de assemblies.
- Definir `Authors` como `Luiz Antonio` e apontar `RepositoryUrl` e `PackageProjectUrl` de cada pacote para o respectivo repositório atual em `https://github.com/Luizinfo/`.
- Documentar que os repositórios são forks mantidos por Luiz Antonio, indicar `https://github.com/Luizinfo/`, explicar a migração dos PackageIds e preservar os créditos, licenças e avisos dos projetos originais.
- Atualizar dependências Microsoft, ferramentas e pacotes de teste para versões fixas compatíveis com .NET 10; fixar as dependências EF Core dos componentes IdentityServer4 em `9.0.9`.
- Substituir referências a pacotes `Duende.*` por pacotes locais ou test doubles independentes.
- Manter `DPoPTests` por meio de um authorization-server de teste mínimo em ASP.NET Core, sem `Duende.IdentityServer` e sem dependência do IdentityServer4.
- Migrar `ConformanceTests` para .NET 10 como consumidor pós-pack dos nupkgs `10.0.0` e adaptá-lo à suíte atual da OpenID Foundation.
- Usar um feed local isolado para staging e validação integrada; depois da aprovação, publicar os pacotes no novo feed NuGet e fazer a CI consumi-los desse feed.
- Excluir da migração `samples/Clients/old`, `samples/KeyManagement` e `MvcAutomaticTokenManagement`.

## Affected Repositories

- `C:\Projetos\Identity\IdentityServer4`
- `C:\Projetos\Identity\IdentityModel`
- `C:\Projetos\Identity\IdentityModel.AspNetCore.AccessTokenValidation`
- `C:\Projetos\Identity\IdentityModel.AspNetCore.OAuth2Introspection`
- `C:\Projetos\Identity\IdentityModel.OidcClient`

## Capabilities

### New Capabilities

- `dotnet-10-runtime-support`: Compilação, testes e publicação dos componentes ativos com SDK e runtime .NET 10.
- `local-identitymodel-package-integration`: Produção, validação, publicação e consumo dos pacotes IdentityModel locais `10.0.0`, sem dependências NuGet Duende.
- `multi-repository-release-orchestration`: Coordenação sequencial dos cinco repositórios, com gates de validação e publicação.
- `fork-package-identity-and-attribution`: Identidade própria dos 11 pacotes mantidos, metadados NuGet atuais, documentação de continuidade e atribuição aos projetos originais.

### Modified Capabilities

Nenhuma.

## Impact

- Afeta SDKs, CI, configurações NuGet, projetos, builds, testes e exemplos nos cinco repositórios listados.
- Altera TFMs, versões de pacote e contratos binários públicos, inclusive os tipos expostos por OAuth2Introspection que hoje vêm de `Duende.IdentityModel`.
- Altera os 11 PackageIds públicos; consumidores precisarão trocar suas `PackageReference`, mas os namespaces e nomes de assemblies existentes permanecerão estáveis.
- Invalida os hashes, a validação pós-pack e a autorização de publicação dos seis artefatos IdentityModel produzidos antes da adoção do prefixo; esses gates deverão ser repetidos.
- Os dois nupkgs EntityFramework `10.0.0` já gerados são artefatos provisórios locais; seus hashes não autorizam a publicação dos pacotes `10.0.1`.
- Requer autorização explícita de uma raiz de edição por grupo de tasks, pois o OpenSpec 1.4.1 não concede múltiplos `allowedEditRoots` em um único apply.
- Requer o nome, endpoint e credencial protegida do novo feed NuGet antes dos gates de publicação e CI.
