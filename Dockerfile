# اختيار صورة الأساس من Docker Hub
FROM mcr.microsoft.com/dotnet/aspnet:8.0

# تعيين مجلد العمل داخل الحاوية
WORKDIR /app

# نسخ جميع الملفات إلى مجلد العمل في الحاوية
COPY . .

# تنفيذ التطبيق عند بدء تشغيل الحاوية
ENTRYPOINT ["dotnet", "ENERGY_MOBILE_APP.dll"]
