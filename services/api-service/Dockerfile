FROM mcr.microsoft.com/dotnet/core/sdk:3.0.100 AS build
WORKDIR /app

COPY . ./
# .NET Core Runtime 有りコンテナ用にビルドする場合
#RUN dotnet publish -c Release -o out
# .NET Core Runtime 無しコンテナ用にビルドする場合（自己完結型）
# alpineで実行するにはmusl指定が必要。ファイルサイズを最小化
RUN dotnet publish -c Release -o out -r linux-musl-x64 -p:PublishSingleFile=true -p:PublishTrimmed=true

# .NET Core Runtime 有りのalpineコンテナ
#FROM mcr.microsoft.com/dotnet/core/aspnet:3.0-alpine AS runtime
# .NET Core Runtime 無しのalpineコンテナ
FROM mcr.microsoft.com/dotnet/core/runtime-deps:3.0.0-alpine
WORKDIR /app
COPY --from=build /app/out .

# .NET Core Runtime 有りの場合
#ENTRYPOINT ["dotnet", "api-service.dll"]
# .NET Core Runtime 無しの場合
ENTRYPOINT ["/app/api-service"]
