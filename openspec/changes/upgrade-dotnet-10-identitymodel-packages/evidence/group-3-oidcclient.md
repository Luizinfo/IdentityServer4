# Evidência do grupo 3 — OidcClient

Grupo executado por subagent exclusivo em `C:\Projetos\Identity\IdentityModel.OidcClient` e validado centralmente pelo orquestrador em `2026-07-20T10:53:31.9539788-03:00`.

## Escopo .NET 10 e dependências

Foram inventariados 11 projetos: três bibliotecas empacotáveis, quatro projetos de teste/análise e quatro clientes. Todos os ativos usam `net10.0`, exceto `clients/ManualModeConsoleClient`, que usa `net10.0-windows` por depender de P/Invoke Windows. `clients/ConformanceTests` permanece fora da solution principal para ser restaurado somente pós-pack.

O SDK está fixado em `10.0.301`, com roll-forward desabilitado. A CI usa `actions/checkout@v7` e `actions/setup-dotnet@v6`; sign, secrets e push automático legado foram removidos para preservar o gate manual. O entrypoint oficial aceita `OIDCCLIENT_NUGET_CONFIG`, restaura com source mapping e produz versões determinísticas por `MinVerVersionOverride=10.0.0`.

As dependências IdentityModel foram fixadas em `[10.0.0]`; JWT, Microsoft.Extensions, SourceLink, TestHost e dependências de teste usam versões fixas compatíveis. Projetos, assets restaurados e nupkgs não contêm PackageId, PackageReference ou ProjectReference iniciado por `Duende.`.

## Build, testes e DPoP

| Evidência | Resultado |
| --- | --- |
| `dotnet --version` | exit 0; `10.0.301` |
| `.\build.ps1 pack` com `OIDCCLIENT_NUGET_CONFIG` | exit 0 |
| build oficial | 0 warnings; 0 errors; inclui três bibliotecas, testes, clientes ativos e cliente Windows |
| `dotnet test IdentityModel.OidcClient.sln --no-restore --no-build -c Release` | exit 0 |
| OidcClient.Tests | 42/42 aprovados |
| JwtValidationTests | 20/20 aprovados |
| DPoPTests | 6/6 aprovados |
| total da solution | 68 aprovados; 0 falhas; 0 ignorados |
| TrimmableAnalysis `win-x64` | publish concluído no build oficial completo |
| `git diff --check` | exit 0 |

`DPoPTests` não referencia mais `Duende.IdentityServer` nem IdentityServer4. O antigo host foi substituído internamente por um ASP.NET Core `TestServer` mínimo de `client_credentials`: valida proof, `htm`, `htu`, `iat`, replay e nonce; emite JWT assinado com `cnf.jkt` e `token_type=DPoP`; o `ApiHost` recebe issuer, audience e chave diretamente, valida `ath` e compara a chave do proof ao `cnf.jkt` do access token.

Os quatro cenários originais passaram: proof no token endpoint, nonce/retry no token endpoint, proof na API e nonce/retry na API. Também passaram os negativos de chave divergente e `ath` divergente, ambos com resposta não autorizada.

## Pacotes

| PackageId | Tamanho | SHA-256 | Assets/dependências principais |
| --- | ---: | --- | --- |
| `IdentityModel.OidcClient` | 124760 bytes | `520331E3448EF3EE50D4421C1A23BB6BD2295EE77140D53619B394CAF224D400` | somente `lib/net10.0`; IdentityModel `[10.0.0]` |
| `IdentityModel.OidcClient.DPoP` | 61868 bytes | `FD759B4D070306C2DE1C47E576098FD83C69665009A83C64C4E5D07DAD8293BA` | somente `lib/net10.0`; OidcClient `10.0.0`, IdentityModel `[10.0.0]` |
| `IdentityModel.OidcClient.IdentityTokenValidator` | 39397 bytes | `52F818DA6ACFD493788FF10B4AF42829CB385841051004E63273F04F72A629EE` | somente `lib/net10.0`; OidcClient `10.0.0` |

Os três arquivos estão em `C:\Projetos\Identity\nuget-packages`, possuem versão `10.0.0`, somente dependency group `net10.0`, zero `Duende.*`, e preservam licença Apache-2.0, READMEs, ícones e atribuições legais.

## ConformanceTests pós-pack

`clients/ConformanceTests` usa `net10.0` e PackageReferences `[10.0.0]` para `IdentityModel`, `IdentityModel.OidcClient` e `IdentityModel.OidcClient.IdentityTokenValidator`, com `ProjectReferenceCount=0`.

O orquestrador criou um cache novo inexistente (`.packages/orchestrator-conformance`) e executou restore com `staging/NuGet.Config`, `--force` e `--no-cache`. As três `.nupkg.metadata` registram source `C:\Projetos\Identity\nuget-packages`. O build pós-pack passou com 0 warnings/0 errors, e os testes automatizados de callback query e `form_post` passaram 2/2.

O runner aceita `discoveryurl`, perfil Basic, redirect URI loopback e URL/ID da execução; usa `JwtHandlerIdentityTokenValidator`, suporta registro dinâmico opcional e não imprime valores de tokens. O procedimento foi revisado inicialmente contra `release-v5.1.45` de 2026-06-17 e está documentado em `clients/ConformanceTests/README.md` com links oficiais da OpenID Foundation; o gate externo posterior foi executado na suíte `release-v5.2.1`.

Todos os 40 arquivos modificados/novos foram verificados como UTF-8 sem BOM. Nenhuma publicação remota foi feita e nenhuma credencial foi lida ou exibida.

## Reabertura após execução externa OIDF

Em 2026-07-20, a primeira execução externa contra a suíte OIDF `5.2.1` alcançou o callback do módulo `oidcc-client-test`, mas o runner pós-pack encerrou com `Unsupported curve type of secp256k1`. A falha ocorre durante a construção das chaves do JWKS no `JwtHandlerIdentityTokenValidator` e invalida temporariamente as evidências de 3.6 a 3.9. Essas tarefas foram reabertas até correção, teste de regressão, novo pack e repetição do gate externo, sem reduzir a validação de assinatura.

### Correção e aceite externo

O JWKS real mistura chaves RSA/P-256 válidas, `secp256k1`, OKP e chaves `use=enc`. O validator passou a descartar chaves não destinadas a assinatura e curvas EC não suportadas como candidatas, com log, preservando a rejeição quando nenhuma chave suportada corresponde ao token. Foram adicionadas regressões positiva e negativa; JWT passou 22/22, OidcClient 42/42 e DPoP 6/6, total 70/70. O build completo teve zero erros; os únicos nove avisos `NU1900` vieram da consulta SSL de auditoria do NuGet.

Os três pacotes foram gerados novamente em `10.0.0`; a tabela acima contém os hashes finais. ConformanceTests restaurou os três IDs do feed local em cache novo, compilou com zero avisos/erros e aprovou 2/2 callbacks, mantendo zero ProjectReference.

Uma repetição na execução original foi corretamente rejeitada pela OIDF porque aquela instância já preservava autenticação de cliente da tentativa anterior. Uma instância limpa do mesmo plano/módulo foi então criada. A execução `k6jzKdyt3YATZkg` (`https://www.certification.openid.net/log-detail.html?log=k6jzKdyt3YATZkg`) terminou `FINISHED / PASSED`, com 53 condições de sucesso, zero failure, zero warning e exit code 0 no runner. Tokens, códigos e claims não foram persistidos nesta evidência.
