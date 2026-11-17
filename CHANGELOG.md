# Changelog

## Não publicado — prontidão para PR

- Exige autenticação do certificado em todos os endpoints mTLS, inclusive os endpoints comuns, e rejeita configuração sem o esquema registrado.
- Exige cadeia emitida por CA confiável para secrets X509Name, preservando suporte explícito a certificados autoassinados por thumbprint e adicionando política de confiança para CAs privadas.
- Rejeita caracteres de controle em URLs de retorno para corrigir o redirecionamento aberto herdado.
- Corrige o executor compartilhado para Bullseye 6 e propaga falhas nos scripts PowerShell.
- Fixa SDK 10.0.401 local e na CI e usa o SourceLink corrigido incluído no SDK.
- Acrescenta testes de regressão de segurança e validação de exemplos, dependências transitivas e migrações SQL Server em bancos exclusivos de teste.
- Corrige a dependência transitiva vulnerável de PKCS nos exemplos com a versão 10.0.13.
- Alinha host e ferramentas de migração AspNetIdentity ao EF Core 10, mantendo EF Core 9.0.9 como mínimo dos pacotes de persistência e preservando os modelos existentes.
- Não altera schema, namespaces ou as versões já publicadas. Uma promoção futura exige novas versões e artefatos finais.


## 11.0.0 — pacotes EntityFramework

- Substitui AutoMapper por C# explícito nos mapeamentos de clientes, recursos de API, recursos de identidade, escopos e grants persistidos.
- Remove a dependência direta e transitiva de AutoMapper dos dois pacotes EntityFramework.
- **Breaking:** remove os cinco tipos públicos `*MapperProfile` e os wrappers de exceção específicos do AutoMapper. Mantém as extensões públicas de conversão.
- Preserva .NET 10, EF Core 9.0.9, os modelos e o schema persistido; os demais pacotes IdentityServer4 permanecem em 10.0.0.
- Amplia os testes de equivalência e adiciona validação relacional com SQLite, incluindo atualização de grant existente com limpeza de campos nulos.

## 10.0.0

- Continuação independente sob os PackageIds `ZinfoFramework.IdentityServer4*`.
- Runtime e assets públicos atualizados exclusivamente para `net10.0`.
- Dependências Microsoft, Entity Framework Core, JWT e testes alinhadas ao .NET 10.
- Dependência interna migrada para `ZinfoFramework.IdentityModel` 10.0.0.
- Namespaces, nomes de assemblies, fluxos OAuth/OIDC e schema persistido preservados.
