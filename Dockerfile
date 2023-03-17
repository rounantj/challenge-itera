#Dockerfile para esta aplicação
FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["IteraEmpresaGrupos/IteraEmpresaGrupos.csproj", "IteraEmpresaGrupos/"]
RUN dotnet restore "IteraEmpresaGrupos/IteraEmpresaGrupos.csproj"
COPY . .
WORKDIR "/src/IteraEmpresaGrupos"
RUN dotnet build "IteraEmpresaGrupos.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "IteraEmpresaGrupos.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "IteraEmpresaGrupos.dll"]