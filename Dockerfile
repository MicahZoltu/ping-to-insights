FROM microsoft/dotnet:2.1-sdk-alpine AS build
WORKDIR /app

# copy csproj and restore as distinct layers
COPY app.csproj /app/app.csproj
RUN dotnet restore

# add IL Linker package
RUN dotnet add app.csproj package ILLink.Tasks -v 0.1.5-preview-1841731 -s https://dotnet.myget.org/F/dotnet-core/api/v3/index.json
RUN dotnet restore

# copy and build app
COPY *.cs /app/
RUN dotnet publish app.csproj -c Release -r linux-musl-x64 -o out /p:ShowLinkerSizeComparison=true


FROM microsoft/dotnet:2.1-runtime-deps-alpine AS runtime
WORKDIR /app
COPY --from=build /app/out /app
ENTRYPOINT ["./app"]
