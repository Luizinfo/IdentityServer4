## Context

O trabalho abrange cinco repositórios locais. `IdentityModel` já possui alterações locais para .NET 10, mas passa a fazer parte integral deste change para validação, build, testes e empacotamento, em vez de ser tratado como pré-condição externa. Os outros três repositórios IdentityModel e o IdentityServer4 ainda possuem TFMs, SDKs e dependências legadas.

O repositório já possui `./nuget`, mas o script Unix apaga essa pasta antes do build. A CI atual faz checkout somente do IdentityServer4 e não recebe artefatos dos demais repositórios. Portanto, a validação precisa de um staging local preservado, e a CI somente poderá migrar depois que os pacotes aprovados forem publicados no novo feed NuGet.

Os 11 PackageIds históricos já existem no NuGet.org sob responsabilidade dos mantenedores originais. A tentativa de publicar `IdentityModel.10.0.0.nupkg` com uma chave válida em formato chegou ao NuGet.org e recebeu `403 Forbidden`, porque uma continuação independente não pode publicar nova versão sob um ID de outro proprietário. O prefixo próprio `ZinfoFramework.` diferencia os forks mantidos por Luiz Antonio sem apagar a proveniência dos projetos Apache-2.0 originais.

Em 2026-07-20, os endpoints flat-container dos 11 PackageIds `ZinfoFramework.*` retornaram `404`, indicando que nenhum deles estava publicado naquele momento. Essa consulta não reserva os IDs; a disponibilidade deverá ser repetida imediatamente antes de cada promoção.

## Goals / Non-Goals

**Goals:**

- Compilar, testar e empacotar os componentes ativos dos cinco repositórios com .NET 10.
- Produzir exatamente 11 nupkgs na versão `10.0.0`, somente com assets `net10.0` e PackageIds prefixados por `ZinfoFramework.`.
- Publicar metadados NuGet que identifiquem Luiz Antonio como mantenedor atual e apontem para os repositórios `Luizinfo` correspondentes.
- Documentar a continuação independente, a migração de PackageIds e os créditos aos autores e projetos originais.
- Remover toda `PackageReference`, `ProjectReference`, assembly resolvida ou dependência transitiva cujo ID comece com `Duende.`.
- Preservar os fluxos OAuth/OIDC existentes, inclusive DPoP e os testes de conformidade aplicáveis.
- Validar localmente os nupkgs antes da promoção manual para o novo feed NuGet.
- Fazer a CI consumir os IDs internos do novo feed e as dependências públicas do NuGet.org.

**Non-Goals:**

- Adicionar novas funcionalidades de protocolo ao IdentityServer4 ou redesenhar APIs além das quebras inevitáveis da substituição de dependências.
- Recriar migrações EF existentes ou alterar o schema persistido.
- Migrar `samples/Clients/old`, `samples/KeyManagement` ou `MvcAutomaticTokenManagement`.
- Executar automaticamente na CI a suíte externa e interativa de conformidade da OpenID Foundation.
- Remover avisos de licença ou copyright existentes; o critério “sem Duende” aplica-se às dependências resolvidas, não à atribuição legal preservada no fork.
- Renomear namespaces públicos ou assemblies apenas para acompanhar o novo PackageId; essa quebra adicional não é necessária para diferenciar os pacotes no NuGet.org.

## Decisions

### Change coordenador multi-repositório

Este change é a fonte de verdade única para cinco áreas. Cada grupo de tasks pertence a exatamente um repositório e deve ser executado por um novo subagent com uma única raiz de edição explicitamente autorizada. O orquestrador registra as evidências e só libera o grupo dependente após build, testes e pack do grupo anterior.

O OpenSpec 1.4.1 não permite aplicar integralmente um workspace change: em `workspace-planning`, `allowedEditRoots` permanece vazio e a aplicação exige selecionar uma área. Por isso, a propriedade multi-repositório é implementada pela coordenação e pelos gates deste change; ele não deve ser convertido para `workspace-planning` enquanto essa limitação existir.

### Runtime único

Bibliotecas, builds, testes e projetos ativos devem usar `net10.0`. Projetos dependentes de APIs Windows devem usar `net10.0-windows`. Pacotes públicos não manterão assets `netstandard2.0`, `netcoreapp*`, `net6.0`, `net8.0` ou `net9.0`.

### Matriz de pacotes 10.0.0

| Repositório | PackageId | Versão |
| --- | --- | --- |
| IdentityModel | `ZinfoFramework.IdentityModel` | `10.0.0` |
| AccessTokenValidation | `ZinfoFramework.IdentityModel.AspNetCore.AccessTokenValidation` | `10.0.0` |
| OAuth2Introspection | `ZinfoFramework.IdentityModel.AspNetCore.OAuth2Introspection` | `10.0.0` |
| OidcClient | `ZinfoFramework.IdentityModel.OidcClient` | `10.0.0` |
| OidcClient | `ZinfoFramework.IdentityModel.OidcClient.DPoP` | `10.0.0` |
| OidcClient | `ZinfoFramework.IdentityModel.OidcClient.IdentityTokenValidator` | `10.0.0` |
| IdentityServer4 | `ZinfoFramework.IdentityServer4` | `10.0.0` |
| IdentityServer4 | `ZinfoFramework.IdentityServer4.Storage` | `10.0.0` |
| IdentityServer4 | `ZinfoFramework.IdentityServer4.EntityFramework.Storage` | `10.0.0` |
| IdentityServer4 | `ZinfoFramework.IdentityServer4.EntityFramework` | `10.0.0` |
| IdentityServer4 | `ZinfoFramework.IdentityServer4.AspNetIdentity` | `10.0.0` |

