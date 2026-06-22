FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY SmartInfluence/SmartInfluence.Api/SmartInfluence.Api.csproj SmartInfluence/SmartInfluence.Api/
COPY SmartInfluence/SmartInfluence.Data/SmartInfluence.Data.csproj SmartInfluence/SmartInfluence.Data/
COPY SmartInfluence/SmartInfluence.Services/SmartInfluence.Services.csproj SmartInfluence/SmartInfluence.Services/

RUN dotnet restore SmartInfluence/SmartInfluence.Api/SmartInfluence.Api.csproj

COPY SmartInfluence/SmartInfluence.Api/ SmartInfluence/SmartInfluence.Api/
COPY SmartInfluence/SmartInfluence.Data/ SmartInfluence/SmartInfluence.Data/
COPY SmartInfluence/SmartInfluence.Services/ SmartInfluence/SmartInfluence.Services/

RUN dotnet publish SmartInfluence/SmartInfluence.Api/SmartInfluence.Api.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://0.0.0.0:10000

ENTRYPOINT ["dotnet", "SmartInfluence.Api.dll"]
