# Tiến độ
Hiện tại: Tuần 1, Ngày 0

## Đã hoàn thành
- Ngày 0 — Chuẩn bị môi trường
- Ngày 1 - dotnet CLI
- Ngày 2 - SDK-style csproj & TFM
- Ngày 3 - NuGet & quản lý dependency
- Ngày 4 - Domain code + test, làm quen C# hiện đại

## Câu hỏi còn treo
(những chỗ chưa hiểu, chưa giải quyết)
1. Vì sao dotnet build không cần Visual Studio, trong khi msbuild của .NET Framework thì gắn với VS?
<br>
MSBuild của .Net framework ban đầu ship cùng bản thân Framework (C:\Windows\Microsoft.NET\Framework64\v4.0.30319\MSBuild.exe) — tức là một tính năng của máy, cài qua Windows Update, một bản duy nhất. Từ MSBuild 15 (VS 2017) nó chuyển vào thành component của VS, và cách duy nhất để có nó là cài VS Build tools. 
<br>
.NET SDK đảo ngược đúng chỗ đó, SDK là artifact tự chứa và version được, bên trong nó có MSBuild, compiler, Nuget Client, dotnet CLI, template engine, cài nhiều bản cạnh nhau được, pin theo repo được(global.json)

2. Microsoft.NETCore.App và Microsoft.AspNetCore.App khác nhau chỗ nào, và vì sao lại tách ra?
<br>
Microsoft.NETCore.App là base shared framework: CLR, GC, JIT, và toàn bộ BCL (System.* — collections, threading, System.Net.Http, IO...). Mọi app .NET đều cần nó, trên mọi OS.
<br>
Microsoft.AspNetCore.App là một framework xếp lớp lên trên base đó, chứa Microsoft.AspNetCore.* (Kestrel, routing, MVC, authentication...) cộng phần lớn Microsoft.Extensions.* (DI, Configuration, Logging — nền của tuần 2).
<br>

---
Vì sao tách:
1. Hướng phụ thuộc phải một chiều. Base framework phải chạy được ở mọi nơi: Linux container, Alpine, mobile, IoT. Nếu web stack nằm trong base thì một console app hay worker service cũng phải mang theo Kestrel và MVC. Tách ra thì app nào dùng gì trả tiền cho cái đó.<br>
2. Server trở thành thứ thay được. ASP.NET Framework không tách được vì System.Web bị hàn chặt vào mô hình hosting của IIS. ASP.NET Core là một layer với server có thể thay: Kestrel, IIS in-process, HTTP.sys, hoặc TestServer. Đúng cái này là lý do ngày 15 bạn test được cả API trong process, không cần deploy — điều bất khả với Web API 2.<br>
3. HttpContext.Current chết ở đây, không phải vì Microsoft chưa làm. Ambient static context chỉ hợp lý khi có duy nhất một hosting model để bám vào. Khi ASP.NET Core là một layer với server thay được và app có thể chạy nhiều host trong cùng process (chính là lúc chạy integration test), một static request context là thiết kế sai — không phải feature bị thiếu.<br>
4. Và ASP.NET Core có thể ship bên trong app bạn (self-contained), thay vì phải cài lên server trước.
---

3. Khi nào cần netstandard2.0 khi migration project từ .Net Framework lên .Net 10?<br>
Đáp án nằm ở giai đoạn giữa của migration. Bạn không chuyển 10 project trong một đêm, nên sẽ có nhiều tháng solution ở trạng thái lai:

| Project | NET version | note |
| --- | --- | --- |
| MiniCrm.Web      |  → net48    |  (chưa chuyển, còn System.Web) |
| MiniCrm.Jobs     |  → net48    |  (chưa chuyển) |
| MiniCrm.NewApi   |  → net10.0  |  (đã chuyển) |
| MiniCrm.Common   |  → ???      |  ← cả 3 project trên đều reference |

MiniCrm.Common target gì? Không thể net48 — project net10.0 không reference được. Không thể net10.0 — project net48 không reference được. Còn đúng hai lựa chọn:

netstandard2.0 — một assembly duy nhất, cả hai bên dùng được<br>
Multi-target net48;net10.0 — hai assembly, mỗi bên lấy bản của mình<br>

