# Matriz de propriedades e equivalência

Cada campo listado é mapeado nos dois sentidos, salvo observação explícita. `ScalarFields_MapIndependentlyInEachDirection` testa individualmente todos os campos escalares com valores não padrão e zero/null/false, em ambas as direções. `PublicProperties_AreExplicitlyClassified` falha quando o contrato ganha ou perde uma propriedade não classificada.

## Tipos principais

### Client

Escalares: `Enabled`, `ClientId`, `ProtocolType`, `RequireClientSecret`, `ClientName`, `Description`, `ClientUri`, `LogoUri`, `RequireConsent`, `AllowRememberConsent`, `AlwaysIncludeUserClaimsInIdToken`, `RequirePkce`, `AllowPlainTextPkce`, `RequireRequestObject`, `AllowAccessTokensViaBrowser`, `FrontChannelLogoutUri`, `FrontChannelLogoutSessionRequired`, `BackChannelLogoutUri`, `BackChannelLogoutSessionRequired`, `AllowOfflineAccess`, `IdentityTokenLifetime`, `AccessTokenLifetime`, `AuthorizationCodeLifetime`, `ConsentLifetime`, `AbsoluteRefreshTokenLifetime`, `SlidingRefreshTokenLifetime`, `RefreshTokenUsage`, `UpdateAccessTokenClaimsOnRefresh`, `RefreshTokenExpiration`, `AccessTokenType`, `EnableLocalLogin`, `IncludeJwtId`, `AlwaysSendClientClaims`, `ClientClaimsPrefix`, `PairWiseSubjectSalt`, `UserSsoLifetime`, `UserCodeType`, `DeviceCodeLifetime`.

Coleções/propriedades especiais: `ClientSecrets`, `AllowedGrantTypes`, `RedirectUris`, `PostLogoutRedirectUris`, `AllowedScopes`, `AllowedIdentityTokenSigningAlgorithms`, `IdentityProviderRestrictions`, `Claims`, `AllowedCorsOrigins`, `Properties`.

### ApiResource

Escalares: `Enabled`, `Name`, `DisplayName`, `Description`, `ShowInDiscoveryDocument`.

Coleções/propriedades especiais: `UserClaims`, `Properties`, `ApiSecrets`, `Scopes`, `AllowedAccessTokenSigningAlgorithms`.

### ApiScope

Escalares: `Enabled`, `Name`, `DisplayName`, `Description`, `ShowInDiscoveryDocument`, `Required`, `Emphasize`.

Coleções/propriedades especiais: `UserClaims`, `Properties`.

### IdentityResource

Escalares: `Enabled`, `Name`, `DisplayName`, `Description`, `ShowInDiscoveryDocument`, `Required`, `Emphasize`.

Coleções/propriedades especiais: `UserClaims`, `Properties`.

### PersistedGrant

Escalares: `Key`, `Type`, `SubjectId`, `SessionId`, `ClientId`, `Description`, `CreationTime`, `Expiration`, `ConsumedTime`, `Data`.

## Conversões específicas

| Campo | Regra | Evidência de teste |
| --- | --- | --- |
| Client.RefreshTokenUsage, RefreshTokenExpiration, AccessTokenType | Cast explícito enum/inteiro, inclusive valores legados fora do enum | ScalarFields_MapIndependentlyInEachDirection |
| Client.AllowedGrantTypes | Projeção dos filhos; setter do modelo preserva a validação de grants | ClientChildren_MapEachDirectionIndependently; InvalidGrantTypes_StillFailModelValidation |
| ApiResource.ApiSecrets | Models.ApiSecrets corresponde a Entities.Secrets | ResourceChildren_MapEachDirectionIndependently |
| Client.Claims | Entidade Type/Value; modelo ValueType sempre String ao ler da entidade | ClaimValueType_IsStringAfterReadingEntity |
| Algoritmos | Coleção/texto, Trim global, deduplicação case-sensitive, List na leitura, coleção com um null gera null | SigningAlgorithms_PreserveTokenWhitespaceAndCase; SigningAlgorithms_WriteInCollectionOrder |
| Properties | Lista de entidades/dicionário case-sensitive; chaves duplicadas/inválidas continuam falhando | DuplicateAndCaseSensitiveProperties_PreserveDictionaryRules; teste legado duplicates_properties_in_db_map |
| ProtocolType e Secret.Type | Null e string vazia sobrescrevem defaults; defaults de construtores permanecem quando a origem já os contém | ExplicitNullMembers_OverwriteDefaultsAndNullGrantUpdateIsNoOp; EmptyAndWhitespaceMembers_AreNotReplacedByDefaults; testes legados missing_values_should_use_defaults |
| PersistedGrant.UpdateEntity | Copia os dez campos na mesma instância, inclusive nulls | GrantUpdate_PreservesInstanceAndCopiesAllFieldsIncludingNull; ExistingGrant_UpdatesSingleRowAndClearsNullableValues |

