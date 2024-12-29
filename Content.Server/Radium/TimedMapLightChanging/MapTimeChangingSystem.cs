using Content.Server.Radium.TimedMapLightChanging.Components;
using Robust.Shared.Map.Components;
using Robust.Shared.Timing;

namespace Content.Server.Radium.TimedMapLightChanging;

/// <summary>
/// This handles timeticking
/// </summary>
public sealed class TimedMapLightChangingSystem : EntitySystem
{
    [Dependency] private readonly IGameTiming _gameTiming = default!;

    public override void Update(float frameTime)
    {
        base.Update(frameTime);

        var query = EntityQueryEnumerator<TimedMapLightChangingComponent, MapLightComponent>();
        while (query.MoveNext(out var uid, out var timedMapLight, out var mapLight))
        {

            var morningDuration = timedMapLight.MorningDuration;
            var dayDuration = timedMapLight.DayDuration;
            var eveningDuration = timedMapLight.EveningDuration;
            var nightDuration = timedMapLight.NightDuration;
            var daycycleDuration = morningDuration + dayDuration + eveningDuration + nightDuration;
            var transitionDuration = daycycleDuration / 2f;

            var t = (float)_gameTiming.CurTime.TotalSeconds % daycycleDuration / daycycleDuration;

            if (t <= 0.25f) // Утро
            {
                var morningColor = timedMapLight.DayColor;
                if (t >= 0.25f - (transitionDuration / morningDuration))
                {
                    var transitionT = (0.25f - t) / (transitionDuration / morningDuration);
                    morningColor = Color.InterpolateBetween(timedMapLight.NightColor,
                        timedMapLight.MorningColor,
                        transitionT);
                }

                mapLight.AmbientLightColor = morningColor;
            }
            else if (t <= 0.5f) // День
            {
                var dayColor = timedMapLight.DayColor;
                if (t >= 0.5f - (transitionDuration / dayDuration))
                {
                    var transitionT = (0.5f - t) / (transitionDuration / dayDuration);
                    dayColor = Color.InterpolateBetween(timedMapLight.NightColor,
                        timedMapLight.DayColor,
                        transitionT);
                }

                mapLight.AmbientLightColor = dayColor;
            }
            else if (t <= 0.75f) // Вечер
            {
                var eveningColor = timedMapLight.EveningColor;
                if (t <= 0.5f + (transitionDuration / eveningDuration))
                {
                    var transitionT = (t - 0.5f) / (transitionDuration / eveningDuration);
                    eveningColor = Color.InterpolateBetween(timedMapLight.DayColor,
                        timedMapLight.EveningColor,
                        transitionT);
                }

                mapLight.AmbientLightColor = eveningColor;
            }
            else // Ночь
            {
                var nightColor = timedMapLight.NightColor;
                if (t <= 1f - (transitionDuration / nightDuration))
                {
                    var transitionT = (t - 0.75f) / (transitionDuration / nightDuration);
                    nightColor = Color.InterpolateBetween(timedMapLight.EveningColor,
                        timedMapLight.NightColor,
                        transitionT);
                }

                mapLight.AmbientLightColor = nightColor;
            }

            Dirty(uid, mapLight);
        }
    }
}
