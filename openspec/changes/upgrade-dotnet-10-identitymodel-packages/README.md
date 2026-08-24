# upgrade-dotnet-10-identitymodel-packages

Change coordenador multi-repositório para migrar o IdentityServer4 e os quatro repositórios locais IdentityModel para .NET 10, versionar os 11 pacotes como `10.0.0` e publicá-los no NuGet.org sob PackageIds `ZinfoFramework.*`, sem dependências de pacote Duende.

Os pacotes são continuações independentes mantidas por Luiz Antonio. O change preserva namespaces, assemblies, licença e créditos históricos, mas usa identidade própria de distribuição, metadados e documentação que apontam para os repositórios atuais em [github.com/Luizinfo](https://github.com/Luizinfo/).

Repositórios coordenados:

- `C:\Projetos\Identity\IdentityServer4`
- `C:\Projetos\Identity\IdentityModel`
- `C:\Projetos\Identity\IdentityModel.AspNetCore.AccessTokenValidation`
- `C:\Projetos\Identity\IdentityModel.AspNetCore.OAuth2Introspection`
- `C:\Projetos\Identity\IdentityModel.OidcClient`

O OpenSpec 1.4.1 não aplica um workspace change inteiro em múltiplas raízes. Este change permanece como fonte coordenadora única, mas cada grupo de tasks deve ser executado por um novo subagent com uma única raiz de edição explicitamente autorizada.
