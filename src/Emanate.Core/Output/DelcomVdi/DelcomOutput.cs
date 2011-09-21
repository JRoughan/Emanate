using Emanate.Core.Input;
using Emanate.Core.Output.DelcomVdi;

namespace Emanate.Core.Output
{
    public class DelcomOutput : IOutput
    {
        private readonly Device device;

        public DelcomOutput()
        {
            device = new Device();
            device.Open();
        }

        public void UpdateStatus(BuildState state)
        {
            switch (state)
            {
                case BuildState.Succeeded:
                    device.TurnOff(Color.Red);
                    device.TurnOn(Color.Green);
                    device.TurnOff(Color.Yellow);
                    break;
                case BuildState.Failed:
                    device.TurnOn(Color.Red);
                    device.TurnOff(Color.Green);
                    device.TurnOff(Color.Yellow);
                    break;
                case BuildState.Running:
                    device.TurnOff(Color.Red);
                    device.TurnOff(Color.Green);
                    device.TurnOn(Color.Yellow);
                    break;
            }
        }
    }
}