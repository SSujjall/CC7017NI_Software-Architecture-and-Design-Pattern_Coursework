FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["BuildingBlocks/BuildingBlocks.csproj", "BuildingBlocks/"]
COPY ["HotelService/HotelService.csproj", "HotelService/"]
RUN dotnet restore "HotelService/HotelService.csproj"
COPY BuildingBlocks/ BuildingBlocks/
COPY HotelService/ HotelService/
WORKDIR "/src/HotelService"
RUN dotnet publish "HotelService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "HotelService.dll"]
