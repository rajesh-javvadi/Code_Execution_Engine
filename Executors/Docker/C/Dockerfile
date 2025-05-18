FROM alpine:latest

# Install build tools + JSON parser
RUN apk update && \
    apk add --no-cache \
    build-base \
    gcc \
    jq \
    bash \
    coreutils

WORKDIR /runner

COPY run_c.sh /runner/run.sh
RUN chmod +x /runner/run.sh && \
    sed -i 's/\r$//' /runner/run.sh

ENTRYPOINT ["/bin/bash", "/runner/run.sh"]
