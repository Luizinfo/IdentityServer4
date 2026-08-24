# Grupo 6 — auditoria provisória dos pacotes

Os artefatos abaixo são provisórios e não podem ser publicados; commit/push e pack definitivo pertencem ao gate posterior com autorização explícita.

| PackageId | Versão | TFM | Dependências internas |
| --- | --- | --- | --- |
| `ZinfoFramework.IdentityServer4` | 10.0.0 | net10.0 | `ZinfoFramework.IdentityModel` 10.0.0; `ZinfoFramework.IdentityServer4.Storage` 10.0.0 |
| `ZinfoFramework.IdentityServer4.Storage` | 10.0.0 | net10.0 | `ZinfoFramework.IdentityModel` 10.0.0 |
| `ZinfoFramework.IdentityServer4.EntityFramework.Storage` | 10.0.0 | net10.0 | `ZinfoFramework.IdentityServer4.Storage` 10.0.0 |
| `ZinfoFramework.IdentityServer4.EntityFramework` | 10.0.0 | net10.0 | `ZinfoFramework.IdentityServer4` e `.EntityFramework.Storage` 10.0.0 |
| `ZinfoFramework.IdentityServer4.AspNetIdentity` | 10.0.0 | net10.0 | `ZinfoFramework.IdentityServer4` 10.0.0 |

Todos os cinco `.nuspec` inspecionados declaram `Authors=Luiz Antonio`, `RepositoryUrl=https://github.com/Luizinfo/IdentityServer4.git`, `PackageProjectUrl=https://github.com/Luizinfo/IdentityServer4`, `README.md` e somente assets `lib/net10.0`. Todos incorporam `ATTRIBUTION.md`; assemblies e namespaces continuam `IdentityServer4*`. Nenhuma dependência `Duende.*` foi encontrada.
