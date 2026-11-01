# Plano técnico: substituir AutoMapper por C# explícito

Data: 25/10/2026. Estado: plano implementado após autorização do usuário; pacotes 11.0.0 gerados e validados localmente. Resultados: `evidence/implementation-validation.md`. As etapas abaixo preservam o roteiro técnico original.

## 1. Resultado e estratégia

Conservar as entradas públicas de conversão e substituir seu interior por código explícito, em etapas pequenas e verificáveis. Manter AutoMapper apenas enquanto os testes de caracterização forem produzidos; remover a dependência quando os cinco grupos estiverem migrados. Não adicionar mecanismo de seleção em runtime nem manter duas implementações de produção ao final.

A referência comportamental é a implementação atual com AutoMapper 16.2.0 e os pacotes efetivamente resolvidos. Não presumir que ReverseMap é simétrico, que os tipos concretos das coleções são irrelevantes ou que valores nulos preservam defaults em todas as direções.

## 2. Inventário e limites

Raiz de execução: `C:\Projetos\Identity\IdentityServer4`.

| Grupo | Pares configurados | Responsabilidade |
| --- | ---: | --- |
| PersistedGrant | 1 | Dez propriedades escalares; criação e atualização de entidade existente |
| IdentityResource | 3 | Recurso, claims e propriedades |
| ApiScope | 3 | Escopo, claims e propriedades |
| ApiResource | 5 | Recurso, secrets, scopes, claims, propriedades e algoritmos |
| Client | 10 | Cliente, secrets, claims, grants, URIs, scopes, CORS, restrições e propriedades |

Os mapeamentos são executados em memória. Não há ProjectTo ou configuração de AutoMapper por DI nos usos encontrados. Client é o último grupo a migrar porque reúne mais campos, enums e coleções.

Arquivos de produção prováveis:

- `src/EntityFramework.Storage/src/Mappers/PersistedGrantMappers.cs`.
- `src/EntityFramework.Storage/src/Mappers/IdentityResourceMappers.cs`.
- `src/EntityFramework.Storage/src/Mappers/ScopeMappers.cs`.
- `src/EntityFramework.Storage/src/Mappers/ApiResourceMappers.cs`.
- `src/EntityFramework.Storage/src/Mappers/ClientMappers.cs`.
- `src/EntityFramework.Storage/src/Mappers/AllowedSigningAlgorithmsConverter.cs`: substituir por helper interno, preferencialmente `SigningAlgorithmMapping.cs`, com serialização e desserialização explícitas.
- Os cinco arquivos `*MapperProfile.cs`: remover após migrar os respectivos grupos e documentar a quebra pública.
- `src/EntityFramework.Storage/src/IdentityServer4.EntityFramework.Storage.csproj`: remover PackageReference AutoMapper.
- `src/Directory.Build.targets`: remover Update AutoMapper e ajustar versões dos pacotes apenas após a decisão de release.
- `src/EntityFramework/src/IdentityServer4.EntityFramework.csproj`: revisar metadados/dependência se a release exigir; não trocar a relação por ProjectReference permanentemente.

Testes prováveis: os cinco arquivos atuais em `src/EntityFramework.Storage/test/UnitTests/Mappers`, novos testes do helper de algoritmos e casos em `test/IntegrationTests/Stores/{ClientStoreTests,ResourceStoreTests,PersistedGrantStoreTests}.cs`. Adicionar validação relacional dedicada com SQLite em banco temporário/isolado, usando as dependências já presentes.

Consumidores a validar: testes unitários e integração do Storage; ConsoleHost e migrations do Storage; biblioteca, testes, Host e migrations de EntityFramework; quickstart 5_EntityFramework. O restante da solução deve continuar compilando quando afetado pelo empacotamento.

## 3. Matriz de equivalência antes de codificar

Produzir uma matriz por propriedade, incluindo propriedades herdadas, para cada um dos cinco modelos principais e para cada tipo filho. Registrar origem, destino, direção, regra de null/default, conversão, campo deliberadamente não mapeado e teste correspondente. Comparar também os modelos dos pacotes resolvidos com os fontes locais; o projeto usa PackageReference para os modelos do Storage.

Cada campo deve ser classificado como mapeado, derivado ou deliberadamente excluído. Cobrir o sentido entidade -> modelo e modelo -> entidade separadamente. Round-trip sozinho pode esconder omissões iguais nas duas direções.

