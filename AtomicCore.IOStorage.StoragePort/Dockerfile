#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
RUN apt update -y && apt install nano -y && apt install tzdata -y
WORKDIR /app
EXPOSE 80 50051

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["AtomicCore.IOStorage.StoragePort/AtomicCore.IOStorage.StoragePort.csproj", "AtomicCore.IOStorage.StoragePort/"]
COPY ["AtomicCore.IOStorage.Core/AtomicCore.IOStorage.Core.csproj", "AtomicCore.IOStorage.Core/"]
COPY ["AtomicCore/AtomicCore.csproj", "AtomicCore/"]
RUN dotnet restore "AtomicCore.IOStorage.StoragePort/AtomicCore.IOStorage.StoragePort.csproj"
COPY . .
WORKDIR "/src/AtomicCore.IOStorage.StoragePort"
RUN dotnet build "AtomicCore.IOStorage.StoragePort.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "AtomicCore.IOStorage.StoragePort.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app

ENV TZ=Asia/Shanghai
# 2G * 75% = 16106127360 
# 75 = 0x4B
#ENV DOTNET_GCHeapHardLimit=16106127360
#ENV DOTNET_GCHeapHardLimitPercent=0x4B

ENV IOSTORAGE_APPTOKEN=
ENV IOSTORAGE_SAVEROOTDIR=
ENV IOSTORAGE_ALLOWFILEEXTS=
ENV IOSTORAGE_ALLOWFILEMBSIZELIMIT=

COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AtomicCore.IOStorage.StoragePort.dll"]