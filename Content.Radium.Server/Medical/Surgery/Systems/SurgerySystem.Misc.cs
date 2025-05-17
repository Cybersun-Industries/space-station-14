using System.Diagnostics.CodeAnalysis;
using Content.Radium.Shared.Medical.Surgery.Prototypes;
using Content.Server.Body.Systems;

namespace Content.Radium.Server.Medical.Surgery.Systems;

public sealed partial class SurgerySystem
{
    [Dependency] private readonly BodySystem _bodySystem = null!;
    public bool TryGetOperationPrototype(string id, [NotNullWhen(true)] out SurgeryOperationPrototype? prototype)
    {
        return _prototypeManager.TryIndex(id, out prototype);
    }

}