| Tema | Regra a caracterizar e preservar |
| --- | --- |
| Null na raiz | Resultado de cada ToModel/ToEntity e comportamento de UpdateEntity com modelo/destino nulos |
| Defaults | Modelos recém-construídos, origem com null, string vazia, false e zero; ProtocolType e Secret.Type merecem testes dedicados |
| Coleções | Null, vazia, populada, duplicada e com elemento nulo; tipo concreto observável, ordem, comparador e tratamento de repetidos |
| Dicionários | Propriedades nos dois sentidos; chaves duplicadas, diferenças de maiúsculas e chaves inválidas; conservar falha em duplicidade em vez de escolher silenciosamente um valor |
| ClientClaim | Type, Value e ValueType: a construção atual usa ClaimValueTypes.String; não persistir informação inexistente na entidade |
| Enums | RefreshTokenUsage, RefreshTokenExpiration e AccessTokenType armazenados como inteiros; testar valores válidos e valores fora do enum sem introduzir nova validação |
| Secrets | Value, Type, Description, Expiration e defaults; criar objetos novos, sem compartilhar instâncias mutáveis com a origem |
| Nomes diferentes | ApiResource.Secrets <-> Models.ApiResource.ApiSecrets; filhos de URI/claim/scope <-> strings |
| Campos da entidade | Ids, FKs, back-references e campos administrativos sem equivalente no modelo; conservar inicializadores e evitar percorrer ciclos de navegação |
| PersistedGrant | Key, Type, SubjectId, SessionId, ClientId, Description, CreationTime, Expiration, ConsumedTime e Data; nulls devem também limpar valores anteriores quando esse for o comportamento atual |

Ponto de atenção: as Conditions atuais de ProtocolType e Secret.Type verificam o objeto de origem no lambda, não explicitamente o membro. Os testes existentes exercitam objetos padrão, mas não demonstram toda a semântica de null. Não traduzir automaticamente para `?? default`; primeiro executar a caracterização. Se houver divergência entre expectativa e comportamento observado, registrá-la e tratar eventual correção funcional separadamente.

Algoritmos de assinatura: preservar exatamente a transformação atual. Coleção nula/vazia produz null; coleção preenchida produz texto com vírgulas. Na leitura, texto nulo/branco produz conjunto vazio, o texto completo recebe Trim, Split remove entradas vazias e os valores são deduplicados com a semântica atual de string. O código atual não aplica Trim a cada token. Cobrir espaços internos, vírgulas repetidas, valores repetidos, caixa diferente e itens nulos/vazios; não normalizar dados adicionalmente nesta migração.

Exceções: registrar quais falhas são do domínio de dados e quais são wrappers do AutoMapper. Preservar as condições de falha e não converter erros em sucesso silencioso. A remoção de AutoMapperMappingException da superfície de falhas será documentada como parte da quebra de compatibilidade; conservar InnerException/tipo exato somente onde houver contrato explícito, sem manter dependência residual.

## 4. Desenho da implementação

- Usar construtores existentes, inicializadores e atribuições por propriedade, mantendo os estilos e aliases do repositório.
- Implementar conversões de filhos por métodos privados ou helpers internos específicos. Extrair compartilhamento apenas onde reduz duplicação real, como algoritmos e secrets; evitar um mapper genérico por reflexão.
- Não usar serialização JSON, MemberwiseClone, dynamic, Expression.Compile, geradores de código ou outra biblioteca como substituição.
- Criar novas coleções e objetos filhos; não reutilizar referências mutáveis da origem. Preservar a semântica caracterizada das coleções, sem aplicar Distinct, ToHashSet ou ordenação indiscriminadamente.
- Não atribuir back-references e IDs que não existam no modelo; deixar o EF fixar relações e identidades conforme o comportamento atual.
- Em PersistedGrant.UpdateEntity, copiar valores para a instância recebida. Não construir entidade substituta, chamar Attach/Update ou alterar a política de concorrência de PersistedGrantStore.
- Manter assinaturas públicas e namespaces das cinco classes de extensão. Remover IMapper e inicializadores estáticos conforme cada grupo deixe de usá-los.
- Não alterar contratos das entidades, opções EF, schema, providers de produção ou fluxos OAuth/OIDC.

## 5. Estratégia de testes

### Antes da substituição

Executar as suítes atuais, registrar ambiente, versões resolvidas, providers reais, contagens e avisos. Os resultados históricos de 16 testes de mapping, 40 de integração do Storage e 6 de EntityFramework são referência documental, não validação atual.

Adicionar casos com valores distintos dos defaults para todas as propriedades mapeadas, incluindo flags false, zero, null e datas. Obter os resultados esperados da implementação atual e convertê-los em asserts explícitos. Esses testes devem sobreviver à remoção da biblioteca. Se usado um comparador diferencial temporário, removê-lo ao final; nenhum teste final pode precisar de AutoMapper.

### Durante a migração

