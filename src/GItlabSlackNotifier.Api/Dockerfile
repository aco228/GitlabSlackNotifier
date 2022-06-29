FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["GItlabSlackNotifier.Api/GItlabSlackNotifier.Api.csproj", "GItlabSlackNotifier.Api/"]
RUN dotnet restore "GItlabSlackNotifier.Api/GItlabSlackNotifier.Api.csproj"
COPY . .
WORKDIR "/src/GItlabSlackNotifier.Api"
RUN dotnet build "GItlabSlackNotifier.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "GItlabSlackNotifier.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "GItlabSlackNotifier.Api.dll"]
