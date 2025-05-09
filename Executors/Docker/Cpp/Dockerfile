FROM gcc:latest

RUN useradd -m runner
WORKDIR /home/runner
COPY run.sh .
RUN chmod +x run.sh

USER runner
CMD ["./run.sh"]
