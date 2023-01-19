FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /app
EXPOSE 80
COPY *.csproj ./
RUN dotnet restore "Auto.Website.csproj"
COPY . ./
RUN dotnet publish "Auto.Website.csproj" -c Release -o /app/publish
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Auto.Website.dll"]
