FROM mcr.microsoft.com/dotnet/sdk:5.0 AS builder
WORKDIR /app

COPY Engine/*.csproj ./Engine/
COPY SharedApplication/*.csproj ./SharedApplication/
COPY Textrude/*.csproj ./Textrude/
RUN dotnet restore Textrude

COPY . ./
ENV DisableGitVersionTask=true
RUN dotnet publish Textrude -c Release -o out

FROM mcr.microsoft.com/dotnet/runtime:5.0
WORKDIR /app
COPY --from=builder /app/out .
ENTRYPOINT ["dotnet", "Textrude.dll"]
