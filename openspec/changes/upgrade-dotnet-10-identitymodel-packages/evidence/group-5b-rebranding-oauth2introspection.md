# Evidência do grupo 5B — rebranding de OAuth2Introspection

Grupo executado pelo subagent novo `/root/grupo_5b_oauth2_rebrand`, modelo `gpt-5.6-sol` com raciocínio `low`, e inspecionado centralmente pelo orquestrador em `2026-07-20T19:08:14-03:00`. A única raiz de código autorizada foi `C:\Projetos\Identity\IdentityModel.AspNetCore.OAuth2Introspection`.

## Resultado

- PackageId `ZinfoFramework.IdentityModel.AspNetCore.OAuth2Introspection`, versão `10.0.0`, somente `net10.0`.
- assembly, RootNamespace e namespaces públicos históricos preservados.
- dependência interna substituída por `ZinfoFramework.IdentityModel` na faixa exata `[10.0.0]`.
- `Authors=Luiz Antonio`, copyright histórico, URLs `Luizinfo`, descrição, tags e release notes atuais.
- README incorporado documenta continuação, instalação, migração e APIs públicas afetadas.
- novos `NOTICE.md`, `CONTRIBUTING.md`, `SECURITY.md`, `SUPPORT.md` e `CHANGELOG.md`; LICENSE sem alteração.

## Validação

| Evidência | Resultado |
| --- | --- |
| restore | três projetos restaurados com staging local e NuGet.org; configuração temporária removida |
| build Release | sucesso, zero erros; dois warnings preexistentes `ASPDEPR004` e `ASPDEPR008` |
| testes | 42/42 aprovados |
| ConsumerCompileTest | compilado e comando de teste retornou zero; não possui casos executáveis |
| pack | sucesso |
| encoding/diff | UTF-8 sem BOM; `git diff --check` sem erros |

## Artefato final promovido ao staging

- origem: `C:\Projetos\Identity\IdentityModel.AspNetCore.OAuth2Introspection\artifacts\ZinfoFramework.IdentityModel.AspNetCore.OAuth2Introspection.10.0.0.nupkg`;
- staging: `C:\Projetos\Identity\nuget-packages\ZinfoFramework.IdentityModel.AspNetCore.OAuth2Introspection.10.0.0.nupkg`;
- tamanho: `65190` bytes;
- SHA-256: `4B416941FFED7A5654CE6ABCB0C9FE78AC31CB6289A7F936F69CBC267F6B019D`;
- branch local/remota: `release/zinfoframework-10.0.0`;
- `repository commit`: `c8d5c1734c336e7d3d7e99d67516435a44a1f0f8`;
- SourceLink: `https://raw.githubusercontent.com/Luizinfo/IdentityModel.AspNetCore.OAuth2Introspection/c8d5c1734c336e7d3d7e99d67516435a44a1f0f8/*`.

O `.nuspec` confirma Authors, copyright `Copyright Dominick Baier & Brock Allen`, URLs atuais, `net10.0`, dependência exata no pacote-base novo e zero `Duende.*`. O ZIP contém README, LICENSE, NOTICE e CHANGELOG; os documentos incorporados verificados não possuem links Markdown relativos.

O commit de release foi confirmado no remote `https://github.com/Luizinfo/IdentityModel.AspNetCore.OAuth2Introspection.git`. A reconstrução final usou cache NuGet novo e comprovou nele o pacote-base final pelo SHA-256 `787CB34BCE0EA42ED97C32C68ABBF23C4839F084F565A58C43EFE82B3D753A42`. Build Release terminou com zero erros e os dois warnings conhecidos `ASPDEPR004`/`ASPDEPR008`; 42/42 testes e o consumidor de compilação passaram. O staging substituiu somente o provisório `356E4E587918B46C9F19C619D96E5D54050C5B9BE698F3F66E9AFD3B5C0AD8E2`. Nenhum pacote foi publicado no NuGet.org e nenhuma credencial foi lida ou registrada.
