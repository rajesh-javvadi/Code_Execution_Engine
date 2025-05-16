FROM alpine:latest
RUN apk update && apk add --no-cache build-base gcc
WORKDIR /home/runner
WORKDIR /app
CMD g++ -o main Main.cpp && ./main < input.txt
