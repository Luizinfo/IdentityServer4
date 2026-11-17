# Prontidão para PR — 17/11/2026

## Plano e critérios de aceite

A revisão parte de IdentityServer4 `927d01fe`, IdentityModel `f321e73`, AccessTokenValidation `2a922b8`, OAuth2Introspection `c8d5c17` e OidcClient `4c8753d`. Os quatro pacotes comparam a branch de correção com `origin/main`; IdentityServer4 compara com `origin/master`.

- Corrigir autenticação mTLS em todos os endpoints e exigir CA confiável para X509Name, com testes negativos e positivos.
- Rejeitar URLs de retorno contendo caracteres de controle e preservar retornos locais válidos.
- Corrigir o build oficial, unificar SDK e CI em 10.0.401 e remover SourceLink 8.0.0.
- Executar builds oficiais, todas as suítes, trimming aplicável, exemplos ativos, auditoria transitiva e migrações em bancos vazios.
- Atualizar documentação e registrar o resultado remoto contra os commits enviados.

Nenhuma publicação NuGet está autorizada por esta tarefa. Os nupkgs produzidos são artefatos locais de validação e não substituem os pacotes públicos. A publicação futura requer novas versões, commit final, novos hashes e autorização.

## Alterações e compatibilidade

A configuração mTLS passa a exigir o esquema de autenticação de certificado registrado. X509Name aceita somente certificado emitido por CA confiável, mesmo quando o handler permite autoassinados. X509Thumbprint continua permitindo autoassinados quando aceitos explicitamente pelo handler. A nova propriedade `MutualTls.ClientCertificateChainPolicy` permite configurar CAs privadas; o construtor existente do validator é preservado.

O executor usa as APIs de Bullseye 6 e não ignora falhas de comandos nativos. SourceLink vem do SDK corrigido. As versões públicas dos pacotes, namespaces, assemblies e schema são preservados nesta etapa.

## Resultados locais concluídos

SDK: 10.0.401. Builds oficiais dos cinco repositórios concluídos, incluindo pack local e trimming win-x64 onde aplicável.

| Suíte | Aprovados | Ignorados |
| --- | ---: | ---: |
| IdentityModel | 296 | 0 |
| AccessTokenValidation | 10 | 0 |
| OAuth2Introspection | 42 | 0 |
| OidcClient — unidade/JWT/DPoP | 72 | 0 |
| OidcClient — callbacks pós-pack | 2 | 0 |
| IdentityServer4 — unidade | 735 | 0 |
| IdentityServer4 — integração | 305 | 1 |
| EntityFramework.Storage — unidade | 308 | 0 |
| EntityFramework.Storage — integração | 43 | 0 |
| EntityFramework | 6 | 0 |

Os 23 novos cenários cobrem URLs e mTLS, incluindo CA confiável, autoassinados, handler ausente, certificado expirado e endpoints comum/alias. O único teste ignorado é `Dynamic_lifetime_should_succeed`, preexistente.

Total: **1.819 testes aprovados, 1 ignorado e 0 falhas**. Os avisos de APIs obsoletas e analisadores já existentes permanecem; não foram suprimidos para obter aprovação.

## Validação ampliada concluída

