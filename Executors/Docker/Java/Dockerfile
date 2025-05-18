FROM openjdk:17-jdk-slim

# Install dependencies
RUN apt-get update && \
    apt-get install -y jq coreutils && \
    rm -rf /var/lib/apt/lists/*

WORKDIR /runner

# Copy and fix script encoding (remove BOM if present)
COPY run_java.sh /runner/run.sh
RUN sed -i '1s/^\xEF\xBB\xBF//' /runner/run.sh && \
    sed -i 's/\r$//' /runner/run.sh && \
    chmod +x /runner/run.sh

ENTRYPOINT ["/bin/bash", "/runner/run.sh"]