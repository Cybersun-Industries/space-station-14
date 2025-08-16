// SPDX-FileCopyrightText: 2024 iNVERTED <alextjorgensen@gmail.com>
// SPDX-FileCopyrightText: 2024 metalgearsloth <31366439+metalgearsloth@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 Aiden <28298836+Aidenkrz@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 CybersunBot <cybersunbot@proton.me>
// SPDX-FileCopyrightText: 2025 SX-7 <sn1.test.preria.2002@gmail.com>
// SPDX-FileCopyrightText: 2025 Tayrtahn <tayrtahn@gmail.com>
// SPDX-FileCopyrightText: 2025 XO6bl4 <49454110+XO6bl4@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 freeze2222 <opop1094@gmail.com>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using Content.Radium.Shared.Audio.Jukebox;
using Content.Shared.Audio.Jukebox;
using Robust.Client.Animations;
using Robust.Client.GameObjects;
using Robust.Shared.Prototypes;

namespace Content.Radium.Client.Audio.Jukebox;


public sealed class RadiumJukeboxSystem : RadiumSharedJukeboxSystem
{
    [Dependency] private readonly IPrototypeManager _protoManager = default!;
    [Dependency] private readonly AnimationPlayerSystem _animationPlayer = default!;
    [Dependency] private readonly SharedAppearanceSystem _appearanceSystem = default!;
    [Dependency] private readonly SharedUserInterfaceSystem _uiSystem = default!;
    [Dependency] private readonly SpriteSystem _sprite = default!;

    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<RadiumJukeboxComponent, AppearanceChangeEvent>(OnAppearanceChange);
        SubscribeLocalEvent<RadiumJukeboxComponent, AnimationCompletedEvent>(OnAnimationCompleted);
        SubscribeLocalEvent<RadiumJukeboxComponent, AfterAutoHandleStateEvent>(OnJukeboxAfterState);

        _protoManager.PrototypesReloaded += OnProtoReload;
    }

    public override void Shutdown()
    {
        base.Shutdown();
        _protoManager.PrototypesReloaded -= OnProtoReload;
    }

    private void OnProtoReload(PrototypesReloadedEventArgs obj)
    {
        if (!obj.WasModified<JukeboxPrototype>())
            return;

        var query = AllEntityQuery<RadiumJukeboxComponent, UserInterfaceComponent>();

        while (query.MoveNext(out var uid, out _, out var ui))
        {
            if (!_uiSystem.TryGetOpenUi<RadiumJukeboxBoundUserInterface>((uid, ui), RadiumJukeboxUiKey.Key, out var bui))
                continue;

            bui.PopulateMusic();
        }
    }

    private void OnJukeboxAfterState(Entity<RadiumJukeboxComponent> ent, ref AfterAutoHandleStateEvent args)
    {
        if (!_uiSystem.TryGetOpenUi<RadiumJukeboxBoundUserInterface>(ent.Owner, RadiumJukeboxUiKey.Key, out var bui))
            return;

        bui.Reload();
    }

    private void OnAnimationCompleted(EntityUid uid, RadiumJukeboxComponent component, AnimationCompletedEvent args)
    {
        if (!TryComp<SpriteComponent>(uid, out var sprite))
            return;

        if (!TryComp<AppearanceComponent>(uid, out var appearance) ||
            !_appearanceSystem.TryGetData<RadiumJukeboxVisualState>(uid, RadiumJukeboxVisuals.VisualState, out var visualState, appearance))
        {
            visualState = RadiumJukeboxVisualState.On;
        }

        UpdateAppearance((uid, sprite), visualState, component);
    }

    private void OnAppearanceChange(EntityUid uid, RadiumJukeboxComponent component, ref AppearanceChangeEvent args)
    {
        if (args.Sprite == null)
            return;

        if (!args.AppearanceData.TryGetValue(RadiumJukeboxVisuals.VisualState, out var visualStateObject) ||
            visualStateObject is not RadiumJukeboxVisualState visualState)
        {
            visualState = RadiumJukeboxVisualState.On;
        }

        UpdateAppearance((uid, args.Sprite), visualState, component);
    }

    private void UpdateAppearance(Entity<SpriteComponent> entity, RadiumJukeboxVisualState visualState, RadiumJukeboxComponent component)
    {
        SetLayerState(RadiumJukeboxVisualLayers.Base, component.OffState, entity);

        switch (visualState)
        {
            case RadiumJukeboxVisualState.On:
                SetLayerState(RadiumJukeboxVisualLayers.Base, component.OnState, entity);
                break;

            case RadiumJukeboxVisualState.Off:
                SetLayerState(RadiumJukeboxVisualLayers.Base, component.OffState, entity);
                break;

            case RadiumJukeboxVisualState.Select:
                PlayAnimation(entity.Owner, RadiumJukeboxVisualLayers.Base, component.SelectState, 1.0f, entity);
                break;
        }
    }

    private void PlayAnimation(EntityUid uid, RadiumJukeboxVisualLayers layer, string? state, float animationTime, SpriteComponent sprite)
    {
        if (string.IsNullOrEmpty(state))
            return;

        if (_animationPlayer.HasRunningAnimation(uid, state))
            return;

        var animation = GetAnimation(layer, state, animationTime);
        _sprite.LayerSetVisible((uid, sprite), layer, true);
        _animationPlayer.Play(uid, animation, state);
    }

    private static Animation GetAnimation(RadiumJukeboxVisualLayers layer, string state, float animationTime)
    {
        return new Animation
        {
            Length = TimeSpan.FromSeconds(animationTime),
            AnimationTracks =
                {
                    new AnimationTrackSpriteFlick
                    {
                        LayerKey = layer,
                        KeyFrames =
                        {
                            new AnimationTrackSpriteFlick.KeyFrame(state, 0f),
                        },
                    },
                },
        };
    }

    private void SetLayerState(RadiumJukeboxVisualLayers layer, string? state, Entity<SpriteComponent> sprite)
    {
        if (string.IsNullOrEmpty(state))
            return;

        _sprite.LayerSetVisible(sprite.AsNullable(), layer, true);
        _sprite.LayerSetAutoAnimated(sprite.AsNullable(), layer, true);
        _sprite.LayerSetRsiState(sprite.AsNullable(), layer, state);
    }
}
