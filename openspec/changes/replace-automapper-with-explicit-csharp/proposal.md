## Why

O pacote `ZinfoFramework.IdentityServer4.EntityFramework.Storage` depende diretamente de AutoMapper `16.2.0`, propagando a dependência para EntityFramework e seus consumidores. O objetivo é eliminar essa dependência por meio de mapeamento explícito em C#, preservando os contratos de conversão e persistência existentes.

O planejamento inicial foi aprovado para implementação e geração local dos pacotes em 25/10/2026. A implementação e a validação dos pacotes 11.0.0 estão registradas em `evidence/implementation-validation.md`. Não houve publicação, push ou alteração do change `upgrade-dotnet-10-identitymodel-packages` por esta execução.

## What Changes

- Substituir cinco grupos de mapeamentos bidirecionais: Client, ApiResource, IdentityResource, ApiScope e PersistedGrant.
- Preservar namespaces, classes de extensão e assinaturas de `ToModel`, `ToEntity` e `UpdateEntity`.
- Implementar atribuições explícitas e projeções de coleções, sem biblioteca substituta, reflexão ou geração dinâmica no código de produção.
- Substituir o conversor de algoritmos por funções internas sem tipos do AutoMapper.
- Remover inicializadores de MapperConfiguration, propriedades internas IMapper, cinco profiles e referências NuGet do AutoMapper.
- **BREAKING** Remover os cinco tipos públicos `*MapperProfile`, que herdam de AutoMapper.Profile. Preservar seus nomes como classes vazias não preservaria o contrato e não é uma solução de compatibilidade.
- Trocar validações de configuração por testes de comportamento, caracterizar o comportamento atual antes da troca e validar persistência com um provedor relacional.
- Gerar e validar novos pacotes Storage e EntityFramework em staging isolado, garantindo ausência transitiva de AutoMapper nos consumidores afetados.

## Scope

Incluído: diretório Mappers do EntityFramework.Storage, testes unitários e de integração relacionados, referência direta do pacote, definição central de versão, validação dos dois pacotes EntityFramework e dos consumidores locais, documentação da quebra pública e evidências desta mudança.

Excluído: alteração de regras OAuth/OIDC, modelos públicos de domínio, entidades e schema do banco, novas migrations, atualização de .NET/EF Core, troca por outra biblioteca de mapping, outros repositórios IdentityModel, limpeza geral de warnings e publicação remota nesta etapa.

## Capabilities

### New Capabilities

- `explicit-entity-model-mapping`: conversões explícitas e verificáveis entre modelos e entidades sem dependência do AutoMapper.

### Modified Capabilities

Nenhuma capability existente será editada por este planejamento.

## Impact and assumptions

- Base observada: .NET 10, EF Core 9.0.9 e AutoMapper 16.2.0. Os dois pacotes EntityFramework estão configurados como 10.0.1 no working tree atual; a publicação dessas versões precisa ser reconfirmada antes da execução.
- O uso está concentrado em cinco profiles, 22 pares de tipos com ReverseMap, cinco classes de extensão e um conversor. Há 16 testes de mapping, cinco deles dependentes de AssertConfigurationIsValid.
- A referência NuGet de EntityFramework para Storage exige validar o pacote recompilado, além dos testes por ProjectReference.
- Há mudanças locais pré-existentes nos projetos e no planejamento da migração para .NET 10. Elas devem ser preservadas e separadas das alterações deste change.
- A remoção dos profiles é uma quebra pública mesmo que nenhum consumidor local os utilize. A estratégia de versão e a comunicação aos consumidores são gates de release, descritos em design.md.
- O histórico do outro change não será reescrito; seus hashes e resultados anteriores não validam os novos binários.

## Acceptance

A implementação só estará concluída quando passar os critérios AC01 a AC12 de design.md, incluindo equivalência de comportamento, persistência relacional, remoção da dependência nos nupkgs e consumo com cache isolado. A sequência executável está em tasks.md e os requisitos em specs/explicit-entity-model-mapping/spec.md.
