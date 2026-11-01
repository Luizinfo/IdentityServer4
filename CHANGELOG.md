# Changelog

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
