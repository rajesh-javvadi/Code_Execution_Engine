#!/bin/bash

cp /mnt/main.py /tmp/main.py
TEST_CASES_JSON=$(cat /mnt/test_cases.json)
RESULTS="["

count=$(echo "$TEST_CASES_JSON" | jq length)
for (( i=0; i<$count; i++ )); do
  input=$(echo "$TEST_CASES_JSON" | jq -r ".[$i].input")
  expected=$(echo "$TEST_CASES_JSON" | jq -r ".[$i].expectedOutput")
  output=$(echo "$input" | timeout 5s python3 /tmp/main.py 2>&1)

  status="fail"
  if [[ "$output" == "$expected" ]]; then
    status="pass"
  fi

  RESULTS+="{\"input\":\"$input\",\"expected\":\"$expected\",\"output\":\"$output\",\"status\":\"$status\"}"
  [ $i -lt $((count - 1)) ] && RESULTS+=","
done

RESULTS+="]"
echo $RESULTS
