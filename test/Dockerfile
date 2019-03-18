FROM microsoft/dotnet:2.1-sdk
COPY . /app
WORKDIR /app/src/SC.DevChallenge.UnitTests
RUN ["dotnet", "restore"]
RUN ["dotnet", "build"]
CMD ["dotnet", "test"]