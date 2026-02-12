# 📊 CampaignBase API

API desenvolvida para processamento e otimização de bases de dados destinadas ao disparo de notificações ativas no WhatsApp através da plataforma **Blip**.

## 📋 Sobre o Projeto

A CampaignBase API facilita o gerenciamento de grandes bases de contatos, dividindo-as em lotes menores conforme a necessidade do usuário, tornando o processo de disparo de mensagens mais eficiente e organizado.

## ✨ Funcionalidades Atuais

- ✅ **Upload de Arquivos**: Suporte para arquivos Excel (`.xls`, `.xlsx`) e CSV (`.csv`)
- ✅ **Particionamento Inteligente**: Divisão automática da base em lotes de:
  - 1.000 registros
  - 2.000 registros
  - 5.000 registros
  - 10.000 registros
- ✅ **Download Automatizado**: Geração de arquivo ZIP contendo todos os lotes processados
- ✅ **Validação de Entrada**: Verificação de formato e tamanho de arquivo
- ✅ **Documentação Interativa**: Interface Swagger para testes e documentação da API

## 🚀 Funcionalidades Futuras

- 🔄 **Validação de Campos em Branco**: Identificação e tratamento de registros com dados incompletos
- 🔄 **Detecção de Duplicatas**: Validação e remoção de números de telefone duplicados
- 🔄 **Relatórios Detalhados**: Geração de relatórios com:
  - Quantidade total de registros processados
  - Número de registros válidos após limpeza
  - Estatísticas de duplicatas removidas
  - Lista de registros inválidos
- 🔄 **Validação de Números**: Verificação de formato de números de telefone

## 🛠️ Tecnologias Utilizadas

- **.NET 10.0**
- **ASP.NET Core Web API**
- **Swagger/OpenAPI** - Documentação da API
- **EPPlus** / **NPOI** - Manipulação de arquivos Excel
- **xUnit** - Testes unitários
- **C#** - Linguagem de programação

## 📦 Pré-requisitos

- [.NET 10.0 SDK](https://dotnet.microsoft.com/download/dotnet/10.0) ou superior
- IDE recomendada: Visual Studio 2022 ou VS Code

## 🔧 Como Executar o Projeto

### 1. Clone o repositório

```bash
git clone https://github.com/Bea-Querubim/CampaignBaseApi.git
cd CampaignBaseApi
```

### 2. Restaure as dependências

```bash
dotnet restore
```

### 3. Execute a aplicação

```bash
cd CampaignBaseAPI
dotnet run
```

### 4. Acesse a documentação Swagger

Abra seu navegador e acesse:
```
http://localhost:5000
```

ou

```
https://localhost:5001
```

## 📌 Como Usar

### Endpoint Principal

**POST** `/api/Sheets/process`

#### Parâmetros

| Campo | Tipo | Descrição | Obrigatório |
|-------|------|-----------|-------------|
| `File` | `IFormFile` | Arquivo Excel ou CSV com a base de contatos | ✅ Sim |
| `Size` | `enum` | Tamanho dos lotes: `um_mil`, `dois_mil`, `cinco_mil`, `dez_mil` | ✅ Sim |

#### Exemplo de Requisição (cURL)

```bash
curl -X POST "https://localhost:5001/api/Sheets/process" \
  -H "Content-Type: multipart/form-data" \
  -F "File=@base_contatos.xlsx" \
  -F "Size=dois_mil"
```

#### Resposta de Sucesso (200 OK)

Retorna um arquivo ZIP contendo os arquivos particionados da base original.

```
base_contatos.zip
├── base_contatos_parte_1.xlsx
├── base_contatos_parte_2.xlsx
└── base_contatos_parte_3.xlsx
```

#### Possíveis Erros

- **400 Bad Request**: Arquivo vazio ou formato inválido
- **500 Internal Server Error**: Erro no processamento

## 📂 Estrutura do Projeto

```
CampaignBaseAPI/
├── Controllers/          # Controladores da API
├── Services/            # Lógica de negócio
├── Facades/             # Camada de fachada
├── DTOs/                # Objetos de transferência de dados
├── Models/              # Modelos de domínio
├── Enums/               # Enumerações
├── Constants/           # Constantes da aplicação
└── Properties/          # Configurações de launch

CampaignBaseAPI.Tests/   # Projeto de testes
├── Services/            # Testes dos serviços
└── TestResults/         # Resultados e cobertura de testes
```

## 🧪 Executar Testes

```bash
dotnet test
```

### Executar testes com cobertura

```bash
dotnet test --collect:"XPlat Code Coverage"
```

## 🤝 Como Contribuir

1. Faça um fork do projeto
2. Crie uma branch para sua feature (`git checkout -b feature/MinhaFeature`)
3. Commit suas mudanças (`git commit -m 'Adiciona MinhaFeature'`)
4. Faça push para a branch (`git push origin feature/MinhaFeature`)
5. Abra um Pull Request

## 📝 Padrões de Código

- Seguir convenções do C# e .NET
- Escrever testes unitários para novas funcionalidades
- Documentar métodos e classes complexas
- Manter a cobertura de testes acima de 80%

## 📄 Licença

Este projeto está sob a licença MIT. Veja o arquivo [LICENSE](LICENSE) para mais detalhes.

## 👩‍💻 Autora

**Beatriz Querubim**

- GitHub: [@Bea-Querubim](https://github.com/Bea-Querubim)

## 📞 Suporte

Se você tiver alguma dúvida ou sugestão, sinta-se à vontade para abrir uma [issue](https://github.com/Bea-Querubim/CampaignBaseApi/issues).

---

⭐ Se este projeto te ajudou, considere dar uma estrela!
