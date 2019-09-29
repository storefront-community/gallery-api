FROM mcr.microsoft.com/dotnet/core/aspnet:2.2
WORKDIR /app
COPY src/StorefrontCommunity.Gallery.API/dist/ ./
ENTRYPOINT ["dotnet", "StorefrontCommunity.Gallery.API.dll"]

LABEL version="1.0.0" maintainer="marxjmoura"
