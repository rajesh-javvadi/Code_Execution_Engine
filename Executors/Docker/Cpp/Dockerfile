FROM gcc:latest
RUN useradd -m runner
WORKDIR /home/runner
WORKDIR /app
USER runner
CMD g++ -o main Main.cpp && ./main < input.txt
