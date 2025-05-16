using Microsoft.AspNetCore.Mvc;
using Core.Interfaces;
using Core.Models;

namespace Code_Execution_Engine.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CodeExecutionController : ControllerBase
    {
        private readonly ICodeExecutor _codeExecutor;
        private readonly ITestCaseExecutor _testCaseExecutor;

        public CodeExecutionController(ICodeExecutor codeExecutor, ITestCaseExecutor testCaseExecutor)
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
                List<TestCase> testCases = new List<TestCase>();
                testCases.Add(new TestCase { Input = "1 5" ,ExpectedOutput = "6"});
                testCases.Add(new TestCase { Input = "2 5", ExpectedOutput = "7" });
                testCases.Add(new TestCase { Input = "3 5" ,ExpectedOutput = "8" });
                testCases.Add(new TestCase { Input = "4 5" ,ExpectedOutput = "9" });
                testCases.Add(new TestCase { Input = "5 5" ,ExpectedOutput = "10" });
                testCases.Add(new TestCase { Input = "6 5" ,ExpectedOutput = "11"});
                var queueId = await _testCaseExecutor.ExecuteTestCases(request, testCases);
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

        [HttpGet("execute/testCases/{queueId}")]
        public async Task<ActionResult<TestSuiteExecutionResult>> GetTestCaseExecutionResult(Guid queueId)
        {
            try
            {
                var result = await _testCaseExecutor.GetTestCaseExecutionResult(queueId);
                if (result == null)
                {
                    return NotFound("Result not found or still processing");
                }
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
    }
}
