﻿using System.Diagnostics.CodeAnalysis;
using Content.Radium.Common.Medical.Surgery;
using Content.Server.Body.Systems;

namespace Content.Radium.Server.Medical.Surgery.Systems;

public sealed partial class SurgerySystem
{
    public bool TryGetOperationPrototype(string id, [NotNullWhen(true)] out SurgeryOperationPrototype? prototype)
    {
        return _prototypeManager.TryIndex(id, out prototype);
    }

}
