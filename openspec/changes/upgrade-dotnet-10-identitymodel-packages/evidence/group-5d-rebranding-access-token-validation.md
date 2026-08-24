# Evidência do grupo 5D — rebranding de AccessTokenValidation

Grupo executado pelo subagent novo `/root/grupo_5d_access_token_rebrand`, modelo `gpt-5.6-sol` com raciocínio `low`, e inspecionado centralmente pelo orquestrador em `2026-07-20T19:18:16-03:00`. A única raiz de código autorizada foi `C:\Projetos\Identity\IdentityModel.AspNetCore.AccessTokenValidation`.

## Resultado

- PackageId `ZinfoFramework.IdentityModel.AspNetCore.AccessTokenValidation`, versão `10.0.0`, somente `net10.0`.
- assembly, namespaces e APIs históricas preservados.
- pacote da biblioteca continua sem dependência pública desnecessária de IdentityModel ou OAuth2Introspection.
- testes migrados para `ZinfoFramework.IdentityModel.AspNetCore.OAuth2Introspection` `[10.0.0]` do staging.
- Authors, copyright histórico, URLs atuais, descrição, tags e release notes atualizados.
- README refeito e novos NOTICE, CHANGELOG, CONTRIBUTING, SECURITY e SUPPORT; LICENSE preservado.

## Validação

| Evidência | Resultado |
| --- | --- |
| hashes de pré-requisito | pacote-base e OAuth2Introspection conferidos exatamente |
| restore | staging + NuGet.org aprovado; configuração temporária removida |
| build Release | zero warnings e zero erros |
| testes | baseline 10/10 e final 10/10 aprovados |
| cobertura final | linhas 71,42%; branches 94,44%; sem refatoração de produção |
| encoding/diff | UTF-8 sem BOM; `git diff --check` sem erro |

## Artefato final promovido ao staging

- origem: `C:\Projetos\Identity\IdentityModel.AspNetCore.AccessTokenValidation\artifacts\ZinfoFramework.IdentityModel.AspNetCore.AccessTokenValidation.10.0.0.nupkg`;
- staging: `C:\Projetos\Identity\nuget-packages\ZinfoFramework.IdentityModel.AspNetCore.AccessTokenValidation.10.0.0.nupkg`;
- tamanho: `13657` bytes;
- SHA-256: `125FB9BD86BDB543FED1D142D49005F5F3E58768FE378F275C98F92D6CF98BAD`;
- branch local/remota: `release/zinfoframework-10.0.0`;
- `repository commit`: `2a922b83a405ebd3f552971970b12ddd2537b850`;
- SourceLink: `https://raw.githubusercontent.com/Luizinfo/IdentityModel.AspNetCore.AccessTokenValidation/2a922b83a405ebd3f552971970b12ddd2537b850/*`.

Inspeção central confirmou ID, versão, Authors, copyright `Copyright Dominick Baier & Brock Allen`, URLs atuais, assets somente `net10.0`, framework reference `Microsoft.AspNetCore.App`, dependency group vazio, zero `Duende.*`, quatro documentos incorporados e zero links Markdown relativos no README/NOTICE.

O commit de release foi confirmado no remote `https://github.com/Luizinfo/IdentityModel.AspNetCore.AccessTokenValidation.git`. A reconstrução final usou cache isolado novo e comprovou os hashes finais de IdentityModel `787CB34B...` e OAuth2Introspection `4B416941...`; build Release terminou com zero warnings/erros e 10/10 testes aprovados. O staging substituiu somente o provisório `CD72F4E871DA9F421CE4B6C14177A27F066B2C5E889CE124DEE0429E1351D5D4`. Nenhum pacote foi publicado no NuGet.org e nenhuma credencial foi lida ou registrada.
