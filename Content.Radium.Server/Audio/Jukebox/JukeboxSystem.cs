// SPDX-FileCopyrightText: 2024 12rabbits <53499656+12rabbits@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 Alzore <140123969+Blackern5000@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 ArtisticRoomba <145879011+ArtisticRoomba@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 Brandon Hu <103440971+Brandon-Huu@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 Dimastra <65184747+Dimastra@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 Dimastra <dimastra@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 Ed <96445749+TheShuEd@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 Emisse <99158783+Emisse@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 Eoin Mcloughlin <helloworld@eoinrul.es>
// SPDX-FileCopyrightText: 2024 IProduceWidgets <107586145+IProduceWidgets@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 JIPDawg <51352440+JIPDawg@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 JIPDawg <JIPDawg93@gmail.com>
// SPDX-FileCopyrightText: 2024 Mervill <mervills.email@gmail.com>
// SPDX-FileCopyrightText: 2024 Moomoobeef <62638182+Moomoobeef@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 Nemanja <98561806+EmoGarbage404@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 PJBot <pieterjan.briers+bot@gmail.com>
// SPDX-FileCopyrightText: 2024 Pieter-Jan Briers <pieterjan.briers+git@gmail.com>
// SPDX-FileCopyrightText: 2024 Pieter-Jan Briers <pieterjan.briers@gmail.com>
// SPDX-FileCopyrightText: 2024 Piras314 <p1r4s@proton.me>
// SPDX-FileCopyrightText: 2024 PopGamer46 <yt1popgamer@gmail.com>
// SPDX-FileCopyrightText: 2024 PursuitInAshes <pursuitinashes@gmail.com>
// SPDX-FileCopyrightText: 2024 QueerNB <176353696+QueerNB@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 Saphire Lattice <lattice@saphi.re>
// SPDX-FileCopyrightText: 2024 ShadowCommander <10494922+ShadowCommander@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 Simon <63975668+Simyon264@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 Spessmann <156740760+Spessmann@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 Tadeo <td12233a@gmail.com>
// SPDX-FileCopyrightText: 2024 Thomas <87614336+Aeshus@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 Winkarst <74284083+Winkarst-cpu@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 deltanedas <39013340+deltanedas@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 deltanedas <@deltanedas:kde.org>
// SPDX-FileCopyrightText: 2024 eoineoineoin <github@eoinrul.es>
// SPDX-FileCopyrightText: 2024 github-actions[bot] <41898282+github-actions[bot]@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 iNVERTED <alextjorgensen@gmail.com>
// SPDX-FileCopyrightText: 2024 lzk <124214523+lzk228@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 metalgearsloth <31366439+metalgearsloth@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 slarticodefast <161409025+slarticodefast@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 stellar-novas <stellar_novas@riseup.net>
// SPDX-FileCopyrightText: 2024 themias <89101928+themias@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 Aiden <28298836+Aidenkrz@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 CybersunBot <cybersunbot@proton.me>
// SPDX-FileCopyrightText: 2025 Fildrance <fildrance@gmail.com>
// SPDX-FileCopyrightText: 2025 SX-7 <sn1.test.preria.2002@gmail.com>
// SPDX-FileCopyrightText: 2025 XO6bl4 <49454110+XO6bl4@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 freeze2222 <opop1094@gmail.com>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using Content.Radium.Shared.Audio.Jukebox;
using Content.Server.Power.Components;
using Content.Server.Power.EntitySystems;
using Content.Shared.Power;
using Robust.Server.GameObjects;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Components;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;

namespace Content.Radium.Server.Audio.Jukebox;
public sealed class JukeboxSystem : Content.Shared.Audio.Jukebox.SharedJukeboxSystem
{
    [Dependency] private readonly IPrototypeManager _protoManager = null!;
    [Dependency] private readonly AppearanceSystem _appearanceSystem = null!;

    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<RadiumJukeboxComponent, RadiumJukeboxSelectedMessage>(OnJukeboxSelected);
        SubscribeLocalEvent<RadiumJukeboxComponent, RadiumJukeboxPlayingMessage>(OnJukeboxPlay);
        SubscribeLocalEvent<RadiumJukeboxComponent, RadiumJukeboxPauseMessage>(OnJukeboxPause);
        SubscribeLocalEvent<RadiumJukeboxComponent, RadiumJukeboxStopMessage>(OnJukeboxStop);
        SubscribeLocalEvent<RadiumJukeboxComponent, RadiumJukeboxSetTimeMessage>(OnJukeboxSetTime);

        SubscribeLocalEvent<RadiumJukeboxComponent, RadiumJukeboxSetVolumeMessage>(OnJukeboxSetVolume);

        SubscribeLocalEvent<RadiumJukeboxComponent, ComponentInit>(OnComponentInit);
        SubscribeLocalEvent<RadiumJukeboxComponent, ComponentShutdown>(OnComponentShutdown);

