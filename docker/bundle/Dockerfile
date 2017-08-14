FROM zhangsiy/linux-dev:0.1.0
# add package folder to container
ARG artifactdir='./output/publishOutput'
ADD $artifactdir ./app
# set up working directory
WORKDIR /app
# Expose Port
ARG containerport=5000
ENV ASPNETCORE_URLS http://*:$containerport
EXPOSE $containerport
# setup entry point
ARG entryassembly
ENV ENTRY=$entryassembly
ENTRYPOINT ["/bin/bash", "-c", "dotnet $ENTRY"]
