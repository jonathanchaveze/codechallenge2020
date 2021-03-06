FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build-env
WORKDIR /app

# Copy csproj and restore as distinct layers
COPY WebApp/Jobsity.CodeChallenge.WebApp/*.csproj ./
RUN dotnet restore

# Copy everything else and build
COPY . ./
RUN dotnet publish "Jobsity Code Challenge.sln" -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1 AS prod-env
WORKDIR /app
COPY --from=build-env /app/out .
CMD ASPNETCORE_URLS=http://*:$PORT dotnet Jobsity.CodeChallenge.WebApp.dll
#ENTRYPOINT ["dotnet", "Jobsity.CodeChallenge.WebApp.dll"]