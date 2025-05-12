#!/bin/bash

cd /app
python3 main.py < input.txt > output.txt 2>&1
cat output.txt