## Tipos filhos e propriedades classificadas

Os filhos são convertidos pelos mapeadores dos respectivos pais. `ClientChildren_MapEachDirectionIndependently` e `ResourceChildren_MapEachDirectionIndependently` verificam os valores de cada direção independentemente; os testes de null/duplicidade completam os limites.

| Tipo | Propriedades conhecidas, incluindo herdadas |
| --- | --- |
| `M.Secret` | `Description`, `Value`, `Expiration`, `Type` |
| `M.ClientClaim` | `Type`, `Value`, `ValueType` |
| `E.ClientSecret` | `Id`, `Description`, `Value`, `Expiration`, `Type`, `Created`, `ClientId`, `Client` |
| `E.ApiResourceSecret` | `Id`, `Description`, `Value`, `Expiration`, `Type`, `Created`, `ApiResourceId`, `ApiResource` |
| `E.ClientClaim` | `Id`, `Type`, `Value`, `ClientId`, `Client` |
| `E.ClientProperty` | `Id`, `Key`, `Value`, `ClientId`, `Client` |
| `E.ApiResourceProperty` | `Id`, `Key`, `Value`, `ApiResourceId`, `ApiResource` |
| `E.ApiScopeProperty` | `Id`, `Key`, `Value`, `ScopeId`, `Scope` |
| `E.IdentityResourceProperty` | `Id`, `Key`, `Value`, `IdentityResourceId`, `IdentityResource` |
| `E.ApiResourceClaim` | `Id`, `Type`, `ApiResourceId`, `ApiResource` |
| `E.ApiScopeClaim` | `Id`, `Type`, `ScopeId`, `Scope` |
| `E.IdentityResourceClaim` | `Id`, `Type`, `IdentityResourceId`, `IdentityResource` |
| `E.ClientCorsOrigin` | `Id`, `Origin`, `ClientId`, `Client` |
| `E.ClientIdPRestriction` | `Id`, `Provider`, `ClientId`, `Client` |
| `E.ClientScope` | `Id`, `Scope`, `ClientId`, `Client` |
| `E.ClientGrantType` | `Id`, `GrantType`, `ClientId`, `Client` |
| `E.ClientRedirectUri` | `Id`, `RedirectUri`, `ClientId`, `Client` |
| `E.ClientPostLogoutRedirectUri` | `Id`, `PostLogoutRedirectUri`, `ClientId`, `Client` |
| `E.ApiResourceScope` | `Id`, `Scope`, `ApiResourceId`, `ApiResource` |

`M` identifica IdentityServer4.Models e `E` identifica IdentityServer4.EntityFramework.Entities. A lista é validada por ChildProperties_AreExplicitlyClassified.

## Exclusões deliberadas

- Id, IDs de pais e back-references de todos os filhos: não existem nos modelos; não são copiados e ficam sob responsabilidade do EF.
- Client e ApiResource: Id, Created, Updated, LastAccessed e NonEditable não existem no modelo; preservam os inicializadores da entidade nova.
- IdentityResource: Id, Created, Updated e NonEditable seguem a mesma regra.
- ApiScope: Id não existe no modelo.
- Secrets de entidade: Id, Created e relações com os pais não existem no modelo. Description, Value, Type e Expiration são copiados integralmente.
- Model.ClientClaim.ValueType não possui coluna na entidade: não é persistido, e a leitura usa ClaimValueTypes.String, como no baseline.

Nenhuma exclusão é aplicada a PersistedGrant. A persistência relacional verifica as relações geradas e a atualização de grants; nenhum schema ou migration foi alterado.
