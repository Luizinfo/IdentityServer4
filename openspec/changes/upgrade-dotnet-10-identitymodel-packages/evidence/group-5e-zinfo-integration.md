# Evidência do grupo 5E — integração da família ZinfoFramework.IdentityModel

> A seção **Revalidação pós-nonce (2026-07-21)** ao final é a evidência vigente e substitui os hashes, contagens e gate externo pendente registrados para a rodada anterior.

Grupo executado pelo subagent novo `/root/grupo_5e_integracao_zinfo`, modelo `gpt-5.6-sol` com raciocínio `low`, e verificado pelo orquestrador em `2026-07-20T20:58:22-03:00`. A única raiz de configuração editada foi `staging`; os outros repositórios foram usados apenas para restore/build/test e outputs gerados.

## Staging e origem

- `NuGet.Config` mapeia exatamente os 11 IDs finais `ZinfoFramework.IdentityModel*` e `ZinfoFramework.IdentityServer4*` ao `local-staging`; zero ID histórico permanece mapeado como interno.
- NuGet.org permanece como origem das dependências públicas.
- `Initialize-Staging.ps1` usa caches novos `artifacts/group-5-zinfo-*` sem apagar caches anteriores.
- os seis hashes finais da task 5.12 conferiram no feed e nos seis nupkgs do cache global novo `group-5-zinfo-final-global-packages`.
- restores `--force --no-cache` usaram caches HTTP, global e de plugins novos.
- seis `.nupkg.metadata` registraram exclusivamente `C:\Projetos\Identity\nuget-packages`.
- consumidor sintético resolveu os seis IDs `10.0.0`, compilou com zero warnings/erros e não possui ProjectReference.
- ConformanceTests resolveu os quatro pacotes OIDC diretos e o fechamento transitivo pós-pack, com zero ProjectReference.

Os três arquivos de staging alterados foram validados como UTF-8 sem BOM. `openspec validate --changes --strict --no-interactive` retornou um change aprovado e zero falhas.

## Matriz integrada

| Suite | Resultado |
| --- | ---: |
| IdentityModel | 296/296 |
| OAuth2Introspection | 42/42 |
| ConsumerCompileTest | build e comando de teste exit 0 |
| OidcClient.Tests | 42/42 |
| JwtValidationTests | 22/22 |
| DPoPTests | 6/6 |
| AccessTokenValidation | 10/10 |
| ConformanceTests pós-pack | 2/2 |
| total executável | 420, zero falhas |

Todos os builds Release finais — quatro repositórios, ConformanceTests e consumidor sintético — concluíram com zero warnings e zero erros. A publicação de TrimmableAnalysis de IdentityModel e OidcClient passou sem warning, erro ou lock.

## Auditoria dos seis nupkgs

Confirmados:

- IDs `ZinfoFramework.IdentityModel*`, versão `10.0.0`, `Authors=Luiz Antonio`;
- copyrights históricos e URLs dos repositórios `Luizinfo`;
- somente assets/dependency groups `net10.0`;
- dependências internas somente sob IDs `ZinfoFramework.*`;
- zero dependência ou entrada `Duende.*`;
- assemblies e namespaces históricos preservados;
- README, LICENSE, NOTICE e CHANGELOG presentes;
- zero link Markdown relativo nos documentos incorporados;
- zero PackageReference operacional aos seis IDs históricos nos projetos ativos;
- zero TFM antigo nas soluções ativas. O `netcoreapp3.1` localizado em `samples/HttpClientFactorySample` está fora da solução ativa.

Alegações de certificação foram qualificadas como história upstream ou evidência de conformidade testada, nunca como certificação atual do fork.

## Gate externo pendente

O usuário autorizou explicitamente nesta task o uso de `https://www.certification.openid.net/` e informou que a página estava aberta e autenticada no navegador do Codex. O subagent leu integralmente a skill de navegador, não encontrou connector/API/CLI específico da OIDF e tentou selecionar explicitamente o backend interno `iab`, conforme solicitado.

A seleção retornou `Browser is not available: iab`; o diagnóstico listou somente a extensão do Chrome. Como a autorização especificava o navegador interno do Codex e a skill proíbe substituí-lo silenciosamente, Chrome não foi usado. Consequentemente:

- nenhum novo plan ID ou execution ID foi criado;
- nenhum runner ou serviço local foi iniciado;
- nenhuma aba, token, cookie ou processo ficou pendente;
- o Basic RP depende da reativação do backend `iab` nesta sessão.

As tasks 5.5 a 5.12 estavam comprovadas ao fim da primeira rodada. A evidência vigente da task 5.13 está registrada abaixo. Nenhuma publicação ou leitura de credencial ocorreu nesta etapa.

## Revalidação pós-nonce (2026-07-21)

O commit público `4c8753d7fc19452e27efaa86edcdcc6f987717be` corrigiu a ausência de nonce no OidcClient. O nonce passa a ser gerado e persistido no `AuthorizeState`, enviado no fluxo normal e PAR, protegido contra override e comparado ordinalmente antes de qualquer chamada ao UserInfo.

Os seis pacotes `10.0.0` vigentes no staging foram conferidos byte a byte:

