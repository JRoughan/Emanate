using System.Threading;
using Emanate.Core.Output.DelcomVdi;
using NUnit.Framework;

namespace Emanate.IntegrationTests
{
    [TestFixture]
    public class delcom_vdi
    {
        [Test]
        public void should_open_single_attached_device()
        {
            using (var device = new Device())
                Assert.DoesNotThrow(device.Open);
        }

        [Test]
        public void should_turn_on_green_light()
        {
            using (var device = new Device())
            {
                device.Open();
                device.TurnOn(Color.Green);
                Thread.Sleep(1000);
            }
        }

        [Test]
        public void should_turn_on_yellow_light()
        {
            using (var device = new Device())
            {
                device.Open();
                device.TurnOn(Color.Yellow);
                Thread.Sleep(1000);
            }
        }

        [Test]
        public void should_turn_on_red_light()
        {
            using (var device = new Device())
            {
                device.Open();
                device.TurnOn(Color.Red);
                Thread.Sleep(1000);
            }
        }

        [Test]
        public void should_flash_green_light()
        {
            using (var device = new Device())
            {
                device.Open();
                device.Flash(Color.Green);
                Thread.Sleep(3000);
            }
        }

        [Test]
        public void should_flash_yellow_light()
        {
            using (var device = new Device())
            {
                device.Open();
                device.Flash(Color.Yellow);
                Thread.Sleep(3000);
            }
        }

        [Test]
        public void should_flash_red_light()
        {
            using (var device = new Device())
            {
                device.Open();
                device.Flash(Color.Red);
                Thread.Sleep(3000);
            }
        }

        [Test]
        public void should_start_buzzer()
        {
            using (var device = new Device())
            {
                device.Open();
                device.StartBuzzer(100, 2, 20, 20);
                Thread.Sleep(3000);
            }
        }

        [Test]
        public void should_ignore_requests_to_stop_if_buzzer_not_started()
        {
            using (var device = new Device())
            {
                device.Open();
                device.StopBuzzer();
            }
        }
    }
}
