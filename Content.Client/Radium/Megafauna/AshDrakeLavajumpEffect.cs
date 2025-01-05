using Content.Shared.Chasm;
using Content.Shared.Radium.Megafauna.Components;
using Robust.Client.Animations;
using Robust.Client.GameObjects;
using Robust.Shared.Animations;

namespace Content.Client.Radium.Megafauna;

/// <summary>
///     Handles the falling animation for entities that fall into a chasm.
/// </summary>
public sealed class AshDrakeLavajumpingVisualsSystem : EntitySystem
{
    [Dependency] private readonly AnimationPlayerSystem _anim = default!;

    private readonly string _ashDrakeLavajumpingAnimationKey = "drake_jump";

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<AshDrakeLavajumpingComponent, ComponentInit>(OnComponentInit);
        SubscribeLocalEvent<AshDrakeLavajumpingComponent, ComponentRemove>(OnComponentRemove);
    }

    private void OnComponentInit(EntityUid uid, AshDrakeLavajumpingComponent component, ComponentInit args)
    {
        if (!TryComp<SpriteComponent>(uid, out var sprite) ||
            TerminatingOrDeleted(uid))
        {
            return;
        }

        component.OriginalScale = sprite.Scale;

        var player = EnsureComp<AnimationPlayerComponent>(uid);
        if (_anim.HasRunningAnimation(player, _ashDrakeLavajumpingAnimationKey))
            return;

        _anim.Play((uid, player), GetFallingAnimation(component), _ashDrakeLavajumpingAnimationKey);
    }

    private void OnComponentRemove(EntityUid uid, AshDrakeLavajumpingComponent component, ComponentRemove args)
    {
        if (!TryComp<SpriteComponent>(uid, out var sprite))
            return;

        var player = EnsureComp<AnimationPlayerComponent>(uid);
        if (_anim.HasRunningAnimation(player, _ashDrakeLavajumpingAnimationKey))
            _anim.Stop(player, _ashDrakeLavajumpingAnimationKey);

        sprite.Scale = component.OriginalScale;
    }

    private Animation GetFallingAnimation(AshDrakeLavajumpingComponent component)
    {
        var length = component.AnimationTime;

        return new Animation()
        {
            Length = length,
            AnimationTracks =
            {
                new AnimationTrackComponentProperty()
                {
                    ComponentType = typeof(SpriteComponent),
                    Property = nameof(SpriteComponent.Scale),
                    KeyFrames =
                    {
                        new AnimationTrackProperty.KeyFrame(component.OriginalScale, 0.0f),
                        new AnimationTrackProperty.KeyFrame(component.AnimationScale, length.Seconds),
                    },
                    InterpolationMode = AnimationInterpolationMode.Cubic
                }
            }
        };
    }
}
