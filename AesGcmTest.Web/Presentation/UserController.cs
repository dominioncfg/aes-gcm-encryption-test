using AesGcmTest.Application;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AesGcmTest.Web.Presentation
{
    public record CreateUserApiRequest
    {
        public Guid TenantId { get; init; }
        public Guid Id { get; init; }
        public string Email { get; init; } = string.Empty;
        public string FirstName { get; init; } = string.Empty;
        public string LastName { get; init; } = string.Empty;
    }

    public record ChangeUserNameApiRequest
    {
        public string FirstName { get; init; } = string.Empty;
        public string LastName { get; init; } = string.Empty;
    }
    public record GetUserByIdQueryResponse
    {
        public Guid TenantId { get; init; }
        public Guid Id { get; init; }
        public string Email { get; init; } = string.Empty;
        public string FirstName { get; init; } = string.Empty;
        public string LastName { get; init; } = string.Empty;
    }

    [ApiController]
    [Route("api/v1/user")]
    public class UserController : ControllerBase
    {
        private readonly ISender _sender;
        public UserController(ISender sender)
        {
            _sender = sender;
        }

        [HttpPost]
        public async Task<ActionResult> CreateAsync(CreateUserApiRequest request, CancellationToken cancellationToken)
        {
            var command = new CreateUserCommand()
            {
                TenantId = request.TenantId,
                Id = request.Id,
                Email = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
            };
            await _sender.Send(command, cancellationToken);
            return Created($"api/v1/user{request.Id}", new { request.Id });
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> ChangeUserNameAsync([FromRoute] Guid id, [FromBody] ChangeUserNameApiRequest request, CancellationToken cancellationToken)
        {
            var command = new ChangeUserNameCommand()
            {
                Id = id,
                FirstName = request.FirstName,
                LastName = request.LastName,
            };
            await _sender.Send(command, cancellationToken);
            return NoContent();
        }

        [HttpGet("{id}")]
        public async Task<GetUserByIdQueryResponse> GetUserByIdAsync([FromRoute] Guid id, CancellationToken cancellationToken)
        {
            var query = new GetUserByIdQuery()
            {
                Id = id,
            };
            var queryResponse = await _sender.Send(query, cancellationToken);
            return new GetUserByIdQueryResponse()
            {
                TenantId = queryResponse.TenantId,
                Id = queryResponse.Id,
                Email = queryResponse.Email,
                FirstName = queryResponse.FirstName,
                LastName = queryResponse.LastName,
            };
        }
    }
}
