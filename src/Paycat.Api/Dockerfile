#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["src/Paycat.Api/Paycat.Api.csproj", "src/Paycat.Api/"]
COPY ["src/Paycat.Messages/Paycat.Messages.csproj", "src/Paycat.Messages/"]
RUN dotnet restore "src/Paycat.Api/Paycat.Api.csproj"
COPY . .
WORKDIR "/src/src/Paycat.Api"
RUN dotnet build "Paycat.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Paycat.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Paycat.Api.dll"]