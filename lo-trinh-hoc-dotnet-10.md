# Lộ trình học .NET 10 — 6 tuần

**Dành cho:** developer đã có kinh nghiệm .NET Framework (ASP.NET MVC, Web API 2, EF6/EDMX)
**Thời lượng:** ~2 giờ/ngày × 5 ngày/tuần = ~60 giờ
**Nguyên tắc:** mỗi ngày phải có code chạy được. Không đọc trước rồi làm sau.

## Cách dùng plan này

Mỗi ngày có 4 phần:

- **Mục tiêu** — một câu, hết ngày phải trả lời được
- **Đọc** (~40 phút) — chủ đề cụ thể, ưu tiên `learn.microsoft.com`, chọn version .NET 10
- **Làm** (~80 phút) — code cụ thể
- **Xong khi** — tiêu chí kiểm tra, tự đánh giá thật, đừng cho qua

Nếu chỉ có 1 giờ/ngày: giãn thành 12 tuần, giữ nguyên thứ tự.
Nếu có 4 giờ/ngày: gộp 2 ngày làm 1, thành 3 tuần.
**Không đảo thứ tự tuần.** Tuần 2 là tiền đề của tất cả các tuần sau.

## Project xuyên suốt: MiniCRM

Toàn bộ 6 tuần build **một** hệ thống, mỗi tuần thêm một tầng. Domain đơn giản để không phải nghĩ về nghiệp vụ:

- `Customer` (Id, Name, Email, CreatedAt, IsActive)
- `Contact` (Id, CustomerId, Type, Value)
- `Note` (Id, CustomerId, Content, AuthorId, CreatedAt)

Cuối 6 tuần bạn có: API có auth, EF Core + SQL Server, chạy trong Docker, có test, có health check và log có cấu trúc.

**Quan trọng:** không dùng project thật của công ty làm bài tập học. Sẽ lẫn giữa "khó vì .NET Core khác" và "khó vì code cũ tệ".

---

# Ngày 0 — Chuẩn bị môi trường (1 giờ)

- Cài **.NET 10 SDK**, verify: `dotnet --info` (kiểm tra SDK version và runtime list)
- Cài IDE: Visual Studio 2026, hoặc VS Code + C# Dev Kit, hoặc Rider. **Gợi ý: dùng VS Code cho tuần 1–2** để không bị Visual Studio che giấu cơ chế project/CLI.
- Cài Docker Desktop (cần từ tuần 4)
- Tạo repo git `minicrm`, commit rỗng đầu tiên
- Tạo file `PROGRESS.md` trong repo, mỗi ngày ghi 3 dòng: đã học gì, chỗ nào chưa hiểu, câu hỏi còn treo

> Việc ghi "chỗ nào chưa hiểu" là phần giá trị nhất của plan này. Cuối mỗi tuần đọc lại và giải quyết các câu hỏi treo.

---

# TUẦN 1 — Tooling & Project System

Mục tiêu tuần: hiểu project system mới đủ để không bao giờ phải Google "vì sao build lỗi" nữa.

## Ngày 1 — dotnet CLI

**Mục tiêu:** làm được toàn bộ vòng đời project bằng dòng lệnh, không cần IDE.

**Đọc:** .NET CLI overview; `dotnet new`, `build`, `run`, `test`, `publish`, `add`, `sln`. Khái niệm SDK vs Runtime, cài side-by-side nhiều version.

**Làm:**
```bash
mkdir minicrm && cd minicrm
dotnet new sln -n MiniCrm
dotnet new classlib -o src/MiniCrm.Domain
dotnet new console  -o src/MiniCrm.Cli
dotnet new xunit    -o tests/MiniCrm.Domain.Tests
dotnet sln add src/**/*.csproj tests/**/*.csproj
dotnet add src/MiniCrm.Cli reference src/MiniCrm.Domain
dotnet add tests/MiniCrm.Domain.Tests reference src/MiniCrm.Domain
dotnet build && dotnet test
```
Chạy `dotnet new list` xem có những template gì. Thử `dotnet run --project src/MiniCrm.Cli`.

**Xong khi:** giải thích được `dotnet build` khác `dotnet publish` ở đâu, và vì sao `bin/Debug/net10.0/` có nhiều file hơn bạn tưởng.

## Ngày 2 — SDK-style csproj & TFM

**Mục tiêu:** đọc và viết được file csproj bằng tay.

**Đọc:** SDK-style project format; Target Framework Monikers; `netstandard2.0` vs `net10.0`; implicit usings; `Nullable` property; multi-targeting.

**Làm:**
- Mở `MiniCrm.Domain.csproj`, so sánh với một csproj .NET Framework 4.6 trong project cũ của bạn. Viết ra 5 điểm khác biệt vào `PROGRESS.md`.
- Bật `<TreatWarningsAsErrors>true</TreatWarningsAsErrors>` và `<Nullable>enable</Nullable>` cho cả solution.
- Thử multi-target: đổi `MiniCrm.Domain` sang `<TargetFrameworks>net48;net10.0</TargetFrameworks>`, build, xem 2 folder output. Thử `#if NET10_0_OR_GREATER`. Sau đó revert về `net10.0`.
- Tạo `Directory.Build.props` ở root, gom các setting chung vào đó.

