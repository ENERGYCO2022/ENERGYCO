# 1️⃣ استخدام صورة الـ SDK لبناء التطبيق
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /source

# نسخ ملفات المشروع فقط (للاستفادة من التخزين المؤقت في Docker)
COPY *.csproj . 
RUN dotnet restore

# نسخ باقي الملفات وبناء المشروع
COPY . . 
RUN dotnet publish -c Release -o /app/publish

# 2️⃣ استخدام صورة الـ Runtime لتشغيل التطبيق
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# نسخ الملفات المنشورة من مرحلة البناء
COPY --from=build /app/publish .

# تعيين متغير البيئة لتشغيل التطبيق على المنفذ الذي توفره البيئة
ENV ASPNETCORE_URLS=http://+:${PORT}

# تشغيل التطبيق
ENTRYPOINT ["dotnet", "ENERGY_MOBILE_APP.dll"]
