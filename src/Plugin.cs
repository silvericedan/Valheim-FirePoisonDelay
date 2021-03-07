using System.Reflection;
using BepInEx;
using HarmonyLib;
using UnityEngine;


namespace BepInExPlugin
{
    [BepInPlugin(PluginInfo.PLUGIN_ID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        public static float lol = 1f;

        private void Awake()
        {
            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), null);
        }

        [HarmonyPatch(typeof(SE_Burning), "Setup")]
        public static class DelayFire
        {
            private static float m_totalDamage2;

            
            private static void Postfix(SE_Burning __instance)
            {
                __instance.m_damageInterval = lol;
            }

            [HarmonyPrefix]
            [HarmonyPatch(typeof(SE_Burning),"AddFireDamage")]
            static bool Prefix(ref float damage, ref float ___m_totalDamage, ref float ___m_damageInterval, 
                ref float ___m_ttl, ref HitData.DamageTypes ___m_damage, SE_Burning __instance)
            {
                //private HitData.DamageTypes m_damage;
                ___m_totalDamage = Mathf.Max(___m_totalDamage, damage);
                int num = (int)(___m_ttl / ___m_damageInterval);
                float fire = ___m_totalDamage / (float)num;
                ___m_damage.m_fire = fire;
                __instance.ResetTime();
                return false;
            }
        }
       
        [HarmonyPatch(typeof(Console), "InputText")]

        public static class Console_Patch
        {

            private static void Postfix(Console __instance)
            {
                string newtext = __instance.m_input.text;
                float.TryParse(newtext, out lol);
            }
        }
    }


}
