FROM python:latest
RUN useradd -m runner
WORKDIR /home/runner
WORKDIR /app
USER runner
CMD python3 main.py < input.txt