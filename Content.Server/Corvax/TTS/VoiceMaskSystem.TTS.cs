﻿using Content.Server.Corvax.TTS;
using Content.Shared.VoiceMask;

namespace Content.Server.VoiceMask;

public partial class VoiceMaskSystem
{
    private void InitializeTTS()
    {
        SubscribeLocalEvent<VoiceMaskComponent, TransformSpeakerVoiceEvent>(OnSpeakerVoiceTransform);
        SubscribeLocalEvent<VoiceMaskComponent, VoiceMaskChangeVoiceMessage>(OnChangeVoice);
    }

    private void OnSpeakerVoiceTransform(EntityUid uid, VoiceMaskComponent component, TransformSpeakerVoiceEvent args)
    {
        if (component.Enabled)
            args.VoiceId = component.VoiceId;
    }

    private void OnChangeVoice(EntityUid uid, VoiceMaskComponent component, VoiceMaskChangeVoiceMessage message)
    {
        component.VoiceId = message.Voice;

        _popupSystem.PopupEntity(Loc.GetString("voice-mask-voice-popup-success"), uid);

        TrySetLastKnownVoice(uid, message.Voice);

        UpdateUI(uid);
    }

    private void TrySetLastKnownVoice(EntityUid maskWearer, string? voiceId)
    {
        if (!HasComp<VoiceMaskComponent>(maskWearer)
            || !_inventory.TryGetSlotEntity(maskWearer, MaskSlot, out var maskEntity)
            || !TryComp<VoiceMaskerComponent>(maskEntity, out var maskComp))
        {
            return;
        }

        maskComp.LastSetVoice = voiceId;
    }
}
