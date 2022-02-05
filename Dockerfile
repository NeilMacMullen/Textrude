FROM mcr.microsoft.com/dotnet/sdk:6.0 AS builder
WORKDIR /app

COPY Engine/*.csproj ./Engine/
COPY SharedApplication/*.csproj ./SharedApplication/
COPY Textrude/*.csproj ./Textrude/
COPY ScriptLibrary/*.csproj ./ScriptLibrary/
RUN dotnet restore Textrude

COPY . ./
# GitVersion does not seem to work in the dotnet/sdk (LibGitSharp cannot be loaded because of a missing libssl1.0.0)
# A newer version seems to be installed - closer investigation needed
# (Of course currently this CANNOT work because of no local git clone)
ENV DisableGitVersionTask=true
RUN dotnet publish Textrude -c Release -o out -r linux-musl-x64 -p:PublishSingleFile=true -p:PublishTrimmed=true

FROM mcr.microsoft.com/dotnet/runtime-deps:6.0
WORKDIR /app
COPY --from=builder /app/out .
ENTRYPOINT ["/app/Textrude"]
