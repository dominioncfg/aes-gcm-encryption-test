using AesGcmTest.Application;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AesGcmTest.Web.Presentation;

[ApiController]
[Route("api/v1/tenants")]
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
