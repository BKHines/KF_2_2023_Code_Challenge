FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 18080
EXPOSE 18443
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["KF_2_2023.csproj", "./"]
RUN dotnet restore "./KF_2_2023.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "KF_2_2023.csproj" -c Release -o /app/build
FROM build AS publish
RUN dotnet publish "KF_2_2023.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "KF_2_2023.dll"]