Migrar na ordem PersistedGrant -> IdentityResource -> ApiScope -> ApiResource -> Client. Após cada grupo, executar os testes daquele grupo e os testes de store correspondentes. Substituir os cinco testes de configuração por casos de comportamento e classificação de propriedades, sem diminuir a cobertura funcional existente.

Usar reflexão apenas em teste para detectar propriedades novas não classificadas, com lista explícita de exclusões e justificativa. Isso substitui a proteção de AssertConfigurationIsValid sem introduzir reflexão em produção ou repetir o código do mapper como teste.

### Persistência relacional

No Windows, IntegrationTest atualmente habilita apenas InMemory; SQLite e LocalDb estão comentados. Portanto, sucesso dos testes existentes não prova o comportamento relacional.

Adicionar testes dedicados com SQLite, sem habilitar banco externo obrigatório para toda a suíte. Manter conexão/contexto e banco isolados por teste. Exercitar gravação, leitura com novo DbContext, chaves geradas, FKs e ausência de entidades duplicadas para clientes e recursos com filhos.

Para grants, gravar um registro, chamar StoreAsync com a mesma Key e valores diferentes, limpar Expiration/ConsumedTime com null e reler em novo contexto. Confirmar uma única linha e os dez valores esperados. Verificar também que UpdateEntity mantém ReferenceEquals com a entidade recebida e que o estado de tracking não é substituído indevidamente.

### Contrato e pacote

Testar um consumidor das extensões públicas contra os nupkgs novos, sem ProjectReference para as bibliotecas sob validação. Cobrir conversões e persistência básica. Inspecionar a API pública e registrar a retirada dos cinco profiles e dos tipos de exceção ligados ao AutoMapper.

## 6. Validação e comandos

Os comandos abaixo são um roteiro para a implementação futura; não foram executados neste planejamento. No PowerShell, executar a partir de `C:\Projetos\Identity\IdentityServer4` e usar a configuração NuGet do staging da nova mudança. Não executar pack/restore contra IDs e versões antigas no cache compartilhado esperando receber binários novos.

```powershell
dotnet test src/EntityFramework.Storage/test/UnitTests/IdentityServer4.EntityFramework.UnitTests.csproj -c Release
dotnet test src/EntityFramework.Storage/test/IntegrationTests/IdentityServer4.EntityFramework.IntegrationTests.csproj -c Release
dotnet test src/EntityFramework/test/IdentityServer4.EntityFramework.Tests/IdentityServer4.EntityFramework.Tests.csproj -c Release
```

Para build e regressão, usar os alvos abaixo; a sequência de pack/restore deve seguir a ordem descrita em seguida:

```powershell
dotnet build src/EntityFramework.Storage/IdentityServer4.EntityFramework.Storage.sln -c Release
dotnet build src/EntityFramework/IdentityServer4.EntityFramework.sln -c Release
dotnet build samples/Quickstarts/5_EntityFramework/Quickstart.sln -c Release
dotnet test src/IdentityServer4/test/IdentityServer.UnitTests/IdentityServer.UnitTests.csproj -c Release
dotnet test src/IdentityServer4/test/IdentityServer.IntegrationTests/IdentityServer.IntegrationTests.csproj -c Release
```

Na execução, substituir os argumentos de restore por uma configuração concreta de staging registrada em evidence, e separar restore, build/test --no-restore e pack --no-restore quando necessário para assegurar o feed correto.

Sequência de pacotes:

1. Criar feed, cache e diretório de evidências exclusivos deste change, sem excluir/modificar caches ou artefatos anteriores.
2. Preparar NuGet.Config com IDs internos apontando para os artefatos internos validados e nuget.org para dependências externas; reaproveitar versões já aprovadas dos demais pacotes. Não copiar credenciais para arquivos versionados.
3. Definir versões inéditas de validação para Storage e EntityFramework. Empacotar Storage primeiro; inspecionar seu nuspec e conteúdo.
4. Restaurar EntityFramework contra esse Storage novo, confirmar project.assets.json, compilar, testar e empacotar EntityFramework. Não confiar no pacote antigo de mesmo ID/versão.
5. Restaurar consumidores e quickstart com cache vazio isolado, confirmar os dois pacotes novos e executar build/test. Validar migrations/hosts compilando suas solutions.
6. Inspecionar nuspec, dependências em project.assets.json, lista transitiva e referências de assembly: nenhum AutoMapper no resultado. Registrar comandos, versões, providers, resultados e hashes SHA-256 dos nupkgs.
7. Repetir o gate pós-pack com as versões finais caso elas difiram das versões de staging; somente os artefatos finais validados poderão ser propostos para publicação.

