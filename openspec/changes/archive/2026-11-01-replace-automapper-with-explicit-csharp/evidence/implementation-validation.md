# Implementação e validação — 25/10/2026

## Resultado

AutoMapper foi removido dos cinco grupos de mapeamento e dos dois pacotes EntityFramework. As extensões públicas mantêm suas assinaturas e namespaces; os cinco profiles públicos foram removidos. Novas versões locais: **11.0.0** para Storage e EntityFramework, sinalizando a quebra da API dos profiles. .NET 10, EF Core 9.0.9, modelos, entidades e schema permanecem preservados.

O usuário autorizou a implementação e a geração dos pacotes após o planejamento. Não houve commit, push, merge ou publicação. Os documentos do change de migração .NET 10 e as alterações locais pré-existentes foram preservados; apenas os trechos necessários dos dois csprojs e do targets compartilhado foram atualizados.

## Ambiente e baseline

- Branch inicial: `master`; HEAD: `e4465e93` (`ci: trigger clean release matrix validation`). Havia alterações locais no `.gitignore`, change .NET 10, targets e nos dois csprojs EF antes desta execução.
- SDK observado: .NET `10.0.113`, Windows, execução dos testes no contexto do usuário para acesso aos certificados da suíte principal.
- Base: AutoMapper `16.2.0`, modelos `ZinfoFramework.IdentityServer4.Storage/10.0.0`, EF Core `9.0.9`; dois pacotes EF inicialmente configurados em `10.0.1`.
- Consulta pública ao flat-container do NuGet.org para os dois PackageIds retornou HTTP 404 em 25/10/2026. Não foi encontrada versão pública nesses IDs; isso não inspeciona feeds privados. A versão 11.0.0 foi escolhida para explicitar a quebra pública e manter os artefatos locais anteriores intactos.
- O cache referenciado pelos assets anteriores já não existia. Comandos sem restore não executaram testes e **não foram contabilizados** como baseline. Após restore isolado válido: 16/16 testes unitários originais e 40/40 testes de integração do Storage passaram.
- Foram adicionados testes de caracterização antes da alteração de produção: **305/305** passaram contra AutoMapper 16.2.0. As quatro falhas iniciais de expectativa sobre HashSet foram corrigidas para a semântica observada de List antes de substituir a implementação.

## Comportamentos preservados

- Null na raiz retorna null nas dez extensões de criação. UpdateEntity com modelo nulo mantém a entidade; destino nulo não provoca efeitos observáveis.
- `ProtocolType` e `Secret.Type` explicitamente nulos **sobrescrevem** os defaults. O baseline demonstrou que as Conditions existentes não preservavam defaults para esses membros nulos; os testes antigos cobriam apenas inicializadores padrão.
- Coleções de destino são listas novas; propriedades usam Dictionary com tratamento case-sensitive e falha em chaves duplicadas/inválidas. AllowedGrantTypes continua usando o setter validador do modelo, incluindo rejeição de combinações e entradas inválidas.
- Claims de cliente recebem ValueType String ao ler da entidade; secrets preservam os quatro campos do modelo e não compartilham instâncias mutáveis com a origem.
- Algoritmos preservam ordem de escrita e, na leitura, Trim do texto completo, entradas vazias removidas e deduplicação case-sensitive, sem Trim individual dos tokens. Uma coleção contendo somente null continua produzindo null, em vez da string vazia que string.Join produziria isoladamente.
- Grants copiam os dez campos e atualizam a mesma entidade rastreada pelo EF, inclusive limpando campos por null.
- IDs, FKs, back-references e metadados administrativos ausentes do modelo continuam com inicializadores próprios nas entidades novas. Não há alteração em entidades, modelos, configuração do schema ou migrations.
- A quebra de compatibilidade inclui a remoção dos tipos públicos `*MapperProfile` e a troca de wrappers de exceção do AutoMapper por falhas nativas. README, changelog e release notes dos pacotes explicam a migração.

## Testes finais

| Suíte | Aprovados | Ignorados | Falhas |
| --- | ---: | ---: | ---: |
| EntityFramework.Storage UnitTests | 308 | 0 | 0 |
| EntityFramework.Storage IntegrationTests | 43 | 0 | 0 |
| EntityFramework Tests | 6 | 0 | 0 |
| IdentityServer UnitTests | 724 | 0 | 0 |
| IdentityServer IntegrationTests | 293 | 1 | 0 |
| **Total** | **1.374** | **1** | **0** |