**Xong khi:** viết được một csproj từ đầu bằng tay mà build được, và giải thích được khi nào cần `netstandard2.0`.

## Ngày 3 — NuGet & quản lý dependency

**Mục tiêu:** hiểu mô hình package mới, khác `packages.config` chỗ nào.

**Đọc:** `PackageReference` vs `packages.config`; transitive dependency; version range; **Central Package Management** (`Directory.Packages.props`); `dotnet list package --outdated` và `--vulnerable`; `global.json`.

**Làm:**
- Thêm `Directory.Packages.props`, chuyển toàn bộ version package lên đó.
- Cài `FluentAssertions` (hoặc `Shouldly`) cho test project, viết lại assertion trong test cho quen.
- Chạy `dotnet list package --include-transitive` — quan sát cây phụ thuộc.
- Tạo `global.json` pin SDK version.
- **Bài tập gắn với việc thật:** chạy `dotnet list package --vulnerable` trên project .NET Framework cũ của bạn (đã convert PackageReference hoặc chưa cũng chạy được với `nuget`), ghi lại kết quả.

**Xong khi:** hiểu vì sao `packages.config` biến mất và transitive dependency giải quyết vấn đề gì.

## Ngày 4 — Domain code + test, làm quen C# hiện đại

**Mục tiêu:** viết code C# hiện đại đầu tiên, cảm nhận nullable reference types.

**Đọc:** `record` vs `class`; `init`-only setter; `required` member; primary constructor; file-scoped namespace; target-typed `new`; collection expression.

**Làm:** trong `MiniCrm.Domain`:
- `Customer`, `Contact`, `Note` — thử cả `record` và `class`, cảm nhận khác biệt về equality
- Enum `ContactType { Email, Phone, Address }`
- `Result<T>` type đơn giản (Success/Failure) — không dùng exception cho validation
- `CustomerValidator` trả về `Result` — dùng pattern matching và switch expression
- `ICustomerRepository` + `InMemoryCustomerRepository`
- Test: ít nhất 8 unit test, có cả case validation fail

**Xong khi:** compiler không còn warning nullable nào, và bạn hiểu vì sao nó warning ở những chỗ đó.

## Ngày 5 — Tổng kết + nullable reference types cho nghiêm túc

**Mục tiêu:** biến nullable từ "cái warning phiền" thành công cụ.

**Đọc:** Nullable reference types đầy đủ — nullable context, `?`, `!` (null-forgiving), attribute `[NotNull]`/`[MaybeNull]`/`[MemberNotNull]`, cách xử lý khi dùng library chưa annotate.

**Làm:**
- Đi lại toàn bộ code tuần 1, bỏ hết `!` mà bạn đã dùng để "cho qua". Nếu buộc phải dùng, viết comment giải thích vì sao.
- Refactor `Result<T>` để nullable-correct hoàn toàn.
- Viết vào `PROGRESS.md`: 3 bug loại `NullReferenceException` mà bạn từng gặp trong project cũ, và nullable reference types có bắt được nó hay không.

**Checkpoint tuần 1 — tự trả lời không cần tra:**
1. Vì sao `netstandard2.0` vẫn còn hữu ích khi đã có `net10.0`?
2. `dotnet publish --self-contained` khác gì mặc định?
3. Cây transitive dependency giải quyết vấn đề gì mà `packages.config` không?
4. `!` (null-forgiving) làm gì ở runtime? (Đáp án đúng khiến nhiều người bất ngờ.)

---

# TUẦN 2 — Host, DI, Configuration, Logging

**Đây là tuần quan trọng nhất của cả 6 tuần.** Đây là phần .NET Framework hoàn toàn không có, và là nền của ASP.NET Core, EF Core, background service, testing. Nếu tuần này học lỏng, tất cả các tuần sau sẽ chỉ là copy-paste.

## Ngày 6 — Generic Host

**Mục tiêu:** hiểu vòng đời ứng dụng .NET hiện đại.

**Đọc:** .NET Generic Host; `Host.CreateApplicationBuilder`; `IHost`, `IHostedService`, `BackgroundService`; graceful shutdown; `IHostApplicationLifetime`.

**Làm:**
- `dotnet new worker -o src/MiniCrm.Worker`, đọc từng dòng `Program.cs`
- Viết một `BackgroundService` in ra số customer trong repo mỗi 5 giây
- Xử lý `CancellationToken` cho đúng — Ctrl+C phải shutdown sạch, không cắt giữa vòng lặp
- Thêm log khi start và khi stop bằng `IHostApplicationLifetime`
- **Đối chiếu:** ghi lại `Global.asax` `Application_Start` / `Application_End` tương ứng với cái gì ở đây

**Xong khi:** Ctrl+C shutdown sạch, và bạn giải thích được ai gọi `ExecuteAsync`.

## Ngày 7 — Dependency Injection (phần 1: cơ chế)

**Mục tiêu:** hiểu container, không chỉ biết dùng.

**Đọc:** DI in .NET; `IServiceCollection`, `IServiceProvider`; ba lifetime Singleton/Scoped/Transient; constructor injection; `IServiceScopeFactory`.

