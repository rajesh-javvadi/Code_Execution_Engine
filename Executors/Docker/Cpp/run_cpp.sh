#!/bin/bash

if ! cp /app/main.cpp /runner/main.cpp; then
  echo "{\"error\": \"/app/main.cpp not found\"}"
  exit 1
fi

if ! cp /app/test_cases.json /runner/test_cases.json; then
  echo "{\"error\": \"/app/test_cases.json not found\"}"
  exit 1
fi

# Compile C++ program
if ! g++ -o /runner/main /runner/main.cpp; then
  echo "{\"error\": \"Failed to compile C++ program\"}"
  exit 1
fi

TEST_CASES_JSON=$(cat /runner/test_cases.json 2>/dev/null)
if [[ -z "$TEST_CASES_JSON" ]]; then
  echo "{\"error\": \"test_cases.json is empty or invalid\"}"
  exit 1
fi

count=$(echo "$TEST_CASES_JSON" | jq length)
pass_count=0
fail_count=0

for (( i=0; i<$count; i++ )); do
  input=$(echo "$TEST_CASES_JSON" | jq -r ".[$i].Input")
  expected=$(echo "$TEST_CASES_JSON" | jq -r ".[$i].ExpectedOutput")

  if [[ "$input" == "null" || -z "$input" ]]; then
    ((fail_count++))
    continue
  fi

  output=$(echo "$input" | timeout 5s /runner/main 2>&1)
  exit_code=$?

  if [[ $exit_code -eq 124 ]]; then
    output="Timeout after 5 seconds"
  fi

  if [[ "$output" == "$expected" ]]; then
    ((pass_count++))
  else
    ((fail_count++))
  fi
done

echo "{\"passed\": $pass_count, \"failed\": $fail_count}"