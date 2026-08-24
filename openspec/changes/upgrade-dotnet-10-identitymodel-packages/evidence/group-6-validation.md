# Grupo 6 — validação IdentityServer4

Data: 2026-07-21

## SDK, restore e build

- SDK efetivo: `10.0.109`; `global.json` fixa `10.0.100` com `latestPatch`.
- Cache isolado: `.artifacts/nuget-cache`; NuGet.org como origem de `ZinfoFramework.IdentityModel*` e dependências públicas; feed local `./nuget` para encadear os cinco pacotes IdentityServer4 ainda não publicados.
- Os 22 projetos ativos em `src/` usam `net10.0`; nenhum `netcoreapp3.1`/`netstandard2.0` permanece nessa árvore.
- Storage: build Release, 0 avisos, 0 erros.
- IdentityServer4: biblioteca, host e dois projetos de testes compilam. Após clean/restore eliminar o adapter xUnit 3.1.5 residual, o runner efetivo é xUnit VSTest Adapter 2.8.2. UnitTests: 724/724. IntegrationTests: 293 aprovados, 1 ignorado (`Dynamic_lifetime_should_succeed` já marcado Skip), 0 falhas, total 294 em 1m04s. TRX finais: `.artifacts/test-results/unit-final-after-token-json.trx` e `.artifacts/test-results/integration-final-serial.trx`.
- EntityFramework.Storage: build, 0 erros; UnitTests 16/16 e IntegrationTests 40/40. Após atualizar AutoMapper/SQLitePCLRaw, rebuild passou com 5 avisos (`ASPDEPR008`/`EF1001`) e nenhum `NU1903`.
- EntityFramework: build, 0 erros; testes 6/6.
- AspNetIdentity: build de biblioteca, host e migração, 0 erros e 8 avisos (`ASP0019`/`ASPDEPR008`).

## Compatibilidade corrigida

- `ZinfoFramework.IdentityModel` 10.0.0 substitui o ID histórico sem alterar namespaces.
- Respostas de teste foram adaptadas de Newtonsoft `JToken` para `JsonElement?`; parâmetros extras usam `Parameters.FromObject`.
- Fixture JWK passou a usar JSON estrito, sem vírgula final.
- AutoMapper 16.2.0 usa `NullLoggerFactory` e validação não genérica dos profiles.
- SQLitePCLRaw.lib.e_sqlite3 3.53.3 foi fixado diretamente nos consumidores SQLite para remover `NU1903`.
- O pipeline de integração usa Data Protection efêmero, eliminando disputa por arquivos de chave entre hosts de teste.
- Client assertions usam `ClientCredentialStyle.PostBody`, exigido pelo IdentityModel 10 quando há `ClientAssertion`.
- `JwtRequestValidator` preserva objetos/arrays de request objects entregues como `JsonElement` pelo JWT 8.
- `TokenExtensions` materializa JSON claims em dicionários/arrays CLR antes de entregá-los ao `JwtPayload`, preservando objetos como `cnf` no JWT 8.

## Migrações

- `dotnet ef migrations has-pending-model-changes` retornou `No changes have been made to the model since the last migration.` para `ConfigurationDbContext` e `PersistedGrantDbContext`.
- As migrações existentes `20200522172542_Config` e `20200522172538_Grants` foram aplicadas com sucesso ao banco LocalDB vazio e isolado `IdentityServer4_Group6_Validation`.
- Nenhuma migração nova foi criada e nenhum arquivo de schema foi alterado.

## Estado do gate

Tasks 6.1–6.9 possuem evidência e estão concluídas. Nenhuma falha foi mascarada; o único teste ignorado já possuía `Skip` no código existente.
