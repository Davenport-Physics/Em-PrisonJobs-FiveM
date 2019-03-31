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

        private int jobs_completed        = 0;
        private int closest_electric_box  = 0;
        private bool started_job          = false;
        private int stop_job_at           = 0;
        private int particle_fx           = 0;

        private int drill_prop;

        private Vector3 client_pos;
        List<uint> electrical_box_hashes     = new List<uint>(){ (uint)API.GetHashKey("prop_elecbox_10") ,
                                                                 (uint)API.GetHashKey("prop_elecbox_10_cr"),
                                                                 (uint)API.GetHashKey("prop_elecbox_09")};
        List<EntityChecker> electrical_boxes = new List<EntityChecker>();
        EntityChecker current_box;

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
                IfJobHasStartedWait();
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
                    AddEletricBoxToListIfNeeded();
                    break;
                }
            }
            
        }

        private void AddEletricBoxToListIfNeeded()
        {
            foreach(EntityChecker box in electrical_boxes)
            {
                if (box.prop_idx == this.closest_electric_box)
                {
                    this.current_box = box;
                    return;
                }
            }
            electrical_boxes.Add(new EntityChecker(this.closest_electric_box));
            this.current_box = electrical_boxes[electrical_boxes.Count - 1];
        }

        private void HandlePlayerInput()
        {
            if (this.closest_electric_box != 0 && !this.started_job)
            {
                if (this.current_box.CanConductMaintenance())
                {
                    Shared.DrawTextSimple("Press ~g~Enter~w~ to start maintenance.");
                    if (API.IsControlJustPressed(1, 18))
                    {
                        StartJob();
                    }
                }
                else
                {
                    Shared.DrawTextSimple("Does not need maintenance right now.");
                }
            }
        }

        private void StartJob()
        {
            this.started_job = true;
            SetPlayerCoords();
            SpawnDrillProp();
            AnimateElectricJob();
            StartSoundEffect();
            StartParticleLoop();
            this.current_box.timer_until_next_maintenance = API.GetGameTimer() + 3 * 60000;
            this.stop_job_at = API.GetGameTimer() + 7000;
        }

        private void StartSoundEffect()
        {
            TriggerEvent("PlaySoundForEveryoneInVicinity", "sounds/PrisonJobs/drilling.mp3");
        }

        private void SetPlayerCoords()
        {
            float entity_heading = API.GetEntityHeading(this.closest_electric_box);
            Vector3 entity_pos   = API.GetEntityCoords(this.closest_electric_box, false);
            API.SetEntityHeading(Game.PlayerPed.Handle, entity_heading);
            Game.PlayerPed.Position = new Vector3(entity_pos[0] + 2f*(float)Math.Sin(entity_heading), entity_pos[1] - 1.5f*(float)Math.Cos(entity_heading), entity_pos[2]);
        }

        private void SpawnDrillProp()
        {
            this.drill_prop = Shared.CreateObjectGen("prop_tool_drill", 90.0f);
        }

        private async void AnimateElectricJob()
        {
            await Shared.AnimatePlayer("anim@heists@fleeca_bank@drilling", "drill_straight_start", Shared.anim_flags_without_movement);
        }

        private void StartParticleLoop()
        {
            this.particle_fx = API.StartParticleFxLoopedOnEntity("ent_dst_elec_fire", this.drill_prop, 0f, 0f, 0f, 0f, 0f, 0f, 1f, false, false, false);
        }

        private void StopParticleLoop()
        {
            API.StopParticleFxLooped(this.particle_fx, false);
        }

        private void IfJobHasStartedWait()
        {
            if (this.started_job && API.GetGameTimer() >= this.stop_job_at)
            {
                this.jobs_completed += 1;
                this.started_job = false;
                HandleTimeAndPayment();
                DespawnDrill();
                StopAnimating();
                StopParticleLoop();
            }
        }

        private void HandleTimeAndPayment()
        {
            if (this.jobs_completed == 6)
            {
                this.jobs_completed = 0;
                TriggerEvent("addMoney", 30);
                TriggerEvent("ReduceJailTime", 2);
                TriggerEvent("ShowInformationLeft", 5000, "Earned $30 and sentenced reduced by 2 months");
            } else
            {
                TriggerEvent("ShowInformationLeft", 5000, string.Format("Do maintenance on {0} more units", 6 - this.jobs_completed));
            }
        }

        private void DespawnDrill()
        {
            API.DeleteObject(ref this.drill_prop);
        }

        private void StopAnimating()
        {
            API.RemoveAnimDict("anim@heists@fleeca_bank@drilling");
            Game.PlayerPed.Task.ClearAll();
        }

        public void ForceStop()
        {
            this.started_job = false;
            StopAnimating();
            DespawnDrill();
            electrical_boxes = new List<EntityChecker>();
        }

    }

    class EntityChecker
    {
        private Vector3 pos;
        public int timer_until_next_maintenance { get; set; }
        public int prop_idx;
        public EntityChecker(int prop_idx)
        {
            this.prop_idx = prop_idx;
            this.pos      = API.GetEntityCoords(this.prop_idx, false);
            this.timer_until_next_maintenance = 0;
        }

        public bool CanConductMaintenance()
        {
            if (API.GetGameTimer() > this.timer_until_next_maintenance)
            {
                return true;
            }
            return false;
        }

    }
}
