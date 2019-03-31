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
            Tick += delivery.HandleDeliveryJob;
            Tick += electrician.HandleElectricianJob;
            EventHandlers["JailTimeOver"]    += new Action(ForceJobStopping);
            EventHandlers["StartPrisonJobs"] += new Action(StartPrisonJobs);
        }

        private void StartPrisonJobs()
        {

        }

        private void ForceJobStopping()
        {
            delivery.ForceStop();
            electrician.ForceStop();
        }

    }
}