A busca final em código/projetos ativos deve retornar zero uso de AutoMapper, IMapper, MapperConfiguration e profiles removidos. Menções em histórico e documentos de migração são permitidas e devem ser separadas da busca de dependência executável.

## 7. Critérios de aceite mensuráveis

| ID | Critério |
| --- | --- |
| AC01 | Os cinco grupos e seus 22 pares de tipos estão implementados com C# explícito, cobrindo os dois sentidos |
| AC02 | Todas as propriedades, incluindo herdadas, estão classificadas na matriz e cobertas por teste ou exclusão justificada; teste detecta adição de propriedade não classificada |
| AC03 | As assinaturas ToModel, ToEntity e UpdateEntity e seus namespaces permanecem; consumidor pós-pack compila e executa |
| AC04 | Todos os casos de null/default, coleções, duplicidade, enums, secrets, claims e algoritmos da matriz passam; nenhum ajuste funcional silencioso |
| AC05 | UpdateEntity preserva a instância; StoreAsync atualiza uma única linha por Key e persiste também valores nulos com SQLite |
| AC06 | Clientes e recursos com filhos persistem e são relidos em novo contexto relacional sem duplicações ou FKs incorretas; schema/migrations permanecem sem alteração |
| AC07 | Zero referência AutoMapper em produção e testes finais, PackageReference/Update, nuspec, grafo transitivo dos consumidores e referências dos assemblies produzidos |
| AC08 | Todas as suítes obrigatórias passam; nenhum novo teste ignorado ou warning introduzido pela mudança; avisos e skips preexistentes documentados |
| AC09 | Builds das duas solutions EF e do quickstart 5 passam usando os pacotes novos corretos, incluindo hosts/migrations |
| AC10 | Quebra dos cinco profiles e mudanças de exceção documentadas, decisão de versão registrada e nenhum pacote já publicado sobrescrito |
| AC11 | Nupkgs finais inspecionados e validados com feed/cache isolados; versões, hashes e comandos registrados; resultados antigos não reutilizados como evidência atual |
| AC12 | Nenhuma nova dependência de mapping, alteração de regra de negócio, schema, .NET/EF ou arquivo fora do escopo; fontes novos/editados em UTF-8 sem BOM e alterações pré-existentes preservadas |

## 8. Riscos, decisões e tratamento

| Risco/decisão | Tratamento e momento |
| --- | --- |
| Remoção de profiles públicos | Tratar como quebra de API desde a proposta. Antes do release, verificar consumidores conhecidos e registrar migração. Sem compatibilidade fictícia por classes vazias |
| Versões 10.0.1 em preparação | Reconfirmar publicação e coordenação com o outro change. Se a API anterior já foi publicada, usar release major para a quebra; se ainda não foi, alinhar a versão de estreia e repetir integralmente seus gates, sem alterar o histórico neste plano |
| Nulls/Conditions diferentes do esperado | Fase de caracterização precede implementação. Documentar divergência e não corrigir comportamento incidentalmente |
| Coleções e campos omitidos | Valores não padrão, testes nos dois sentidos, matriz completa e proteção contra propriedades novas |
| Uso de pacote Storage antigo | Pack em ordem, versões inéditas e cache isolado; confirmar assets antes de testar consumidores |
| Testes InMemory mascararem problemas EF | Testes SQLite dedicados com releitura em contexto novo; não alegar validação SQL Server sem executá-la |
| Exceções específicas da biblioteca | Documentar substituição por falhas nativas; não engolir dados inválidos nem criar dependência só para preservar wrappers |
| Alterações locais em andamento | Registrar baseline/diff antes de executar; mudar somente trechos necessários e nunca fazer reset/clean automático |

Não há decisão técnica pendente que impeça detalhar a implementação. A versão final e a coordenação da publicação são decisões de release ainda não tomadas. As regras finas de equivalência serão obtidas por testes na primeira fase, sem consultar segredos ou modificar dados reais.

## 9. Entrega e reversão

Entregar commits pequenos e revisáveis: caracterização; grants; recursos/escopos; API resources; clients; remoção da biblioteca/API; validação de pacotes e documentação. Ajustar a divisão para que cada etapa compile e seus testes passem.

Antes da publicação, rollback consiste em reverter somente os commits desta mudança e restaurar as referências de pacote anteriores, preservando trabalho alheio. Não há rollback de banco, pois não há mudança de schema/dados. Após uma publicação futura, publicar correção em nova versão ou orientar retorno à versão anterior; nunca sobrescrever um nupkg publicado.

Publicação, push, merge e alteração do outro change permanecem fora do escopo. A execução autorizada termina com a implementação, as evidências e os dois pacotes locais revisáveis; não promove artefatos para um feed remoto.
