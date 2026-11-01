# Auditoria de promoção — 01/11/2026

O usuário autorizou a publicação dos cinco pacotes após validação dos artefatos escolhidos. A auditoria interrompeu a promoção antes de qualquer envio porque os arquivos não satisfazem o gate de correspondência com o código validado e com o commit de origem.

## Artefatos escolhidos

Origem: `C:\Projetos\Identity\IdentityServer4\nuget`.

| Pacote | Versão | SHA-256 |
| --- | --- | --- |
| ZinfoFramework.IdentityServer4.Storage | 10.0.0 | `38d287a0192c259fac2de4da12b92dba52d447eb015f860d28a4f040c5f5fe83` |
| ZinfoFramework.IdentityServer4 | 10.0.0 | `ca5eb30ee3a9032f3940bcf3bded29db0d509d73a9a9173a23af27aa4e3f6248` |
| ZinfoFramework.IdentityServer4.AspNetIdentity | 10.0.0 | `11ec7827eef06cdf678fe96005b7ed34a9479fe1e3b75acd5a0924991e39596f` |
| ZinfoFramework.IdentityServer4.EntityFramework.Storage | 11.0.0 | `38d7f3df28237debd2278cd7d155e76cd970008a6a4f225df65a89cec323fbf3` |
| ZinfoFramework.IdentityServer4.EntityFramework | 11.0.0 | `e4b055969ddf495f7ebd3eca83638ce4167b9fea15433bc1b06972de0d6a030f` |

Os cinco têm assets net10.0, metadados de autor e repositório esperados e dependências na ordem aprovada. EntityFramework 11.0.0 referencia EntityFramework.Storage 11.0.0. Os dois pacotes EF incluem LICENSE; os três pacotes 10.0.0 têm licença declarada por expressão e incluem README e ATTRIBUTION, mas não o arquivo LICENSE que foi acrescentado na revisão de release posterior.

## Gates que falharam

1. Os três pacotes 10.0.0 foram produzidos a partir do commit `5bcc2abbcf6c47200c4eb3b77e756f1bc0e04358`. A revisão de release posterior incluiu alterações em `TokenExtensions.cs` e `JwtRequestValidator.cs` para serialização de claims e tratamento de `JsonElement`, além de metadados e LICENSE. Essas mudanças não constam no pacote principal escolhido. O commit 5bcc está acessível no repositório remoto, mas não representa a revisão atual validada.
2. Os dois pacotes 11.0.0 declaram o commit `e4465e932355642a695dd6db84a9e8951012a9d3`, que é acessível remotamente, mas as alterações de substituição do AutoMapper ainda estavam no working tree e não constam desse commit. Os PDBs incorporam fontes, mas isso não torna a referência de commit/Source Link representativa do código compilado.
3. O consumidor pós-pack anterior usou exatamente os arquivos escolhidos para Storage, IdentityServer4 e os dois EF, conforme os SHA-512 de `project.assets.json`; ele não referencia AspNetIdentity. Isso comprova quais pacotes foram consumidos, mas não resolve as divergências de origem acima.

## Publicação e credenciais

As cinco combinações de ID e versão consultadas em 01/11/2026 retornaram 404 no flat-container público do NuGet.org. Os seis IdentityModel 10.0.0 existentes tiveram conteúdo interno comparado aos arquivos locais: os arquivos comuns são idênticos e os pacotes públicos acrescentam `.signature.p7s`; não é necessário republicá-los por essa diferença.

Há uma entrada não vazia `NUGET_KEY` em `C:\Projetos\Identity\project.env`, sem leitura ou registro do valor nesta auditoria. Disponibilidade da entrada não comprova validade, titularidade ou permissão para publicar os cinco IDs. Nenhuma chamada de push NuGet foi executada e nenhum pacote foi publicado.

## Próximos passos

- Registrar em Git a revisão exata da implementação e garantir que o commit esteja acessível no repositório remoto. Preservar e revisar as alterações locais preexistentes, sem incluir mudanças alheias inadvertidamente.
- Gerar um novo conjunto dos três pacotes 10.0.0 a partir da revisão atualizada e os dois 11.0.0 a partir da revisão que contém o mapeamento explícito. Não substituir os nupkgs escolhidos silenciosamente; usar um novo diretório de promoção.
- Revalidar metadados, Source Link, hashes e os cinco pacotes juntos com cache isolado, incluindo AspNetIdentity. Registrar as novas evidências.
- Somente depois dos gates, verificar a chave em memória durante a tentativa de publicação, sem registrar o valor, e publicar em ordem de dependência. Interromper no primeiro erro e registrar qualquer publicação parcial.

## Pré-validação de candidatos alternativos

Foi preparado `artifacts/publication-preflight-20261101/feed`, sem sobrescrever `nuget`, com os três artefatos de `artifacts/release-10.0.0` e os dois EF 11.0.0 escolhidos. Um consumidor com cache inicialmente vazio restaurou os cinco, incluindo AspNetIdentity, diretamente desse feed; todos os SHA-512 do `project.assets.json` conferem com os cinco nupkgs e não há AutoMapper no grafo. As conversões e a atualização de grant com SQLite passaram.

Os SHA-256 dos candidatos alternativos para Storage, IdentityServer4 e AspNetIdentity são, respectivamente, `420010bfd7c0542156b072fa6aaaba5ad06d0bd4816428edaa5a3027f353c2ec`, `70677b20b8db019d213839ab1dbe618c349cbd1b45e72609ac11b2981e31f1b5` e `09bca54cd92bf00f1c6699ec5fee925ae7461d083524b07ae545d4796694b491`. Estes candidatos são somente evidência de pré-validação: os dois EF ainda precisam ser reconstruídos a partir de um commit público que contenha suas alterações, e o conjunto final precisa ser novamente validado.
