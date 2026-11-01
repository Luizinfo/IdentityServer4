## 0. Preparar a coordenação multi-repositório

- [x] 0.1 Registrar branch e commit inicial dos cinco repositórios coordenados.
- [x] 0.2 Autorizar explicitamente a raiz de edição de cada repositório antes de iniciar seu grupo; usar um novo subagent por grupo.
- [x] 0.3 Registrar a matriz dos 11 PackageIds, todos na versão determinística `10.0.0`.
- [x] 0.4 Definir nome e endpoint do novo feed NuGet e o secret de CI, sem persistir credenciais no repositório nem expô-las nos logs.
- [x] 0.5 Preparar configuração de staging local isolado e source mapping para separar IDs internos do NuGet.org.

## 1. Validar e empacotar IdentityModel

- [x] 1.1 No repositório `C:\Projetos\Identity\IdentityModel`, inventariar projetos ativos e concluir o retarget para `net10.0` de biblioteca, build e testes em escopo.
- [x] 1.2 Alinhar SDK, CI, ferramentas e dependências a versões fixas compatíveis com .NET 10.
- [x] 1.3 Executar build e testes completos com somente o SDK .NET 10.
- [x] 1.4 Empacotar `IdentityModel` `10.0.0` no staging e inspecionar versão, TFM e dependências.

## 2. Atualizar OAuth2Introspection

- [x] 2.1 No repositório `C:\Projetos\Identity\IdentityModel.AspNetCore.OAuth2Introspection`, atualizar SDK, CI, build, biblioteca e testes para `net10.0`.
- [x] 2.2 Substituir `Duende.IdentityModel` e namespaces `Duende.IdentityModel.*` por `IdentityModel` `10.0.0` do staging.
- [x] 2.3 Documentar a quebra dos tipos públicos e adicionar teste de compilação de consumidor.
- [x] 2.4 Executar build e testes e empacotar `IdentityModel.AspNetCore.OAuth2Introspection` `10.0.0` no staging.

## 3. Atualizar OidcClient

- [x] 3.1 No repositório `C:\Projetos\Identity\IdentityModel.OidcClient`, atualizar SDK, CI, build, bibliotecas, testes e clientes ativos para .NET 10.
- [x] 3.2 Atualizar IdentityModel, JWT, Microsoft.Extensions e demais dependências para versões fixas compatíveis.
- [x] 3.3 Migrar `DPoPTests` para `net10.0` e remover `Duende.IdentityServer`.
- [x] 3.4 Substituir `IdentityServerHost` por test double ASP.NET Core mínimo, com proof, nonce, emissão JWT `cnf.jkt` e validação direta no `ApiHost`.
- [x] 3.5 Preservar os quatro cenários DPoP atuais e adicionar cenários negativos para chave e `ath` divergentes.
- [x] 3.6 Executar build e todas as suítes automatizadas do OidcClient, incluindo DPoPTests, antes do empacotamento.
- [x] 3.7 Empacotar `IdentityModel.OidcClient`, `IdentityModel.OidcClient.DPoP` e `IdentityModel.OidcClient.IdentityTokenValidator` `10.0.0` no staging.
- [x] 3.8 Migrar `clients/ConformanceTests` para `net10.0` como consumidor pós-pack dos três pacotes locais necessários, sem `ProjectReference` para as bibliotecas testadas.
- [x] 3.9 Adaptar ConformanceTests à suíte atual da OpenID Foundation, configurar o validator JWT e automatizar testes do callback loopback; documentar a execução externa manual.

## 4. Atualizar AccessTokenValidation

- [x] 4.1 No repositório `C:\Projetos\Identity\IdentityModel.AspNetCore.AccessTokenValidation`, atualizar biblioteca, build e testes para `net10.0` e dependências fixas compatíveis.
- [x] 4.2 Fazer os testes consumirem OAuth2Introspection `10.0.0` do staging sem adicionar dependência pública desnecessária de IdentityModel à biblioteca.
- [x] 4.3 Executar build e testes e empacotar `IdentityModel.AspNetCore.AccessTokenValidation` `10.0.0` no staging.