O único teste ignorado é `ExtensionGrantClient.Dynamic_lifetime_should_succeed`, preexistente. Nenhum skip foi adicionado.

Os 40 testes de integração anteriores usam InMemory no Windows. Os três testes novos `ExplicitMappingRelationalTests` usam **SQLite em memória**, com conexão mantida e releitura em novo DbContext: cliente com todos os filhos, recursos/escopos com filhos e atualização de grant pela mesma Key. Foram verificados IDs gerados, FKs, tracking e linha única do grant. SQL Server não foi executado.

Cobertura medida com Microsoft.CodeCoverage, já trazido pelo Test SDK: **226/226 linhas dos mapeadores e helpers (100%)**, incluindo as linhas das closures geradas. Isso é cobertura de linhas da área modificada, não de toda a solução nem afirmação de 100% de branches. Relatório final, incluindo os casos adicionais de strings vazias e filhos nulos: `artifacts/explicit-mapping-11.0.0/final-coverage-complete/5c3e7051-76ef-4b4b-a272-bd6ea45e694a/*.cobertura.xml`.

Os testes de classificação de propriedades abrangem os cinco tipos principais e 19 tipos de modelo/entidade filhos, incluindo propriedades herdadas e exclusões explícitas. A matriz detalhada está em `property-matrix.md`.

## Builds e consumo

As duas solutions EF compilaram em Release, incluindo os hosts e projetos de migrations. O quickstart 5_EntityFramework também compilou em Release usando os dois nupkgs 11.0.0.

O consumidor `validation/PackageConsumer` não possui ProjectReference. Restaurou os pacotes finais em um terceiro cache inicialmente vazio e executou:

- Os cinco grupos de conversão por extensões públicas.
- Gravação e atualização de grant com SQLite, confirmando a limpeza de Expiration e a linha única após nova leitura.
- Versão major 11 dos dois assemblies e ausência dos profiles exportados.
- Ausência de referência de assembly ao AutoMapper no Storage.

Os SHA-512 registrados nos assets desse consumidor foram comparados aos bytes dos **dois nupkgs finais**: ambos coincidem. Os dez projetos antes afetados foram inspecionados em project.assets.json: nenhum contém AutoMapper. A busca em código de produção e testes não encontrou usos residuais da biblioteca, reflexão em produção ou biblioteca substituta.

## Pacotes finais

| Pacote | Versão | SHA-256 |
| --- | --- | --- |
| ZinfoFramework.IdentityServer4.EntityFramework.Storage | 11.0.0 | `38D7F3DF28237DEBD2278CD7D155E76CD970008A6A4F225DF65A89CEC323FBF3` |
| ZinfoFramework.IdentityServer4.EntityFramework | 11.0.0 | `E4B055969DDF495F7EBD3ECA83638CE4167B9FEA15433BC1B06972DE0D6A030F` |

Arquivos disponíveis em `C:\Projetos\Identity\IdentityServer4\nuget` e, com os mesmos hashes, em `artifacts/explicit-mapping-11.0.0/final`. Os artefatos antigos 10.0.0/10.0.1 foram mantidos. O diretório `feed` deste staging contém pacotes intermediários de desenvolvimento e dependências de baseline; somente `final` e as cópias verificadas em `nuget` são a entrega desta execução.

Os dois nupkgs contêm somente assets net10.0, README e PDB com os fontes incorporados por EmbedAllSources, permitindo depuração do working tree sem depender de um commit remoto novo. A inspeção de nuspec confirmou:

- Storage: Microsoft.EntityFrameworkCore.Relational >= 9.0.9 e ZinfoFramework.IdentityServer4.Storage >= 10.0.0.
- EntityFramework: ZinfoFramework.IdentityServer4 >= 10.0.0 e ZinfoFramework.IdentityServer4.EntityFramework.Storage >= 11.0.0.
- Nenhuma dependência de AutoMapper, mantendo licença Apache-2.0 e atribuições existentes.

## Comandos e isolamento

Execução a partir de `C:\Projetos\Identity\IdentityServer4`. Logs em `artifacts/explicit-mapping-11.0.0/logs`.

