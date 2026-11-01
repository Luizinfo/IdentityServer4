# Tentativa de publicação — 01/11/2026

O usuário autorizou registrar e enviar a revisão de release e continuar a publicação. O commit `e578a6d606648224fa51e9bce7927931dca1064d` foi criado e enviado para `origin/master`; as alterações locais preexistentes do outro change foram preservadas.

Os três pacotes 10.0.0 da revisão de release `547d51dba7b9b39ea5c54a13526dad1655aa682d` foram copiados para um novo feed isolado. Os dois pacotes EF 11.0.0 foram reconstruídos a partir do commit `e578a6d606648224fa51e9bce7927931dca1064d`, com fontes incorporadas ao PDB. Seus nuspecs apontam para o novo commit público. Os cinco contêm README, LICENSE e ATTRIBUTION, são net10.0 e não dependem de AutoMapper.

| Pacote | Versão | SHA-256 |
| --- | --- | --- |
| ZinfoFramework.IdentityServer4.Storage | 10.0.0 | `420010bfd7c0542156b072fa6aaaba5ad06d0bd4816428edaa5a3027f353c2ec` |
| ZinfoFramework.IdentityServer4 | 10.0.0 | `70677b20b8db019d213839ab1dbe618c349cbd1b45e72609ac11b2981e31f1b5` |
| ZinfoFramework.IdentityServer4.AspNetIdentity | 10.0.0 | `09bca54cd92bf00f1c6699ec5fee925ae7461d083524b07ae545d4796694b491` |
| ZinfoFramework.IdentityServer4.EntityFramework.Storage | 11.0.0 | `d52176109fa1d32bd968b42aa68a958164ab9763bbd1808349750bed3c3e5aad` |
| ZinfoFramework.IdentityServer4.EntityFramework | 11.0.0 | `b6989aa6eea606c2e004ec518ef21cabc394cd8edf142aedec1f4a5dae85c815` |

Os arquivos estão em `C:\Projetos\Identity\IdentityServer4\artifacts\publication-final-20261101\feed`. O consumidor independente restaurou os cinco em cache vazio; os hashes SHA-512 dos assets coincidem com os nupkgs, não há AutoMapper no grafo, e as conversões e o teste de atualização de grant com SQLite passaram. As suítes EF passaram: 308 testes unitários, 43 de integração e 6 de EntityFramework. Os logs estão em `artifacts/publication-final-20261101/logs`.

O primeiro push, de `ZinfoFramework.IdentityServer4.Storage` 10.0.0, foi recusado pelo NuGet.org com HTTP 403: a chave é inválida, expirou ou não tem permissão para o pacote. A execução parou imediatamente, antes dos demais envios. Uma nova consulta ao flat-container dos cinco IDs retornou 404 para todos; nenhum dos cinco foi publicado. O valor da credencial não foi registrado.

Para retomar, disponibilizar em `C:\Projetos\Identity\project.env` uma entrada `NUGET_KEY` válida com permissão para criar/publicar esses cinco IDs na conta apropriada. Sem imprimir a chave, conferir novamente hashes, disponibilidade dos IDs e dependências, publicar na ordem aprovada e validar o consumo público e a CI. Não reconstruir nem alterar os artefatos após a validação sem repetir os gates.

## Nova tentativa após atualização da chave

O usuário informou que a chave havia sido atualizada. O hash de Storage 10.0.0 e a disponibilidade de IdentityModel 10.0.0 foram novamente conferidos. O NuGet.org respondeu HTTP 403 informando especificamente que a chave utilizada **expirou** e orientando criar uma nova chave. A sequência foi interrompida imediatamente. Uma consulta posterior aos cinco IDs retornou 404 para todos, confirmando que nenhum dos cinco foi publicado. A credencial foi lida exclusivamente em memória e não foi registrada.

## Retomada

