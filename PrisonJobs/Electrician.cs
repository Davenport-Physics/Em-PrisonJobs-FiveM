﻿using System;
using System.Collections.Generic;
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
        readonly List<uint> electrical_box_hashes     = new List<uint>(){ (uint)API.GetHashKey("prop_elecbox_10") ,
                                                                 (uint)API.GetHashKey("prop_elecbox_10_cr"),
                                                                 (uint)API.GetHashKey("prop_elecbox_09")};
        private List<EntityChecker> electrical_boxes = new List<EntityChecker>();
        private int current_box_idx;

        public Electrician()
        {
            // TODO add electrical fires to props that need maintenance.
            // "core" -> "ent_dst_elec_fire"
        }

        public void HandleElectricianJob()
        {
            SetClosestElectricBox();
            HandlePlayerInput();
            IfJobHasStartedWait();
        }

        private void SetClosestElectricBox()
        {
            this.client_pos           = Game.PlayerPed.Position;
            this.closest_electric_box = Shared.GetClosestObjectID(this.electrical_box_hashes, this.client_pos);

            if (this.closest_electric_box != 0)
            {
                AddEletricBoxToListIfNeeded();
            }
        }

        private void AddEletricBoxToListIfNeeded()
        {
            for (int i = 0; i < this.electrical_boxes.Count; i++)
            {
                if (this.electrical_boxes[i].prop_idx == this.closest_electric_box)
                {
                    this.current_box_idx = i;
                    return;
                }
            }
            this.electrical_boxes.Add(new EntityChecker(this.closest_electric_box));
            this.current_box_idx = this.electrical_boxes.Count - 1;
        }

        private void HandlePlayerInput()
        {
            if (this.closest_electric_box != 0 && !this.started_job)
            {
                if (this.electrical_boxes[this.current_box_idx].CanConductMaintenance())
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
            this.electrical_boxes[this.current_box_idx] = new EntityChecker(this.closest_electric_box, API.GetGameTimer() + 3 * 60000);
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
            Game.PlayerPed.Position = new Vector3(entity_pos[0] + 2f*(float)Math.Sin(entity_heading), entity_pos[1] - 1.0f*(float)Math.Cos(entity_heading), entity_pos[2]);
        }

        private void SpawnDrillProp()
        {
            this.drill_prop = Shared.CreateObjectGen("prop_tool_drill", 90.0f);
        }

        private async void AnimateElectricJob()
        {
            Debug.WriteLine("Running Animation");
            await Shared.AnimatePlayer("anim@heists@fleeca_bank@drilling", "drill_straight_start", Shared.anim_flags_without_movement);
        }

        private async void StartParticleLoop()
        {
            await Shared.LoadParticleFx("core");
            API.SetPtfxAssetNextCall("core");
            this.particle_fx = API.StartParticleFxLoopedOnEntity("ent_col_electrical", this.drill_prop, 0f, 0f, 0f, 0f, 0f, 0f, 1f, false, false, false);
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
}