| PackageId | SHA-256 final |
| --- | --- |
| `ZinfoFramework.IdentityModel` | `787CB34BCE0EA42ED97C32C68ABBF23C4839F084F565A58C43EFE82B3D753A42` |
| `ZinfoFramework.IdentityModel.AspNetCore.OAuth2Introspection` | `4B416941FFED7A5654CE6ABCB0C9FE78AC31CB6289A7F936F69CBC267F6B019D` |
| `ZinfoFramework.IdentityModel.AspNetCore.AccessTokenValidation` | `125FB9BD86BDB543FED1D142D49005F5F3E58768FE378F275C98F92D6CF98BAD` |
| `ZinfoFramework.IdentityModel.OidcClient` | `57FACE2895BEDD7BCD8D6364F9815DB26FD03C7354EFCCCAB088C650498C6CF6` |
| `ZinfoFramework.IdentityModel.OidcClient.DPoP` | `66C3DE9822EFBAEB99E2CEDD3E7952D83298AE4895E3146646E9EC2E4052EF65` |
| `ZinfoFramework.IdentityModel.OidcClient.IdentityTokenValidator` | `8FDDA927699D5887C6C400A1FA37CA1E4C93C7B478F758D09F81DB968366D35D` |

Os três artefatos OIDC estão congelados em `C:\Projetos\Identity\IdentityModel.OidcClient\artifacts\postnonce-4c8753d7\nupkgs`: somente `lib/net10.0`, `Authors=Luiz Antonio`, URLs `Luizinfo`, Source Link/repository commit `4c8753d7fc19452e27efaa86edcdcc6f987717be`, dependências internas `ZinfoFramework.*` `10.0.0`, README/LICENSE/NOTICE/CHANGELOG/créditos upstream e zero dependência ou metadado `Duende.*`.

Um cache isolado `group-5-postnonce-20260721` confirmou, por `project.assets.json` e `.nupkg.metadata`, os seis IDs resolvidos como pacotes exclusivamente de `C:\Projetos\Identity\nuget-packages`, sem `ProjectReference` ou `Duende.*`. O consumidor pós-pack e o runner Basic RP foram recompilados contra os hashes novos com zero `ProjectReference`, zero warnings e zero erros. As suítes integradas passaram 420/420: IdentityModel 296, OAuth2Introspection 42, OidcClient 72 e AccessTokenValidation 10. Trimming `win-x64` de IdentityModel e OidcClient passou sem warnings. OAuth2Introspection mantém apenas dois warnings preexistentes `ASPDEPR004`/`ASPDEPR008` no utilitário de testes `test/Tests/Util/PipelineFactory.cs:23`; a biblioteca compila sem warnings.

O plano privado OIDF `9fYmlNohmInNu` (suite 5.2.1, Basic RP, `dynamic_client`, `plain_http_request`) foi repetido integralmente pelo runner pós-pack. Todas as execuções mais recentes foram `FINISHED/PASSED`:

| Cenário | Execution ID |
| --- | --- |
| `oidcc-client-test` | `crNcqlhljSYLBrO` |
| `oidcc-client-test-invalid-iss` | `GZNTZiVfHbt2dgS` |
| `oidcc-client-test-missing-sub` | `xpf1YaVfSG8OiQo` |
| `oidcc-client-test-invalid-aud` | `rlUika00y7iMIDO` |
| `oidcc-client-test-missing-iat` | `q4iUeZX0ChMj3ra` |
| `oidcc-client-test-kid-absent-single-jwks` | `HVK4Tg1v2pylTWK` |
| `oidcc-client-test-kid-absent-multiple-jwks` | `B0G9WyYzerwO468` |
| `oidcc-client-test-idtoken-sig-rs256` | `5LKVcRH9ClvY36V` |
| `oidcc-client-test-idtoken-sig-none` | `sZ0Pfl9Vd5aT0JD` |
| `oidcc-client-test-invalid-sig-rs256` | `nj4jXf5aFCidRhq` |
| `oidcc-client-test-userinfo-invalid-sub` | `EljljFjbGU6zqn2` |
| `oidcc-client-test-nonce-invalid` | `1Qjv0jZPEkvL9ZA` |
| `oidcc-client-test-scope-userinfo-claims` | `TJwn084ODoTNtVh` |
| `oidcc-client-test-client-secret-basic` | `gRlHa8rDAMUW01K` |

Em particular, `oidcc-client-test-nonce-invalid` passou e confirma que o token inválido é rejeitado antes do UserInfo. Esta é evidência de conformidade testada, não uma certificação oficial do fork, pacote ou mantenedor.

## Promoção NuGet.org (2026-07-21)

Antes da promoção, os seis endpoints flat-container retornaram `404`, confirmando a disponibilidade dos IDs naquele instante. O usuário autorizou explicitamente a publicação vinculada aos seis SHA-256 acima. Cada `dotnet nuget push` retornou `Created`; não houve falha nem repetição. Após a propagação do catálogo, os seis endpoints `index.json` confirmaram a versão `10.0.0` como disponível publicamente. As tasks 5.5 a 5.15 estão comprovadas.
