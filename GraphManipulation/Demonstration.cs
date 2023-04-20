using GraphManipulation.Linking;
using GraphManipulation.Logging;
using GraphManipulation.Managers.Interfaces;
using GraphManipulation.Models;

namespace GraphManipulation;

public class Demonstration<TMI, TVI, TPI, TOI>
{
    public Demonstration(
        IMetadataManager<TMI> metadataManager,
        IVacuumingManager<TVI> vacuumingManager,
        IPurposeManager<TPI> purposeManager,
        IOriginManager<TOI> originManager,
        ILogger logger,
        ILinker<VacuumingRule<TVI>, Purpose<TPI>, TVI, TPI> vacuumingPurposeLinker,
        ILinker<Purpose<TPI>, Metadata<TMI>, TPI, TMI> purposeMetadataLinker,
        ILinker<Origin<TOI>, Metadata<TMI>, TOI, TMI> originMetadataLinker)
    {
        
    }
}