**Làm:**
- Register `ICustomerRepository` → `InMemoryCustomerRepository` với cả 3 lifetime, mỗi lần in ra `GetHashCode()` của instance để **thấy** khác biệt
- Viết `ICustomerService` phụ thuộc `ICustomerRepository` + `ILogger`, inject 3 tầng lồng nhau
- Trong `BackgroundService` (Singleton), thử inject một Scoped service → quan sát exception, đọc kỹ message
- Sửa đúng cách bằng `IServiceScopeFactory` + `CreateScope()`
- Thử register nhiều implementation cho cùng interface, inject `IEnumerable<IValidator>`

**Xong khi:** giải thích được lỗi "Cannot consume scoped service from singleton" mà không cần Google, và biết 2 cách sửa.

## Ngày 8 — Dependency Injection (phần 2: pattern & bẫy)

**Mục tiêu:** viết được DI setup ở mức production.

**Đọc:** `TryAdd*`; keyed services (`AddKeyedSingleton`); factory registration; `IOptions` với DI; captive dependency; `IDisposable` và ai gọi `Dispose`; extension method `IServiceCollection` để gom registration.

**Làm:**
- Viết extension `services.AddMiniCrmDomain()` gom toàn bộ registration của Domain layer
- Dùng keyed service: 2 implementation `INotificationSender` (email/sms), resolve theo key
- Viết một service implement `IDisposable`, register Scoped, verify `Dispose` được gọi khi scope kết thúc
- **Đối chiếu:** viết vào `PROGRESS.md` so sánh với Unity/Ninject/Autofac nếu project cũ của bạn có dùng

**Xong khi:** biết `AddSingleton<IFoo, Foo>()` khác `AddSingleton<IFoo>(sp => new Foo(...))` khi nào thì quan trọng.

## Ngày 9 — Configuration & Options pattern

**Mục tiêu:** không bao giờ dùng `ConfigurationManager.AppSettings` nữa.

**Đọc:** Configuration in .NET — providers và thứ tự ưu tiên (`appsettings.json` → `appsettings.{Environment}.json` → User Secrets → environment variables → command line); binding; **Options pattern**: `IOptions<T>` vs `IOptionsSnapshot<T>` vs `IOptionsMonitor<T>`; options validation.

**Làm:**
- Tạo `appsettings.json` với section `MiniCrm: { PageSize, EnableNotifications, ConnectionStrings }`
- Bind sang `MiniCrmOptions` bằng `services.Configure<MiniCrmOptions>(...)`
- Thêm validation: `.ValidateDataAnnotations().ValidateOnStart()` — cố tình để config sai, xem app fail lúc startup thay vì lúc runtime
- Override cùng một key bằng 4 nguồn khác nhau (json → json env → env var → CLI arg), verify thứ tự ưu tiên bằng log
- Dùng `dotnet user-secrets set` cho một secret
- Thử `IOptionsMonitor` + sửa `appsettings.json` lúc app đang chạy → quan sát reload

**Xong khi:** giải thích được khi nào dùng `IOptions`, khi nào `IOptionsSnapshot`, khi nào `IOptionsMonitor`. (Chọn sai là nguồn bug âm thầm rất phổ biến.)

## Ngày 10 — Logging có cấu trúc

**Mục tiêu:** log để truy vấn được, không phải log để đọc bằng mắt.

**Đọc:** Logging in .NET; `ILogger<T>`; log level; **message template vs string interpolation** (đây là điểm cốt lõi); log scope; `LoggerMessage` source generator; filter theo category; Serilog để tham khảo.

**Làm:**
- Thay tất cả `Console.WriteLine` bằng `ILogger<T>`
- Viết log **sai cách** (`_logger.LogInformation($"Customer {id} created")`) rồi **đúng cách** (`_logger.LogInformation("Customer {CustomerId} created", id)`), so sánh output JSON
- Dùng `using _logger.BeginScope(...)` để gắn correlation id vào toàn bộ log của một operation
- Dùng `[LoggerMessage]` source generator cho 2 log message nóng
- Cấu hình log level khác nhau theo category trong `appsettings.json`

**Xong khi:** hiểu vì sao string interpolation trong log là anti-pattern, và log của bạn có thể query được theo `CustomerId`.

**Checkpoint tuần 2 — mốc quan trọng nhất:**
1. Vẽ ra giấy vòng đời `IHost` từ `CreateApplicationBuilder` đến shutdown.
2. Giải thích captive dependency là gì và vì sao nguy hiểm.
3. Kể 4 nguồn config theo đúng thứ tự ưu tiên.
4. Vì sao `IOptionsSnapshot` không dùng được trong Singleton?

> Nếu có câu nào trả lời không chắc: dành thêm 1–2 ngày ở đây. Đừng sang tuần 3. Chi phí học lại sau đắt hơn nhiều.

---

# TUẦN 3 — ASP.NET Core

Từ tuần này kinh nghiệm MVC/Web API của bạn bắt đầu phát huy. Trọng tâm là **hai pipeline MVC 5 và Web API 2 giờ hợp nhất thành một** — mọi thứ cross-cutting phải học lại một lần.

