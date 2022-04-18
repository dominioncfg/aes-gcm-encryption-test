using AesGcmTest.Domain;
using MediatR;

namespace AesGcmTest.Application;

public record ChangeUserNameCommand : IRequest
{
    public Guid Id { get; init; }
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;

    public class ChangeUserNameCommandHandler : IRequestHandler<ChangeUserNameCommand>
    {
        private readonly IUserRepository _userRepository;

        public ChangeUserNameCommandHandler(IUserRepository repository)
        {
            _userRepository = repository;
        }

        public async Task<Unit> Handle(ChangeUserNameCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(request.Id, cancellationToken);
            user.ChangeName(request.FirstName, request.LastName);
            await _userRepository.UpdateAsync(user,cancellationToken);
            return Unit.Value;
        }
    }
}
