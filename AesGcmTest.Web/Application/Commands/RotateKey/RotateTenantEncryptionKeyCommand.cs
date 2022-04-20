using MediatR;

namespace AesGcmTest.Application;

public record RotateTenantEncryptionKeyCommand : IRequest
{
    public Guid TenantId { get; init; }

    public class RotateKeyCommandHandler : IRequestHandler<RotateTenantEncryptionKeyCommand>
    {
        private readonly ITenancySymmetricKeyService _tenancySymmetricKeyService;

        public RotateKeyCommandHandler(ITenancySymmetricKeyService tenancySymmetricKeyService)
        {
            _tenancySymmetricKeyService = tenancySymmetricKeyService;
        }

        public async Task<Unit> Handle(RotateTenantEncryptionKeyCommand request, CancellationToken cancellationToken)
        {
            await _tenancySymmetricKeyService.RotateSymmetricEncryptionKeyAsync(request.TenantId, cancellationToken);
            return Unit.Value;
        }
    }
}
