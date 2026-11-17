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

Instale o SDK **10.0.401**, definido em `global.json`, e execute `./build.ps1` no Windows ou `./build.sh` em Linux/macOS. Dependências `ZinfoFramework.IdentityModel*` são obtidas do NuGet.org; dependências públicas continuam vindo do mesmo feed. O SourceLink é fornecido pelo SDK corrigido, sem referência ao pacote vulnerável `Microsoft.Build.Tasks.Git` 8.0.0.

Depois do build completo, execute `pwsh ./eng/Validate-Release.ps1` para compilar os seis quickstarts e os clientes ativos, auditar dependências transitivas e, no Windows com SQL Server LocalDB instalado, validar as migrações em bancos novos de uma instância exclusiva de teste. Cada banco recebe um nome único iniciado por `IdentityPrReadiness_`; bancos existentes da aplicação não são usados. A opção `-SkipMigrations` permite executar somente os demais checks e não conclui o gate de migrações.

Os contextos de configuração e grants usam EF Core 9.0.9. O host e as ferramentas do exemplo AspNetIdentity usam EF Core 10 para acompanhar ASP.NET Core Identity 10; essa configuração não altera o mínimo dos pacotes de persistência. O validador cobre os três contextos SQL Server. Os exemplos também fixam PKCS 10.0.13 para eliminar os avisos de negação de serviço conhecidos nas versões transitivas anteriores.

A CI usa o mesmo `global.json` em Windows, Linux e macOS e executa a validação SQL Server no Windows. Builds de PR não assinam nem publicam pacotes. O merge exige conferir os resultados remotos da revisão enviada.

## Correções de segurança preparadas para a próxima versão

As correções de mTLS e URLs de retorno nesta branch ainda não fazem parte dos pacotes publicados `10.0.0`/`11.0.0`. Os pacotes gerados durante os checks são artefatos locais de validação; uma publicação futura exige nova versão, commit final, hashes e autorização de promoção.

- O esquema configurado em `MutualTls.ClientCertificateAuthenticationScheme` deve estar registrado, normalmente com `services.AddAuthentication().AddCertificate(...)`. Todos os endpoints que recebem credenciais mTLS exigem que esse esquema autentique o certificado, inclusive `/connect/token`, sem depender do alias `/connect/mtls/token`.
- Secrets `X509Name` exigem uma cadeia emitida por CA confiável, com uso para autenticação de cliente. Um certificado autoassinado não autentica por nome, mesmo quando o handler permite `CertificateTypes.All`. Para certificados autoassinados autorizados, use secrets `X509Thumbprint` e configure explicitamente o handler para aceitá-los.
- Para uma CA privada, configure `ChainTrustValidationMode`/`CustomTrustStore` no handler e `MutualTls.ClientCertificateChainPolicy.TrustMode`/`CustomTrustStore` no IdentityServer. A política adicional de cadeia preserva a validação de validade e, por padrão, a consulta de revogação. Não desative a revogação em produção; o uso de `NoCheck` nos testes é exclusivo da CA sintética sem endpoint de CRL.
- Quando houver proxy reverso, aceite certificados encaminhados somente de proxies confiáveis que removam headers de certificado enviados pelo cliente.
- URLs de retorno com caracteres de controle são rejeitadas antes de serem consideradas locais. Valide também URLs próprias da aplicação antes de redirecionar.

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
