namespace Content.Server.Radium.DayCycleMap.Components
{
    [RegisterComponent]
    public sealed partial class DayLightComponent : Component
    {
        [ViewVariables(VVAccess.ReadWrite)]
        [DataField]
        public Color MorningColor = Color.FromHex("#0232DD");

        [ViewVariables(VVAccess.ReadWrite)]
        [DataField]
        public Color DayColor = Color.FromHex("#666666");

        [ViewVariables(VVAccess.ReadWrite)]
        [DataField]
        public Color EveningColor = Color.FromHex("#FAD6A5");

        [ViewVariables(VVAccess.ReadWrite)]
        [DataField]
        public Color NightColor = Color.FromHex("#000000");

        [ViewVariables(VVAccess.ReadWrite)]
        [DataField]
        public float MorningDuration = 600;

        [ViewVariables(VVAccess.ReadWrite)]
        [DataField]
        public float DayDuration = 1200;

        [ViewVariables(VVAccess.ReadWrite)]
        [DataField]
        public float EveningDuration = 600;

        [ViewVariables(VVAccess.ReadWrite)]
        [DataField]
        public float NightDuration = 1200;
    }
}

