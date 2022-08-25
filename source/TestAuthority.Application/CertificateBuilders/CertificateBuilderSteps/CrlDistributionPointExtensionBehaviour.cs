using MediatR;
using Microsoft.Extensions.Options;
using Org.BouncyCastle.Asn1.X509;
using TestAuthority.Domain.Models;

namespace TestAuthority.Application.CertificateBuilders.CertificateBuilderSteps;

public class CrlDistributionPointExtensionBehaviour : IPipelineBehavior<CertificateBuilderRequest, CertificateWithKey>
{
    private readonly IOptions<CrlSettings> crlSettings;

    public CrlDistributionPointExtensionBehaviour(IOptions<CrlSettings> crlSettings)
    {
        this.crlSettings = crlSettings;
    }

    public async Task<CertificateWithKey> Handle(CertificateBuilderRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<CertificateWithKey> next)
    {
        var distributionPointUris = crlSettings.Value?.CrlDistributionPoints?.ToList() ?? new List<string>();
        if (distributionPointUris.Any() == false)
        {
            return await next();
        }
        var distributionPoints = new List<DistributionPoint>();

        foreach (var distributionPointUri in distributionPointUris)
        {
            var generalNames = new GeneralNames(new GeneralName(GeneralName.UniformResourceIdentifier,  distributionPointUri));
            var distributionPointName = new DistributionPointName(generalNames);
            var crlDistributionPoint = new DistributionPoint(distributionPointName,null,null);
            distributionPoints.Add(crlDistributionPoint);
        }


        CrlDistPoint extension = new CrlDistPoint(distributionPoints.ToArray());
        request.CertificateGenerator.AddExtension(X509Extensions.CrlDistributionPoints.Id, false, extension);
        return await next();
    }
}
