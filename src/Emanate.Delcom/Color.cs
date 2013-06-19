namespace Emanate.Core.Output.DelcomVdi
{
    class Color
    {
        private Color(int setId, int dutyId, int offsetId, int powerId, int minPower = 10, int maxPower = 80)
        {
            MinPower = (byte)minPower;
            MaxPower = (byte)maxPower;
            SetId = (byte)setId;
            DutyId = (byte)dutyId;
            OffsetId = (byte)offsetId;
            PowerId = (byte)powerId;
        }

        public byte SetId { get; private set; }
        public byte DutyId { get; private set; }
        public byte OffsetId { get; private set; }
        public byte PowerId { get; private set; }
        // TODO: Move these power properties to config
        public byte MinPower { get; private set; }
        public byte MaxPower { get; private set; }

        public static readonly Color Green = new Color(1, 21, 26, 0);
        public static readonly Color Yellow = new Color(4, 23, 28, 2);
        public static readonly Color Red = new Color(2, 22, 27, 1);
    }
}