Após solicitação de continuação, o envio de `ZinfoFramework.IdentityServer4.Storage` 10.0.0, SHA-256 `420010bfd7c0542156b072fa6aaaba5ad06d0bd4816428edaa5a3027f353c2ec`, retornou **HTTP 201 Created**. O NuGet informou que a chave expira em oito dias. A disponibilização do pacote no flat-container e na página pública ainda está pendente; os quatro pacotes dependentes não foram enviados. Não repetir o push do Storage enquanto seu processamento estiver pendente.

Atualização: Storage 10.0.0 ficou disponível no flat-container. O envio de `ZinfoFramework.IdentityServer4` 10.0.0, SHA-256 `70677b20b8db019d213839ab1dbe618c349cbd1b45e72609ac11b2981e31f1b5`, também retornou HTTP 201 Created. Aguardar a disponibilidade pública antes de enviar AspNetIdentity e EntityFramework. Não repetir o push já aceito.

Atualização seguinte: IdentityServer4 10.0.0 ficou disponível no flat-container e o envio de `ZinfoFramework.IdentityServer4.AspNetIdentity` 10.0.0, SHA-256 `09bca54cd92bf00f1c6699ec5fee925ae7461d083524b07ae545d4796694b491`, retornou HTTP 201 Created. Aguardar a propagação antes do próximo pacote; não repetir o push aceito.

AspNetIdentity 10.0.0 ficou disponível. O envio de `ZinfoFramework.IdentityServer4.EntityFramework.Storage` 11.0.0, SHA-256 `d52176109fa1d32bd968b42aa68a958164ab9763bbd1808349750bed3c3e5aad`, retornou HTTP 201 Created. Aguardar sua propagação antes do envio do EntityFramework; não repetir o push aceito. Os conteúdos públicos de Storage 10.0.0 e IdentityServer4 10.0.0 foram comparados aos nupkgs locais: os arquivos comuns são idênticos e a assinatura NuGet foi acrescentada.

EntityFramework.Storage 11.0.0 ficou disponível. O envio do último pacote, `ZinfoFramework.IdentityServer4.EntityFramework` 11.0.0, SHA-256 `b6989aa6eea606c2e004ec518ef21cabc394cd8edf142aedec1f4a5dae85c815`, retornou HTTP 201 Created. Aguardar sua propagação antes da validação pós-publicação; não repetir o push aceito.

## Confirmação pós-publicação

Os cinco pacotes IdentityServer4 ficaram disponíveis no flat-container do NuGet.org: os três pacotes básicos em 10.0.0 e os dois EntityFramework em 11.0.0. Os seis IdentityModel 10.0.0 também continuam disponíveis. A comparação dos 11 nupkgs públicos com os artefatos locais confirmou conteúdo idêntico nos arquivos comuns e somente a adição de `.signature.p7s` nos pacotes públicos.

Um consumidor independente restaurou os cinco pacotes exclusivamente do NuGet.org, em cache inicialmente vazio, e executou os cinco grupos de conversão e a atualização de grant com SQLite. O grafo restaurado contém as versões esperadas e não contém AutoMapper. O quickstart 5_EntityFramework também foi restaurado exclusivamente do NuGet.org e compilou em Release. Permanecem os avisos previamente registrados NU1902 e NU1903 de `System.Security.Cryptography.Pkcs` 9.0.4 no quickstart; eles não foram corrigidos nesta publicação.

O arquivo de origem do Source Link no commit `e578a6d606648224fa51e9bce7927931dca1064d` retornou HTTP 200. A API pública de GitHub Actions lista o workflow CI como ativo, mas retornou zero execuções para esse commit e zero execuções para o repositório; a consulta de permissões retornou HTTP 401. Portanto, **a CI remota não foi confirmada nem considerada aprovada**. A restauração e compilação locais não substituem a matriz remota. É necessária a verificação de permissões/configuração do GitHub Actions e a execução da CI antes de encerrar esse gate.

O NuGet informou durante os envios que a chave utilizada expira em oito dias. Nenhuma credencial foi registrada. Os artefatos locais e hashes promovidos permanecem em `artifacts/publication-final-20261101/feed` e na tabela acima; logs do quickstart público estão em `artifacts/publication-final-20261101/logs`.
