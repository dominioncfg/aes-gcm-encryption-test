using AesGcmTest.Domain;
using MediatR;

namespace AesGcmTest.Application;

public record GetAllUsersByTenantIdQuery : IRequest<GetAllUsersByTenantIdQueryResponse>
{
    public Guid TenantId { get; init; }

    public class GetAllUsersByTenantIdQueryHandler : IRequestHandler<GetAllUsersByTenantIdQuery, GetAllUsersByTenantIdQueryResponse>
    {
        private readonly IUserRepository _userRepository;

        public GetAllUsersByTenantIdQueryHandler(IUserRepository repository)
        {
            _userRepository = repository;
        }

        public async Task<GetAllUsersByTenantIdQueryResponse> Handle(GetAllUsersByTenantIdQuery request, CancellationToken cancellationToken)
        {
            var users = await _userRepository.GetAllByTenantIdAsync(request.TenantId, cancellationToken);

            var responseUsers = users
                .Select(user => new GetAllUsersByTenantIdUserQueryResponse()
                {
                    TenantId = user.TenantId,
                    Id = user.Id,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                })
                .ToList();

            return new GetAllUsersByTenantIdQueryResponse()
            {
                Users = responseUsers,
            };
        }
    }
}

public record GetAllUsersByTenantIdQueryResponse
{
    public IEnumerable<GetAllUsersByTenantIdUserQueryResponse> Users { get; init; } = Array.Empty<GetAllUsersByTenantIdUserQueryResponse>();
}

public record GetAllUsersByTenantIdUserQueryResponse
{
    public Guid TenantId { get; init; }
    public Guid Id { get; init; }
    public string Email { get; init; } = string.Empty;
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
}
