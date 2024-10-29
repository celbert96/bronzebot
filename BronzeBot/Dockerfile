FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
USER $APP_UID
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
#ARG BRONZE_BOT_TOKEN=0
#RUN --mount=type=secret,id=BRONZE_BOT_TOKEN,env=BRONZE_BOT_TOKEN
#ENV BRONZE_BOT_TOKEN=${BRONZE_BOT_TOKEN}
#RUN --mount=type=secret,id=RL_SERVER_ID,env=RL_SERVER_ID
#RUN --mount=type=secret,id=RL_VOICE_CHANNEL_ID,env=RL_VOICE_CHANNEL_ID
#ARG RL_SERVER_ID=0
#ENV RL_SERVER_ID=${RL_SERVER_ID}
#ARG RL_VOICE_CHANNEL_ID=0
#ENV RL_VOICE_CHANNEL_ID=${RL_VOICE_CHANNEL_ID}
WORKDIR /src
COPY . ./
RUN dotnet restore
COPY . .
RUN dotnet build "BronzeBot.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "BronzeBot.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BronzeBot.dll"]
