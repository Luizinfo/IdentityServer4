## ADDED Requirements

### Requirement: Os forks usam identidade própria no NuGet
Os 11 pacotes coordenados SHALL usar os PackageIds `ZinfoFramework.*` definidos na matriz do change, sem publicar novas versões sob os IDs pertencentes aos projetos originais.

#### Scenario: Auditoria da identidade dos pacotes
- **WHEN** os `.nuspec` dos 11 nupkgs são inspecionados
- **THEN** todos os IDs começam com `ZinfoFramework.`, correspondem exatamente à matriz aprovada e nenhuma dependência interna usa um PackageId histórico

#### Scenario: Compatibilidade de namespaces e assemblies
- **WHEN** a troca de PackageId é aplicada
- **THEN** namespaces públicos e nomes de assemblies históricos não são renomeados apenas por causa do prefixo NuGet

### Requirement: Metadados identificam o mantenedor e os repositórios atuais
Cada pacote SHALL declarar `Authors` como `Luiz Antonio`, `RepositoryType` como `git`, `RepositoryUrl` como a URL clonável do remote `origin` do repositório correspondente e `PackageProjectUrl` como a página HTTPS desse mesmo repositório em `https://github.com/Luizinfo/`.

#### Scenario: Inspeção de metadados NuGet
- **WHEN** o `.nuspec` de cada pacote é aberto
- **THEN** Authors, RepositoryUrl e PackageProjectUrl correspondem exatamente ao repositório que produz o pacote e não apontam para a organização upstream

#### Scenario: Proveniência da revisão publicada
- **WHEN** o pacote definitivo expõe repository commit ou Source Link
- **THEN** a revisão existe no RepositoryUrl e contém exatamente o código-fonte usado para produzir o nupkg validado

#### Scenario: Descoberta da continuação
- **WHEN** um consumidor visualiza o pacote no NuGet.org
- **THEN** descrição, tags, release notes e README identificam `ZinfoFramework`, Luiz Antonio e a natureza de continuação independente

### Requirement: Documentação explica a migração dos PackageIds
Os READMEs raiz e os READMEs incorporados aos nupkgs SHALL apresentar os IDs históricos e novos, o comando de instalação atual, o requisito `net10.0`, as quebras conhecidas e o caminho de migração dos consumidores.

#### Scenario: Consumidor migra do pacote histórico
- **WHEN** um consumidor consulta a documentação de um pacote `ZinfoFramework.*`
- **THEN** ele consegue identificar qual `PackageReference` histórica substituir, qual novo ID usar e quais namespaces e assemblies permanecem estáveis

### Requirement: A continuação preserva proveniência e créditos
Cada repositório SHALL manter a licença Apache-2.0, copyrights e avisos existentes e SHALL possuir atribuição visível que ligue o fork ao upstream, credite autores e contribuidores originais e diferencie autoria histórica de manutenção atual.

#### Scenario: Revisão de atribuição
- **WHEN** README, LICENSE e documento de atribuição são revisados em conjunto
- **THEN** Luiz Antonio aparece como mantenedor atual sem remover nem assumir os créditos dos autores e contribuidores originais

#### Scenario: Conteúdo incorporado ao pacote
- **WHEN** um nupkg é inspecionado
- **THEN** ele contém ou referencia de forma funcional README, licença e atribuição correspondentes ao repositório e ao pacote publicado

### Requirement: Canais comunitários pertencem ao fork atual
A documentação SHALL orientar issues, contribuições, vulnerabilidades e suporte para recursos atuais do repositório `Luizinfo`, sem inventar endereço de e-mail ou manter instruções operacionais pertencentes ao upstream.

#### Scenario: Auditoria de links e políticas
- **WHEN** READMEs, CONTRIBUTING, SECURITY, suporte, changelog, badges e links são auditados
- **THEN** links operacionais apontam para o fork atual, links upstream úteis são rotulados como históricos e alegações de TFM, manutenção ou certificação refletem o estado validado do fork

#### Scenario: Conformidade não é apresentada como certificação
- **WHEN** resultados da suíte OpenID Foundation são documentados
- **THEN** uma execução Basic RP aprovada é descrita como evidência de conformidade testada, não como certificação oficial do pacote ou do mantenedor