## Ngày 11 — WebApplication & middleware pipeline

**Mục tiêu:** hiểu pipeline như chuỗi delegate lồng nhau, không phải chuỗi event.

**Đọc:** ASP.NET Core fundamentals; `WebApplication.CreateBuilder`; middleware; `Use` vs `Run` vs `Map`; thứ tự middleware và vì sao nó quan trọng; built-in middleware list.

**Làm:**
- `dotnet new webapi -o src/MiniCrm.Api --use-controllers`, đọc từng dòng `Program.cs`
- Viết 3 middleware inline bằng `app.Use(async (ctx, next) => ...)`, in ra log trước và sau `next()` để **thấy** cấu trúc lồng nhau
- Viết một middleware thành class riêng: `RequestTimingMiddleware` (log thời gian xử lý request) + extension `UseRequestTiming()`
- Cố tình đặt middleware sai thứ tự để thấy nó hỏng thế nào
- **Đối chiếu:** viết bảng `HttpModule` / `HttpHandler` / `DelegatingHandler` (Web API 2) tương ứng với cái gì

**Xong khi:** vẽ được pipeline của app mình và giải thích vì sao `UseAuthentication` phải sau `UseRouting` và trước `UseAuthorization`.

## Ngày 12 — Routing & Controllers

**Mục tiêu:** nắm controller-based API trong mô hình hợp nhất.

**Đọc:** Endpoint routing; attribute routing; `ControllerBase` vs `Controller`; `[ApiController]` và các hành vi tự động của nó; `IActionResult` vs `ActionResult<T>`; `Results`/`TypedResults`; Minimal API (đọc để biết, chưa dùng).

**Làm:**
- `CustomersController` với đầy đủ CRUD trên `IInMemoryCustomerRepository`
- Route: `[Route("api/[controller]")]`, route constraint (`{id:int:min(1)}`), route với nhiều segment
- Trả về đúng status code: 200/201 (kèm `Location` header)/204/404/400
- Xóa `[ApiController]` rồi thêm lại, quan sát khác biệt về xử lý ModelState invalid
- **Đối chiếu:** viết bảng chuyển đổi từ Web API 2: `ApiController`→?, `IHttpActionResult`→?, `Request.CreateResponse()`→?, `HttpResponseMessage`→?

**Xong khi:** giải thích được `[ApiController]` bật những hành vi gì, và biết cách tắt/tuỳ biến response 400 tự động.

## Ngày 13 — Model binding, validation, System.Text.Json

**Mục tiêu:** kiểm soát được input/output, tránh bẫy JSON.

**Đọc:** Model binding; `[FromBody]`/`[FromQuery]`/`[FromRoute]`/`[FromServices]`; validation với DataAnnotations; `ProblemDetails`; **System.Text.Json**: naming policy, converter, `JsonSerializerOptions`, khác biệt so với Newtonsoft.

**Làm:**
- Tách DTO riêng khỏi domain entity (`CreateCustomerRequest`, `CustomerResponse`)
- Validation bằng DataAnnotations, quan sát `ProblemDetails` response
- Viết một `JsonConverter` tuỳ biến (ví dụ cho `DateOnly` hoặc format tiền tệ)
- **Bài tập quan trọng:** liệt kê 5 khác biệt hành vi giữa `System.Text.Json` và Newtonsoft mà có thể phá vỡ API client hiện có (gợi ý: casing, số trong string, trailing comma, comment, `TypeNameHandling`)
- Cài `Microsoft.AspNetCore.Mvc.NewtonsoftJson`, bật lên, so sánh output — biết đường lùi khi cần

**Xong khi:** biết chính xác cần đổi gì để `System.Text.Json` sinh JSON giống Web API 2 cũ.

## Ngày 14 — Filters, exception handling, HttpContext

**Mục tiêu:** làm được cross-cutting concern đúng cách.

**Đọc:** Filters (Authorization/Resource/Action/Exception/Result) và thứ tự thực thi; filter vs middleware — chọn cái nào; `IExceptionHandler`; `UseExceptionHandler`; `ProblemDetails` cho lỗi; `IHttpContextAccessor`.

**Làm:**
- Viết `ActionFilter` log tên action + tham số, register global và register theo attribute
- Viết `ExceptionFilter` hoặc `IExceptionHandler` map domain exception → status code + `ProblemDetails`
- Filter có dependency: dùng `[ServiceFilter]` / `[TypeFilter]`
- Viết `ICurrentUserService` dùng `IHttpContextAccessor`, và **viết vào `PROGRESS.md` vì sao cách này vẫn kém hơn truyền tham số tường minh**
- **Đối chiếu:** MVC 5 có `ActionFilterAttribute` riêng và Web API 2 có bản riêng — giờ chỉ còn một; ghi lại các filter cũ trong project bạn cần viết lại

**Xong khi:** giải thích được khi nào dùng middleware, khi nào dùng filter, và thứ tự chạy của các loại filter.

## Ngày 15 — Testing ASP.NET Core

**Mục tiêu:** test được API mà không cần deploy — điều gần như bất khả thi với ASP.NET Framework.

**Đọc:** Integration test trong ASP.NET Core; `WebApplicationFactory<T>`; `TestServer`; override service trong test; unit test controller.

