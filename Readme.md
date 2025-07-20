---

# 📦 AWS Parameter Store - Retorno de parametros (.NET)

Este projeto é um exemplo simples de como integrar o **AWS Systems Manager Parameter Store** com uma API ASP.NET Core, utilizando a biblioteca `Amazon.Extensions.Configuration.SystemsManager`.

---

## 🔧 Tecnologias utilizadas

* .NET 8
* AWS Systems Manager (SSM) 
* `Amazon.Extensions.Configuration.SystemsManager` NuGet 3.0.0
* Swagger

---

## 📁 Estrutura do Projeto

* `ParameterStoreController.cs`: Controller com um endpoint que demonstra como acessar os valores do Parameter Store de diferentes formas.
* `AwsParameterCommon.cs`: Classe de configuração com os parâmetros mapeados.
* `Program.cs`: Configuração do SSM provider e injeção de dependência das opções.

---

## 📦 Como o Parameter Store está sendo utilizado

### 📌 1. Instalando a biblioteca necessária

Adicione o pacote via NuGet:

```bash
dotnet add package Amazon.Extensions.Configuration.SystemsManager
```

---

### 📌 2. Configurando o Provider no `Program.cs`

```csharp
var enviroment = builder.Environment.EnvironmentName.ToLower();
builder.Configuration.AddSystemsManager($"/{enviroment}/ssm", TimeSpan.FromSeconds(5));
```

> Isso adiciona o Systems Manager como uma fonte de configuração, com polling automático a cada 5 segundos. O prefixo da chave é baseado no ambiente (Ex: `Development/ssm`, `Production/ssm`, etc).

---

### 📌 3. Mapeando para a classe de configurações

```csharp
builder.Services.Configure<AwsParameterCommon>(builder.Configuration.GetSection("Settings"));
```

> A seção `Settings` é mapeada diretamente da configuração carregada do Parameter Store.

Exemplo de parâmetro no SSM:

```
Name: /development/ssm/Settings:ConnectionString
Type: String
Value: Server=mydb;Database=main;
```

---

## ✅ Como os valores são acessados

O endpoint `/api/v1/parameterstore/connection-string` retorna os valores usando quatro abordagens distintas:

```csharp
return Ok(new
{
    fromOption = _settings.ConnectionString,
    fromConfig = value,
    fromOptionMonitor = _settingsOptionsMonitor.ConnectionString,
    fromOptionSnapshot = _settingsOptionsSnapshot.ConnectionString,
});
```

### 🔍 Comparação das abordagens:

| Abordagem            | Interface usada       | Reflete mudanças no SSM? | Observações                                 |
| -------------------- | --------------------- | ------------------------ | ------------------------------------------- |
| `fromOption`         | `IOptions<T>`         | ❌ Não                    | Carregado na inicialização                  |
| `fromConfig`         | `IConfiguration`      | ✅ Sim                    | Leitura direta do provider                  |
| `fromOptionMonitor`  | `IOptionsMonitor<T>`  | ✅ Sim                    | Escuta mudanças em tempo real               |
| `fromOptionSnapshot` | `IOptionsSnapshot<T>` | ✅ Na próxima requisição  | Escopo por request, útil em serviços scoped |

---

## 🔄 Exemplo de retorno JSON

```json
{
  "fromOption": "Server=mydb;Database=main;",
  "fromConfig": "Server=mydb;Database=main;",
  "fromOptionMonitor": "Server=mydb;Database=main;",
  "fromOptionSnapshot": "Server=mydb;Database=main;"
}
```

---

## 🛡️ Segurança

Certifique-se de que a aplicação tenha permissão no IAM para acessar os parâmetros no SSM:

```json
{
  "Effect": "Allow",
  "Action": [
    "ssm:GetParametersByPath",
    "ssm:GetParameters",
    "ssm:GetParameter"
  ],
  "Resource": "arn:aws:ssm:REGIAO:ID_DA_CONTA:parameter/development/ssm/*"
}
```

---

## 🚀 Executando o projeto

```bash
dotnet run
```

Acesse:
[https://localhost:5001/swagger](https://localhost:5001/swagger)

---

## 📚 Referências

* [Amazon.Extensions.Configuration.SystemsManager - GitHub](https://github.com/aws/aws-sdk-net)
* [Documentação oficial da AWS – Parameter Store](https://docs.aws.amazon.com/systems-manager/latest/userguide/systems-manager-parameter-store.html)
* [Options Pattern em ASP.NET Core](https://learn.microsoft.com/aspnet/core/fundamentals/configuration/options)

---


