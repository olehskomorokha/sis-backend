FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY SmartInfluence.Api/SmartInfluence.Api.csproj SmartInfluence.Api/
COPY SmartInfluence.Data/SmartInfluence.Data.csproj SmartInfluence.Data/
COPY SmartInfluence.Services/SmartInfluence.Services.csproj SmartInfluence.Services/

RUN dotnet restore SmartInfluence.Api/SmartInfluence.Api.csproj

COPY SmartInfluence.Api/ SmartInfluence.Api/
COPY SmartInfluence.Data/ SmartInfluence.Data/
COPY SmartInfluence.Services/ SmartInfluence.Services/

RUN dotnet publish SmartInfluence.Api/SmartInfluence.Api.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://0.0.0.0:$7000

ENTRYPOINT ["dotnet", "SmartInfluence.Api.dll"]