**Làm:**
- Viết integration test cho toàn bộ CRUD bằng `WebApplicationFactory<Program>` — gọi HTTP thật, không mock
- Override `ICustomerRepository` bằng fake trong test
- Test cả happy path và error path (404, 400 validation)
- Đo: `dotnet test` phải chạy xong dưới 5 giây

**Xong khi:** có ≥10 integration test xanh, chạy được trên máy sạch chỉ với `dotnet test`.

**Checkpoint tuần 3:**
1. Vẽ pipeline từ lúc request đến khi vào action method, đi qua những gì.
2. Middleware vs filter — tiêu chí chọn?
3. `[ApiController]` bật những hành vi nào?
4. Ba khác biệt `System.Text.Json` có thể phá vỡ client hiện có?

---

# TUẦN 4 — EF Core 10

Tuần này kinh nghiệm EDMX của bạn là **con dao hai lưỡi**: bạn hiểu ORM rất sâu, nhưng phản xạ "designer + database-first + lazy loading mặc định" cần gạt sang một bên. **Học code-first trước cho sạch đầu**, database-first để cuối tuần.

## Ngày 16 — DbContext & code-first

**Mục tiêu:** dựng được data layer EF Core từ đầu.

**Đọc:** EF Core overview; `DbContext`; `AddDbContext` và **DbContext lifetime là Scoped**; `DbSet<T>`; convention mapping; Fluent API vs Data Annotations; `IEntityTypeConfiguration<T>`.

**Làm:**
- Chạy SQL Server trong Docker: `docker run -e "ACCEPT_EULA=Y" -e "MSSQL_SA_PASSWORD=Your_Strong_P@ss" -p 1433:1433 -d mcr.microsoft.com/mssql/server:2022-latest`
- Cài `Microsoft.EntityFrameworkCore.SqlServer` + `Microsoft.EntityFrameworkCore.Design`
- `MiniCrmDbContext` với 3 `DbSet`, mapping bằng `IEntityTypeConfiguration<T>` riêng cho từng entity
- `AddDbContext` + connection string từ configuration
- **Đối chiếu:** viết vào `PROGRESS.md` — mỗi thứ bạn từng làm bằng chuột trong EDMX designer, giờ tương ứng với dòng Fluent API nào

**Xong khi:** hiểu vì sao `DbContext` phải là Scoped, và điều gì xảy ra nếu register Singleton.

## Ngày 17 — Migrations

**Mục tiêu:** quản lý schema bằng code — thứ EDMX database-first không cho bạn.

**Đọc:** EF Core Migrations; `dotnet ef` tool; `migrations add`/`update`/`script`/`remove`; migration trong CI/CD; idempotent script; `MigrateAsync` lúc startup (và vì sao không nên dùng ở production).

**Làm:**
- `dotnet tool install --global dotnet-ef`
- `dotnet ef migrations add InitialCreate`, **đọc kỹ file migration được sinh ra** — cả `Up`, `Down` và snapshot
- `dotnet ef database update`, kiểm tra schema trong DB
- Thêm một property, tạo migration thứ hai, xem diff
- `dotnet ef migrations script --idempotent -o migrate.sql`, đọc file SQL
- Viết seed data bằng `HasData` hoặc seeding code

**Xong khi:** giải thích được `__EFMigrationsHistory` làm gì và migration khác `Database.EnsureCreated()` chỗ nào.

## Ngày 18 — Query: những chỗ EF6 và EF Core khác nhau nhất

**Mục tiêu:** không mang phản xạ EF6 sang gây bug âm thầm. **Đây là ngày quan trọng nhất tuần 4.**

**Đọc:** Querying data; `Include`/`ThenInclude`; projection; `AsNoTracking`; **client vs server evaluation**; query splitting vs single query; `AsSplitQuery`; tracking behavior; bật log SQL.

**Làm:**
- Bật `LogTo(Console.WriteLine)` + `EnableSensitiveDataLogging()` (chỉ dev) — **để mở suốt tuần này**
- Viết query có `Include` nhiều tầng, đọc SQL sinh ra, quan sát cartesian explosion
- Áp `AsSplitQuery()`, so sánh SQL
- So sánh `Include` toàn bộ entity vs projection sang DTO — đọc SQL của cả hai
- **Cố tình tạo lỗi client evaluation:** viết query dùng một C# method không dịch được sang SQL. EF6 sẽ âm thầm kéo cả bảng về máy; EF Core **throw exception**. Đọc kỹ message.
- Tạo tình huống N+1, quan sát nó trong log SQL, rồi sửa
- Thử `AsNoTracking` và đo khác biệt

**Xong khi:** đọc được SQL do EF Core sinh và chỉ ra được vì sao một query bị N+1. Và bạn coi "EF Core throw thay vì client-evaluate" là **tính năng**, không phải bất tiện.

## Ngày 19 — Change tracking, transaction, concurrency

**Mục tiêu:** hiểu tầng ghi dữ liệu.

**Đọc:** Change tracking; `EntityState`; `SaveChangesAsync`; transaction (`BeginTransactionAsync`, execution strategy khi bật retry); optimistic concurrency với `RowVersion`; `ExecuteUpdateAsync`/`ExecuteDeleteAsync` (bulk operation, không cần load entity); interceptor.

