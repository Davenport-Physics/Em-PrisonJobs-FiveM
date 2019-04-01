using System;
using System.Drawing;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;

namespace PrisonJobs
{
    public class PrisonJobs : BaseScript
    {
        private FoodDelivery delivery   = new FoodDelivery();
        private Electrician electrician = new Electrician();

        private readonly Vector3 point_in_prison = new Vector3(1694.8f, 2563.99f, 45.57f);

        public PrisonJobs()
        {
            EventHandlers["JailTimeOver"] += new Action(ForceJobStopping);
            StartPrisonJobs();
        }

        private async void StartPrisonJobs()
        {
            while (true)
            {
                if (Vector3.Distance(Game.PlayerPed.Position, point_in_prison) <= 100)
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
