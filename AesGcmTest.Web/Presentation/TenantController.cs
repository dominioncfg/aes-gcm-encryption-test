using AesGcmTest.Application;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AesGcmTest.Web.Presentation
{
    public record GetAllUsersByTenantIdApiQueryResponse
    {
        public IEnumerable<GetAllUsersByTenantIdUserApiQueryResponse> Users { get; init; } = Array.Empty<GetAllUsersByTenantIdUserApiQueryResponse>();
    }

    public record GetAllUsersByTenantIdUserApiQueryResponse
    {
        public Guid TenantId { get; init; }
        public Guid Id { get; init; }
        public string Email { get; init; } = string.Empty;
        public string FirstName { get; init; } = string.Empty;
        public string LastName { get; init; } = string.Empty;
    }

    [ApiController]
    [Route("api/v1/tenant")]
    public class TenantController : ControllerBase
    {
        private readonly ISender _sender;

        public TenantController(ISender sender)
        {
            _sender = sender;
        }

        [HttpGet("{id}/users")]
        public async Task<GetAllUsersByTenantIdApiQueryResponse> GetUserByIdAsync([FromRoute] Guid id, CancellationToken cancellationToken)
        {
            var query = new GetAllUsersByTenantIdQuery()
            {
                TenantId = id,
            };
            var queryResponse = await _sender.Send(query, cancellationToken);
            var responseUserItems = queryResponse.Users
                .Select(x =>
                new GetAllUsersByTenantIdUserApiQueryResponse()
                {
                    TenantId = x.TenantId,
                    Id = x.Id,
                    Email = x.Email,
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                }).ToList();
            return new GetAllUsersByTenantIdApiQueryResponse()
            {
                Users = responseUserItems,
            };
        }

        [HttpPut("{id}/rotate-key")]
        public async Task<IActionResult> RotateKeyAsync([FromRoute] Guid id, CancellationToken cancellationToken)
        {
            var cmd = new RotateTenantEncryptionKeyCommand()
            {
                TenantId = id,
            };
            await _sender.Send(cmd, cancellationToken);
            return NoContent();
        }
    }
}
