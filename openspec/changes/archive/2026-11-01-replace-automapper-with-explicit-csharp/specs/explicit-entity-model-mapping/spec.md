## ADDED Requirements

### Requirement: Explicit bidirectional mapping

O componente SHALL converter Client, ApiResource, IdentityResource, ApiScope e PersistedGrant entre modelos e entidades usando C# explícito e sem biblioteca de mapping.

#### Scenario: Conversion through existing extension methods

- **WHEN** um consumidor chamar as extensões públicas ToModel ou ToEntity existentes
- **THEN** as assinaturas e namespaces SHALL permanecer utilizáveis e todos os campos classificados SHALL seguir a matriz de equivalência nos dois sentidos.
- **AND** campos excluídos SHALL ter justificativa e não SHALL ser copiados por reflexão ou convenção implícita.

### Requirement: Preserve mapping semantics

As conversões SHALL preservar as regras caracterizadas de nulls, defaults, coleções, duplicidade, enums, claims, secrets e algoritmos; diferenças funcionais SHALL ser tratadas separadamente desta substituição.

#### Scenario: Nulls and defaults

- **WHEN** a origem for nula ou contiver membros nulos, vazios, false ou zero
- **THEN** a conversão SHALL reproduzir o comportamento caracterizado por direção, sem aplicar defaults adicionais indiscriminadamente.

#### Scenario: Collections and invalid properties

- **WHEN** a origem contiver coleções ou propriedades duplicadas
- **THEN** o destino SHALL usar objetos novos preservando a semântica observável de coleção e as condições de falha existentes.
- **AND** chaves duplicadas não SHALL ser resolvidas silenciosamente por first/last wins.

#### Scenario: Signing algorithm representation

- **WHEN** converter algoritmos entre coleção e texto separado por vírgulas
- **THEN** o componente SHALL preservar os casos caracterizados de null, branco, entradas vazias, caixa, duplicatas e espaços por token.

### Requirement: Preserve tracked grant updates

UpdateEntity SHALL atualizar a instância recebida e SHALL preservar a persistência por Key de PersistedGrantStore.

#### Scenario: Existing grant changes and nullable values are cleared

- **WHEN** um grant existente for salvo novamente com a mesma Key, valores alterados e Expiration/ConsumedTime nulos
- **THEN** os dez campos SHALL corresponder aos valores esperados após releitura relacional em novo contexto.
- **AND** SHALL existir uma única linha para a Key e a instância recebida por UpdateEntity não SHALL ser substituída.

### Requirement: Remove AutoMapper completely from delivered artifacts

As bibliotecas, testes finais e pacotes afetados SHALL deixar de depender direta ou transitivamente de AutoMapper sem introduzir outra biblioteca de mapping.

#### Scenario: Isolated package consumer

- **WHEN** Storage e EntityFramework novos forem empacotados em ordem e consumidos com cache isolado
- **THEN** nuspec, project.assets.json e referências de assembly SHALL estar livres de AutoMapper.
- **AND** o consumidor sem ProjectReference para as bibliotecas validadas SHALL compilar e executar as extensões públicas.

### Requirement: Document public API removal

A entrega SHALL declarar a remoção dos cinco profiles públicos e das exceções específicas do AutoMapper como impacto de compatibilidade e SHALL definir a estratégia de versão antes da publicação.

#### Scenario: Release review

- **WHEN** os pacotes finais forem preparados
- **THEN** a revisão SHALL incluir diferenças de API, guia de migração, versões inéditas, hashes e resultados pós-pack.
- **AND** nenhum pacote publicado SHALL ser sobrescrito e evidências históricas não SHALL ser apresentadas como validação dos binários novos.

### Requirement: Verify behavior and relational persistence

A implementação SHALL possuir testes de caracterização independentes de AutoMapper ao final, proteção contra propriedades não classificadas e testes relacionais de clientes, recursos e grants.

#### Scenario: Windows validation

- **WHEN** a suíte padrão usar apenas EF InMemory no Windows
- **THEN** testes SQLite dedicados SHALL validar gravação e releitura com novos contextos antes de considerar a migração concluída.
- **AND** relatórios SHALL indicar os providers efetivamente exercitados sem alegar validação de SQL Server não executada.

#### Scenario: Scope preservation

- **WHEN** revisar o diff final
- **THEN** schema, migrations, modelos e regras OAuth/OIDC SHALL permanecer inalterados.
- **AND** arquivos novos/editados SHALL usar UTF-8 sem BOM e mudanças locais anteriores SHALL ser preservadas.