        SubscribeLocalEvent<RadiumJukeboxComponent, PowerChangedEvent>(OnPowerChanged);
    }

    private void OnComponentInit(EntityUid uid, RadiumJukeboxComponent component, ComponentInit args)
    {
        if (HasComp<ApcPowerReceiverComponent>(uid))
        {
            TryUpdateVisualState(uid, component);
        }
    }

    private void OnJukeboxPlay(EntityUid uid, RadiumJukeboxComponent component, ref RadiumJukeboxPlayingMessage args)
    {
        if (Exists(component.AudioStream))
        {
            Audio.SetState(component.AudioStream, AudioState.Playing);
        }
        else
        {
            component.AudioStream = Audio.Stop(component.AudioStream);

            if (string.IsNullOrEmpty(component.SelectedSongId) ||
                !_protoManager.TryIndex(component.SelectedSongId, out var jukeboxProto))
            {
                return;
            }

            component.AudioStream = Audio.PlayPvs(jukeboxProto.Path, uid, AudioParams.Default.WithMaxDistance(10f).WithVolume(-6f))?.Entity; // Goobstation - the jukebox doesn't break your ears anymore
            Dirty(uid, component);
        }
    }

    private void OnJukeboxPause(Entity<RadiumJukeboxComponent> ent, ref RadiumJukeboxPauseMessage args)
    {
        Audio.SetState(ent.Comp.AudioStream, AudioState.Paused);
    }

    private void OnJukeboxSetTime(EntityUid uid, RadiumJukeboxComponent component, RadiumJukeboxSetTimeMessage args)
    {
        if (!TryComp(args.Actor, out ActorComponent? actorComp))
            return;

        var offset = actorComp.PlayerSession.Channel.Ping * 1.5f / 1000f;
        Audio.SetPlaybackPosition(component.AudioStream, args.SongTime + offset);
    }

    private void OnJukeboxSetVolume(EntityUid uid, RadiumJukeboxComponent comp, RadiumJukeboxSetVolumeMessage args)
    {
        if (!Audio.IsPlaying(comp.AudioStream))
            return;
        Audio.SetVolume(comp.AudioStream, args.Volume * 0.2f - 18f); // magic fucking numbers thanks to how audio works in this game. WHY THE FUCK WIZARDS???
    }

    private void OnPowerChanged(Entity<RadiumJukeboxComponent> entity, ref PowerChangedEvent args)
    {
        TryUpdateVisualState(entity);

        if (!this.IsPowered(entity.Owner, EntityManager))
        {
            Stop(entity);
        }
    }

    private void OnJukeboxStop(Entity<RadiumJukeboxComponent> entity, ref RadiumJukeboxStopMessage args)
    {
        Stop(entity);
    }

    private void Stop(Entity<RadiumJukeboxComponent> entity)
    {
        Audio.SetState(entity.Comp.AudioStream, AudioState.Stopped);
        Dirty(entity);
    }

    private void OnJukeboxSelected(EntityUid uid, RadiumJukeboxComponent component, RadiumJukeboxSelectedMessage args)
    {
        if (!Audio.IsPlaying(component.AudioStream))
        {
            component.SelectedSongId = args.SongId;
            DirectSetVisualState(uid, RadiumJukeboxVisualState.Select);
            component.Selecting = true;
            component.AudioStream = Audio.Stop(component.AudioStream);
        }

        Dirty(uid, component);
    }

    public override void Update(float frameTime)
    {
        base.Update(frameTime);

        var query = EntityQueryEnumerator<RadiumJukeboxComponent>();
        while (query.MoveNext(out var uid, out var comp))
        {
            if (!comp.Selecting)
                continue;

            comp.SelectAccumulator += frameTime;

            if (!(comp.SelectAccumulator >= 0.5f))
                continue;

            comp.SelectAccumulator = 0f;
            comp.Selecting = false;

            TryUpdateVisualState(uid, comp);
        }
    }

    private void OnComponentShutdown(EntityUid uid, RadiumJukeboxComponent component, ComponentShutdown args)
    {
        component.AudioStream = Audio.Stop(component.AudioStream);
    }

    private void DirectSetVisualState(EntityUid uid, RadiumJukeboxVisualState state)
    {
        _appearanceSystem.SetData(uid, RadiumJukeboxVisuals.VisualState, state);
    }

    private void TryUpdateVisualState(EntityUid uid, RadiumJukeboxComponent? jukeboxComponent = null)
    {
        if (!Resolve(uid, ref jukeboxComponent))
            return;

        var finalState = RadiumJukeboxVisualState.On;

        if (!this.IsPowered(uid, EntityManager))
        {
            finalState = RadiumJukeboxVisualState.Off;
        }

        _appearanceSystem.SetData(uid, RadiumJukeboxVisuals.VisualState, finalState);
    }
}
