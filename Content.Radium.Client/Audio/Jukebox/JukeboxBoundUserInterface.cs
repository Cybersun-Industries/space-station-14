// SPDX-FileCopyrightText: 2024 Kara <lunarautomaton6@gmail.com>
// SPDX-FileCopyrightText: 2024 Nemanja <98561806+EmoGarbage404@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 iNVERTED <alextjorgensen@gmail.com>
// SPDX-FileCopyrightText: 2024 metalgearsloth <31366439+metalgearsloth@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 Aiden <28298836+Aidenkrz@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 CybersunBot <cybersunbot@proton.me>
// SPDX-FileCopyrightText: 2025 XO6bl4 <49454110+XO6bl4@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 freeze2222 <opop1094@gmail.com>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using Content.Radium.Shared.Audio.Jukebox;
using Content.Shared.Audio.Jukebox;
using JetBrains.Annotations;
using Robust.Client.Audio;
using Robust.Client.UserInterface;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Components;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Prototypes;

namespace Content.Radium.Client.Audio.Jukebox;

[UsedImplicitly]
public sealed class RadiumJukeboxBoundUserInterface : BoundUserInterface
{
    [Dependency] private readonly IPrototypeManager _protoManager = null!;
    [Dependency] private readonly SharedAudioSystem _audio = null!;

    [ViewVariables]
    private RadiumJukeboxMenu? _menu;

    public RadiumJukeboxBoundUserInterface(EntityUid owner, Enum uiKey) : base(owner, uiKey)
    {
        IoCManager.InjectDependencies(this);
    }

    protected override void Open()
    {
        base.Open();

        _menu = this.CreateWindow<RadiumJukeboxMenu>();

        _menu.OnPlayPressed += args =>
        {
            if (args)
            {
                SendMessage(new RadiumJukeboxPlayingMessage());
            }
            else
            {
                SendMessage(new RadiumJukeboxPauseMessage());
            }
        };

        _menu.OnStopPressed += () =>
        {
            SendMessage(new RadiumJukeboxStopMessage());
        };

        _menu.OnSongSelected += SelectSong;

        _menu.SetTime += SetTime;
        _menu.SetVolume += SetVolume;

        PopulateMusic();
        Reload();
    }

    /// <summary>
    /// Reloads the attached menu if it exists.
    /// </summary>
    public void Reload()
    {
        if (_menu == null || !EntMan.TryGetComponent(Owner, out RadiumJukeboxComponent? jukebox))
            return;

        _menu.SetAudioStream(jukebox.AudioStream);

        if (!_protoManager.TryIndex(jukebox.SelectedSongId, out var songProto))
        {
            _menu.SetSelectedSong(string.Empty, 0f);
            return;
        }

        var resolvedSound = _audio.ResolveSound(new SoundPathSpecifier(songProto.Path.Path));
        var length = EntMan.System<AudioSystem>().GetAudioLength(resolvedSound);
        _menu.SetSelectedSong(songProto.Name, (float) length.TotalSeconds);
    }

    public void PopulateMusic()
    {
        _menu?.Populate(_protoManager.EnumeratePrototypes<JukeboxPrototype>());
    }

    public void SelectSong(ProtoId<JukeboxPrototype> songid)
    {
        SendMessage(new RadiumJukeboxSelectedMessage(songid));
    }

    public void SetVolume(float volume)
    {
        if (EntMan.TryGetComponent(Owner, out RadiumJukeboxComponent? jukebox) &&
            EntMan.TryGetComponent(jukebox.AudioStream, out AudioComponent? audioComp))
        {
            audioComp.Volume = volume;
        }
        SendMessage(new RadiumJukeboxSetVolumeMessage(volume));
    }

    public void SetTime(float time)
    {
        // You may be wondering, what the fuck is this
        // Well we want to be able to predict the playback slider change, of which there are many ways to do it
        // We can't just use SendPredictedMessage because it will reset every tick and audio updates every frame
        // so it will go BRRRRT
        // Using ping gets us close enough that it SHOULD, MOST OF THE TIME, fall within the 0.1 second tolerance
        // that's still on engine so our playback position never gets corrected.
        //
        // Maintainer's note:
        // I'm not even trying to fix midi predictions cause I want to keep my sanity.. For now.
        // That *probably* works.
        // LGTM! ^w^

        if (EntMan.TryGetComponent(Owner, out RadiumJukeboxComponent? jukebox) &&
            EntMan.TryGetComponent(jukebox.AudioStream, out AudioComponent? audioComp))
        {
            audioComp.PlaybackPosition = time;
        }

        SendMessage(new RadiumJukeboxSetTimeMessage(time));
    }
}
