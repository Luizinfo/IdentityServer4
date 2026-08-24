## ADDED Requirements

### Requirement: Os quatro repositórios IdentityModel produzem pacotes 10.0.0
Os repositórios IdentityModel, AccessTokenValidation, OAuth2Introspection e OidcClient SHALL produzir `ZinfoFramework.IdentityModel`, `ZinfoFramework.IdentityModel.AspNetCore.AccessTokenValidation`, `ZinfoFramework.IdentityModel.AspNetCore.OAuth2Introspection`, `ZinfoFramework.IdentityModel.OidcClient`, `ZinfoFramework.IdentityModel.OidcClient.DPoP` e `ZinfoFramework.IdentityModel.OidcClient.IdentityTokenValidator` na versão `10.0.0`, somente com assets `net10.0` e com referência a `ZinfoFramework.IdentityModel` `10.0.0` quando aplicável.

#### Scenario: Empacotamento da família IdentityModel
- **WHEN** os quatro repositórios são compilados e empacotados na ordem definida
- **THEN** os seis nupkgs usam os novos IDs `ZinfoFramework.IdentityModel*`, possuem versão `10.0.0`, contêm somente `net10.0` e usam dependências internas `ZinfoFramework.*` `10.0.0`

#### Scenario: Compatibilidade de código após troca do PackageId
- **WHEN** assemblies e APIs públicas são comparadas antes e depois do rebranding NuGet
- **THEN** namespaces e nomes de assemblies históricos permanecem inalterados, e somente a identidade de distribuição e as referências de pacote mudam

### Requirement: Artefatos não resolvem pacotes Duende
Nenhum projeto em escopo, nupkg produzido ou árvore de restore SHALL possuir `PackageReference`, `ProjectReference`, assembly resolvida ou dependência transitiva cujo ID comece com `Duende.`.

#### Scenario: Auditoria de dependências
- **WHEN** os `.nuspec`, `project.assets.json` e grafos transitivos dos projetos e exemplos em escopo são inspecionados
- **THEN** nenhuma dependência resolvida possui ID iniciado por `Duende.`

### Requirement: Staging local precede publicação no novo feed
Os seis pacotes da família IdentityModel SHALL ser restaurados e validados primeiro em um feed local isolado; somente depois da aprovação do usuário SHALL ser publicados no novo feed NuGet.

#### Scenario: Validação integrada local
- **WHEN** o staging contém os seis nupkgs `10.0.0` e o cache NuGet isolado está vazio
- **THEN** os restores resolvem os IDs internos exclusivamente do staging e as dependências públicas do NuGet.org

#### Scenario: Promoção para o novo feed
- **WHEN** build, testes, pack e auditoria dos seis pacotes locais foram aprovados
- **THEN** o usuário pode publicar exatamente esses artefatos imutáveis no novo feed NuGet

### Requirement: CI consome o novo feed NuGet
Após a promoção, a CI SHALL resolver os IDs `ZinfoFramework.IdentityModel` e `ZinfoFramework.IdentityModel.*` versão `10.0.0` exclusivamente do novo feed, mantendo o NuGet.org para dependências públicas.

#### Scenario: Restore da CI com cache limpo
- **WHEN** a CI executa restore com o novo feed configurado por source mapping e credencial protegida
- **THEN** os pacotes internos vêm do novo feed, as dependências públicas vêm do NuGet.org e nenhuma credencial aparece no repositório ou nos logs

### Requirement: DPoPTests não depende de um IdentityServer externo
`DPoPTests` SHALL usar um test double local em ASP.NET Core `TestServer` para validar o token endpoint e a API DPoP, sem depender de `Duende.IdentityServer` ou IdentityServer4.

#### Scenario: Token endpoint DPoP com nonce
- **WHEN** o token endpoint exige nonce
- **THEN** a primeira chamada retorna `DPoP-Nonce`, o handler repete exatamente uma vez com novo proof e recebe um token `DPoP` contendo `cnf.jkt` correspondente à chave

#### Scenario: API rejeita proof incompatível
- **WHEN** a API recebe proof assinado por chave diferente de `cnf.jkt` ou com `ath` divergente
- **THEN** a requisição é rejeitada

### Requirement: ConformanceTests valida os pacotes produzidos
`clients/ConformanceTests` SHALL usar `net10.0` e consumir `ZinfoFramework.IdentityModel`, `ZinfoFramework.IdentityModel.OidcClient` e `ZinfoFramework.IdentityModel.OidcClient.IdentityTokenValidator` `10.0.0` como pacotes pós-pack, sem `ProjectReference` para as bibliotecas testadas.

#### Scenario: Build pós-pack do runner
- **WHEN** o runner é restaurado após o empacotamento usando o staging isolado
- **THEN** ele compila com .NET 10 e resolve os três PackageIds na versão `10.0.0` sem dependência Duende

#### Scenario: Execução manual de conformidade
- **WHEN** o runner recebe `discoveryurl`, perfil e redirect URI de uma execução da suíte atual da OpenID Foundation
- **THEN** ele executa o perfil Basic RP e registra a URL ou ID da execução como evidência manual, sem apresentar o resultado como certificação do fork

### Requirement: Exemplos em escopo consomem pacotes 10.0.0
`ResourceBasedApi` SHALL manter `ZinfoFramework.IdentityModel.AspNetCore.AccessTokenValidation` e `ZinfoFramework.IdentityModel.AspNetCore.OAuth2Introspection`; `ConsoleCode` e `WindowsConsoleSystemBrowser` SHALL consumir `ZinfoFramework.IdentityModel.OidcClient`. Todos SHALL usar os PackageIds `ZinfoFramework.*` `10.0.0` do staging ou do novo feed conforme a fase.

#### Scenario: Compilação dos exemplos integrados
- **WHEN** os exemplos em escopo são compilados após a promoção
- **THEN** eles restauram os pacotes `10.0.0` do novo feed e compilam sem dependência transitiva Duende
