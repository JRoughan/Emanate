using Emanate.Core.Input;

namespace Emanate.Core.Output.DelcomVdi
{
    public class DelcomOutput : IOutput
    {
        private readonly Device device;
        private BuildState lastState;

        public DelcomOutput()
        {
            device = new Device();
            device.Open();
        }

        public void UpdateStatus(BuildState state)
        {
            if (state == lastState)
                return;

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
                    device.StartBuzzer(100, 2, 20, 20);
                    break;
                case BuildState.Running:
                    device.TurnOff(Color.Red);
                    device.TurnOff(Color.Green);
                    device.Flash(Color.Yellow);
                    break;
            }
            lastState = state;
        }
    }
}