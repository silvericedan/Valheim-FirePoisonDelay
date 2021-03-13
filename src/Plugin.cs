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

        [HarmonyPatch(typeof(SE_Burning), "AddFireDamage")]
        public static class DelayFire
        {
            
            private static void Postfix(SE_Burning __instance)
            {
                __instance.m_damageInterval = lol;
            }

            [HarmonyPrefix]
            static bool AddFireDamage(SE_Burning __instance, ref float damage)
            {
                Debug.Log("damageeeeefireee: " + damage);
                __instance.m_totalDamage = Mathf.Max(__instance.m_totalDamage, damage);
                int num = (int)(__instance.m_ttl / __instance.m_damageInterval);
                float fire = __instance.m_totalDamage / (float)num;
                __instance.m_damage.m_fire = fire;
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

        [HarmonyPatch(typeof(SE_Poison), "AddDamage")]
        public static class DelayPoison
        {
            [HarmonyPrefix]
            static bool AddDamage(SE_Poison __instance, ref float damage)
            {
                Debug.Log("damageeeee: " + damage);

                if (damage >= __instance.m_damageLeft)
                {
                    __instance.m_damageLeft = damage;
                    float num = (__instance.m_character.IsPlayer() ? __instance.m_TTLPerDamagePlayer : __instance.m_TTLPerDamage);
                    __instance.m_ttl = __instance.m_baseTTL + Mathf.Pow(__instance.m_damageLeft * num, __instance.m_TTLPower);
                    int num2 = (int)(__instance.m_ttl / __instance.m_damageInterval);
                    __instance.m_damagePerHit = __instance.m_damageLeft / (float)num2;
                    __instance.m_damagePerHit = __instance.m_damagePerHit * 0.5f;
                    __instance.ResetTime();
                }
                return false;
            }
        }

    }


}
