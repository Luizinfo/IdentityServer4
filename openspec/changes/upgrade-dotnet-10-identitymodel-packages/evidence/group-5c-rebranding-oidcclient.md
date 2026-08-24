# Evidência do grupo 5C — rebranding de OidcClient

Grupo executado pelo subagent novo `/root/grupo_5c_oidcclient_rebrand`, modelo `gpt-5.6-sol` com raciocínio `low`, e inspecionado centralmente pelo orquestrador em `2026-07-20T19:12:59-03:00`. A única raiz de código autorizada foi `C:\Projetos\Identity\IdentityModel.OidcClient`.

## Resultado

Foram produzidos os PackageIds `ZinfoFramework.IdentityModel.OidcClient`, `ZinfoFramework.IdentityModel.OidcClient.DPoP` e `ZinfoFramework.IdentityModel.OidcClient.IdentityTokenValidator` `10.0.0`, somente `net10.0`. Assemblies, RootNamespaces e APIs `IdentityModel.OidcClient*` foram preservados.

Os três projetos receberam Authors, copyright histórico, URLs atuais e metadados ZinfoFramework. PackageReferences para o pacote-base e referências do ConformanceTests usam os novos IDs. README raiz, três READMEs incorporados, README do runner, NOTICE, CONTRIBUTING, SECURITY, SUPPORT e CHANGELOG foram atualizados; LICENSE foi preservado. Alegações de certificação upstream foram removidas ou qualificadas, e conformidade testada não foi apresentada como certificação.

## Validação automatizada

| Evidência | Resultado |
| --- | --- |
| restore/build Release | sucesso, zero warnings e zero erros |
| OidcClient.Tests | 42/42 aprovados |
| JwtValidationTests | 22/22 aprovados |
| DPoPTests | 6/6 aprovados, incluindo regressão secp256k1 |
| ConformanceTests pós-pack | restore/build e 2/2 testes aprovados em cache isolado |
| isolamento | assets resolveram pacote-base e três pacotes OIDC `ZinfoFramework.*` `10.0.0`; zero ProjectReference no runner |
| encoding/diff | 14/14 textos UTF-8 sem BOM; `git diff --check` sem erros |

## Artefatos finais promovidos ao staging

| Pacote | Bytes | SHA-256 |
| --- | ---: | --- |
| `ZinfoFramework.IdentityModel.OidcClient.10.0.0.nupkg` | 129556 | `2BB013CB14E0F24AE7BB12BF103C5245498982CEF63E0742C6D7897216A41CDB` |
| `ZinfoFramework.IdentityModel.OidcClient.DPoP.10.0.0.nupkg` | 67197 | `4A6034E7AC39EF63E95971A67A827335F8D4E136C94F16CD603C69150C5B13B0` |
| `ZinfoFramework.IdentityModel.OidcClient.IdentityTokenValidator.10.0.0.nupkg` | 44624 | `15B191F24F9E74A0D1EEF9373644F691FB6443CE523A4D005C5BA2DEA77A191B` |

Os três foram copiados de `artifacts\final-packages-787cb34b` para `C:\Projetos\Identity\nuget-packages`. Inspeção central confirmou IDs, versão, Authors, copyright, URLs, único TFM `net10.0`, zero `Duende.*`, README/Licença/NOTICE/CHANGELOG e zero links Markdown relativos nos documentos incorporados. Dependências para `ZinfoFramework.IdentityModel` são exatas em `[10.0.0]`; dependências geradas por ProjectReference para `ZinfoFramework.IdentityModel.OidcClient` usam a versão `10.0.0` convencional do NuGet.

A branch local/remota `release/zinfoframework-10.0.0` e o `repository commit`/SourceLink dos três pacotes apontam para `db022fca2b5e4e16a77b11233593038b8ee9bdce` em `https://github.com/Luizinfo/IdentityModel.OidcClient.git`. A reconstrução final direta e sequencial usou o pacote-base final `787CB34BCE0EA42ED97C32C68ABBF23C4839F084F565A58C43EFE82B3D753A42`, passou com zero warnings/erros e manteve 42/42 + 22/22 + 6/6 testes aprovados. As DLLs extraídas dos nupkgs referenciam o assembly IdentityModel `0.0.0.0`, compatível com o pacote-base final. Um consumidor Conformance em feed/cache novos comprovou os quatro hashes finais e passou 2/2 testes sem ProjectReference.

Os três provisórios `A405A6C3...`, `EE3EFE7A...` e `BC9C1493...` foram substituídos de forma individual no staging. Nenhum pacote foi publicado no NuGet.org e nenhuma credencial foi lida ou registrada.
