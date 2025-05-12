FROM mcr.microsoft.com/dotnet/sdk:7.0

RUN useradd -m runner
WORKDIR /home/runner
COPY run.sh .
RUN chmod +x run.sh

USER runner
CMD ["./run.sh"]


