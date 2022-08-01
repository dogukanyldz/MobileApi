#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["Mobile.Web.Api/Mobile.Web.Api.csproj", "Mobile.Web.Api/"]
COPY ["Mobile.Entities/Mobile.Entities.csproj", "Mobile.Entities/"]
COPY ["Mobile.Dal/Mobile.Dal.csproj", "Mobile.Dal/"]
COPY ["Shared/Shared.csproj", "Shared/"]
RUN dotnet restore "Mobile.Web.Api/Mobile.Web.Api.csproj"
COPY . .
WORKDIR "/src/Mobile.Web.Api"
RUN dotnet build "Mobile.Web.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Mobile.Web.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Mobile.Web.Api.dll"]