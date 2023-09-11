FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["StudentCI/StudentCI.csproj", "StudentCI/"]
RUN dotnet restore "StudentCI/StudentCI.csproj"
COPY . .
WORKDIR "/src/StudentCI"
RUN dotnet build "StudentCI.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "StudentCI.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "StudentCI.dll"]