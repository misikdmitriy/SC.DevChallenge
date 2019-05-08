FROM microsoft/dotnet:2.2-sdk
COPY . /app
WORKDIR /app
RUN ["dotnet", "restore"]
RUN ["dotnet", "build"]
RUN chmod +x ./entrypoint.sh
RUN chmod 777 ./src/SC.DevChallenge.Core/Scripts/script.sql
CMD /bin/bash ./entrypoint.sh