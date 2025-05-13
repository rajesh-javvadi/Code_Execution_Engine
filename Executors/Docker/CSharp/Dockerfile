FROM  mono:latest
RUN useradd -m runner
WORKDIR /home/runner
WORKDIR /app
USER runner
CMD mcs -out:main.exe Main.cs > /dev/null 2>&1 && mono main.exe < input.txt
