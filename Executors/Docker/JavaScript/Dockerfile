FROM node:latest
RUN useradd -m runner
WORKDIR /home/runner
WORKDIR /app
USER runner
CMD node Main.js < input.txt
