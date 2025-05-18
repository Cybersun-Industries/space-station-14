using System.Collections.Frozen;
using System.Collections.ObjectModel;
using Content.Client._Shitcode.UserInterface.Systems.Surgery.Widgets.Systems;
using Content.Radium.Common.Medical.Surgery;
using Content.Shared.Body.Part;
using Content.Shared.MedicalScanner;
using JetBrains.Annotations;
using Robust.Client.UserInterface;

namespace Content.Client.HealthAnalyzer.UI
{
    [UsedImplicitly]
    public sealed class HealthAnalyzerBoundUserInterface(EntityUid owner, Enum uiKey) : BoundUserInterface(owner, uiKey)
    {
        [Dependency] private readonly ClientDamagePartsSystem _damageParts = null!;
        [Dependency] private readonly EntityManager _entityManager = default!;

        [ViewVariables]
        private HealthAnalyzerWindow? _window;

        protected override void Open()
        {
            base.Open();

            _window = this.CreateWindow<HealthAnalyzerWindow>();

            _window.Title = EntMan.GetComponent<MetaDataComponent>(Owner).EntityName;
        }

        protected override void ReceiveMessage(BoundUserInterfaceMessage message)
        {
            if (_window == null)
                return;

            if (message is not HealthAnalyzerScannedUserMessage cast)
                return;

            var targetEntity = _entityManager.GetEntity(cast.TargetEntity);

            if (targetEntity != null)
            {
                var damagedParts = _damageParts.GetDamagedParts<BodyPartType, BodyPartSymmetry>(targetEntity.Value);
                cast.DamagedBodyParts = damagedParts.ToFrozenDictionary();
            }

            _window.Populate(cast);
        }
    }
}
