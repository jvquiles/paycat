#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:7.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["src/Paycat.Processor/Paycat.Processor.csproj", "src/Paycat.Processor/"]
COPY ["src/Paycat.Messages/Paycat.Messages.csproj", "src/Paycat.Messages/"]
RUN dotnet restore "src/Paycat.Processor/Paycat.Processor.csproj"
COPY . .
WORKDIR "/src/src/Paycat.Processor"
RUN dotnet build "Paycat.Processor.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Paycat.Processor.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Paycat.Processor.dll"]