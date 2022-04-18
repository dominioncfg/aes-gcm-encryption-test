using AesGcmTest.Domain;
using MediatR;

namespace AesGcmTest.Application;

public record CreateUserCommand : IRequest
{
    public Guid TenantId { get; init; }
    public Guid Id { get; init; }
    public string Email { get; init; } = string.Empty;
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;

    public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand>
    {
        private readonly IUserRepository _userRepository;
     
        public CreateUserCommandHandler(IUserRepository repository)
        {
            _userRepository = repository;
        }

        public async Task<Unit> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            var user = User.Create(request.TenantId, request.Id, request.Email, request.FirstName, request.LastName);
            await _userRepository.AddAsync(user, cancellationToken);
            return Unit.Value;
        }
    }
}
