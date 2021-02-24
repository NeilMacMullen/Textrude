FROM mcr.microsoft.com/dotnet/sdk:5.0-alpine AS builder
WORKDIR /app

COPY src/Engine/*.csproj ./src/Engine/
COPY src/SharedApplication/*.csproj ./src/SharedApplication/
COPY src/Textrude/*.csproj ./src/Textrude/
COPY src/ScriptLibrary/*.csproj ./src/ScriptLibrary/
RUN dotnet restore src/Textrude

COPY . ./
# GitVersion does not seem to work in the dotnet/sdk (LibGitSharp cannot be loaded because of a missing libssl1.0.0)
# A newer version seems to be installed - closer investigation needed
# (Of course currently this CANNOT work because of no local git clone)
ENV DisableGitVersionTask=true
RUN dotnet publish src/Textrude -c Release -o out -r linux-musl-x64 -p:PublishSingleFile=true -p:PublishTrimmed=true

FROM mcr.microsoft.com/dotnet/runtime-deps:5.0-alpine3.12
WORKDIR /app
COPY --from=builder /app/out .
ENTRYPOINT ["/app/Textrude"]