## 5. Validar e publicar a família IdentityModel

As tasks 5.1 a 5.4 registram a validação concluída para os PackageIds históricos antes da decisão de adotar `ZinfoFramework.`. Seus artefatos e sua autorização de publicação não são reutilizáveis; as tasks 5.5 a 5.15 substituem o gate de promoção anterior.

- [x] 5.1 Restaurar, compilar e testar os quatro repositórios com cache isolado, resolvendo os seis IDs internos exclusivamente do staging.
- [x] 5.2 Inspecionar os seis nupkgs, `.nuspec`, `project.assets.json` e grafos transitivos para confirmar versão `10.0.0`, somente `net10.0` e ausência de `Duende.*`.
- [x] 5.3 Compilar ConformanceTests pós-pack e executar manualmente ao menos o perfil Basic RP da suíte atual, registrando a evidência.
- [x] 5.4 Solicitar confirmação explícita do usuário para publicar exatamente os seis nupkgs validados no novo feed NuGet.
- [x] 5.5 Alterar os seis PackageIds para `ZinfoFramework.IdentityModel`, `ZinfoFramework.IdentityModel.AspNetCore.AccessTokenValidation`, `ZinfoFramework.IdentityModel.AspNetCore.OAuth2Introspection`, `ZinfoFramework.IdentityModel.OidcClient`, `ZinfoFramework.IdentityModel.OidcClient.DPoP` e `ZinfoFramework.IdentityModel.OidcClient.IdentityTokenValidator`, preservando namespaces e nomes de assemblies.
- [x] 5.6 Definir nos seis pacotes `Authors=Luiz Antonio`, `RepositoryUrl` igual ao remote Git clonável atual e `PackageProjectUrl` igual à página HTTPS do respectivo repositório `Luizinfo`; atualizar descrições, tags e release notes para identificar `ZinfoFramework` e a continuação independente.
- [x] 5.7 Atualizar dependências internas, testes consumidores, ConformanceTests, staging, `packageSourceMapping`, scripts e exemplos para os novos PackageIds, sem alterar referências públicas `System.IdentityModel.*`.
- [x] 5.8 Atualizar os READMEs raiz e os READMEs incorporados aos seis nupkgs com aviso destacado de continuação por Luiz Antonio, link para `https://github.com/Luizinfo/`, tabela de migração dos IDs históricos, instalação, requisito `net10.0`, breaking changes, suporte e links atuais de projeto/issues.
- [x] 5.9 Preservar integralmente LICENSE, copyrights e avisos existentes; adicionar em cada repositório IdentityModel um documento de atribuição que ligue o fork ao upstream, credite autores e contribuidores originais e deixe claro que `Authors=Luiz Antonio` representa a manutenção atual, não a autoria histórica exclusiva.
- [x] 5.10 Adicionar ou atualizar `CONTRIBUTING.md`, `SECURITY.md`, política de suporte e changelog/release notes usando canais do GitHub atual, sem inventar contatos; incluir os documentos aplicáveis nos nupkgs ou referenciá-los pelos READMEs.
- [x] 5.11 Auditar e corrigir links, badges e alegações desatualizadas nos quatro repositórios: IDs antigos do NuGet, organizações/repositórios anteriores, redirecionamentos para Duende, ReadTheDocs, TFMs legados e certificação OIDC; tratar conteúdo upstream ainda útil e certificações anteriores como histórico claramente atribuído.
- [x] 5.12 Após autorização explícita para registrar as revisões de release, confirmar que cada commit referenciado existe no `RepositoryUrl`, reconstruir os seis nupkgs definitivos `ZinfoFramework.IdentityModel*` `10.0.0` sem recompilar entre validação e promoção, gerar novos SHA-256 e inspecionar PackageId, Authors, Copyright, URLs, repository commit/Source Link, versão, TFM, dependências, LICENSE, atribuição e README renderizável.
- [x] 5.13 Restaurar, compilar e testar novamente os quatro repositórios com cache isolado usando somente os novos IDs internos; repetir a validação pós-pack e o perfil Basic RP da suíte atual da OpenID Foundation, sem declarar certificação do fork.
- [x] 5.14 Reconfirmar no NuGet.org a disponibilidade dos seis PackageIds e solicitar nova confirmação explícita do usuário para publicar exatamente os seis nupkgs renomeados e identificados por seus SHA-256; a autorização concedida aos IDs históricos não se aplica.
- [x] 5.15 Publicar exatamente os seis artefatos aprovados e confirmar que o NuGet.org disponibiliza todas as versões `10.0.0` antes de migrar a CI do IdentityServer4.