A versão de validação deve ser definida deterministicamente no build, sem depender de tags MinVer ausentes ou divergentes.

### Identidade do fork e metadados NuGet

O prefixo `ZinfoFramework.` altera somente a identidade de distribuição. Namespaces e `AssemblyName` históricos serão preservados para que a migração do consumidor se limite, sempre que possível, à troca da `PackageReference`. Todas as dependências internas, testes pós-pack, exemplos, `packageSourceMapping` e documentação usarão os novos PackageIds; referências a `System.IdentityModel.*` não serão alteradas.

Os 11 pacotes usarão `Authors=Luiz Antonio`. `RepositoryUrl` será a URL Git clonável do remote `origin` correspondente, com sufixo `.git`, e `PackageProjectUrl` será a página HTTPS do mesmo repositório, sem o sufixo. As descrições e tags devem identificar `ZinfoFramework` e a manutenção independente, sem sugerir endosso dos mantenedores originais.

O pacote final não poderá apontar `repository commit` ou Source Link para uma revisão que não contenha o código empacotado. Antes do pack definitivo, as mudanças deverão estar registradas em commits acessíveis nos respectivos `RepositoryUrl`; commit e push dependem de autorização explícita do usuário. Artefatos provisórios produzidos antes desse gate podem alimentar o staging, mas seus hashes não poderão ser promovidos.

| Repositório | RepositoryUrl | PackageProjectUrl |
| --- | --- | --- |
| IdentityModel | `https://github.com/Luizinfo/IdentityModel.git` | `https://github.com/Luizinfo/IdentityModel` |
| AccessTokenValidation | `https://github.com/Luizinfo/IdentityModel.AspNetCore.AccessTokenValidation.git` | `https://github.com/Luizinfo/IdentityModel.AspNetCore.AccessTokenValidation` |
| OAuth2Introspection | `https://github.com/Luizinfo/IdentityModel.AspNetCore.OAuth2Introspection.git` | `https://github.com/Luizinfo/IdentityModel.AspNetCore.OAuth2Introspection` |
| OidcClient | `https://github.com/Luizinfo/IdentityModel.OidcClient.git` | `https://github.com/Luizinfo/IdentityModel.OidcClient` |
| IdentityServer4 | `https://github.com/Luizinfo/IdentityServer4.git` | `https://github.com/Luizinfo/IdentityServer4` |

Cada repositório terá aviso destacado no README e um documento de atribuição incluído no pacote quando aplicável. Eles identificarão Luiz Antonio e `https://github.com/Luizinfo/` como manutenção atual, ligarão o fork ao repositório upstream, preservarão a licença Apache-2.0 e os copyrights existentes e creditarão os autores e contribuidores originais. A documentação não substituirá créditos históricos por `Authors`; os dois papéis serão explicados separadamente.

Os READMEs raiz e os READMEs incorporados aos nupkgs deverão conter: tabela de IDs históricos e novos; instalação; requisitos .NET 10; migração; links atuais para código, issues e licença; política de suporte e segurança; e indicação clara de que se trata de continuação independente. Links para pacotes antigos, repositórios antigos, ReadTheDocs, TFMs legados e alegações de certificação OIDC serão removidos ou qualificados como históricos. Aprovação em execução de conformidade não será descrita como certificação do fork.

### Ordem de produção e publicação

1. Validar e empacotar `ZinfoFramework.IdentityModel` `10.0.0` no staging local.
2. Produzir OAuth2Introspection e OidcClient contra esse staging.
3. Produzir AccessTokenValidation contra OAuth2Introspection `10.0.0`.
4. Validar os seis pacotes `ZinfoFramework.IdentityModel*`, seus metadados, documentação incorporada e árvores de dependência.
5. O usuário promove os seis pacotes aprovados para o novo feed NuGet.
6. Atualizar e validar o IdentityServer4 e seus exemplos consumindo os pacotes publicados.
7. Produzir e validar os cinco pacotes IdentityServer4 `10.0.0`.
8. O usuário promove os cinco pacotes IdentityServer4 aprovados para o novo feed.

O NuGet.org permanece disponível para dependências públicas. `packageSourceMapping` deve restringir os IDs internos ao staging durante a validação local e ao novo feed na CI. Credenciais nunca serão gravadas no repositório.

### DPoPTests sem Duende.IdentityServer

`DPoPTests` permanecerá em `net10.0`. O `IdentityServerHost` será substituído por um test double local baseado em ASP.NET Core `TestServer`, limitado ao fluxo `client_credentials` necessário aos testes. O host validará o proof DPoP, responderá ao desafio `DPoP-Nonce` e emitirá JWT assinado com `cnf.jkt` e `token_type=DPoP`.

