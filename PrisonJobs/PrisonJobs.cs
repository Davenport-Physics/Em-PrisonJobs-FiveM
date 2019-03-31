using System;
using System.Drawing;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;

namespace PrisonJobs
{
    public class PrisonJobs : BaseScript
    {
        FoodDelivery delivery   = new FoodDelivery();
        Electrician electrician = new Electrician();

        public PrisonJobs()
        {
            Tick += StartPrisonJobs;
            EventHandlers["JailTimeOver"] += new Action(ForceJobStopping);
        }

        private async Task StartPrisonJobs()
        {
            Vector3 client_pos;
            while (true)
            {
                client_pos = Game.PlayerPed.Position;
                if (API.GetDistanceBetweenCoords(client_pos[0], client_pos[1], client_pos[2], 1694.8f, 2563.99f, 45.57f, true) <= 100)
                {
                    await Delay(5);
                    delivery.HandleDeliveryJob();
                    electrician.HandleElectricianJob();
                } else
                {
                    await Delay(10000);
                }
            }

        }

        private void ForceJobStopping()
        {
            delivery.ForceStop();
            electrician.ForceStop();
        }

    }
}
