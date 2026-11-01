## 0. Preparação

- [x] 0.1 Registrar branch, commit, estado dos arquivos e alterações locais pré-existentes; preservar integralmente o trabalho em andamento.
- [x] 0.2 Confirmar versão efetivamente resolvida dos modelos e AutoMapper, SDK, EF e estado de publicação dos dois pacotes EF.
- [x] 0.3 Identificar consumidores conhecidos dos profiles públicos e registrar a quebra; definir estratégia de versão antes do gate final de release.
- [x] 0.4 Criar staging/cache isolados deste change sem sobrescrever artefatos anteriores; registrar configuração sem credenciais.
- [x] 0.5 Executar baseline de testes atuais e registrar providers, contagens, warnings e skips. Não considerar o histórico como teste atual.

## 1. Caracterização dos contratos

- [x] 1.1 Inventariar todas as propriedades dos cinco modelos, entidades e tipos filhos, incluindo herança e membros deliberadamente excluídos.
- [x] 1.2 Criar matriz de equivalência por direção e ligar cada regra a teste ou exclusão justificada.
- [x] 1.3 Testar valores diferentes dos defaults para cada campo e os dez campos de PersistedGrant; comparar cada direção independentemente.
- [x] 1.4 Caracterizar null na raiz, UpdateEntity com argumentos nulos, coleções nulas/vazias, elementos nulos, duplicatas, ordem e comparadores.
- [x] 1.5 Caracterizar ProtocolType e Secret.Type explicitamente nulos/vazios; registrar eventual divergência entre expectativa e Conditions atuais sem corrigir nesta mudança.
- [x] 1.6 Caracterizar enums, ClientClaim.ValueType, secrets, propriedades duplicadas e campos de entidade sem correspondente.
- [x] 1.7 Caracterizar algoritmos: null/vazio/branco, vírgulas repetidas, espaços por token, duplicatas, diferenças de caixa e itens nulos/vazios.
- [x] 1.8 Consolidar asserts permanentes independentes do AutoMapper e proteção de propriedades novas; confirmar que passam na implementação atual.

## 2. PersistedGrant

- [x] 2.1 Implementar ToModel e ToEntity explícitos para os dez campos com a semântica caracterizada.
- [x] 2.2 Implementar UpdateEntity por atribuições na entidade recebida, incluindo limpeza de campos por null.
- [x] 2.3 Remover mapper estático/profile deste grupo e substituir seu teste de configuração.
- [x] 2.4 Validar identidade de objeto, tracking e StoreAsync com Key existente; executar testes unitários e de store correspondentes.

## 3. IdentityResource e ApiScope

- [x] 3.1 Implementar os campos próprios e herdados nos dois sentidos para IdentityResource.
- [x] 3.2 Implementar claims e propriedades com coleções novas e tratamento caracterizado de null/duplicidade.
- [x] 3.3 Repetir para ApiScope, preservando Required, Emphasize, ShowInDiscoveryDocument e demais campos classificados.
- [x] 3.4 Remover os dois mappers estáticos/profiles e substituir testes de configuração; executar testes dos grupos e ResourceStore.

## 4. ApiResource e algoritmos

- [x] 4.1 Substituir AllowedSigningAlgorithmsConverter por funções internas sem tipos do AutoMapper e executar os casos de caracterização.
- [x] 4.2 Implementar ApiResource nos dois sentidos, incluindo Secrets/ApiSecrets, scopes, claims, propriedades e algoritmos.
- [x] 4.3 Preservar defaults e campos de secrets e não percorrer relações de retorno das entidades.
- [x] 4.4 Remover mapper estático/profile do grupo e substituir teste de configuração; executar testes do grupo e ResourceStore.

## 5. Client

- [x] 5.1 Implementar campos escalares, flags, tempos, logout e defaults conforme a matriz.
- [x] 5.2 Implementar as conversões de enum/inteiro e de algoritmos, sem validação nova de valores legados.
- [x] 5.3 Implementar secrets, claims, grant types, scopes, redirect URIs, post-logout URIs, CORS e restrições de provedores.
- [x] 5.4 Implementar propriedades e preservar condições de erro, cópias de coleções e semântica dos defaults.
- [x] 5.5 Remover mapper estático/profile restante e substituir teste de configuração; executar testes completos de Client e ClientStore.

## 6. Persistência relacional e remoção da dependência

- [x] 6.1 Adicionar testes SQLite dedicados para salvar/reler clientes e recursos com filhos em novo contexto, verificando IDs, FKs e duplicações.
- [x] 6.2 Adicionar teste SQLite de atualização de grant pela mesma Key, com alterações dos campos e limpeza por null, confirmando uma linha ao final.
- [x] 6.3 Remover PackageReference e definição central AutoMapper e qualquer uso residual em produção/testes; não apagar histórico documental.
- [x] 6.4 Confirmar ausência de reflexão/serialização/mapeamento dinâmico em produção e de dependência substituta.
- [x] 6.5 Executar as três suítes EF completas; confirmar providers efetivamente exercitados e ausência de novos skips/warnings.

## 7. Pacotes e consumidores

- [x] 7.1 Empacotar Storage primeiro em versão inédita de staging e inspecionar nuspec, TFM, referências de assembly e dependências.
- [x] 7.2 Restaurar EntityFramework contra Storage novo, confirmar assets e então compilar/testar/empacotar EntityFramework.
- [x] 7.3 Restaurar os consumidores com cache isolado e inspecionar o grafo: zero AutoMapper e versões novas corretas.
- [x] 7.4 Compilar solutions EF, hosts, migrations e quickstart 5; executar consumidor pós-pack sem ProjectReference para bibliotecas validadas.
- [x] 7.5 Executar regressão das suítes UnitTests e IntegrationTests de IdentityServer4 e comparar com baseline.
- [x] 7.6 Confirmar diff de schema/migrations vazio e API das extensões preservada; documentar profiles e exceções removidos.

## 8. Encerramento e preparação de release

- [x] 8.1 Documentar migração, quebra pública, versões e eliminação do licenciamento da dependência removida, preservando atribuições existentes.
- [x] 8.2 Reconciliar a decisão de versão com a release .NET 10 em andamento sem reescrever evidências antigas nem reutilizar seus hashes.
- [x] 8.3 Se mudar de versão de staging para final, reconstruir em ordem e repetir testes pós-pack com os artefatos finais.
- [x] 8.4 Registrar comandos, resultados, providers, grafo transitivo, versões e SHA-256 dos pacotes finais; verificar AC01 a AC12.
- [x] 8.5 Revisar diff e codificação UTF-8 sem BOM, demonstrando que alterações pré-existentes foram preservadas.
- [x] 8.6 Apresentar os artefatos e o plano de reversão para decisão de release. Publicação/push/merge ficam fora desta etapa e deste pedido de planejamento.

## Evidência de execução

Implementação autorizada pelo usuário em 25/10/2026. Resultados, ressalvas de auditoria de dependências, hashes e reversão: [implementation-validation.md](evidence/implementation-validation.md). A fase de caracterização precedeu a alteração de produção; a geração final usou feed e caches isolados. Nenhum commit/push/publicação foi realizado.
