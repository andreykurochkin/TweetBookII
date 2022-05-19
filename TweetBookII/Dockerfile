FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
ARG BUILDCONFIG=RELEASE
ARG VERSION=1.0.0

COPY TweetBookII.csproj /build/

RUN dotnet restore ./build/TweetBookII.csproj

COPY . ./build/
WORKDIR /build/
RUN dotnet publish ./TweetBookII.csproj -c $BUILDCONFIG -o out /p:Version=$VERSION

FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app

COPY --from=build /build/out .
ENTRYPOINT ["dotnet", "TweetBookII.dll", "http://+:80;https://+:443"]