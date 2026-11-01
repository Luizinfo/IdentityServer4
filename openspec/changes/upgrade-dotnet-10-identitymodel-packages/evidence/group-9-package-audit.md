# Auditoria dos pacotes publicados — 01/11/2026

Foram inspecionados os `.nuspec` e conteúdos dos cinco nupkgs promovidos de `artifacts/publication-final-20261101/feed` e dos seis nupkgs IdentityModel de `C:\Projetos\Identity\nuget-packages`. A [evidência pós-publicação](../../archive/2026-11-01-replace-automapper-with-explicit-csharp/evidence/publication-attempt-2026-11-01.md) registra a comparação de todos os arquivos internos desses 11 nupkgs com os pacotes disponíveis no NuGet.org: conteúdo idêntico, com adição da assinatura `.signature.p7s` pelo NuGet.

Todos declaram `Authors=Luiz Antonio`, `PackageProjectUrl` e `RepositoryUrl` no repositório correspondente em `github.com/Luizinfo`, e somente assets `lib/net10.0`. Os cinco IdentityServer4 contêm `README.md`, `LICENSE` e `ATTRIBUTION.md`; os seis IdentityModel contêm `README.md`, `LICENSE`, `NOTICE.md` e `CHANGELOG.md`. Nenhum dependency group declara pacote `Duende.*`.

| PackageId | Versão | Dependências declaradas |
| --- | --- | --- |
| ZinfoFramework.IdentityModel | 10.0.0 | Nenhuma |
| ZinfoFramework.IdentityModel.AspNetCore.AccessTokenValidation | 10.0.0 | Nenhuma |
| ZinfoFramework.IdentityModel.AspNetCore.OAuth2Introspection | 10.0.0 | IdentityModel `[10.0.0]` |
| ZinfoFramework.IdentityModel.OidcClient | 10.0.0 | Microsoft.Extensions.Logging.Abstractions `10.0.8`; IdentityModel `[10.0.0]` |
| ZinfoFramework.IdentityModel.OidcClient.DPoP | 10.0.0 | OidcClient `10.0.0`; Microsoft.Extensions.Logging.Abstractions `10.0.8`; System.IdentityModel.Tokens.Jwt `8.15.0`; IdentityModel `[10.0.0]` |
| ZinfoFramework.IdentityModel.OidcClient.IdentityTokenValidator | 10.0.0 | OidcClient `10.0.0`; Microsoft.Extensions.Logging.Abstractions `10.0.8`; System.IdentityModel.Tokens.Jwt `8.15.0` |
| ZinfoFramework.IdentityServer4.Storage | 10.0.0 | Newtonsoft.Json `13.0.4`; IdentityModel `10.0.0` |
| ZinfoFramework.IdentityServer4 | 10.0.0 | Microsoft.AspNetCore.Authentication.OpenIdConnect `10.0.0`; Microsoft.IdentityModel.Protocols.OpenIdConnect `8.15.0`; Newtonsoft.Json `13.0.4`; IdentityModel `10.0.0`; IdentityServer4.Storage `10.0.0` |
| ZinfoFramework.IdentityServer4.AspNetIdentity | 10.0.0 | IdentityServer4 `10.0.0` |
| ZinfoFramework.IdentityServer4.EntityFramework.Storage | 11.0.0 | Microsoft.EntityFrameworkCore.Relational `9.0.9`; IdentityServer4.Storage `10.0.0` |
| ZinfoFramework.IdentityServer4.EntityFramework | 11.0.0 | IdentityServer4 `10.0.0`; IdentityServer4.EntityFramework.Storage `11.0.0` |

Na tabela, nomes internos abreviados pertencem ao prefixo `ZinfoFramework.` e versões apresentadas sem colchetes são as declarações literais do nuspec. A auditoria não confirma titularidade na conta NuGet nem reserva do prefixo, tratados separadamente na tarefa 9.4. A vulnerabilidade `NU1903` de `System.Security.Cryptography.Pkcs` 9.0.4 no quickstart e o gate de CI remota permanecem registrados na evidência pós-publicação.
