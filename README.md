# Проект по курсу "Domain Driven Design и Clean Architecture на языке C#"

📚 Подробнее о курсе: [microarch.ru/courses/ddd/languages/csharp](https://microarch.ru/courses/ddd/languages/csharp?utm_source=gitlab&utm_medium=repository)

---

---

## Инфраструктура

[Поднять всю зависимую инфру через docker-compose ](https://gitlab.com/microarch-ru/ddd-in-practice/infrastructure)

---

# OpenApi 
Вызывать из папки DeliveryApp.Api/Adapters/Http/Contract
```
cd DeliveryApp.Api/Adapters/Http/Contract/
openapi-generator generate -i https://gitlab.com/microarch-ru/microservices/dotnet/system-design/-/raw/main/services/delivery/contracts/openapi.yml -g aspnetcore -o . --package-name OpenApi --additional-properties classModifier=abstract --additional-properties operationResultTask=true
```
Для запуска генерации Api в изолирвованном Docker контейнере (без установки OpenAPI Generator CLI)
```
cd DeliveryApp.Api/Adapters/Http/Contract/
docker run --rm -v ${PWD}:/local openapitools/openapi-generator-cli generate `
    -i https://gitlab.com/microarch-ru/microservices/dotnet/system-design/-/raw/main/services/delivery/contracts/openapi.yml `
    -g aspnetcore `
    -o /local `
    --package-name OpenApi `
    --additional-properties classModifier=abstract `
    --additional-properties operationResultTask=true
```
# БД
```
dotnet tool install --global dotnet-ef
dotnet tool update --global dotnet-ef
dotnet add package Microsoft.EntityFrameworkCore.Design
```
[Подробнее про dotnet cli](https://learn.microsoft.com/ru-ru/ef/core/cli/dotnet)

## Миграции
Накатятся автоматом благодаря:
```
// Apply Migrations
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.Migrate();
}
```
Или:
```
dotnet ef migrations add Init --startup-project ./DeliveryApp.Api --project ./DeliveryApp.Infrastructure --output-dir ./Adapters/Postgres/Migrations
dotnet ef database update --startup-project ./DeliveryApp.Api --connection "Server=localhost;Port=5432;User Id=username;Password=secret;Database=delivery;"
```
