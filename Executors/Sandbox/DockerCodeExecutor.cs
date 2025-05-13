﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Core.Interfaces;
using Core.Models;
using System.Diagnostics;
using static IronPython.Modules._ast;

namespace Executors.Sandbox
{
    public class DockerCodeExecutor : ICodeExecutor
    {
        private readonly string _baseTempPath = Path.Combine(Directory.GetCurrentDirectory(), "TempExecutions");

        public DockerCodeExecutor()
        {
            Directory.CreateDirectory(_baseTempPath);
        }

        public async Task<CodeExecutionResponse> ExecuteCodeAsync(CodeExecutionRequest request)
        {
            var tempDir = Path.Combine(_baseTempPath, Guid.NewGuid().ToString());
            Directory.CreateDirectory(tempDir);

            var language = request.Language.ToLower();
            var sourceFileName = GetSourceFileName(language);
            var inputFileName = "input.txt";

            var sourceFilePath = Path.Combine(tempDir, sourceFileName);
            var inputFilePath = Path.Combine(tempDir, inputFileName);

            try
            {
                await File.WriteAllTextAsync(sourceFilePath, request.Code ?? "");
                await File.WriteAllTextAsync(inputFilePath, request.Input ?? "");

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
                var dockerPath = ConvertToDockerPath(tempDir);
                Console.WriteLine($"[DEBUG] Docker mount path: {dockerPath}");

                using var process = new Process { StartInfo = processInfo };
                process.Start();

                // Wait for exit with 5 seconds timeout
                bool exited = process.WaitForExit(5000);
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

