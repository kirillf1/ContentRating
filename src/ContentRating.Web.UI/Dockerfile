# Base nginx image for final stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
# Install necessary packages and set timezone
RUN apt-get update && apt-get install -y moreutils &&  \
    DEBIAN_FRONTEND=noninteractive apt-get install -y tzdata && \
    ln -sf /usr/share/zoneinfo/Europe/Moscow /etc/localtime && \
    echo "Europe/Moscow" > /etc/timezone

# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
RUN apt-get update -y
RUN apt-get install -y python3
RUN dotnet workload install wasm-tools
WORKDIR /src

# Copy project files for restore
COPY ./src/ContentRating.Web.UI/ContentRating.Web.UI.csproj ./ContentRating.Web.UI/
COPY ./src/ContentRating.Web.Contracts/ContentRating.Web.Contracts.csproj ./ContentRating.Web.Contracts/

# Restore dependencies
RUN dotnet restore ./ContentRating.Web.UI/ContentRating.Web.UI.csproj

# Copy all source files
COPY ./src/ ./

# Build the application
RUN dotnet build ./ContentRating.Web.UI/ContentRating.Web.UI.csproj -c Release -o /app

# Publish stage
FROM build AS publish
RUN dotnet publish ./ContentRating.Web.UI/ContentRating.Web.UI.csproj -c Release -o /app/publish

# Final stage with nginx
FROM nginx:alpine AS final
WORKDIR /var/www/web

# Install necessary packages
RUN apk add --no-cache jq moreutils

# Copy published app to nginx web root
COPY --from=publish /app/publish/wwwroot .
COPY ["./src/ContentRating.Web.UI/update-blazor-env.sh", "/docker-entrypoint.d/40-update-blazor-env.sh"]
RUN chmod +x /docker-entrypoint.d/40-update-blazor-env.sh

COPY ./src/ContentRating.Web.UI/nginx.conf /etc/nginx/nginx.conf


# Copy Windows local certificate (you'll need to mount or copy these)
# These should be provided at runtime via volume mount or copied manually
# Example: docker run -v /path/to/certs:/https ...

# Expose ports
EXPOSE 80
EXPOSE 443