**Làm:**
- Thêm `RowVersion` (`[Timestamp]`) vào `Customer`, tạo tình huống `DbUpdateConcurrencyException` và xử lý
- Viết một operation cần transaction bao nhiều `SaveChanges`
- Dùng `ExecuteUpdateAsync` cho một bulk update, so sánh SQL với cách load-rồi-sửa
- Viết một `SaveChangesInterceptor` tự động set `CreatedAt`/`UpdatedAt`
- Bật `EnableRetryOnFailure` và tìm hiểu vì sao nó xung đột với transaction thủ công

**Xong khi:** biết `ExecuteUpdateAsync` khác `SaveChanges` ở chỗ nào về change tracking, và khi nào dùng cái nào.

## Ngày 20 — Database-first & cắm EF Core vào API

**Mục tiêu:** biết đường đi từ database có sẵn — trực tiếp liên quan đến project EDMX của bạn.

**Đọc:** Reverse engineering (`dotnet ef dbcontext scaffold`); T4 template tuỳ biến cho scaffold; **EF Core không hỗ trợ EDMX/visual designer** (khác biệt kiến trúc, không phải "chưa hỗ trợ"); công cụ EF Core Power Tools; stored procedure với `FromSql`/`SqlQuery<T>`; lazy loading proxies.

**Làm:**
- Scaffold lại từ DB vừa tạo: `dotnet ef dbcontext scaffold "..." Microsoft.EntityFrameworkCore.SqlServer -o Scaffolded`, so sánh code sinh ra với code code-first bạn viết tay
- Gọi một stored procedure bằng `FromSql`
- Bật lazy loading proxies (`Microsoft.EntityFrameworkCore.Proxies` + `UseLazyLoadingProxies()` + navigation `virtual`), quan sát N+1 xuất hiện — hiểu vì sao EF Core không bật mặc định như EDMX
- Thay `InMemoryCustomerRepository` bằng EF Core trong API, chạy lại toàn bộ integration test tuần 3
- Cho integration test dùng SQL Server trong container hoặc `Microsoft.EntityFrameworkCore.InMemory` (biết giới hạn của InMemory)

**Xong khi:** test tuần 3 vẫn xanh với EF Core thật. Và bạn viết được vào `PROGRESS.md` một đoạn đánh giá: với EDMX hiện tại của mình, đi EF Core hay giữ EF6 — **lúc này bạn đã đủ kiến thức để tự quyết định**.

**Checkpoint tuần 4:**
1. Vì sao `DbContext` là Scoped?
2. Client evaluation — EF6 và EF Core xử lý khác nhau thế nào, hệ quả?
3. `Include` vs projection — chọn cái nào khi nào?
4. Ba thứ EDMX làm được mà EF Core không có tương đương trực tiếp?

---

# TUẦN 5 — Auth, HttpClient, Cache, và các mảnh còn lại

## Ngày 21 — Authentication: khái niệm & cookie

**Mục tiêu:** hiểu mô hình auth mới — đây là phần khác `FormsAuthentication` nhiều nhất.

**Đọc:** Authentication overview; **authentication scheme**; `ClaimsPrincipal`, `ClaimsIdentity`, claim; cookie authentication; `SignInAsync`/`SignOutAsync`; `UseAuthentication` vs `UseAuthorization`.

**Làm:**
- Cấu hình cookie authentication với một login endpoint đơn giản (hardcode user, chưa cần Identity)
- Tự tạo `ClaimsPrincipal` với claim: sub, name, role, và một custom claim (tenant id)
- `[Authorize]` trên controller, verify 401 vs 403
- Đọc claim ra trong controller
- **Đối chiếu:** viết bảng `FormsAuthentication` / Membership Provider / `Roles.IsUserInRole` tương ứng với cái gì

**Xong khi:** giải thích rõ authentication ≠ authorization, và scheme là gì.

## Ngày 22 — JWT bearer & authorization policy

**Mục tiêu:** làm auth cho API đúng cách.

**Đọc:** JWT bearer authentication; token validation parameter; **policy-based authorization**; requirement + handler; `[Authorize(Policy = ...)]`; role-based vs claim-based vs policy-based; multi-scheme.

**Làm:**
- Thêm JWT bearer, endpoint `/login` phát token
- Register 3 policy: theo role, theo claim, và một policy có `IAuthorizationRequirement` + handler tự viết (ví dụ "chỉ sửa được customer của tenant mình")
- Cấu hình cả cookie và JWT cùng lúc, chỉ định scheme trên từng controller
- Test authorization bằng integration test (fake authentication handler)

**Xong khi:** phân biệt được scheme, policy, requirement, handler — và biết khi nào cần policy thay vì role.

## Ngày 23 — HttpClientFactory & resilience

**Mục tiêu:** gọi HTTP ra ngoài đúng cách.

**Đọc:** `IHttpClientFactory`; named vs typed client; **vấn đề socket exhaustion của `new HttpClient()`** và vấn đề DNS của singleton `HttpClient`; `DelegatingHandler`; `Microsoft.Extensions.Http.Resilience` (retry, circuit breaker, timeout).

