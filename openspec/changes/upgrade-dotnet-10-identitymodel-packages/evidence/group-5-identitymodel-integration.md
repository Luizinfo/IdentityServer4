# Evidência do grupo 5 — validação integrada da família IdentityModel

Grupo executado por subagent exclusivo e validado centralmente pelo orquestrador em `2026-07-20T13:42:21.0376780-03:00`. Esta etapa cobre integralmente 5.1 e 5.2 e somente a parcela local automatizável de 5.3. Não houve publicação remota, uso de segredo ou avanço para o IdentityServer4.

## Staging e cache isolado

- Feed local: `C:\Projetos\Identity\nuget-packages`.
- Configuração: `C:\Projetos\Identity\IdentityServer4\openspec\changes\upgrade-dotnet-10-identitymodel-packages\staging\NuGet.Config`.
- Cache novo: `C:\Projetos\Identity\IdentityServer4\openspec\changes\upgrade-dotnet-10-identitymodel-packages\staging\artifacts\group-5-global-packages`.
- Restore de cada repositório: `--configfile <staging> --packages <cache> --force --no-cache`.
- Build e testes posteriores: `--no-restore`.
- Ordem validada: IdentityModel → OAuth2Introspection/OidcClient → AccessTokenValidation.

Os quatro repositórios foram executados pelos entrypoints oficiais. As seis entradas `.nupkg.metadata` internas apontam para `C:\Projetos\Identity\nuget-packages`. A checagem central encontrou 18 `project.assets.json` gerados com o cache do grupo 5 e zero ocorrência de `Duende.*`.

## Build e testes integrados

| Repositório | Build | Testes | Resultado |
| --- | --- | ---: | --- |
| IdentityModel | 0 avisos, 0 erros | 296/296 | exit 0 |
| OAuth2Introspection | 2 avisos ASPDEPR, 0 erros | 42/42 | exit 0; ConsumerCompileTest compilado |
| OidcClient | 0 avisos, 0 erros | 68/68 | exit 0; 42 OidcClient, 20 JWT e 6 DPoP |
| AccessTokenValidation | 0 avisos, 0 erros | 10/10 | exit 0 |

Total: 416 testes aprovados, zero falhas.

## Consumidor global e auditoria dos pacotes

Foi criado somente nos artefatos `_audit/AllPackagesConsumer.csproj`, com referências exatas `[10.0.0]` aos seis IDs, e `_audit/global.json`, fixando o SDK `10.0.301`. A primeira execução fora de um repositório selecionou o SDK 3.1; a correção permaneceu restrita aos artefatos. O restore e o build finais passaram com 0 avisos e 0 erros.

A validação central repetiu `dotnet build --no-restore`, inspecionou `project.assets.json` e executou `dotnet list package --include-transitive --no-restore`: os seis pacotes foram resolvidos exatamente em `10.0.0`, o target foi `net10.0` e o grafo não contém `Duende.*`. Uma tentativa de `dotnet list` sem `--no-restore` tentou restaurar novamente e falhou; a repetição correta com `--no-restore` passou.

| PackageId | Bytes | SHA-256 |
| --- | ---: | --- |
| `IdentityModel` | 173826 | `6F10EF774043A8B090EAF19D47797CD1006DDAA3BAE049B23F0529D00BE4EA64` |
| `IdentityModel.AspNetCore.OAuth2Introspection` | 58291 | `65AAC4781D4DDF2128F50F100235AFDA1CB6D90853C0A37E45EC3097AE53985D` |
| `IdentityModel.OidcClient` | 124760 | `520331E3448EF3EE50D4421C1A23BB6BD2295EE77140D53619B394CAF224D400` |
| `IdentityModel.OidcClient.DPoP` | 61868 | `FD759B4D070306C2DE1C47E576098FD83C69665009A83C64C4E5D07DAD8293BA` |
| `IdentityModel.OidcClient.IdentityTokenValidator` | 39397 | `52F818DA6ACFD493788FF10B4AF42829CB385841051004E63273F04F72A629EE` |
| `IdentityModel.AspNetCore.AccessTokenValidation` | 11831 | `811C052744B797FDA448B0DADB88D42154A17ECA2A33E3A568BF44F22A463D60` |

Cada nupkg tem PackageId/versão corretos e DLL somente em `lib/net10.0`. A inspeção central de ZIP e `.nuspec` encontrou zero entrada, assembly ou dependência `Duende.*`; os hashes coincidem com as validações isoladas dos grupos 1 a 4.

## ConformanceTests pós-pack e bloqueio de 5.3

`clients/ConformanceTests/ConformanceTests.csproj` usa apenas PackageReference exata a IdentityModel, OidcClient e IdentityTokenValidator `10.0.0`; há zero ProjectReference. Restore e build pós-pack passaram em `net10.0`, e a repetição central dos callbacks query e `form_post` aprovou 2/2 testes. O comando `--help` confirmou `--discoveryurl`, `--profile basic`, `--redirect-uri`, `--execution-id` e os argumentos opcionais de cliente.

