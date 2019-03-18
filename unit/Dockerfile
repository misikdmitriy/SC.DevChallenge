FROM microsoft/dotnet:2.1-sdk
COPY . /app
WORKDIR /app
RUN ["dotnet", "restore"]
WORKDIR /app/src/SC.DevChallenge.UnitTests
RUN ["dotnet", "build"]
CMD ["dotnet", "test"]