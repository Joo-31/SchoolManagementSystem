# ✅ Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# نسخ ملف الـ csproj وتنزيل الحزم
COPY ["SchoolManagementAPI.csproj", "./"]
RUN dotnet restore "SchoolManagementAPI.csproj"

# نسخ باقي الملفات
COPY . .

# بناء التطبيق
RUN dotnet build "SchoolManagementAPI.csproj" -c Release -o /app/build

# ✅ Stage 2: Publish
FROM build AS publish
RUN dotnet publish "SchoolManagementAPI.csproj" -c Release -o /app/publish /p:UseAppHost=false

# ✅ Stage 3: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .

# المنفذ
EXPOSE 80
EXPOSE 443

# الأمر اللي بيشغل التطبيق
ENTRYPOINT ["dotnet", "SchoolManagementAPI.dll"]