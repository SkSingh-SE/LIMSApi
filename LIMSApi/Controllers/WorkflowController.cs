using LIMSApi.Dtos;
using LIMSApi.Models;
using LIMSApi.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using Twilio.TwiML.Voice;


namespace LIMSApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WorkflowController : ControllerBase
    {
        private readonly IWorkflowService _service;

        public WorkflowController(IWorkflowService service)
        {
            _service = service;
        }

        [HttpGet("details/{id}")]
        public async Task<IActionResult> GetWorkflow(long id)
        {
            var workflow = await _service.GetWorkflow(id);
            return workflow == null ? NotFound() : Ok(workflow);
        }

        [HttpPost("list")]
        public async Task<IActionResult> GetAllWorkflows(PageFilter filter)
        {
            var workflows = await _service.GetAllWorkflows(filter);
            return Ok(workflows);
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateWorkflow(WorkflowDto workflow)
        {
            await _service.CreateWorkflow(workflow);
            return Ok(new
            {
                status = "success",
                message = $"Workflow '{workflow.Name}' created successfully."
            });
        }
        [HttpPut("update")]
        public async Task<IActionResult> UpdateWorkflow(WorkflowDto workflow)
        {
            await _service.UpdateWorkflow(workflow);
            return Ok(new
            {
                status = "success",
                message = $"Workflow '{workflow.Name}' created successfully."
            });
        }

        [HttpPost("start")]
        public async Task<IActionResult> StartWorkflow(long entityId, string entityType)
        {
            await _service.StartWorkflow(entityId, entityType);
            return Ok(new
            {
                status = "success",
                message = $"Workflow Started"
            });
        }

        [HttpPost("perform-action")]
        public async Task<IActionResult> PerformAction(WorkflowActionRequestDto request)
        {
            await _service.PerformWorkflowActionAsync(request);
            return Ok(new
            {
                status = "success",
                message = $"Action performed"
            });
        }
        [HttpPost("log-history/{workflowId}")]
        public async Task<IActionResult> WorkflowActionHistory(long workflowId)
        {
            return Ok(await _service.GetWorkflowActionHistory(workflowId));
        }
    }
}
