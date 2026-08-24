# Grupo 8 — validação de release do IdentityServer4

Data: 2026-08-18

## Ambiente e restore

- SDK efetivo no repositório: .NET SDK `10.0.302`, selecionado pelo `global.json` (`10.0.100` com `latestPatch`).
- Os cinco componentes foram restaurados com `--force --no-cache` em cache isolado `.nuget-cache-group8`, usando `NuGet.config` e os pacotes provisórios locais `ZinfoFramework.IdentityServer4*` somente para a família ainda não publicada.
- Após o restore completo, os assets resolveram `AutoMapper` `16.2.0` e `SQLitePCLRaw.lib.e_sqlite3` `3.53.3`, sem alertas `NU1903` nos componentes de `src/`.

## Task 8.1 — builds e testes

Todos os builds Release dos cinco componentes concluíram sem erros:

| Componente | Resultado |
| --- | --- |
| Storage | 0 avisos, 0 erros |
| IdentityServer4 | 0 avisos, 0 erros |
| EntityFramework.Storage | 5 avisos conhecidos (`ASPDEPR008` e `EF1001`), 0 erros |
| EntityFramework | 8 avisos conhecidos (`ASPDEPR008` e `ASP0019`), 0 erros |
| AspNetIdentity | 8 avisos conhecidos (`ASPDEPR008` e `ASP0019`), 0 erros |

As suítes foram executadas diretamente no contexto de usuário do Windows; esse detalhe é necessário para o carregamento dos certificados PFX de teste. Resultado final:

| Suíte | Resultado |
| --- | --- |
| `IdentityServer.UnitTests` | 724/724 aprovados |
| `IdentityServer.IntegrationTests` | 293 aprovados, 1 ignorado já existente (`Dynamic_lifetime_should_succeed`) |
| `IdentityServer4.EntityFramework.UnitTests` | 16/16 aprovados |
| `IdentityServer4.EntityFramework.IntegrationTests` | 40/40 aprovados |
| `IdentityServer4.EntityFramework.Tests` | 6/6 aprovados |
| **Total** | **1.079 aprovados, 1 ignorado, 0 falhas** |

## Task 8.2 — exemplos em escopo

- `samples/Clients/Clients.sln` compilou os 23 projetos ativos, incluindo `WindowsConsoleSystemBrowser` com `net10.0-windows`, sem erros.
- Os seis quickstarts ativos (`1_ClientCredentials` a `6_AspNetIdentity`) compilaram em Release sem erros.
- `samples/Clients/old`, `samples/KeyManagement` e `MvcAutomaticTokenManagement` permaneceram excluídos, conforme o escopo.
- O quickstart `6_AspNetIdentity` recebeu referência direta a `SQLitePCLRaw.lib.e_sqlite3` `3.53.3`, eliminando o alerta `NU1903` transitivo da versão `2.1.11`; seu build final apresentou somente seis avisos de API obsoleta, sem erros.

## Próximos gates

As tasks 8.3 a 8.6 permanecem pendentes. A task 8.3 requer autorização explícita para registrar a revisão de release antes de produzir os cinco nupkgs definitivos. Nenhum pacote foi empacotado ou publicado nesta validação.
