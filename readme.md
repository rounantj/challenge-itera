# Resumo do Projeto

Este projeto consiste em uma aplicação de gerenciamento de grupos, empresas e custos desenvolvida utilizando a plataforma .NET Core. A aplicação possui um sistema de autenticação de usuários que concede acesso a recursos protegidos através de tokens JWT.

### Requisitos

Para rodar a aplicação, é necessário ter instalado o .NET Core e possuir uma conexão com um banco de dados MySQL.
Alterar no arquivo `appsettings.json` a string de conexão do banco, a database será criada automáticamente.

### Configurações

As configurações da aplicação estão armazenadas em um arquivo chamado appsettings.json. Esse arquivo contém as seguintes configurações:

- Logging: configuração de log da aplicação.
- AllowedHosts: configuração do endereço IP ou nome do host permitido.
- TitleProject: título do projeto utilizado na documentação do Swagger.
- JwtKey: chave secreta utilizada na geração e validação dos tokens JWT.
- ConnectionStrings: string de conexão com o banco de dados MySQL.

### Como testar

Pra testar siga as seguintes etapas:

- Start: Rode a aplicação utilizando o comando `dotnet watch run`.
- Crie um usuário: Utilize o Swagger que abrirá pra criar um usuário.
- Faça login: Faaça login pela rota login do swagger.
- Teste os endpoints: Todos os endpoints solicitados (e outros) estarão disponíveis agora que está autenticado.
- Dados: O Startup roda um seed para criar dados mockados para testes exceto para usuários.

## Detalhes técnicos

### Startup.cs

O arquivo Startup.cs é responsável por configurar a aplicação e adicionar os serviços necessários ao contêiner de injeção de dependência. O arquivo inclui os seguintes métodos:

### ConfigureServices

Este método é responsável por adicionar serviços ao contêiner de injeção de dependência. A configuração inclui:

Configuração do serviço de serialização JSON.
Configuração do serviço de autenticação JWT.
Configuração do serviço do Swagger.
Configuração do serviço do banco de dados MySQL.
Registro dos serviços da aplicação.

### Configure

Este método é responsável por configurar o pipeline de requisições da aplicação. A configuração inclui:

Configuração do ambiente de desenvolvimento.
Configuração do redirecionamento HTTPS.
Configuração do roteamento de requisições.
Configuração da autenticação JWT.
Configuração da autorização de acesso aos recursos protegidos.
Configuração da inicialização do banco de dados e da execução do seeder.

### Conclusão

O arquivo Startup.cs é fundamental para a configuração da aplicação e adição dos serviços necessários ao contêiner de injeção de dependência. Com uma configuração adequada, a aplicação é capaz de atender às necessidades dos usuários e garantir a segurança dos recursos protegidos.

Ps.: O projeto utiliza o Entity Framework Core para interagir com um banco de dados MySQL e é configurado para criar a base de dados automaticamente caso ela ainda não exista. No entanto, é importante lembrar que é necessário que a string de conexão do banco de dados seja configurada no arquivo appsettings.json antes de executar a aplicação.