O resource server receberá issuer, audience e chave de teste diretamente, sem discovery, `Duende.IdentityServer` ou referência ao IdentityServer4. Os quatro fluxos atuais serão preservados e haverá cenário negativo para chave ou `ath` divergente.

### ConformanceTests permanece como validação pós-pack

`clients/ConformanceTests` será migrado para `net10.0`, mas permanecerá fora do build pré-pack para evitar que `ProjectReference` mascare problemas dos pacotes. Ele deverá consumir `IdentityModel`, `IdentityModel.OidcClient` e `IdentityModel.OidcClient.IdentityTokenValidator` `10.0.0` do staging ou do novo feed.

O runner deixará de usar as URLs do sistema Python aposentado e receberá os dados de execução da suíte atual da OpenID Foundation por configuração. A CI fará restore e build do runner; a execução contra o serviço externo permanecerá manual e registrará a URL ou ID da execução como evidência.

### ResourceBasedApi mantém os dois pacotes

`ResourceBasedApi` usa `Selector.ForwardReferenceToken` e `AddScopeTransformation` de AccessTokenValidation, além de OAuth2Introspection. Ambos serão mantidos e atualizados para `10.0.0`.

### Exclusões explícitas

`MvcAutomaticTokenManagement` permanece fora do escopo porque não existe repositório local informado para `IdentityModel.AspNetCore`. `samples/Clients/old` e `samples/KeyManagement` também permanecem fora dos builds e da CI .NET 10.

## Risks / Trade-offs

- [OpenSpec não aplica múltiplas raízes] → executar um grupo por vez, com novo subagent e raiz explicitamente autorizada.
- [Feed novo indisponível ou sem credencial] → manter o staging local como validação prévia e bloquear CI/publicação até o usuário confirmar o feed.
- [Seleção de pacote público ou cache antigo] → usar source mapping, cache isolado e inspeção de `project.assets.json` e `.nuspec`.
- [Quebra binária em OAuth2Introspection] → publicar `10.0.0` e adicionar teste de compilação de consumidor.
- [Regressão DPoP no test double] → validar assinatura, nonce, `cnf.jkt`, `ath`, `htm`, `htu`, replay e chave divergente.
- [Conformance depende de serviço externo] → compilar deterministicamente na CI e manter execução externa como gate manual documentado.
- [Mudanças de ASP.NET Core ou EF Core 10] → executar suítes completas e validar aplicação das migrações existentes sem gerar novo schema.
- [Confusão entre fork e projeto original] → usar prefixo próprio, aviso de continuação independente, links atuais e atribuição explícita em README e documento de notice.
- [Perda de créditos ou violação de avisos] → preservar LICENSE, copyrights e avisos existentes; adicionar manutenção atual sem reescrever a autoria histórica.
- [Documentação ou selo de certificação desatualizado] → auditar READMEs incorporados, badges e links; tratar certificações anteriores apenas como histórico até eventual certificação própria.
- [Hashes e aprovações anteriores inválidos] → reconstruir os seis pacotes após o rebranding, gerar novos hashes, repetir testes/conformidade e obter nova autorização explícita antes do push.
- [Outro publicador ocupa um novo ID antes da promoção] → repetir a consulta de disponibilidade imediatamente antes do push, interromper em qualquer colisão e não escolher outro prefixo sem decisão do usuário.
- [Metadado de repositório aponta para commit anterior às mudanças] → distinguir pacotes provisórios do pack final, registrar as revisões de release com autorização e verificar que `repository commit`/Source Link correspondem ao conteúdo publicado.

## Migration Plan

1. Autorizar as cinco raízes e registrar branch/commit de cada repositório.
2. Aplicar os PackageIds `ZinfoFramework.IdentityModel*`, metadados, documentação e atribuição nos quatro repositórios IdentityModel.
3. Reconstruir os seis nupkgs no staging e repetir a validação local, inclusive DPoPTests e ConformanceTests pós-pack.
4. Após nova aprovação do usuário, publicar esses seis pacotes no novo feed NuGet.
5. Migrar IdentityServer4, exemplos e CI consumindo os novos IDs do feed.
6. Aplicar os PackageIds `ZinfoFramework.IdentityServer4*`, metadados, documentação e atribuição no repositório IdentityServer4.
7. Validar e empacotar os cinco pacotes IdentityServer4 `10.0.0`.
8. Após aprovação do usuário, publicar os cinco pacotes IdentityServer4.
9. Executar a auditoria final dos 11 nupkgs, builds, testes, metadados, documentação e árvores resolvidas.

Rollback: interromper a promoção ou voltar os consumidores para as versões anteriores. Pacotes `10.0.0` já publicados são imutáveis e não devem ser sobrescritos ou apagados; qualquer correção posterior deve receber nova versão.

## Open Questions

Nenhuma. O feed é o NuGet.org, o endpoint V3 é `https://api.nuget.org/v3/index.json` e a credencial protegida permanece referenciada somente como `NUGET_KEY`.
