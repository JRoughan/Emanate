namespace Emanate.Delcom
{
    public class Color
    {
        private Color(string name, int setId, int dutyId, int offsetId, int powerId, int minPower = 10, int maxPower = 80)
        {
            Name = name;
            MinPower = (byte)minPower;
            MaxPower = (byte)maxPower;
            SetId = (byte)setId;
            DutyId = (byte)dutyId;
            OffsetId = (byte)offsetId;
            PowerId = (byte)powerId;
        }

        public string Name { get; set; }
        public byte SetId { get; private set; }
        public byte DutyId { get; private set; }
        public byte OffsetId { get; private set; }
        public byte PowerId { get; private set; }
        // TODO: Move these power properties to config
        public byte MinPower { get; private set; }
        public byte MaxPower { get; private set; }

        public static readonly Color Green = new Color("Green", 1, 21, 26, 0);
        public static readonly Color Yellow = new Color("Yellow", 4, 23, 28, 2);
        public static readonly Color Red = new Color("Red", 2, 22, 27, 1);
    }
}