FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["BuildingBlocks/BuildingBlocks.csproj", "BuildingBlocks/"]
COPY ["EmailService/EmailService.csproj", "EmailService/"]
RUN dotnet restore "EmailService/EmailService.csproj"
COPY BuildingBlocks/ BuildingBlocks/
COPY EmailService/ EmailService/
WORKDIR "/src/EmailService"
RUN dotnet publish "EmailService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "EmailService.dll"]
