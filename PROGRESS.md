# Tiến độ
Hiện tại: Tuần __, Ngày __

## Đã hoàn thành
(ngày nào, làm gì, checkpoint tuần trả lời được chưa)

## Câu hỏi còn treo
(những chỗ chưa hiểu, chưa giải quyết)

## Bảng đối chiếu .NET Framework → .NET 10




| Cũ | Mới | Ghi chú |
| --- | --- | --- |
| .NET Framework 4.6 → 4.7 → 4.8 là in-place update | .NET 10 là side-by-side | .NET Framework: Một máy chỉ có một CLR v4, gắn với OS, cài qua Windows Update <br> .Net 10: Nhiều SDK và nhiều runtime sống cạnh nhau trong C:\Program Files\dotnet\sdk\ và C:\Program Files\dotnet\shared\Microsoft.NETCore.App\. Không còn GAC. Mỗi project tự chọn runtime qua TFM |
| App .NET Framework 4.6 chỉ chạy được trong Windows container — base image mcr.microsoft.com/dotnet/framework/aspnet nhiều GB, bắt buộc Windows host, license Windows Server cho node chạy nó | App .NET 10 chạy trong Linux container, base image aspnet khoảng 100–200MB, bản chiseled còn nhỏ hơn, host nào cũng được, node pool thường. | Đây thường là lý do kinh doanh thực sự đứng sau quyết định migrate, không phải "C# 14 có cú pháp đẹp hơn". |
