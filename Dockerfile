FROM mcr.microsoft.com/dotnet/sdk:5.0 AS builder
WORKDIR /app

COPY Engine/*.csproj ./Engine/
COPY SharedApplication/*.csproj ./SharedApplication/
COPY Textrude/*.csproj ./Textrude/
RUN dotnet restore Textrude

COPY . ./
# GitVersion does not seem to work in the dotnet/sdk (LibGitSharp cannot be loaded because of a missing libssl1.0.0)
# A newer version seems to be installed - closer investigation needed
# (Of course currently this CANNOT work because of no local git clone)
ENV DisableGitVersionTask=true
RUN dotnet publish Textrude -c Release -o out

FROM mcr.microsoft.com/dotnet/runtime:5.0
WORKDIR /app
COPY --from=builder /app/out .
ENTRYPOINT ["dotnet", "Textrude.dll"]