A tarefa 5.3 permanece aberta: o perfil Basic RP externo da OIDF exige sessão autenticada, plano/teste ativo, `discoveryurl`, URL ou ID de execução e interação no navegador. Nenhuma execução externa foi alegada.

Na tentativa central de verificar a disponibilidade do portal, o navegador bloqueou especificamente a navegação para `https://www.certification.openid.net/` por uma restrição de segurança associada à sessão. A aba foi encerrada e não houve tentativa de contorno por outro navegador, URL alternativa ou automação indireta. É necessária autorização explícita do usuário para retomar o acesso, além da sessão/plano e dos dados de execução da OIDF.

Após autorização do usuário, foi criado um plano privado Basic RP na suíte `5.2.1`, ID `dNFkcFpYGBtx7`, variante `dynamic_client` + `plain_http_request`. A execução do módulo positivo `oidcc-client-test`, ID `cejrWXjbZePiurN`, exportou o discovery URL e chegou ao callback loopback, mas o runner pós-pack falhou com `Unsupported curve type of secp256k1`. Não foi registrada aprovação falsa. Como a correção afetará o pacote IdentityTokenValidator, 5.1 e 5.2 foram reabertas junto com 3.6–3.9; 5.3 permanece aberta até nova execução externa aprovada.

A correção segura foi aplicada e validada com duas regressões. A execução original não pôde ser reutilizada porque mantinha autenticação de cliente da primeira tentativa; a repetição limpa `k6jzKdyt3YATZkg`, no mesmo plano Basic RP, terminou `FINISHED / PASSED` na OIDF `5.2.1`. O runner concluiu o fluxo com exit 0, e a página da OIDF registrou 53 success, zero failure e zero warning.

## Revalidação final pós-OIDF

O grupo foi repetido com cache completamente novo em `staging/artifacts/group-5-global-packages-post-oidf`. Todos os restores usaram `--configfile`, `--packages`, `--force` e `--no-cache`; build e testes posteriores usaram `--no-restore`.

| Evidência final | Resultado |
| --- | --- |
| quatro repositórios | 418/418 testes: 296 IdentityModel, 42 OAuth2Introspection, 70 OidcClient e 10 AccessTokenValidation |
| ConformanceTests pós-pack | build 0 avisos/0 erros; callbacks 2/2; zero ProjectReference |
| consumidor sintético SDK 10.0.301 | seis referências `[10.0.0]`; restore/build/run aprovados; todos os IDs resolvidos em `10.0.0` |
| origens internas | seis `.nupkg.metadata` com source `C:\Projetos\Identity\nuget-packages` |
| assets e projetos | 28 `project.assets.json` e todos os csproj com zero `Duende.*` |
| nupkgs e nuspecs | somente `net10.0`; zero `Duende.*`; hashes finais conforme tabela |

Com essa repetição, 5.1, 5.2 e 5.3 foram aceitas. Não houve publicação remota nem persistência de segredo, token, código de autorização ou claims.

## Gate de publicação

Em 2026-07-20, o usuário autorizou explicitamente publicar exatamente os seis pacotes IdentityModel `10.0.0` validados no NuGet.org, usando o service index `https://api.nuget.org/v3/index.json` e a variável protegida `NUGET_KEY`. O valor da API key não foi registrado. Essa autorização conclui 5.4; 5.5 permanece aberta até a confirmação remota das seis versões.

O executor de publicação confirmou novamente a existência e os SHA-256 exatos dos seis arquivos, mas `NUGET_KEY` estava ausente ou vazia no ambiente. A checagem central também confirmou ausência nos escopos Process, User e Machine, sem exibir qualquer valor. Nenhum `dotnet nuget push` foi executado e nenhum pacote remoto foi alterado. A publicação permanece bloqueada até a variável estar disponível ao processo do Codex.

Uma segunda tentativa foi solicitada pelo usuário e executada com um processo novo. `NUGET_KEY` continuou ausente nos três escopos; a execução parou antes de chamar `dotnet nuget push`. Nenhum pacote ou estado remoto foi alterado.

Em `2026-07-20T18:15:29-03:00`, uma nova tentativa carregou `NUGET_KEY` exclusivamente em memória a partir de `C:\Projetos\Identity\project.env`, sem registrar seu valor. Antes do envio, os seis arquivos em `C:\Projetos\Identity\nuget-packages` foram novamente conferidos contra os SHA-256 aprovados. O primeiro comando, para `IdentityModel.10.0.0.nupkg`, alcançou `https://www.nuget.org/api/v2/package`, mas o NuGet.org respondeu `403 Forbidden`: a chave é inválida, expirou ou não possui permissão sobre o ID existente. A execução parou imediatamente; nenhum dos seis pacotes foi publicado e 5.5 permanece aberta. A estrutura local da variável foi validada como uma única entrada não vazia, sem aspas, espaços ou placeholder; a correção necessária está na validade, no escopo de push e/ou na titularidade do pacote pela conta da chave.
