FROM openjdk:17-slim

RUN apt-get update && apt-get install -y jq coreutils && rm -rf /var/lib/apt/lists/*

WORKDIR /runner

COPY run_java.sh /runner/run.sh
RUN chmod +x /runner/run.sh

ENTRYPOINT ["/runner/run.sh"]
