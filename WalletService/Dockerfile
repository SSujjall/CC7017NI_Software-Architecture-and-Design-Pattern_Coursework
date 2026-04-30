FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["BuildingBlocks/BuildingBlocks.csproj", "BuildingBlocks/"]
COPY ["WalletService/WalletService.csproj", "WalletService/"]
RUN dotnet restore "WalletService/WalletService.csproj"
COPY BuildingBlocks/ BuildingBlocks/
COPY WalletService/ WalletService/
WORKDIR "/src/WalletService"
RUN dotnet publish "WalletService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "WalletService.dll"]