Chọn cái nào: netstandard2.0 đơn giản hơn nhưng bị khoá ở C# ~7.3 và không dùng được Span<T>, IAsyncEnumerable, nullable annotation đầy đủ. Multi-target phức tạp hơn nhưng nhánh net10.0 được hưởng trọn C# 14. Với project dùng chung nhiều và sống lâu, multi-target đáng công hơn.<br>
netstandard2.0 bắt buộc khi assembly được tiêu thụ như một file mà bên tiêu thụ không có cơ chế chọn TFM — <Reference> + HintPath, plugin load theo thư mục, host bên thứ ba, Roslyn analyzer. Khi bên tiêu thụ là ProjectReference hoặc PackageReference thì nó không bắt buộc, chỉ là lựa chọn rẻ hơn multi-targeting: đổi chi phí build và #if lấy việc bị khoá ở mẫu số chung API và C# 7.3. Như vậy tùy vào project MiniCrm.Common, với case này, câu trả lời trung thực là: đây là đánh đổi chi phí, không phải ràng buộc kỹ thuật. netstandard2.0 cho library nhỏ, ổn định, ít API; multi-target cho library lớn, sống lâu, và bạn muốn nhánh mới hưởng C# 14.

## Bảng đối chiếu .NET Framework → .NET 10




