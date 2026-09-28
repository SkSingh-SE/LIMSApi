using System.Security.Claims;
using System.Threading.Tasks;
using LIMSApi.Dtos;
using LIMSApi.Helpers;
using LIMSApi.Middleware;
using LIMSApi.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LIMSApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UniversalReviewController : ControllerBase
    {
        private readonly IUniversalReviewService _review;
        private readonly IBranchContext _branchContext;

        public UniversalReviewController(IUniversalReviewService review, IBranchContext branchContext)
        {
            _review = review;
            _branchContext = branchContext;
        }

        private (long userId, long branchId, long organizationId) GetContext()
        {
            var user = LoggedInUserProvider.CurrentUser;
            long userId = user?.UserId ?? 0;
            if (userId == 0)
            {
                var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                long.TryParse(claim, out userId);
            }
            long branchId = _branchContext.CurrentBranchID ?? user?.BranchID ?? 0;
            long organizationId = _branchContext.CurrentOrganizationID ?? user?.OrganizationID ?? 0;
            return (userId, branchId, organizationId);
        }

        [HttpPost("{resultId}/assign-reviewer")]
        [RequirePermission(Permissions.UniversalReview.Review)]
        public async Task<IActionResult> AssignReviewer(long resultId, [FromBody] AssignReviewerRequestDto dto)
        {
            var ctx = GetContext();
            if (ctx.userId == 0 || ctx.branchId == 0 || ctx.organizationId == 0)
                return Unauthorized("Branch, user, or organization context missing.");
            var result = await _review.AssignReviewerAsync(resultId, dto.ReviewerID, ctx.userId, ctx.branchId, ctx.organizationId, dto.Remarks);
            return Ok(new { success = true, data = result });
        }

        [HttpPost("{resultId}/findings")]
        [RequirePermission(Permissions.UniversalReview.Review)]
        public async Task<IActionResult> CreateFinding(long resultId, [FromBody] CreateFindingRequestDto dto)
        {
            var ctx = GetContext();
            if (ctx.userId == 0 || ctx.branchId == 0 || ctx.organizationId == 0)
                return Unauthorized("Branch, user, or organization context missing.");
            var finding = await _review.CreateFindingAsync(resultId, dto, ctx.userId, ctx.branchId, ctx.organizationId);
            return Ok(new { success = true, data = finding });
        }

        [HttpPost("findings/{findingId}/resolve")]
        [RequirePermission(Permissions.UniversalReview.Review)]
        public async Task<IActionResult> ResolveFinding(long findingId, [FromBody] ResolveFindingRequestDto dto)
        {
            var ctx = GetContext();
            if (ctx.userId == 0 || ctx.branchId == 0 || ctx.organizationId == 0)
                return Unauthorized("Branch, user, or organization context missing.");
            var finding = await _review.ResolveFindingAsync(findingId, dto.Resolution, ctx.userId, ctx.branchId, ctx.organizationId);
            return Ok(new { success = true, data = finding });
        }

        [HttpPost("{resultId}/request-rework")]
        [RequirePermission(Permissions.UniversalReview.Review)]
        public async Task<IActionResult> RequestRework(long resultId, [FromBody] ReviewActionRequestDto dto)
        {
            var ctx = GetContext();
            if (ctx.userId == 0 || ctx.branchId == 0 || ctx.organizationId == 0)
                return Unauthorized("Branch, user, or organization context missing.");
            var result = await _review.RequestReworkAsync(resultId, dto.ConcurrencyToken, ctx.userId, ctx.branchId, ctx.organizationId, dto.Remarks);
            return Ok(new { success = true, data = result });
        }

        [HttpPost("{resultId}/verify")]
        [RequirePermission(Permissions.UniversalReview.Verify)]
        public async Task<IActionResult> Verify(long resultId, [FromBody] ReviewActionRequestDto dto)
        {
            var ctx = GetContext();
            if (ctx.userId == 0 || ctx.branchId == 0 || ctx.organizationId == 0)
                return Unauthorized("Branch, user, or organization context missing.");
            var result = await _review.VerifyAsync(resultId, dto.ConcurrencyToken, ctx.userId, ctx.branchId, ctx.organizationId, dto.Remarks);
            return Ok(new { success = true, data = result });
        }

        [HttpPost("{resultId}/approve")]
        [RequirePermission(Permissions.UniversalReview.Approve)]
        public async Task<IActionResult> Approve(long resultId, [FromBody] ReviewActionRequestDto dto)
        {
            var ctx = GetContext();
            if (ctx.userId == 0 || ctx.branchId == 0 || ctx.organizationId == 0)
                return Unauthorized("Branch, user, or organization context missing.");
            var result = await _review.ApproveAsync(resultId, dto.ConcurrencyToken, ctx.userId, ctx.branchId, ctx.organizationId, dto.Remarks);
            return Ok(new { success = true, data = result });
        }

        [HttpPost("{resultId}/reject")]
        [RequirePermission(Permissions.UniversalReview.Approve)]
        public async Task<IActionResult> Reject(long resultId, [FromBody] ReviewActionRequestDto dto)
        {
            var ctx = GetContext();
            if (ctx.userId == 0 || ctx.branchId == 0 || ctx.organizationId == 0)
                return Unauthorized("Branch, user, or organization context missing.");
            var result = await _review.RejectApprovalAsync(resultId, dto.ConcurrencyToken, ctx.userId, ctx.branchId, ctx.organizationId, dto.Remarks);
            return Ok(new { success = true, data = result });
        }
    }
}
