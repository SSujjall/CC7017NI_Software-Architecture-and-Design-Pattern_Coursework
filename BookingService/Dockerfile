FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["BuildingBlocks/BuildingBlocks.csproj", "BuildingBlocks/"]
COPY ["BookingService/BookingService.csproj", "BookingService/"]
RUN dotnet restore "BookingService/BookingService.csproj"
COPY BuildingBlocks/ BuildingBlocks/
COPY BookingService/ BookingService/
WORKDIR "/src/BookingService"
RUN dotnet publish "BookingService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "BookingService.dll"]