| Khía cạnh | Cũ | Mới | Khác biệt gi |
| --- | --- | --- | --- |
|| .NET Framework 4.6 → 4.7 → 4.8 là in-place update | .NET 10 là side-by-side | .NET Framework: Một máy chỉ có một CLR v4, gắn với OS, cài qua Windows Update <br> .Net 10: Nhiều SDK và nhiều runtime sống cạnh nhau trong C:\Program Files\dotnet\sdk\ và C:\Program Files\dotnet\shared\Microsoft.NETCore.App\. Không còn GAC. Mỗi project tự chọn runtime qua TFM. Cho phép migrate từng project, không all-or-nothing |
|| App .NET Framework 4.6 chỉ chạy được trong Windows container — base image mcr.microsoft.com/dotnet/framework/aspnet nhiều GB, bắt buộc Windows host, license Windows Server cho node chạy nó | App .NET 10 chạy trong Linux container, base image aspnet khoảng 100–200MB, bản chiseled còn nhỏ hơn, host nào cũng được, node pool thường. | Đây thường là lý do kinh doanh thực sự đứng sau quyết định migrate, không phải "C# 14 có cú pháp đẹp hơn". |
|| MSBuild gắn với VS / thư mục Windows | MSBuild nằm trong SDK | Build server không cần cài VS |
|| GAC | Không còn; mọi thứ là package, copy local | Hết chuyện "hoạt động trên máy tôi" vì DLL trong GAC |
|| System.Web là phần của framework | Microsoft.AspNetCore.App là shared framework riêng | HttpContext.Current không phải "chưa port" mà là không tồn tại |
|| .exe là managed assembly, có CLR header, window only | .exe chỉ là một native shim đi tìm runtime, code nằm trong dll | Không có sự tách đôi này thì không có Linux container ở tuần 6. |
|| Assembly resolve tại Runtime — probe GAC rồi bin/ | Assembly resolve tại Build time — graph đã tính sẵn trong deps.json | nên .NET 10 ko có chuyện Could not load file or assembly ... Version=... lúc chạy production |
|| Metadata assembly nằm trong AssemblyInfo.cs, tức là trong source code | Metadata assembly là property, có thể dùng trong CLI dotnet build -p:Version=1.2.3 | Metadata như: AssemblyName, RootNamespace, Version, VersionPrefix, VersionSuffix, AssemblyVersion, FileVersion, InformationalVersion, Company, Product, GenerateAssemblyInfo. Chuyển version từ source thành tham số build là thay đổi nhỏ về mặt cú pháp nhưng xoá hẳn một lớp hack. |
| Danh tính project & solution | `<ProjectGuid>` + `<ProjectTypeGuids>` trong csproj; .sln đầy GUID và NestedProjects |	Sdk="Microsoft.NET.Sdk" xác định loại project; .slnx chỉ liệt kê đường dẫn, không GUID. |Loại project chuyển từ GUID tra bảng sang SDK import lúc evaluate. Bẫy: để sót `<ProjectTypeGuids>` khiến VS cố nạp flavor cũ và không load được project; .vcxproj thì vẫn cần GUID trong reference; vài tool CI đời cũ (SonarQube bản cũ) còn đòi `<ProjectGuid>` |
| Tham chiếu framework | Opt-in từng assembly: `<Reference Include="System.Xml" />`, resolve qua GAC |	Cả framework là một khối, SDK tự thêm FrameworkReference theo `<TargetFramework>` |	Không còn GAC — framework là targeting pack trong packs. Bẫy: `<Reference>` tên trần sót lại cho cảnh báo MSB3245; đừng nhầm ImplicitUsings (sinh global using, tức namespace) với assembly reference — hai tầng độc lập |
| Tham chiếu package | `<Reference>` + `<HintPath>` ..\packages\X.6.0.4\lib\net45\X.dll + packages.config + binding redirect |	`<PackageReference Include="X" Version="13.0.3" />`, restore ghi đường dẫn vào `obj/project.assets.json`; version gom vào `Directory.Packages.props` |	Chuyển từ khai vị trí file sang khai ý định; dependency bắc cầu tự có. Bẫy: HintPath cho DLL rời (vendor SDK) vẫn hợp lệ nhưng không truyền bắc cầu và không tự vào .nupkg khi `dotnet pack` |
| Import props/targets | Tự gõ 2 dòng: Microsoft.Common.props ở đầu, Microsoft.CSharp.targets ở cuối |	Sdk="..." là cú pháp rút gọn của đúng 2 dòng đó (Sdk.props / Sdk.targets) |	Đây là thứ không bị thay thế, chỉ bị giấu đi. Bẫy: thứ tự props-trước/targets-sau vẫn quyết định ai đè ai; Directory.Build.props dừng ở file đầu tiên tìm thấy ngược lên cây thư mục, nhiều tầng phải tự GetPathOfFileAbove để nối |
| Danh sách file & giá trị mặc định |	Liệt kê từng `<Compile Include="Models\Invoice.cs" />`; chép lại mọi default (FileAlignment, WarningLevel, 2 khối Debug/Release) |	Glob ngầm gom **/*.cs; chỉ ghi phần lệch khỏi mặc định — csproj 61 dòng còn 9 |	Đảo từ "khai hết" sang convention-over-configuration; hết merge conflict khi hai người cùng thêm file. Bẫy: glob nhặt cả file bạn không muốn — tắt bằng `<EnableDefaultCompileItems>false</EnableDefaultCompileItems>` (đặt giữa file vẫn hiệu lực, vì property được đánh giá xong hết trước item) |

## C# hiện đại
| # | Cũ (.NET Framework / C# 6) | Mới (.NET 10 / C# 14) | Khác biệt |
| --- | --- | --- | --- |
| 1 | class | record = class + code do compiler sinh thêm | Lợi ích: Giảm mã lặp, Dễ so sánh nhanh dữ liệu, Hỗ trợ tốt cho DTO / Snapshot<br> Hại gì: Vi phạm nguyên lý về Entity (Domain-Driven Design) - Một Entity được định nghĩa bởi định danh (Id), chứ không phải bởi các thuộc tính của nó; Lỗi logic khi lưu vào HashSet / Dictionary - vì mã hash sẽ giống nhau cho các đối tượng có cùng property value; Hiệu năng kém hơn khi so sánh lớn|
| 2 | enum | Lưu enum là số thì phải gán số tường minh ngay từ đầu (Email = 1) để ko ai vô tình chèn member vào giữa | Lưu string thì sẽ ít bị hỏng hơn khi có cập nhật |
| 3 | Value object viết tay 30 dòng | record một dòng | Value equality, ToString, with được sinh tự động |
| 4 | Constructor nhiều tham số hoặc nhiều overload | required + init | Tổ hợp được required/optional, 0 constructor |
| 5 | throw ValidationException từ business layer | Result<T> | Khả năng thất bại nằm trong kiểu trả về; domain không biết HTTP |
| 6 | FirstOrDefault() trả null ngầm | Task<Customer?> | Compiler ép caller xử lý null |
| 7 | Dictionary + lock thủ công | ConcurrentDictionary | Không còn tự quản lý lock |

## Blocker migration
| Issue | .Net Framework | .Net 10 | Cách sửa |
| --- | --- | --- | --- |
| Encoding.Default | ANSI codepage của hệ thống (trên Windows tiếng Việt: 1258) | Luôn luôn UTF-8 | Codepage không còn built-in. .NET 10 chỉ có sẵn ASCII, UTF-8, UTF-16, UTF-32, Latin1. Mọi codepage kiểu Windows (1258 tiếng Việt, 1252, 932...) không có. Encoding.GetEncoding(1258) sẽ throw lúc runtime, không phải lỗi compile. <br>Cách sửa: thêm package `System.Text.Encoding.CodePages`, rồi đăng ký một lần lúc khởi động<br> `Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);` |
