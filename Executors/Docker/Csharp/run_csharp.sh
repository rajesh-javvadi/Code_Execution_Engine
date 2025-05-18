#!/bin/bash

# Copy input files
if ! cp /app/Main.cs /runner/Main.cs; then
  echo "{\"error\": \"/app/Main.cs not found\"}"
  exit 1
fi

if ! cp /app/test_cases.json /runner/test_cases.json; then
  echo "{\"error\": \"/app/test_cases.json not found\"}"
  exit 1
fi

# Compile with Mono C# compiler (mcs)
if ! mcs -out:/runner/Main.exe /runner/Main.cs; then
  echo "{\"error\": \"Failed to compile C# program\"}"
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

  # Run the compiled executable directly
  output=$(echo "$input" | timeout 5s mono /runner/Main.exe 2>&1 || true)

  # Normalize output (remove Windows line endings if any)
  output=$(echo "$output" | sed 's/\r$//')
  expected=$(echo "$expected" | sed 's/\r$//')

  if [[ "$output" == "$expected" ]]; then
    ((pass_count++))
  else
    ((fail_count++))
  fi
done

# Output result as valid JSON
echo "{\"passed\": $pass_count, \"failed\": $fail_count}"