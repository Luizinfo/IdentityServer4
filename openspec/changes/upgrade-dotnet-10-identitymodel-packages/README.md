# upgrade-dotnet-10-identitymodel-packages

Change coordenador multi-repositório para migrar o IdentityServer4 e os quatro repositórios locais IdentityModel para .NET 10 e publicar os pacotes no NuGet.org sob PackageIds `ZinfoFramework.*`, sem dependências de pacote Duende. A matriz publicada contém nove pacotes `10.0.0` (seis IdentityModel e três IdentityServer4) e os dois pacotes `ZinfoFramework.IdentityServer4.EntityFramework*` `11.0.0`, com dependências EF Core mínimas `9.0.9`. Os artefatos EF `10.0.0` e `10.0.1` foram provisórios e não integram a matriz publicada. Consulte as evidências de publicação referenciadas em `tasks.md`.

As correções de prontidão para PR de 17/11/2026 atualizam segurança, SDK, build e validações. Os artefatos reconstruídos nesta revisão são somente candidatos locais e não alteram as versões já publicadas. Consulte a [evidência de prontidão](evidence/pr-readiness-2026-11-17.md) para resultados e limitações da revisão atual.

Os pacotes são continuações independentes mantidas por Luiz Antonio. O change preserva namespaces, assemblies, licença e créditos históricos, mas usa identidade própria de distribuição, metadados e documentação que apontam para os repositórios atuais em [github.com/Luizinfo](https://github.com/Luizinfo/).

Repositórios coordenados:

- `C:\Projetos\Identity\IdentityServer4`
- `C:\Projetos\Identity\IdentityModel`
- `C:\Projetos\Identity\IdentityModel.AspNetCore.AccessTokenValidation`
- `C:\Projetos\Identity\IdentityModel.AspNetCore.OAuth2Introspection`
- `C:\Projetos\Identity\IdentityModel.OidcClient`

O OpenSpec 1.4.1 não aplica um workspace change inteiro em múltiplas raízes. Este change permanece como fonte coordenadora única, mas cada grupo de tasks deve ser executado por um novo subagent com uma única raiz de edição explicitamente autorizada.
