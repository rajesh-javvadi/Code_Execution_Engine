using Microsoft.AspNetCore.Mvc;
using Core.Interfaces;
using Core.Models;
using System.Text.Json;
using Executors.Sandbox;

namespace Code_Execution_Engine.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CodeExecutionController : ControllerBase
    {
        private readonly ICodeExecutor _codeExecutor;
        private readonly ITestCasesExecutor _testCaseExecutor;

        public CodeExecutionController(ICodeExecutor codeExecutor, ITestCasesExecutor testCaseExecutor)
        {
            _codeExecutor = codeExecutor;
            _testCaseExecutor = testCaseExecutor;
        }

        [HttpPost("execute")]
        public async Task<ActionResult<CodeExecutionResponse>> Execute([FromBody] CodeExecutionRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Language) || string.IsNullOrWhiteSpace(request.Code))
            {
                return BadRequest("Invalid request");
            }

            try
            {
                var result = await _codeExecutor.ExecuteCodeAsync(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new CodeExecutionResponse
                {
                    Output = "",
                    Error = $"Internal Server Error: {ex.Message}",
                    ExitCode = -1,
                    Success = false
                });
            }
        }


        [HttpPost("execute/testCases")]
        public async Task<ActionResult<Guid>> ExecuteTestCases([FromBody] CodeExecutionRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Language) || string.IsNullOrWhiteSpace(request.Code))
            {
                return BadRequest("Invalid request");
            }
            try
            {
                List<TestCase> testCases = new List<TestCase>
{
    new TestCase { Input = "1 2 3 4 5", ExpectedOutput = "5 4 3 2 1" },
    new TestCase { Input = "1 2", ExpectedOutput = "2 1" },
    new TestCase { Input = "1", ExpectedOutput = "1" },
    new TestCase { Input = "10 20 30 40", ExpectedOutput = "40 30 20 10" },
    new TestCase { Input = "7 7 7", ExpectedOutput = "7 7 7" },
    new TestCase { Input = "5 4 3 2 1", ExpectedOutput = "1 2 3 4 5" },
    new TestCase { Input = "100", ExpectedOutput = "100" },
    new TestCase { Input = "0 0 0 0 0", ExpectedOutput = "0 0 0 0 0" },
    new TestCase { Input = "9 8 7 6 5 4 3 2 1", ExpectedOutput = "1 2 3 4 5 6 7 8 9" }
};



                string json = JsonSerializer.Serialize(testCases, new JsonSerializerOptions { WriteIndented = true });
                Console.WriteLine(json);

                var queueId = await _testCaseExecutor.ExecuteCodeAsync(request, json);
                return Ok(queueId);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new CodeExecutionResponse
                {
                    Output = "",
                    Error = $"Internal Server Error: {ex.Message}",
                    ExitCode = -1,
                    Success = false
                });
            }
        }

        //[HttpGet("execute/testCases/{queueId}")]
        //public async Task<ActionResult<TestSuiteExecutionResult>> GetTestCaseExecutionResult(Guid queueId)
        //{
        //    try
        //    {
        //        var result = await _testCaseExecutor.GetTestCaseExecutionResult(queueId);
        //        if (result == null)
        //        {
        //            return NotFound("Result not found or still processing");
        //        }
        //        return Ok(result);
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, new CodeExecutionResponse
        //        {
        //            Output = "",
        //            Error = $"Internal Server Error: {ex.Message}",
        //            ExitCode = -1,
        //            Success = false
        //        });
        //    }
        //}
    }
}
