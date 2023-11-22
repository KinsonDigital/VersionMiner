# Set the base image as the .NET 6.0 SDK (this includes the runtime)
FROM mcr.microsoft.com/dotnet/sdk:8.0 as build-env

# Copy everything and publish the release (publish implicitly restores and builds)
COPY . ./
RUN dotnet publish ./VersionMiner/VersionMiner.csproj -c Release -o out --no-self-contained

# Label the container
LABEL maintainer="Calvin Wilkinson <kinsondigital@gmail.com>"
LABEL repository="https://github.com/KinsonDigital/VersionMiner"
LABEL homepage="https://github.com/KinsonDigital/VersionMiner"

# Label as GitHub action
LABEL com.github.actions.name="Version Miner"

# Relayer the .NET SDK, anew with the build output
FROM mcr.microsoft.com/dotnet/sdk:8.0
COPY --from=build-env /out .
ENTRYPOINT [ "dotnet", "/VersionMiner.dll" ]