**Làm:**
- Typed client gọi một public API bất kỳ
- Thêm `DelegatingHandler` tự động gắn auth header và log request/response
- Thêm resilience: retry với exponential backoff + circuit breaker + timeout
- Test bằng cách trỏ vào endpoint lỗi, quan sát retry trong log
- **Đối chiếu:** ghi lại vì sao `using (var client = new HttpClient())` — pattern rất phổ biến trong code .NET Framework — là bug

**Xong khi:** giải thích được cả hai vấn đề (socket exhaustion và DNS staleness) và `IHttpClientFactory` giải quyết chúng thế nào.

## Ngày 24 — Caching, async cho đúng

**Mục tiêu:** hiệu năng và concurrency.

**Đọc:** `IMemoryCache`; `IDistributedCache`; `HybridCache`; response caching; output caching; **async best practice**: không `.Result`/`.Wait()`, `ConfigureAwait` (và vì sao ASP.NET Core không cần nó như ASP.NET Framework), `CancellationToken` truyền xuyên suốt, `ValueTask`, `IAsyncEnumerable`.

**Làm:**
- Cache danh sách customer bằng `IMemoryCache` với expiration, kiểm tra cache hit/miss bằng log
- Đổi sang Redis qua `IDistributedCache` (Redis trong Docker)
- Rà toàn bộ code: mọi method async phải nhận và truyền `CancellationToken` xuống tới EF Core
- Tìm và xoá mọi `.Result`, `.Wait()`, `Task.Run` không cần thiết
- Viết một endpoint stream dữ liệu bằng `IAsyncEnumerable`
- **Quan trọng:** đọc về `SynchronizationContext` và vì sao deadlock kiểu ASP.NET Framework không xảy ra trên ASP.NET Core — nhưng `.Result` vẫn là anti-pattern vì lý do khác

**Xong khi:** không còn sync-over-async trong codebase, và `CancellationToken` đi được từ HTTP request xuống query SQL.

## Ngày 25 — Health checks, OpenAPI, hoàn thiện

**Mục tiêu:** đưa app lên mức "vận hành được".

**Đọc:** Health checks; OpenAPI trong .NET (`Microsoft.AspNetCore.OpenApi`, Scalar hoặc Swagger UI); CORS; rate limiting middleware; `IProblemDetailsService`.

**Làm:**
- Health check cho DB và Redis, endpoint `/health` (liveness) và `/health/ready` (readiness)
- OpenAPI document + UI, có mô tả và security scheme cho JWT
- CORS policy đúng (không dùng `AllowAnyOrigin` với credential)
- Rate limiting cho endpoint `/login`
- Chuẩn hoá toàn bộ error response thành `ProblemDetails`

**Xong khi:** `/health` trả về đúng trạng thái khi bạn tắt container DB, và OpenAPI UI gọi được endpoint có auth.

**Checkpoint tuần 5:**
1. Authentication scheme, policy, requirement — mỗi cái là gì?
2. Hai vấn đề của `new HttpClient()` trong vòng lặp?
3. `IMemoryCache` vs `IDistributedCache` — chọn khi nào?
4. Vì sao `.Result` vẫn xấu dù ASP.NET Core không deadlock?

---

# TUẦN 6 — Publish, Container, Vận hành

## Ngày 26 — Publish & deployment model

**Đọc:** `dotnet publish`; framework-dependent vs self-contained; single-file; trimming; ReadyToRun; Native AOT (và vì sao **không phù hợp** với app MVC/EF Core nhiều reflection); runtime identifier (RID).

**Làm:** publish API theo 4 cách (framework-dependent, self-contained, single-file, R2R), so sánh kích thước output và thời gian startup. Thử `-r linux-x64` từ máy Windows.

**Xong khi:** biết chọn chế độ nào cho hoàn cảnh nào, và vì sao không nên bật Native AOT cho project của mình.

## Ngày 27 — Docker

**Đọc:** Dockerfile cho ASP.NET Core; multi-stage build; base image `mcr.microsoft.com/dotnet/aspnet` vs `sdk` vs chiseled/alpine; layer caching; non-root user; `dotnet publish /t:PublishContainer` (không cần Dockerfile).

**Làm:** viết Dockerfile multi-stage tối ưu layer cache, chạy non-root. Sau đó thử `dotnet publish /t:PublishContainer`, so sánh. Build image, chạy container.

**Xong khi:** image dưới 250MB và app chạy được với user non-root.

## Ngày 28 — docker compose & cấu hình theo môi trường

**Đọc:** `ASPNETCORE_ENVIRONMENT`; config qua environment variable (quy ước `__` cho nested key); secret management; `IHostEnvironment`; Kestrel config; chạy sau reverse proxy (`UseForwardedHeaders`) và host trên IIS qua ASP.NET Core Module.

**Làm:** `docker-compose.yml` gồm API + SQL Server + Redis. Toàn bộ config qua environment variable, không hardcode. Migration chạy bằng script riêng, không chạy lúc startup.

**Xong khi:** `docker compose up` trên máy sạch là chạy được toàn bộ hệ thống.

## Ngày 29 — Observability

