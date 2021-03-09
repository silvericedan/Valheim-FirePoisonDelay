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

        public static class DelayFire
        {
            
            private static void Postfix(SE_Burning __instance)
            {
                __instance.m_damageInterval = lol;
            }

            [HarmonyPrefix]
            [HarmonyPatch(typeof(SE_Burning),"AddFireDamage")]
            static bool AddFireDamage(ref float damage, ref float ___m_totalDamage, ref float ___m_damageInterval, 
                ref float ___m_ttl, ref HitData.DamageTypes ___m_damage, SE_Burning __instance)
            {
                ___m_totalDamage = Mathf.Max(___m_totalDamage, damage);
                int num = (int)(___m_ttl / ___m_damageInterval);
                float fire = ___m_totalDamage / (float)num;
                ___m_damage.m_fire = fire * 0.5f;
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

        public static class DelayPoison
        {
            [HarmonyPrefix]
            [HarmonyPatch(typeof(SE_Poison), "AddDamage")]
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