- Os seis quickstarts e a solução de clientes ativos compilaram em Release, incluindo WindowsConsoleSystemBrowser.
- As auditorias com dependências transitivas passaram nos quatro repositórios IdentityModel, nos cinco módulos IdentityServer4 e nas sete soluções de exemplos, sem vulnerabilidades reportadas na matriz restaurada.
- Os exemplos receberam PKCS 10.0.13 após a validação identificar versões 9.0.0/9.0.4 vulneráveis a CVE-2026-70162 e CVE-2026-84008.
- A checagem AspNetIdentity revelou mistura de ferramentas/provedor EF 9 com Identity EF 10. Host e ferramentas foram alinhados ao EF Core 10.0.0. Os pacotes de persistência preservam o mínimo EF Core 9.0.9.
- O script `eng/Validate-Release.ps1` terminou com código 0. Aplicou as migrações de ConfigurationDbContext, PersistedGrantDbContext e ApplicationDbContext em três bancos vazios, com nomes únicos, na instância exclusiva IdentityPrReadiness20261117. Não houve mudanças pendentes no modelo antes/depois, e a segunda aplicação de cada contexto não alterou o banco. Nenhuma migration de produto foi criada.
- Os onze nupkgs foram inspecionados: nove 10.0.0, dois EntityFramework 11.0.0, somente net10.0, README/licença presentes, sem dependências Duende nem ferramentas SourceLink vulneráveis.
- Um consumidor com os onze pacotes foi restaurado em cache isolado, com source mapping exclusivo do staging para ZinfoFramework.*, e passou na compilação/execução dos contratos públicos, mapeamento EF e regressão de URL de retorno. O binário IdentityServer4 do nupkg foi comparado byte a byte ao binário testado.
- Os dois callbacks OIDC passaram contra os nupkgs candidatos usando o mesmo cache isolado.
- Workflows e global.json foram validados; UTF-8 sem BOM, links locais e diff sem erros foram verificados.

Logs, hashes e dependências locais estão em `artifacts/pr-readiness-20261117/`: `extended-validation-final.log`, `package-validation.json` e logs dos builds oficiais. Esses artefatos são candidatos de validação, não uma publicação, e o cache isolado não substitui a inspeção Source Link do commit final antes de uma promoção.

## Estado dos PRs e CI

As branches `fix/pr-readiness-20261117` foram enviadas aos cinco repositórios Luizinfo após autorização explícita. Os quatro PRs IdentityModel têm base main; IdentityServer4 tem base master.

A primeira rodada remota aprovou Linux nos cinco repositórios e Windows nos quatro IdentityModel. O Windows do IdentityServer4 identificou uma falha do script em ambiente limpo: algumas versões de SqlLocalDB retornam zero ao consultar uma instância inexistente. A inicialização agora consulta o inventário de instâncias, cria a instância ausente e confirma sua existência antes de iniciar. O script corrigido passou novamente com a instância inédita IdentityPrReadinessFresh20261117 e os três bancos/contextos. Log: `extended-validation-fresh.log`.

A imagem macOS padrão permaneceu aguardando executor. A alternativa suportada macos-26-intel iniciou e aprovou os três jobs do AccessTokenValidation na execução 44106643715. A matriz dos cinco repositórios foi padronizada nessa imagem Intel; os builds e testes macOS permanecem obrigatórios. A arquitetura ARM64 não foi validada nesta rodada. A aprovação remota integral dos demais repositórios continua pendente.

Resultados oficiais por branch:

- [IdentityServer4](https://github.com/Luizinfo/IdentityServer4/actions?query=branch%3Afix%2Fpr-readiness-20261117)
- [IdentityModel](https://github.com/Luizinfo/IdentityModel/actions?query=branch%3Afix%2Fpr-readiness-20261117)
- [AccessTokenValidation](https://github.com/Luizinfo/IdentityModel.AspNetCore.AccessTokenValidation/actions?query=branch%3Afix%2Fpr-readiness-20261117)
- [OAuth2Introspection](https://github.com/Luizinfo/IdentityModel.AspNetCore.OAuth2Introspection/actions?query=branch%3Afix%2Fpr-readiness-20261117)
- [OidcClient](https://github.com/Luizinfo/IdentityModel.OidcClient/actions?query=branch%3Afix%2Fpr-readiness-20261117)


Os gates históricos de publicação em tasks.md não são autorização para promover estes candidatos. Uma nova versão de segurança precisa de versões novas e validação dos artefatos finais antes de publicação.


## Referências de segurança

- [mTLS: GHSA-mv65-jhg4-3qh6](https://github.com/DuendeSoftware/products/security/advisories/GHSA-mv65-jhg4-3qh6).
- [Redirecionamento: GHSA-55p7-v223-x366](https://github.com/advisories/GHSA-55p7-v223-x366).
- [SourceLink: CVE-2026-62900](https://github.com/advisories/GHSA-23fw-v26w-5fgq).
