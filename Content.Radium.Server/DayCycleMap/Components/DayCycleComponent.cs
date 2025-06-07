// SPDX-FileCopyrightText: 2025 freeze2222 <opop1094@gmail.com>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

namespace Content.Radium.Server.DayCycleMap.Components
{
    [RegisterComponent]
    public sealed partial class DayCycleComponent : Component
    {
        [DataField]
        public Color MorningColor = Color.FromHex("#0232DD");

        [DataField]
        public Color DayColor = Color.FromHex("#666666");

        [DataField]
        public Color EveningColor = Color.FromHex("#FAD6A5");

        [DataField]
        public Color NightColor = Color.FromHex("#000000");

        [DataField]
        public float MorningDuration = 600;

        [DataField]
        public float DayDuration = 1200;

        [DataField]
        public float EveningDuration = 600;

        [DataField]
        public float NightDuration = 1200;
    }
}

