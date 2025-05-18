using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Core.Models;


namespace Executors.Sandbox
{
    public class TestCasesExecutor : ITestCasesExecutor
    {
        private readonly string _baseTempPath = Path.Combine(Directory.GetCurrentDirectory(), "TempExecutions");

        public TestCasesExecutor()
        {
            Directory.CreateDirectory(_baseTempPath);
        }
        public async Task<CodeExecutionResponse> ExecuteCodeAsync(CodeExecutionRequest request,string testCases)
        {
            var tempDir = Path.Combine(_baseTempPath, Guid.NewGuid().ToString());
            Directory.CreateDirectory(tempDir);

            var language = request.Language.ToLower();
            var sourceFileName = GetSourceFileName(language);
            var inputFileName = "input.txt";
            var testCaseFileName = "test_cases.json";

            var sourceFilePath = Path.Combine(tempDir, sourceFileName);
            var inputFilePath = Path.Combine(tempDir, inputFileName);
            var testCasesFilePath = Path.Combine(tempDir, testCaseFileName);

            try
            {
                await File.WriteAllTextAsync(sourceFilePath, request.Code ?? "");
                await File.WriteAllTextAsync(inputFilePath, request.Input ?? "");
                await File.WriteAllTextAsync(testCasesFilePath, testCases);

                var imageName = GetDockerImage(language);

                var containerName = $"code_exec_{Guid.NewGuid().ToString().Replace("-", "")}";

                var processInfo = new ProcessStartInfo
                {
                    FileName = "docker",
                    Arguments = $"run --rm --name {containerName} -v \"{ConvertToDockerPath(tempDir)}:/app\" {imageName}",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using var process = new Process { StartInfo = processInfo };
                process.Start();

                // Wait for exit with 8 seconds timeout
                bool exited = process.WaitForExit(8000);
                if (!exited)
                {
                    try
                    {
                        // Kill the docker CLI process
                        process.Kill();
                    }
                    catch { /* ignore exceptions on kill */ }

                    try
                    {
                        // Stop the running container
                        var stopProcessInfo = new ProcessStartInfo
                        {
                            FileName = "docker",
                            Arguments = $"stop {containerName}",
                            RedirectStandardOutput = true,
                            RedirectStandardError = true,
                            UseShellExecute = false,
                            CreateNoWindow = true
                        };
                        using var stopProcess = new Process { StartInfo = stopProcessInfo };
                        stopProcess.Start();
                        stopProcess.WaitForExit();
                    }
                    catch { /* ignore exceptions on stop */ }

                    return new CodeExecutionResponse
                    {
                        Output = "",
                        Error = "Time limit exceeded",
                        ExitCode = -1,
                        Success = false
                    };
                }

                string stdout = await process.StandardOutput.ReadToEndAsync();
                string stderr = await process.StandardError.ReadToEndAsync();
                //if(process.ExitCode == 0)
                //{
                //    List<TestCaseState> testCasesResults = JsonSerializer.Deserialize<List<TestCaseState>>(stdout);
                //    Console.WriteLine(testCasesResults);
                //}

                return new CodeExecutionResponse
                {
                    Output = stdout,
                    Error = stderr,
                    ExitCode = process.ExitCode,
                    Success = process.ExitCode == 0 && string.IsNullOrWhiteSpace(stderr)
                };
            }
            catch (Exception ex)
            {
                return new CodeExecutionResponse
                {
                    Output = "",
                    Error = $"Docker execution failed: {ex.Message}",
                    ExitCode = -1,
                    Success = false
                };
            }
            finally
            {
                try
                {
                    Directory.Delete(tempDir, recursive: true);
                }
                catch { /* ignore */ }
            }
        }
        private string ConvertToDockerPath(string windowsPath)
        {
            // Example: C:\Users\Rajesh\Temp\abc123 → /c/Users/Rajesh/Temp/abc123
            var driveLetter = Path.GetPathRoot(windowsPath)?.Substring(0, 1).ToLower();
            var pathWithoutDrive = windowsPath.Substring(3).Replace("\\", "/");
            return $"/{driveLetter}/{pathWithoutDrive}";
        }

        private string GetDockerImage(string language) => language switch
        {
            "python" => "code-runner-python",
            "cpp" => "code-runner-cpp",
            "java" => "code-runner-java",
            "csharp" => "code-runner-csharp",
            "javascript" => "code-runner-js",
            "c" => "code-runner-c",
            _ => throw new Exception($"Unsupported language: {language}")
        };

        private string GetSourceFileName(string language) => language switch
        {
            "python" => "main.py",
            "cpp" => "main.cpp",
            "java" => "Main.java",
            "csharp" => "Main.cs",
            "javascript" => "main.js",
            "c" => "main.c",
            _ => throw new Exception($"Unsupported language: {language}")
        };
    }
}
