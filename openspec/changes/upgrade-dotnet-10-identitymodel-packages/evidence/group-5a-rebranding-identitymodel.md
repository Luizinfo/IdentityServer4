# Evidência do grupo 5A — rebranding de IdentityModel

Grupo executado pelo subagent novo `/root/grupo_5a_identitymodel_rebrand`, modelo `gpt-5.6-sol` com raciocínio `low`, e inspecionado centralmente pelo orquestrador em `2026-07-20T18:54:50-03:00`. A única raiz de código autorizada foi `C:\Projetos\Identity\IdentityModel`.

## Alterações em escopo

- `src/IdentityModel.csproj`: PackageId `ZinfoFramework.IdentityModel`, versão `10.0.0`, `Authors=Luiz Antonio`, copyright histórico, URLs atuais, tags, descrição e release notes.
- `README.md`: continuação independente, migração de PackageId, requisito `net10.0`, recursos atuais e créditos upstream.
- novos `NOTICE.md`, `CONTRIBUTING.md`, `SECURITY.md`, `SUPPORT.md` e `CHANGELOG.md`.
- namespaces e assembly `IdentityModel` preservados; `LICENSE` preservado sem alteração.
- links dos documentos incorporados convertidos para URLs absolutas do repositório atual.

## Build, testes e pack

| Evidência | Resultado |
| --- | --- |
| restore | sucesso após acesso ao NuGet.org |
| build Release | sucesso, zero warnings e zero erros |
| testes | 296/296 aprovados |
| pack Release | sucesso e recompilação confirmada |
| diff | `git diff --check` sem erro; somente avisos informativos LF/CRLF |
| encoding | sete textos editados validados como UTF-8 sem BOM |

Uma publicação adicional do projeto de análise de trimming encontrou `Access Denied` em `obj` devido a processos `dotnet` persistentes. O restore desse projeto passou e o bloqueio não afetou build, 296 testes ou pack; nenhuma tentativa foi feita de encerrar processos possivelmente compartilhados.

## Artefato final promovido ao staging

- caminho de origem: `C:\Projetos\Identity\IdentityModel\artifacts\ZinfoFramework.IdentityModel.10.0.0.nupkg`;
- staging: `C:\Projetos\Identity\nuget-packages\ZinfoFramework.IdentityModel.10.0.0.nupkg`;
- tamanho: `179651` bytes;
- SHA-256: `787CB34BCE0EA42ED97C32C68ABBF23C4839F084F565A58C43EFE82B3D753A42`;
- branch local/remota: `release/zinfoframework-10.0.0`;
- `repository commit`: `f321e73b7dcc254c477f4a69eb64c4e58644cb9f`;
- SourceLink: `https://raw.githubusercontent.com/Luizinfo/IdentityModel/f321e73b7dcc254c477f4a69eb64c4e58644cb9f/*`.

Inspeção central do nupkg confirmou:

- ID `ZinfoFramework.IdentityModel`, versão `10.0.0`, `Authors=Luiz Antonio`;
- copyright `Copyright 2017-2018 Brock Allen & Dominick Baier`;
- `RepositoryUrl=https://github.com/Luizinfo/IdentityModel.git` e `PackageProjectUrl=https://github.com/Luizinfo/IdentityModel`;
- único dependency group e únicos assets em `net10.0`;
- `README.md`, `LICENSE`, `NOTICE.md` e `CHANGELOG.md` presentes;
- zero entrada ou dependência `Duende.*`;
- zero link Markdown relativo nos documentos incorporados verificados.

O commit de release foi confirmado no remote `https://github.com/Luizinfo/IdentityModel.git`. O pacote foi reconstruído a partir dessa revisão, reinspecionado e substituiu no staging somente o artefato provisório de hash `E68CCFEAE77327853CB445AC7AA7F535363CA4C55993C7EAD8671EBFF8B416EA`. Build Release permaneceu com zero warnings/erros e 296/296 testes aprovados. Nenhum pacote foi publicado no NuGet.org e nenhuma credencial foi lida ou registrada.