```powershell
dotnet restore src/EntityFramework.Storage/IdentityServer4.EntityFramework.Storage.sln --configfile artifacts/explicit-mapping-11.0.0/Final.NuGet.Config
dotnet build src/EntityFramework.Storage/IdentityServer4.EntityFramework.Storage.sln -c Release --no-restore -p:EmbedAllSources=true
dotnet pack src/EntityFramework.Storage/src/IdentityServer4.EntityFramework.Storage.csproj -c Release --no-build --no-restore -o artifacts/explicit-mapping-11.0.0/final
dotnet restore src/EntityFramework/IdentityServer4.EntityFramework.sln --configfile artifacts/explicit-mapping-11.0.0/Final.NuGet.Config
dotnet build src/EntityFramework/IdentityServer4.EntityFramework.sln -c Release --no-restore -p:EmbedAllSources=true
dotnet test src/EntityFramework/test/IdentityServer4.EntityFramework.Tests/IdentityServer4.EntityFramework.Tests.csproj -c Release --no-build --no-restore
dotnet pack src/EntityFramework/src/IdentityServer4.EntityFramework.csproj -c Release --no-build --no-restore -o artifacts/explicit-mapping-11.0.0/final
dotnet test src/EntityFramework.Storage/test/UnitTests/IdentityServer4.EntityFramework.UnitTests.csproj -c Release --no-restore -p:EmbedAllSources=true --collect 'Code Coverage;Format=Cobertura' --results-directory artifacts/explicit-mapping-11.0.0/final-coverage-complete
dotnet test src/EntityFramework.Storage/test/IntegrationTests/IdentityServer4.EntityFramework.IntegrationTests.csproj -c Release --no-build --no-restore
dotnet restore openspec/changes/replace-automapper-with-explicit-csharp/validation/PackageConsumer/PackageConsumer.csproj --configfile artifacts/explicit-mapping-11.0.0/Final.NuGet.Config --packages artifacts/explicit-mapping-11.0.0/consumer-packages
dotnet run --project openspec/changes/replace-automapper-with-explicit-csharp/validation/PackageConsumer/PackageConsumer.csproj -c Release --no-restore
dotnet restore samples/Quickstarts/5_EntityFramework/Quickstart.sln --configfile artifacts/explicit-mapping-11.0.0/Final.NuGet.Config
dotnet build samples/Quickstarts/5_EntityFramework/Quickstart.sln -c Release --no-restore
```

As duas suítes principais foram restauradas com a configuração de staging e executadas por `dotnet test <csproj> -c Release --no-restore`. A configuração final usa source mapping exato dos dois IDs EF para `final`, os demais IDs ZinfoFramework para dependências locais validadas e nuget.org para pacotes externos. Caches de desenvolvimento, de validação final e do consumidor estão separados.

## Avisos e limites

- Nenhum warning de compilação foi introduzido pelos arquivos de mapeamento ou pelos testes novos. Permanecem avisos existentes ASPDEPR008/ASPDEPR004, ASP0019 e EF1001 em outras áreas.
- O restore atual sinalizou **NU1902** em `Microsoft.Build.Tasks.Git 8.0.0`, ferramenta transitiva de SourceLink já existente: GHSA-23fw-v26w-5fgq. Essa ferramenta não é dependência pública dos nupkgs.
- O quickstart sinalizou **NU1902 e NU1903** em `System.Security.Cryptography.Pkcs 9.0.4`: GHSA-2fmv-rv2c-59x7 e GHSA-846f-f764-76mx. Os pais observados incluem Microsoft.Build.Tasks.Core, Microsoft.CodeAnalysis.Workspaces.MSBuild e Microsoft.Data.SqlClient, provenientes das dependências do exemplo. As versões dessas dependências não foram alteradas por esta refatoração. A correção desses alertas requer trabalho separado; o build aprovado não equivale a uma auditoria sem vulnerabilidades.
- Não há prova sobre consumidores externos desconhecidos dos profiles removidos; a quebra está explicitada e a versão major foi elevada.
- A consulta pública não comprova inexistência de versões em feeds privados. Nenhum feed remoto foi escrito.

## Aceite e reversão

AC01–AC07 e AC09–AC12 foram atendidos pelas implementações, testes, inspeções e artefatos acima. AC08 está atendido quanto a testes, skips e ausência de novos warnings nos arquivos alterados; os alertas de auditoria de dependências existentes estão explicitamente ressalvados acima.

Para reversão local, reverter apenas os trechos desta implementação e as referências de versão 11.0.0, preservando as alterações anteriores da migração .NET 10. Os pacotes anteriores continuam disponíveis. Não há migração de banco a reverter. A futura publicação da família completa precisa reconciliar os gates ainda pendentes do change .NET 10; seus hashes históricos não validam os novos pacotes.
