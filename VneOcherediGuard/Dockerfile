FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY *.sln ./
COPY VneOcherediGuard/*.csproj VneOcherediGuard/
COPY VneOcherediGuard.Infrastructure/*.csproj VneOcherediGuard.Infrastructure/

RUN dotnet restore

COPY . .
RUN dotnet publish ./VneOcherediGuard -c Release -o /app/publish 


FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 50

ENTRYPOINT ["dotnet", "VneOcherediGuard.dll"]



