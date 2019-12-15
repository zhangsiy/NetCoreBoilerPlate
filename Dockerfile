# Build stage

FROM microsoft/dotnet:2.2-sdk AS build
WORKDIR /build

COPY src/ src/
COPY test/ test/
COPY *.sln ./
RUN dotnet restore *.sln

COPY . .

# See that it builds
RUN dotnet build *.sln -c Release

# See that tests pass (exclude acceptance tests)
RUN dotnet test test/NetCoreSample.UnitTests/*.csproj

# Package it up for deployment
RUN dotnet publish -c Release -o /app src/NetCoreSample/*.csproj


# Run stage

FROM microsoft/dotnet:2.2-aspnetcore-runtime
WORKDIR /app
EXPOSE 80

COPY --from=0 /app .

ENTRYPOINT ["dotnet", "NetCoreSample.dll"]
