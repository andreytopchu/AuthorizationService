#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 5001

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["src/Identity/Identity/Identity.csproj", "Identity/Identity/"]
COPY ["src/Shared/Shared/Shared.csproj", "Shared/Shared/"]
COPY ["src/Shared/Shared.Exceptions/Shared.Exceptions.csproj", "Shared/Shared.Exceptions/"]
COPY ["src/Shared/gRPC.UserStore/gRPC.UserStore.csproj", "Shared/gRPC.UserStore/"]
COPY ["src/Shared/Shared.Validation/Shared.Validation.csproj", "Shared/Shared.Validation/"]
COPY ["src/Shared/gRPC.UserStore/gRPC.UserStore.csproj", "Shared/gRPC.UserStore/gRPC.UserStore/"]
COPY ./src/certs/rootCA.crt /usr/local/share/ca-certificates
COPY ./*.sh /src
RUN update-ca-certificates && chmod +x entrypoint_identity.sh && mkdir -p /etc/certs
COPY ./src/certs/ /etc/certs/
RUN dotnet restore "Identity/Identity/Identity.csproj"
COPY . .
WORKDIR "/src/Identity/Identity"
RUN dotnet build "Identity.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Identity.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Identity.dll"]