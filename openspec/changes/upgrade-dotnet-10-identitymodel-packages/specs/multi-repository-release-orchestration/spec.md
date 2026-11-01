## ADDED Requirements

### Requirement: Um change coordena cinco repositórios
O change SHALL manter uma única fonte de verdade para IdentityServer4, IdentityModel, AccessTokenValidation, OAuth2Introspection e OidcClient, com paths, dependências e gates explícitos.

#### Scenario: Seleção de uma área de implementação
- **WHEN** um grupo de tasks é iniciado
- **THEN** um novo subagent recebe exatamente um repositório, sua raiz de edição autorizada, seus pré-requisitos e seus critérios de aceite

### Requirement: Grupos respeitam a ordem de dependência
O orquestrador SHALL concluir os grupos na ordem IdentityModel, OAuth2Introspection/OidcClient, AccessTokenValidation, rebranding/documentação/revalidação e publicação dos seis pacotes `ZinfoFramework.IdentityModel*`, IdentityServer4 e publicação dos cinco pacotes `ZinfoFramework.IdentityServer4*`.

#### Scenario: Dependência ainda não aprovada
- **WHEN** um grupo depende de pacote que ainda não foi empacotado e validado no estágio anterior
- **THEN** o grupo dependente não é iniciado

### Requirement: Evidências controlam os gates
Cada grupo SHALL registrar build, testes, pack, versão, TFM e auditoria de dependências antes de ser marcado como concluído.

#### Scenario: Falha em um repositório
- **WHEN** qualquer evidência obrigatória falha ou está ausente
- **THEN** a publicação e os grupos dependentes permanecem bloqueados

### Requirement: Publicação é um gate manual
A publicação no novo feed NuGet SHALL ocorrer somente após confirmação explícita do usuário para os PackageIds, metadados e SHA-256 atuais e SHALL usar os nupkgs já validados, sem recompilação. Qualquer alteração de PackageId, metadado incorporado ou conteúdo após a confirmação SHALL invalidar a autorização e exigir nova validação e nova confirmação.

Os seis pacotes IdentityModel `10.0.0` já foram publicados; os cinco pacotes IdentityServer4 SHALL permanecer locais até o gate manual. Os dois nupkgs EntityFramework `10.0.0` produzidos na validação anterior SHALL permanecer apenas como evidência histórica; a aprovação de publicação dos cinco restantes SHALL identificar os dois substitutos `10.0.1` e os três nupkgs IdentityServer4 não EF `10.0.0`.

#### Scenario: Promoção aprovada
- **WHEN** o usuário aprova a promoção de um conjunto validado
- **THEN** exatamente os cinco nupkgs IdentityServer4 auditados são publicados e a disponibilidade dos três `10.0.0` e dois EntityFramework `10.0.1` é confirmada antes da auditoria final
