# ZinfoFramework IdentityServer4

> Continuação independente mantida por **Luiz Antonio** em [Luizinfo](https://github.com/Luizinfo/). Este fork não é afiliado nem endossado pelos mantenedores originais e não declara certificação OpenID Connect própria.

IdentityServer4 é um framework OpenID Connect e OAuth 2.0 para ASP.NET Core. Esta distribuição atualiza os componentes ativos para .NET 10, preservando os namespaces e nomes de assemblies históricos.

## Instalação

Os pacotes `10.0.0` requerem `net10.0`:

```shell
dotnet add package ZinfoFramework.IdentityServer4 --version 10.0.0
```

Adicione também o pacote de persistência ou integração necessário ao seu projeto.

## EntityFramework 11.0.0: mapeamento explícito

`ZinfoFramework.IdentityServer4.EntityFramework` e `ZinfoFramework.IdentityServer4.EntityFramework.Storage` passam para `11.0.0`, com mapeamentos explícitos em C# e sem dependência do AutoMapper. Os demais pacotes desta distribuição permanecem em `10.0.0`. Ambos continuam requerendo .NET 10 e EF Core 9.0.9 ou superior.

As extensões `ToModel()`, `ToEntity()` e `PersistedGrant.UpdateEntity()` mantêm suas assinaturas e namespaces. Não há alteração do schema ou necessidade de nova migration por esta substituição.

**Quebra de API:** os tipos públicos `ClientMapperProfile`, `ApiResourceMapperProfile`, `IdentityResourceMapperProfile`, `ScopeMapperProfile` e `PersistedGrantMapperProfile` foram removidos. Consumidores que os registravam em configurações próprias devem usar as extensões de conversão ou implementar seus próprios mapeamentos. As conversões não lançam mais wrappers `AutoMapperMappingException`; entradas inválidas continuam falhando por exceções nativas. Remova capturas específicas do AutoMapper nesse caminho.

Atualize conjuntamente os dois pacotes EntityFramework para `11.0.0` quando ambos forem referenciados diretamente. Outras dependências de AutoMapper pertencentes à aplicação consumidora devem ser avaliadas separadamente.

## Migração dos IDs

| ID histórico | Novo ID 10.0.0 |
| --- | --- |
| `IdentityServer4` | `ZinfoFramework.IdentityServer4` |
| `IdentityServer4.Storage` | `ZinfoFramework.IdentityServer4.Storage` |
| `IdentityServer4.EntityFramework.Storage` | `ZinfoFramework.IdentityServer4.EntityFramework.Storage` |
| `IdentityServer4.EntityFramework` | `ZinfoFramework.IdentityServer4.EntityFramework` |
| `IdentityServer4.AspNetIdentity` | `ZinfoFramework.IdentityServer4.AspNetIdentity` |

Troque apenas a `PackageReference` e o TFM para `net10.0`. Namespaces e assemblies permanecem `IdentityServer4.*`. Revise as breaking changes das dependências .NET/ASP.NET Core/EF Core 10 e valide seus fluxos OAuth/OIDC e migrações existentes antes de implantar. O schema persistido não foi redesenhado por esta atualização.

## Build

Instale o SDK .NET 10 e execute `build.ps1` no Windows ou `build.sh` em Linux/macOS. Dependências `ZinfoFramework.IdentityModel*` são obtidas do NuGet.org; dependências públicas continuam vindo do mesmo feed.

## Projeto e comunidade

- Código e issues: [Luizinfo/IdentityServer4](https://github.com/Luizinfo/IdentityServer4)
- Suporte: [SUPPORT.md](SUPPORT.md)
- Segurança: [SECURITY.MD](SECURITY.MD)
- Contribuição: [CONTRIBUTING.md](CONTRIBUTING.md)
- Alterações: [CHANGELOG.md](CHANGELOG.md)
- Atribuição e histórico: [ATTRIBUTION.md](ATTRIBUTION.md)
- Licença: [Apache-2.0](LICENSE)

## Histórico upstream

O IdentityServer4 original foi fundado e mantido por Dominick Baier e Brock Allen, com contribuições da comunidade e participação histórica da .NET Foundation. Certificações e documentação do projeto original pertencem ao contexto histórico upstream e não certificam este fork. Consulte a [atribuição](ATTRIBUTION.md) para os links históricos preservados.

## Agradecimentos

Crédito aos autores e a todos os [contribuidores do projeto original](https://github.com/IdentityServer/IdentityServer4/graphs/contributors), além dos projetos ASP.NET Core, Entity Framework Core, IdentityModel, Bullseye, SimpleExec, MinVer, Newtonsoft.Json, xUnit e Fluent Assertions.
