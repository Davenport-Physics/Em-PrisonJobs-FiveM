using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;

namespace PrisonJobs
{
    class Electrician : BaseScript
    {

        private int jobs_completed       = 0;
        private int closest_electric_box = 0;
        private bool started_job          = false;

        private Vector3 client_pos;
        List<uint> electrical_box_hashes = new List<uint>(){ (uint)API.GetHashKey("prop_elecbox_10") , (uint)API.GetHashKey("prop_elecbox_10_cr") };

        public Electrician()
        {

        }

        public async Task HandleElectricianJob()
        {
            while (true)
            {
                await Delay(5);
                SetClosestElectricBox();
                HandlePlayerInput();
                // TODO, check if actually in prison.
            }
        }

        private void SetClosestElectricBox()
        {
            client_pos = Game.PlayerPed.Position;
            foreach (uint box in electrical_box_hashes)
            {
                this.closest_electric_box = API.GetClosestObjectOfType(client_pos[0], client_pos[1], client_pos[2], 1.5f, box, false, false, false);
                if (this.closest_electric_box != 0)
                {
                    break;
                }
            }
            
        }

        private void HandlePlayerInput()
        {
            if (this.closest_electric_box != 0 && !this.started_job)
            {
                Shared.DrawTextSimple("Press ~g~Enter~w~ to start maintenance.");
                if (API.IsControlJustPressed(1, 18))
                {
                    //this.started_job = true;
                    SetPlayerCoords();
                    AnimateElectricJob();
                }
            }
        }

        private void SetPlayerCoords()
        {
            float entity_heading = API.GetEntityHeading(this.closest_electric_box);
            Vector3 entity_pos   = API.GetEntityCoords(this.closest_electric_box, false);
            API.SetEntityHeading(Game.PlayerPed.Handle, entity_heading);
            Game.PlayerPed.Position = new Vector3(entity_pos[0] + 2f*(float)Math.Sin(entity_heading), entity_pos[1] - 1.5f*(float)Math.Cos(entity_heading), client_pos[2]);
        }

        private async void AnimateElectricJob()
        {
            await Shared.AnimatePlayer("anim@heists@fleeca_bank@drilling", "bag_drill_straight_idle", Shared.anim_flags_without_movement);
        }

        public void ForceStop()
        {

        }

    }
}
