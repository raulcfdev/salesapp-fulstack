# SalesApp Fullstack

## Visão Geral
Este repositório contém dois projetos : uma API RESTful desenvolvida em .NET 8 e um Frontend desenvolvido em Angular.

## Funcionalidades Principais

### Backend (.NET 8)
- **Autenticação e Autorização:** Implementação de JWT (JSON Web Tokens). Para simplificar a avaliação via Swagger, a implementação do atributo `Authorize` foi omitida, mas pode ser implementada futuramente.
- **Testes Unitários:** Cobertura de testes para as funcionalidades essenciais.
- **API RESTful:** Endpoints desenvolvidos seguindo as melhores práticas REST.
- **Swagger:** Documentação automática e interativa da API.
- **Entity Framework Core:** Utilizado como ORM para facilitar o acesso ao banco de dados.
- **Integração com RabbitMQ:** As credenciais do RabbitMQ estão definidas no arquivo `appsettings.json`.

### Frontend (Angular)
- **Angular Material:** Componentes visuais padronizados para uma interface consistente.
- **Formulários Reativos:** Validação e gerenciamento avançado de formulários.
- **Serviços:** Comunicação eficiente com a API backend.
- **Interceptors:** Gerenciamento de tokens JWT para as requisições.
- **Lazy Loading:** Otimização do carregamento dos módulos da aplicação.
- **Cadastro de Usuário:** Não há credenciais pré-cadastradas. Ao acessar o frontend, é necessário realizar o cadastro do usuário para acessar os métodos da aplicação corretamente.

## Instruções de Inicialização

### Pré-requisitos
- Visual Studio 2022 (ou superior)
- .NET 8 SDK
- Node.js e npm
- Angular CLI
- RabbitMQ Server

### Iniciando o Backend (.NET 8)
1. Abra o arquivo de solução (`.sln`) no Visual Studio.
2. Configure a conexão com o banco de dados no arquivo `appsettings.json`, definindo a string de conexão, por exemplo:
   - **ConnectionStrings:** `"DefaultConnection": "Server=(seuservidor);Database=Sales_Back;Trusted_Connection=True;TrustServerCertificate=True;"`
3. No Console do Gerenciador de Pacotes, execute o comando para criar o banco de dados (por exemplo, utilizando `Update-Database`).
4. Configure os parâmetros do RabbitMQ no `appsettings.json`. As credenciais de acesso estão definidas neste arquivo.
5. Execute a API pressionando F5 ou clicando em "Iniciar Depuração" no Visual Studio.

### Iniciando o Frontend (Angular)
1. Abra um terminal na pasta do projeto Angular.
2. Execute o comando `npm install` para instalar todas as dependências.
3. Após a instalação, inicie a aplicação com o comando `ng serve`.
4. Acesse a aplicação pelo navegador através do endereço: [http://localhost:4200](http://localhost:4200).
5. Realize o cadastro do usuário diretamente no frontend para acessar as funcionalidades da aplicação.

## Informações Adicionais
- A documentação completa da API está disponível via Swagger em: [https://localhost:7001/swagger](https://localhost:7001/swagger).
- Os testes unitários podem ser executados através do Test Explorer no Visual Studio.
- Certifique-se de que o RabbitMQ está em execução para que a integração ocorra corretamente.
