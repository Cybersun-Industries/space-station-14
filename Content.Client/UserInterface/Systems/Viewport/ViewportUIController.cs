using Content.Client.UserInterface.Controls;
using Content.Client.UserInterface.Systems.Gameplay;
using Content.Shared.CCVar;
using Robust.Client.Graphics;
using Robust.Client.Player;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controllers;
using Robust.Shared.Configuration;
using Robust.Shared.Map;
using Robust.Shared.Timing;

namespace Content.Client.UserInterface.Systems.Viewport;

public sealed class ViewportUIController : UIController
{
    [Dependency] private readonly IEyeManager _eyeManager = default!;
    [Dependency] private readonly IPlayerManager _playerMan = default!;
    [Dependency] private readonly IEntityManager _entMan = default!;
    [Dependency] private readonly IConfigurationManager _configurationManager = default!;
    [UISystemDependency] private readonly SharedTransformSystem? _transformSystem = default!;
    public static readonly Vector2i ViewportSize = (EyeManager.PixelsPerMeter * 21, EyeManager.PixelsPerMeter * 15);
    public const int ViewportHeight = 15;
    private MainViewport? Viewport => UIManager.ActiveScreen?.GetWidget<MainViewport>();

    public override void Initialize()
    {
        _configurationManager.OnValueChanged(CCVars.ViewportMinimumWidth, _ => UpdateViewportRatio());
        _configurationManager.OnValueChanged(CCVars.ViewportMaximumWidth, _ => UpdateViewportRatio());
        _configurationManager.OnValueChanged(CCVars.ViewportWidth, _ => UpdateViewportRatio());
        _configurationManager.OnValueChanged(CCVars.ViewportVerticalFit, _ => UpdateViewportRatio());

        //START RADIUM: GENOCIDE OF HEIGHT LINES

        _configurationManager.OnValueChanged(CCVars.ViewportMinimumWidth, _ => UpdateViewportRatio());
        _configurationManager.OnValueChanged(CCVars.ViewportMaximumWidth, _ => UpdateViewportRatio());
        _configurationManager.OnValueChanged(CCVars.ViewportHeight, _ => UpdateViewportRatio());

        //END RADIUM: GENOCIDE OF HEIGHT LINES

        var gameplayStateLoad = UIManager.GetUIController<GameplayStateLoadController>();
        gameplayStateLoad.OnScreenLoad += OnScreenLoad;
    }

    private void OnScreenLoad()
    {
        ReloadViewport();
    }

    private void UpdateViewportRatio()
    {
        if (Viewport == null)
        {
            return;
        }

        var minWidth = _configurationManager.GetCVar(CCVars.ViewportMinimumWidth);
        var maxWidth = _configurationManager.GetCVar(CCVars.ViewportMaximumWidth);
        var width = _configurationManager.GetCVar(CCVars.ViewportWidth);

        //START RADIUM: GENOCIDE OF HEIGHT LINES

        var minHeight = _configurationManager.GetCVar(CCVars.ViewportMinimumHeight);
        var maxHeight = _configurationManager.GetCVar(CCVars.ViewportMaximumHeight);
        var height = _configurationManager.GetCVar(CCVars.ViewportHeight);

        //END RADIUM: GENOCIDE OF HEIGHT LINES

        var verticalfit = _configurationManager.GetCVar(CCVars.ViewportVerticalFit) &&
                          _configurationManager.GetCVar(CCVars.ViewportStretch);

        if (verticalfit)
        {
            width = maxWidth;
            height = maxHeight; //RADIUM: GENOCIDE OF HEIGHT LINES
        }
        else
        {
            if (width < minWidth || width > maxWidth)
            {
                width = CCVars.ViewportWidth.DefaultValue;
            }

            if (height < minHeight || height > maxHeight)
            {
                height = CCVars.ViewportHeight.DefaultValue; //RADIUM: GENOCIDE OF HEIGHT LINES
            }
        }

        Viewport.Viewport.ViewportSize =
            (EyeManager.PixelsPerMeter * width, EyeManager.PixelsPerMeter * height); //RADIUM: GENOCIDE OF HEIGHT LINES
        Viewport.UpdateCfg();
    }

    public void ReloadViewport()
    {
        if (Viewport == null)
        {
            return;
        }

        UpdateViewportRatio();
        Viewport.Viewport.HorizontalExpand = true;
        Viewport.Viewport.VerticalExpand = true;
        _eyeManager.MainViewport = Viewport.Viewport;
    }

    public override void FrameUpdate(FrameEventArgs e)
    {
        if (Viewport == null)
        {
            return;
        }

        base.FrameUpdate(e);

        Viewport.Viewport.Eye = _eyeManager.CurrentEye;

        // verify that the current eye is not "null". Fuck IEyeManager.

        var ent = _playerMan.LocalEntity;
        if (_eyeManager.CurrentEye.Position != default || ent == null)
            return;

        _entMan.TryGetComponent(ent, out EyeComponent? eye);

        if (eye?.Eye == _eyeManager.CurrentEye
            && _entMan.GetComponent<TransformComponent>(ent.Value).MapID == MapId.Nullspace)
        {
            // nothing to worry about, the player is just in null space... actually that is probably a problem?
            return;
        }

        // Currently, this shouldn't happen. This likely happened because the main eye was set to null. When this
        // does happen it can create hard to troubleshoot bugs, so lets print some helpful warnings:
        Logger.Warning(
            $"Main viewport's eye is in nullspace (main eye is null?). Attached entity: {_entMan.ToPrettyString(ent.Value)}. Entity has eye comp: {eye != null}");
    }
}
