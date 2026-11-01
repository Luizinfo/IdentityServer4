## ADDED Requirements

### Requirement: Componentes ativos usam .NET 10
O sistema SHALL compilar bibliotecas, hosts, migrações, builds, testes e projetos ativos dos cinco repositórios coordenados com o SDK .NET 10 e `net10.0`; projetos dependentes de APIs Windows SHALL usar `net10.0-windows`.

#### Scenario: Build limpo por repositório
- **WHEN** o build oficial de cada repositório é executado em ambiente limpo com somente o SDK .NET 10
- **THEN** todos os projetos em escopo são restaurados e compilados sem SDK ou targeting pack anterior ao .NET 10

#### Scenario: Cliente Windows
- **WHEN** o job Windows da CI compila os projetos dependentes de APIs Windows
- **THEN** eles usam `net10.0-windows` e compilam sem depender do .NET Framework

### Requirement: Pacotes coordenados usam as versões da matriz e net10.0
Os 11 pacotes coordenados SHALL conter somente assets `net10.0`. Os seis pacotes IdentityModel já publicados e os três pacotes IdentityServer4 não EF SHALL usar a versão exata `10.0.0`; `ZinfoFramework.IdentityServer4.EntityFramework.Storage` e `ZinfoFramework.IdentityServer4.EntityFramework` SHALL usar a versão exata `10.0.1` na primeira publicação da família IdentityServer4.

#### Scenario: Inspeção do conjunto coordenado
- **WHEN** os seis nupkgs IdentityModel publicados e os cinco nupkgs IdentityServer4 preparados para publicação são inspecionados conforme a matriz do change
- **THEN** os dois pacotes EntityFramework possuem versão `10.0.1`, os outros nove possuem versão `10.0.0` e nenhum pacote contém assets de outro TFM além de `net10.0`

### Requirement: Migrações existentes permanecem compatíveis
Os componentes EntityFramework do IdentityServer4 SHALL usar dependências Microsoft.EntityFrameworkCore `9.0.9` no build e declarar `Microsoft.EntityFrameworkCore.Relational` `9.0.9` no pacote Storage. A combinação com SDK e TFM .NET 10 SHALL preservar as migrações e o schema existentes, sem gerar uma migração apenas por causa da mudança de versão de dependência.

#### Scenario: Restore dos consumidores EntityFramework
- **WHEN** um consumidor restaura `ZinfoFramework.IdentityServer4.EntityFramework` `10.0.1` a partir do staging isolado
- **THEN** ele resolve `ZinfoFramework.IdentityServer4.EntityFramework.Storage` `10.0.1` e `Microsoft.EntityFrameworkCore.Relational` `9.0.9` sem selecionar os nupkgs EntityFramework provisórios `10.0.0`

#### Scenario: Quickstart EntityFramework
- **WHEN** o quickstart `5_EntityFramework` é restaurado com cache limpo antes da publicação do IdentityServer4
- **THEN** ele consome os dois pacotes EntityFramework `10.0.1` do staging e os pacotes EF Core `9.0.9`, preservando os pacotes ASP.NET Core `10.0.0`

#### Scenario: Validação de banco e migrações
- **WHEN** as migrações existentes são aplicadas a um banco vazio e o modelo é comparado ao schema esperado
- **THEN** a aplicação conclui sem nova migração pendente nem alteração não planejada de schema

### Requirement: Exemplos legados permanecem fora da matriz
O build e a CI .NET 10 SHALL excluir `samples/Clients/old`, `samples/KeyManagement` e `MvcAutomaticTokenManagement`.

#### Scenario: Execução da matriz .NET 10
- **WHEN** a matriz de build .NET 10 é executada
- **THEN** ela não restaura nem compila os projetos explicitamente excluídos