## 6. Migrar IdentityServer4

- [x] 6.1 No repositório `C:\Projetos\Identity\IdentityServer4`, atualizar `global.json`, ferramentas e CI para o SDK .NET 10.
- [x] 6.2 Atualizar NuGet e builds para resolver `ZinfoFramework.IdentityModel` e `ZinfoFramework.IdentityModel.*` `10.0.0` exclusivamente do novo feed e dependências públicas do NuGet.org.
- [x] 6.3 Retargetar módulos, hosts, migrações, testes e builds ativos para `net10.0`; usar `net10.0-windows` nos projetos dependentes de Windows.
- [x] 6.4 Alinhar ASP.NET Core, EF Core, Microsoft.Extensions, JWT, EF tools e pacotes de teste a versões fixas compatíveis com .NET 10; a validação inicial usou EF Core `10.0.0` e o ajuste local anterior à primeira publicação fixou as dependências EF Core em `9.0.9`.
- [x] 6.5 Corrigir incompatibilidades de compilação e testes sem alterar fluxos OAuth/OIDC nem o schema existente.
- [x] 6.6 Validar as migrações em banco vazio e confirmar ausência de nova migração pendente.
- [x] 6.7 Configurar os cinco PackageIds `ZinfoFramework.IdentityServer4*` somente com assets `net10.0` e sem renomear namespaces ou assemblies. A validação inicial usou `10.0.0` para todos; antes da primeira publicação, os dois PackageIds EntityFramework passaram a `10.0.1` e os outros três permaneceram em `10.0.0`.
- [x] 6.8 Definir nos cinco pacotes `Authors=Luiz Antonio`, `RepositoryUrl=https://github.com/Luizinfo/IdentityServer4.git`, `PackageProjectUrl=https://github.com/Luizinfo/IdentityServer4` e metadados que identifiquem `ZinfoFramework` e a continuação independente.
- [x] 6.9 Atualizar README e documentação incorporada do IdentityServer4 com aviso de manutenção por Luiz Antonio, link para `https://github.com/Luizinfo/`, tabela de IDs históricos/novos, instalação, migração, suporte, segurança, contribuição, changelog e créditos aos autores/contribuidores originais; preservar LICENSE, copyrights e avisos.

## 7. Atualizar exemplos em escopo

- [x] 7.1 Atualizar quickstarts e exemplos ativos para .NET 10 e para os pacotes `ZinfoFramework.*` `10.0.0` do novo feed. Após o ajuste dos dois pacotes EntityFramework para `10.0.1`, seus consumidores deverão ser revalidados na task 8.4.
- [x] 7.2 Manter AccessTokenValidation e OAuth2Introspection no `ResourceBasedApi`, validando forwarding de reference token e transformação de scopes.
- [x] 7.3 Atualizar `ConsoleCode` e `WindowsConsoleSystemBrowser` para `ZinfoFramework.IdentityModel.OidcClient` `10.0.0`.
- [x] 7.4 Excluir explicitamente `samples/Clients/old`, `samples/KeyManagement` e `MvcAutomaticTokenManagement` dos builds e da CI .NET 10.

## 8. Validar e publicar IdentityServer4

> Atualização de 01/11/2026: a matriz aprovada para a publicação usa Storage, IdentityServer4 e AspNetIdentity `10.0.0`, e os dois pacotes EntityFramework `11.0.0`. As referências a `10.0.1` nas tasks históricas abaixo não definem o conjunto atual de promoção. A auditoria dos arquivos inicialmente escolhidos identificou divergências de código e Source Link; veja [auditoria da promoção](../replace-automapper-with-explicit-csharp/evidence/publication-audit-2026-11-01.md). Nenhuma publicação foi executada nesta etapa.

