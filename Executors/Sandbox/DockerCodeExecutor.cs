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
            var outputFileName = "output.txt";

            var sourceFilePath = Path.Combine(tempDir, sourceFileName);
            var inputFilePath = Path.Combine(tempDir, inputFileName);

            try
            {
                await File.WriteAllTextAsync(sourceFilePath, request.Code ?? "");
                await File.WriteAllTextAsync(inputFilePath, request.Input ?? "");
                Console.WriteLine($"[DEBUG] Written files to: {tempDir}");
                Console.WriteLine($"main.py exists: {File.Exists(sourceFilePath)}");
                Console.WriteLine($"input.txt exists: {File.Exists(inputFilePath)}");

                var imageName = GetDockerImage(language);

                var processInfo = new ProcessStartInfo
                {
                    FileName = "docker",
                    Arguments = $"run --rm -v \"{ConvertToDockerPath(tempDir)}:/app\" {imageName}",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };
                var dockerPath = ConvertToDockerPath(tempDir);
                Console.WriteLine($"[DEBUG] Docker mount path: {dockerPath}");

                using var process = new Process { StartInfo = processInfo };
                process.Start();

                string stdout = await process.StandardOutput.ReadToEndAsync();
                string stderr = await process.StandardError.ReadToEndAsync();

                process.WaitForExit();

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
            "csharp" =>  "code-runner-csharp",
            _ => throw new Exception($"Unsupported language: {language}")
        };

        private string GetSourceFileName(string language) => language switch
        {
            "python" => "main.py",
            "cpp" => "main.cpp",
            "java" => "Main.java",
            "csharp" => "Main.cs",
            _ => throw new Exception($"Unsupported language: {language}")
        };
    }
}

