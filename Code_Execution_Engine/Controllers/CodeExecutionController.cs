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

        public CodeExecutionController(ICodeExecutor codeExecutor)
        {
            _codeExecutor = codeExecutor;
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
    }
}