> Resultado posterior: os cinco pacotes da matriz atual foram publicados e validados no NuGet.org; os seis IdentityModel também foram conferidos. A restauração e o quickstart passaram com cache isolado. A API pública do GitHub retornou zero execuções de CI, portanto o gate da matriz remota permanece pendente. Consulte a [evidência da publicação](../replace-automapper-with-explicit-csharp/evidence/publication-attempt-2026-11-01.md); as tasks históricas abaixo preservam a matriz anterior.

- [x] 8.1 Executar builds e todas as suítes unitárias e de integração do IdentityServer4 com somente o SDK .NET 10.
- [x] 8.2 Compilar quickstarts e exemplos em escopo nas plataformas declaradas, incluindo o cliente Windows no job Windows.
- [x] 8.3 Após autorização explícita para registrar a revisão de release, empacotar e inspecionar os cinco nupkgs `ZinfoFramework.IdentityServer4*` `10.0.0`. A evidência registrada permanece histórica e local; os dois nupkgs EntityFramework `10.0.0` são provisórios e não serão publicados.
- [ ] 8.4 Produzir os dois pacotes EntityFramework `10.0.1` provisórios no staging local; atualizar `samples/Quickstarts/Directory.Build.targets` para o quickstart EF consumir `10.0.1` e dependências EF Core `9.0.9` sem alterar o ASP.NET Core `10.0.0`. Com cache isolado, restaurar, compilar e executar todas as suítes e exemplos em escopo usando as versões da matriz atual; aplicar as migrações a banco vazio, confirmar ausência de migração pendente e inspecionar `project.assets.json` e grafos transitivos.
- [ ] 8.5 Após autorização explícita para registrar e enviar a revisão de release, reconstruir os dois nupkgs EntityFramework `10.0.1` sem recompilar entre validação e promoção. Inspecionar os cinco nupkgs finais quanto a PackageId, versão, SHA-256, Authors, Copyright, URLs, repository commit/Source Link acessível, TFM, dependências, LICENSE, atribuição e README; confirmar que `EntityFramework` depende de `EntityFramework.Storage` `10.0.1` e que os artefatos EF `10.0.0` não estão no conjunto de promoção.
- [ ] 8.6 Reconfirmar no NuGet.org a disponibilidade das cinco combinações exatas de PackageId e versão e solicitar confirmação explícita do usuário para publicar exatamente os três nupkgs IdentityServer4 `10.0.0` e os dois nupkgs EntityFramework `10.0.1` identificados por seus SHA-256.
- [ ] 8.7 Publicar exatamente os cinco artefatos aprovados e confirmar sua disponibilidade no NuGet.org antes da auditoria final.
- [ ] 8.8 Executar a CI com cache limpo contra os pacotes publicados no novo feed, comprovar que exemplos e consumidores resolvem os dois PackageIds EntityFramework `10.0.1` e confirmar que o secret permanece mascarado e ausente dos logs.

## 9. Auditoria final

- [ ] 9.1 Auditar os 11 nupkgs `ZinfoFramework.*` publicados quanto a ID, Authors, RepositoryUrl, PackageProjectUrl, versões da matriz da primeira publicação (nove `10.0.0` e dois EntityFramework `10.0.1`), TFM, dependency groups, documentação, atribuição e ausência de `Duende.*`.
- [ ] 9.2 Consolidar evidências de build, testes, DPoP, conformidade, migrações EF, exemplos, CI, metadados e documentação por repositório.
- [ ] 9.3 Confirmar que nenhuma task foi concluída sem a evidência obrigatória de seu gate.
- [ ] 9.4 Confirmar a titularidade dos 11 pacotes pela conta ou organização de Luiz Antonio no NuGet.org e registrar a solicitação de reserva do prefixo `ZinfoFramework` quando os critérios do NuGet.org forem atendidos.
