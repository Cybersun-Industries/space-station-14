using Robust.Shared.Audio;

namespace Content.Radium.Server.Medical.Surgery.Components;

[RegisterComponent]
public sealed partial class DrapesComponent : Component
{
    /// <summary>
    ///     Sound played on healing begin
    /// </summary>
    [DataField]
    public SoundSpecifier? BeginSound;

    /// <summary>
    ///     Sound played on healing end
    /// </summary>
    [DataField]
    public SoundSpecifier? EndSound;
}
