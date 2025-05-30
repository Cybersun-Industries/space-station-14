﻿using Robust.Shared.Serialization;

namespace Content.Radium.Shared.Changeling.Events;

[Serializable, NetSerializable]
public sealed class ConfirmTransformation : BoundUserInterfaceMessage
{
    public NetEntity Uid;
    public int ServerIdentityIndex;
}

[Serializable, NetSerializable]
public sealed class ConfirmTransformSting : BoundUserInterfaceMessage
{
    public NetEntity Uid;
    public int ServerIdentityIndex;
}
