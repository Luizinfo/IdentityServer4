## ADDED Requirements

### Requirement: Componentes ativos usam .NET 10
O sistema SHALL compilar bibliotecas, hosts, migrações, builds, testes e projetos ativos dos cinco repositórios coordenados com o SDK .NET 10 e `net10.0`; projetos dependentes de APIs Windows SHALL usar `net10.0-windows`.

#### Scenario: Build limpo por repositório
- **WHEN** o build oficial de cada repositório é executado em ambiente limpo com somente o SDK .NET 10
- **THEN** todos os projetos em escopo são restaurados e compilados sem SDK ou targeting pack anterior ao .NET 10

#### Scenario: Cliente Windows
- **WHEN** o job Windows da CI compila os projetos dependentes de APIs Windows
- **THEN** eles usam `net10.0-windows` e compilam sem depender do .NET Framework

### Requirement: Todos os pacotes usam versão 10.0.0 e net10.0
Os 11 pacotes coordenados SHALL ser produzidos na versão exata `10.0.0` e SHALL conter somente assets `net10.0`.

#### Scenario: Inspeção dos pacotes publicados
- **WHEN** os 11 nupkgs `ZinfoFramework.IdentityModel*` e `ZinfoFramework.IdentityServer4*` definidos na matriz do change são inspecionados
- **THEN** cada pacote possui versão `10.0.0`, contém assets `net10.0` e não contém pastas de outro TFM

### Requirement: Migrações existentes permanecem compatíveis
A atualização para EF Core 10 SHALL preservar as migrações e o schema existentes, sem gerar uma migração de alteração apenas por causa da atualização de framework.

#### Scenario: Validação de banco e migrações
- **WHEN** as migrações existentes são aplicadas a um banco vazio e o modelo é comparado ao schema esperado
- **THEN** a aplicação conclui sem nova migração pendente nem alteração não planejada de schema

### Requirement: Exemplos legados permanecem fora da matriz
O build e a CI .NET 10 SHALL excluir `samples/Clients/old`, `samples/KeyManagement` e `MvcAutomaticTokenManagement`.

#### Scenario: Execução da matriz .NET 10
- **WHEN** a matriz de build .NET 10 é executada
- **THEN** ela não restaura nem compila os projetos explicitamente excluídos
