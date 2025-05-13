FROM openjdk:17-slim
RUN useradd -m runner
WORKDIR /home/runner
WORKDIR /app
USER runner
CMD javac Main.java && java Main < input.txt