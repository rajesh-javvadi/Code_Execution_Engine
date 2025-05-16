FROM alpine:latest
RUN apk update && apk add --no-cache build-base gcc
WORKDIR /home/runner
WORKDIR /app
CMD gcc -o main Main.c && ./main < input.txt
