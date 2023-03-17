#Dockerfile para esta aplicação
FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["IteraCompanyGroups/IteraCompanyGroups.csproj", "IteraCompanyGroups/"]
RUN dotnet restore "IteraCompanyGroups/IteraCompanyGroups.csproj"
COPY . .
WORKDIR "/src/IteraCompanyGroups"
RUN dotnet build "IteraCompanyGroups.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "IteraCompanyGroups.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "IteraCompanyGroups.dll"]