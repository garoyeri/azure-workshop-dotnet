FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base

FROM mcr.microsoft.com/dotnet/sdk:5.0-buster-slim as build
WORKDIR /src
COPY ["src/AzureHelloWorldWeb/AzureHelloWorldWeb.csproj", "AzureHelloWorldWeb/"]
RUN dotnet restore AzureHelloWorldWeb/AzureHelloWorldWeb.csproj

WORKDIR "/work"
COPY . .
RUN dotnet build --configuration Release --output /app/build

FROM build AS publish
RUN dotnet publish "src/AzureHelloWorldWeb/AzureHelloWorldWeb.csproj" \
            --configuration Release \ 
            --runtime linux-x64 \
            --self-contained false \ 
            --output /app/publish \
            -p:PublishReadyToRun=true  

FROM base AS final
WORKDIR /var/task
COPY --from=publish /app/publish .
EXPOSE 80
ENTRYPOINT ["dotnet", "AzureHelloWorldWeb.dll"]
