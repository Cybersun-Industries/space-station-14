using Content.Server.Radium.DayCycleMap.Components;
using Robust.Shared.Map.Components;
using Robust.Shared.Timing;

namespace Content.Radium.Server.DayCycleMap;

public sealed class TimedMapLightChangingSystem : EntitySystem
{
    [Dependency] private readonly IGameTiming _gameTiming = null!;

    public override void Update(float frameTime)
    {
        base.Update(frameTime);

        var query = EntityQueryEnumerator<Components.DayCycleComponent, MapLightComponent>();
        while (query.MoveNext(out var uid, out var dayCycleComponent, out var mapLight))
        {
            var morningDuration = Math.Max(1, dayCycleComponent.MorningDuration);
            var dayDuration = Math.Max(1, dayCycleComponent.DayDuration);
            var eveningDuration = Math.Max(1, dayCycleComponent.EveningDuration);
            var nightDuration = Math.Max(1, dayCycleComponent.NightDuration);
            var cycleDuration = morningDuration + dayDuration + eveningDuration + nightDuration;
            var transitionDuration = cycleDuration / 2f;

            var t = (float)_gameTiming.CurTime.TotalSeconds % cycleDuration / cycleDuration;

            switch (t)
            {
                // Morning
                case <= 0.25f:
                {
                    var morningColor = dayCycleComponent.MorningColor;
                    if (t >= 0.25f - transitionDuration / morningDuration)
                    {
                        var transitionT = (0.25f - t) / (transitionDuration / morningDuration);
                        morningColor = Color.InterpolateBetween(dayCycleComponent.NightColor,
                            dayCycleComponent.MorningColor,
                            transitionT);
                    }

                    mapLight.AmbientLightColor = morningColor;
                    break;
                }
                // Day
                case <= 0.5f:
                {
                    var dayColor = dayCycleComponent.DayColor;
                    if (t >= 0.5f - transitionDuration / dayDuration)
                    {
                        var transitionT = (0.5f - t) / (transitionDuration / dayDuration);
                        dayColor = Color.InterpolateBetween(dayCycleComponent.NightColor,
                            dayCycleComponent.DayColor,
                            transitionT);
                    }

                    mapLight.AmbientLightColor = dayColor;
                    break;
                }
                // Evening
                case <= 0.75f:
                {
                    var eveningColor = dayCycleComponent.EveningColor;
                    if (t <= 0.5f + transitionDuration / eveningDuration)
                    {
                        var transitionT = (t - 0.5f) / (transitionDuration / eveningDuration);
                        eveningColor = Color.InterpolateBetween(dayCycleComponent.DayColor,
                            dayCycleComponent.EveningColor,
                            transitionT);
                    }

                    mapLight.AmbientLightColor = eveningColor;
                    break;
                }
                // Night
                default:
                {
                    var nightColor = dayCycleComponent.NightColor;
                    if (t <= 1f - transitionDuration / nightDuration)
                    {
                        var transitionT = (t - 0.75f) / (transitionDuration / nightDuration);
                        nightColor = Color.InterpolateBetween(dayCycleComponent.EveningColor,
                            dayCycleComponent.NightColor,
                            transitionT);
                    }

                    mapLight.AmbientLightColor = nightColor;
                    break;
                }
            }

            Dirty(uid, mapLight);
        }
    }
}
