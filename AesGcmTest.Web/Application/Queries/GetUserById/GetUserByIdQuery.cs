using AesGcmTest.Domain;
using MediatR;

namespace AesGcmTest.Application;

public record GetUserByIdQuery : IRequest<GetUserByIdQueryResponse>
{
    public Guid Id { get; init; }

    public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, GetUserByIdQueryResponse>
    {
        private readonly IUserRepository _userRepository;

        public GetUserByIdQueryHandler(IUserRepository repository)
        {
            _userRepository = repository;
        }

        public async Task<GetUserByIdQueryResponse> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(request.Id, cancellationToken);

            return new GetUserByIdQueryResponse()
            {
                TenantId = user.TenantId,
                Id = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
            };
        }
    }
}

public record GetUserByIdQueryResponse
{
    public Guid TenantId { get; init; }
    public Guid Id { get; init; }
    public string Email { get; init; } = string.Empty;
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
}
