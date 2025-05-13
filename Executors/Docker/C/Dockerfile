FROM gcc:latest
RUN useradd -m runner
WORKDIR /home/runner
WORKDIR /app
USER runner
CMD gcc -o main Main.c && ./main < input.txt