**Đọc:** OpenTelemetry trong .NET; `ActivitySource` (tracing) và `Meter` (metrics); `System.Diagnostics.Metrics`; structured logging + correlation; `dotnet-counters`, `dotnet-trace`, `dotnet-dump`.

**Làm:** thêm OpenTelemetry cho trace + metric, export ra console (hoặc Jaeger/Aspire dashboard trong Docker). Tạo custom `ActivitySource` cho một business operation. Verify trace đi từ HTTP request xuống query SQL. Dùng `dotnet-counters` xem GC và thread pool lúc load.

**Xong khi:** xem được một request trace xuyên qua middleware → controller → EF Core → SQL.

## Ngày 30 — Tổng kết & bắc cầu sang việc thật

**Làm:**
- Đọc lại toàn bộ `PROGRESS.md`, giải quyết mọi câu hỏi còn treo
- Refactor toàn bộ MiniCRM một lượt bằng mắt của người đã học 6 tuần — bạn sẽ thấy code tuần 1–2 của mình khá tệ, đó là dấu hiệu tốt
- Đảm bảo: 0 warning, test xanh, `docker compose up` chạy
- **Viết một tài liệu 2 trang** cho project thật của bạn: các thay đổi kiến trúc bắt buộc, danh sách blocker (EDMX, auth hiện tại, NuGet không hỗ trợ, `HttpContext.Current`, bundling), phương án cho từng cái, và ước lượng công sức
- Push repo MiniCRM lên GitHub làm tài liệu tham chiếu cho bản thân sau này

---

# Track song song: C# 7 → 14 (30 phút/ngày)

.NET Framework 4.6 gắn với C# 6; .NET 10 dùng C# 14. Học theo **mức độ ảnh hưởng đến code hàng ngày**, không theo version.

| Tuần | Chủ đề |
|---|---|
| 1 | Nullable reference types (đã nằm trong ngày 4–5) |
| 2 | Pattern matching, switch expression, `is` pattern, property/list pattern |
| 3 | `record`, `init`, `required`, `with`, target-typed `new`, tuple deconstruction |
| 4 | File-scoped namespace, global using, implicit using, top-level statement, primary constructor, raw string literal, `nameof` mở rộng |
| 5 | Collection expression, spread `..`, `IAsyncEnumerable`, `await using`, range/index `[^1]`, `..` slicing |
| 6 | **C# 14:** `field` keyword (property có logic không cần backing field thủ công), **extension members** (thêm cả property, operator, static member cho type có sẵn qua `extension(...)` block, không chỉ extension method); `Span<T>`/`Memory<T>` nếu quan tâm hiệu năng |

Cách học: mỗi ngày lấy một class **trong code chính bạn vừa viết** và refactor bằng tính năng mới. Không làm bài tập rời.

---

# Chủ động BỎ QUA (để 6 tuần thực sự là 6 tuần)

Blazor · MAUI · gRPC · SignalR · Orleans · .NET Aspire · Native AOT · source generator tự viết · F# 10 · Microsoft Agent Framework / MCP · Kubernetes · Minimal API nâng cao · Identity Server / OpenIddict

Không cái nào là tiền đề của cái khác. Học khi cần, mất 2–5 ngày mỗi cái.

---

# Cách học hiệu quả nhất cho người có nền .NET Framework

1. **Đối chiếu, đừng học tuần tự.** Với mỗi khái niệm mới, hỏi ngay: "cái này thay thế cái gì trong .NET Framework của tôi?" Duy trì một bảng đối chiếu trong `PROGRESS.md` — đây sẽ là tài liệu giá trị nhất bạn có được sau 6 tuần.

2. **Đọc template thay vì tutorial.** `dotnet new webapi`, `worker`, `mvc` rồi đọc từng dòng `Program.cs`. Nhanh hơn mọi khoá học.

3. **Đọc source ASP.NET Core.** Nó open source, và với nền .NET của bạn thì đọc được. Một lần đọc source `UseRouting` hoặc `ServiceProvider` giá trị hơn năm bài blog.

4. **Bật hết log/SQL/trace.** Đừng học ASP.NET Core và EF Core như hộp đen.

5. **Chỉ dùng tài liệu chính thức.** `learn.microsoft.com/aspnet/core` và `learn.microsoft.com/ef/core`, nhớ **chọn đúng version .NET 10** ở dropdown. Blog tổng hợp phần lớn viết cho .NET Core 3.1 hoặc .NET 6 — cú pháp hosting đã khác và sẽ làm bạn bối rối.

6. **Đừng dùng project công ty làm bài tập học.** Lỗi phổ biến nhất. Học trên MiniCRM sạch, rồi mới quay lại.

7. **Đo bằng "giải thích được", không phải "làm được".** Mỗi checkpoint hãy tự nói to câu trả lời. Nếu phải mở tab tra, coi như chưa xong.

---

## Ghi chú về version

Plan này dựa trên .NET 10 (LTS, phát hành 11/2025, hỗ trợ đến ~11/2028), C# 14, EF Core 10, EF6 6.5.2. Các số version package cụ thể nên kiểm tra lại trên NuGet lúc bắt đầu, vì bản patch ra khá thường xuyên.
