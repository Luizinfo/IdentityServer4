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

## Task 8.3 — pack e inspeção final

- Autorização explícita recebida para registrar e enviar a revisão de release.
- Commits de release enviados para `origin/master`: `630704a4` (preparação), `be8fe888` (copyright) e `547d51db` (LICENSE). O último commit foi confirmado em `refs/heads/master` e um arquivo do Source Link retornou HTTP `200`.
- Os cinco pacotes foram reconstruídos em ordem de dependência contra feed e cache isolados em `artifacts/release-10.0.0`, sem publicação.

| Pacote | SHA-256 |
| --- | --- |
| `ZinfoFramework.IdentityServer4` | `70677B20B8DB019D213839AB1DBE618C349CBD1B45E72609AC11B2981E31F1B5` |
| `ZinfoFramework.IdentityServer4.AspNetIdentity` | `09BCA54CD92BF00F1C6699EC5FEE925AE7461D083524B07AE545D4796694B491` |
| `ZinfoFramework.IdentityServer4.EntityFramework` | `3640A51663F885412F13D41E0A0532B7A55BE7D1F0F704BE14E4D5F5C91C7BE2` |
| `ZinfoFramework.IdentityServer4.EntityFramework.Storage` | `292FCCD58CD773B0E65F01F61A2BE169D654F726473EFB80A8141DC3849DAE34` |
| `ZinfoFramework.IdentityServer4.Storage` | `420010BFD7C0542156B072FA6AAABA5AD06D0BD4816428EDAA5A3027F353C2EC` |

Todos os `.nuspec` confirmam `Version=10.0.0`, `Authors=Luiz Antonio`, copyright de Luiz Antonio e dos contribuidores de IdentityServer4, URLs do repositório Luizinfo, `Apache-2.0`, `net10.0`, dependências esperadas, e `repository commit=547d51dba7b9b39ea5c54a13526dad1655aa682d`. Cada pacote contém `LICENSE`, `README.md`, `ATTRIBUTION.md` e PDB com Source Link `https://raw.githubusercontent.com/Luizinfo/IdentityServer4/547d51dba7b9b39ea5c54a13526dad1655aa682d/*` acessível.

O pacote `EntityFramework.Storage` declara `AutoMapper 16.2.0`; o teste em feed/cache novo confirmou que o pacote `EntityFramework` resolve essa versão, sem `NU1903`.

## Próximos gates

As tasks 8.4 a 8.6 permanecem pendentes. Nenhum pacote foi publicado nesta validação.
