using System.Drawing;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;

namespace PrisonJobs
{
    class Shared : BaseScript
    {

        public static AnimationFlags anim_flags_with_movement    = AnimationFlags.AllowRotation | AnimationFlags.UpperBodyOnly | AnimationFlags.Loop;
        public static AnimationFlags anim_flags_without_movement = AnimationFlags.StayInEndFrame | AnimationFlags.Loop | AnimationFlags.UpperBodyOnly;

        public static void DrawTextSimple(string text)
        {
            Shared.DrawTextHandler(text, 1, true, 0.5f, 0.85f, 0.8f, 255, 255, 255, 255);
        }

        public static void DrawTextHandler(string text, int font, bool center, float x, float y, float scale, int r, int g, int b, int a)
        {
            API.SetTextFont(font);
            API.SetTextProportional(false);
            API.SetTextScale(scale, scale);
            API.SetTextColour(r, g, b, a);
            API.SetTextDropShadow();
            API.SetTextEdge(1, 0, 0, 0, 255);
            API.SetTextDropShadow();
            API.SetTextOutline();
            API.SetTextCentre(true);
            API.SetTextEntry("STRING");
            API.AddTextComponentString(text);
            API.DrawText(x, y);
        }

        public static async Task RequestAnimationDictionary(string dict)
        {
            if (!API.DoesAnimDictExist(dict))
            {
                Debug.WriteLine(string.Format("Animation {0} does not exist\n", dict));
                return;
            }

            if (API.HasAnimDictLoaded(dict))
            {
                return;
            }

            API.RequestAnimDict(dict);
            while (API.HasAnimDictLoaded(dict))
            {
                await Delay(1);
            }
        }

        public static void PlayAnimation(string dict, string animation, AnimationFlags anim_flags)
        {
            Game.PlayerPed.Task.ClearAll();
            Game.PlayerPed.Task.PlayAnimation(dict, animation, -1, -1, anim_flags);
        }

        public static async Task AnimatePlayer(string dict, string animation, AnimationFlags anim_flags)
        {
            await RequestAnimationDictionary(dict);
            PlayAnimation(dict, animation, anim_flags);
        }

        public static int CreateObjectGen(string prop, float zRot = 0.0f)
        {
            int bone   = API.GetPedBoneIndex(Game.PlayerPed.Handle, 28422);
            int entity = API.CreateObject(API.GetHashKey(prop), 0f, 0f, 0f, true, true, true);
            API.AttachEntityToEntity(entity, Game.PlayerPed.Handle, bone, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, zRot, true, true, false, false, 2, true);
            return entity;
        }
    }
}
