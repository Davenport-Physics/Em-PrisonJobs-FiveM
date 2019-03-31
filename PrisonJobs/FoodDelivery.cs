using System.Drawing;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;

namespace PrisonJobs
{
    class FoodDelivery : BaseScript
    {
        private bool has_box = false;
        private float distance_to_boxes;
        private float distance_to_food_place;

        private int box_entity;
        private int boxes_delivered = 0;

        private Vector3 box_location     = new Vector3(1689.332f, 2551.65f, 44.5649f);
        private Vector3 food_location    = new Vector3(1661.514f, 2566.631f, 45.56488f);
        private Vector3 box_marker_dir   = new Vector3(0, 0, 0);
        private Vector3 box_marker_rot   = new Vector3(0, 0, 50);
        private Vector3 box_marker_scale = new Vector3(1.01f, 1.01f, 1.01f);
        private Color box_marker_color   = Color.FromArgb(150, 255, 255, 0);

        public FoodDelivery()
        {
            
        }

        public void HandleDeliveryJob()
        {
            SetDistance();
            DrawMarkerIfNecessary();
            HandleInputIfNecessary();
            StopRunningIfHoldingBox();
            HoldingBoxHandler();
            HandleBeingNearFoodPoint();
        }



        private void SetDistance()
        {
            this.distance_to_boxes      = GetDistanceToPosition(box_location);
            this.distance_to_food_place = GetDistanceToPosition(food_location);
        }

        private float GetDistanceToPosition(Vector3 vec)
        {
            Vector3 ped_vec = Game.PlayerPed.Position;
            return API.GetDistanceBetweenCoords(ped_vec[0], ped_vec[1], ped_vec[2], vec[0], vec[1], vec[2], true);
        }

        private void DrawMarkerIfNecessary()
        {
            if (this.distance_to_boxes <= 15)
            {

                World.DrawMarker(MarkerType.VerticalCylinder, box_location, box_marker_dir, box_marker_rot, box_marker_scale, box_marker_color);

            }
            else if (this.distance_to_food_place <= 15)
            {

                World.DrawMarker(MarkerType.ThickChevronUp, food_location, box_marker_dir, box_marker_rot, box_marker_scale, box_marker_color);

            }
        }

        private void HandleBeingNearFoodPoint()
        {
            if (this.distance_to_food_place <= 2)
            {
                if (this.has_box)
                {
                    DealWithBoxesDelivered();
                    API.RemoveAnimDict("anim@heists@box_carry@");
                    Game.PlayerPed.Task.ClearAll();
                }
                else
                {
                    Shared.DrawTextSimple("Food deliver point");
                }

            }

        }

        private void DealWithBoxesDelivered()
        {
            DespawnBox();

            this.has_box = false;
            this.boxes_delivered += 1;
            int diff = 3 - this.boxes_delivered;

            if (diff == 0)
            {
                this.boxes_delivered = 0;
                TriggerEvent("ReduceJailTime", 2);
                TriggerEvent("addMoney", 30);
                TriggerEvent("ShowInformationLeft", 5000, "You received $30 and your sentence was reduced by 2 months");
            }
            else
            {
                TriggerEvent("ShowInformationLeft", 5000, string.Format("Deliver {0} more boxes for money and time off.", diff));
            }

        }

        private  void HandleInputIfNecessary()
        {
            if (this.distance_to_boxes <= 3 && !this.has_box)
            {
                Shared.DrawTextSimple("Press ~g~Enter~w~ to pick up box of food.");
                if (API.IsControlJustPressed(1, 18))
                {
                    this.has_box = true;
                    SpawnBox();
                    BeginAnimation();
                }
            }
        }

        private async void BeginAnimation()
        {
            await Shared.AnimatePlayer("anim@heists@box_carry@", "idle", Shared.anim_flags_with_movement);
        }

        private void SpawnBox()
        {
            this.box_entity = Shared.CreateObjectGen("ng_proc_box_01a");
        }

        private void DespawnBox()
        {
            API.DeleteEntity(ref this.box_entity);
            this.box_entity = 0;
        }

        private void StopRunningIfHoldingBox()
        {
            if (this.has_box)
            {
                API.DisableControlAction(1, 21, true);
                API.DisableControlAction(1, 22, true);
            }
        }

        private void HoldingBoxHandler()
        {
            if (this.has_box && !API.IsEntityPlayingAnim(Game.PlayerPed.Handle, "anim@heists@box_carry@", "idle", 3))
            {
                Game.PlayerPed.Task.PlayAnimation("anim@heists@box_carry@", "idle", -1, -1, Shared.anim_flags_with_movement);
            }
        }

        public void ForceStop()
        {
            DespawnBox();
            this.has_box = false;
            API.RemoveAnimDict("anim@heists@box_carry@");
            Game.PlayerPed.Task.ClearAll();
        }

    }
}
