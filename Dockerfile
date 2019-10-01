FROM mcr.microsoft.com/dotnet/core/aspnet:2.2

WORKDIR /app

RUN apt-get update

RUN apt-get update \
  && apt-get install -y --no-install-recommends libgdiplus libc6-dev \
  && apt-get clean \
  && rm -rf /var/lib/apt/lists/*

COPY src/StorefrontCommunity.Gallery.API/dist/ ./

ENTRYPOINT ["dotnet", "StorefrontCommunity.Gallery.API.dll"]

LABEL version="1.0.0" maintainer="marxjmoura"
