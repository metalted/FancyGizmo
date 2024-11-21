using BepInEx;
using HarmonyLib;
using UnityEngine;
using BepInEx.Configuration;

namespace ZeepkistPluginBase
{
    [BepInPlugin(pluginGUID, pluginName, pluginVersion)]
    public class Plugin : BaseUnityPlugin
    {
        public const string pluginGUID = "com.metalted.zeepkist.fancygizmo";
        public const string pluginName = "FancyGizmo";
        public const string pluginVersion = "1.0";

        public Material XAxisMaterial = null;
        public Material YAxisMaterial = null;
        public Material ZAxisMaterial = null;
        public Material HoverMaterial = null;

        public ConfigEntry<int> xAxisID;
        public ConfigEntry<int> yAxisID;
        public ConfigEntry<int> zAxisID;
        public ConfigEntry<int> hoverID;

        public static Plugin Instance;
        public void Awake()
        {
            Harmony harmony = new Harmony(pluginGUID);
            harmony.PatchAll();
            xAxisID = Config.Bind("Colors", "X Color", 0, "The paint ID of the X Axis");
            yAxisID = Config.Bind("Colors", "Y Color", 0, "The paint ID of the Y Axis");
            zAxisID = Config.Bind("Colors", "Z Color", 0, "The paint ID of the Z Axis");
            hoverID = Config.Bind("Colors", "Hover", 0, "The paint ID when hovering over the gizmo");

            Instance = this;

            Config.SettingChanged += Config_SettingChanged;
        }

        private void Config_SettingChanged(object sender, SettingChangedEventArgs e)
        {
            ReloadMaterials();
        }

        public void ReloadMaterials()
        {
            try
            {
                XAxisMaterial = MaterialManager.AllMaterials[xAxisID.Value].material;
            }
            catch
            {
                XAxisMaterial = null;
            }

            try
            {
                YAxisMaterial = MaterialManager.AllMaterials[yAxisID.Value].material;
            }
            catch
            {
                YAxisMaterial = null;
            }

            try
            {
                ZAxisMaterial = MaterialManager.AllMaterials[zAxisID.Value].material;
            }
            catch
            {
                ZAxisMaterial = null;
            }

            try
            {
                HoverMaterial = MaterialManager.AllMaterials[hoverID.Value].material;
            }
            catch
            {
                HoverMaterial = null;
            }

            Debug.Log("Reload materials: " + XAxisMaterial + "," + YAxisMaterial + "," + ZAxisMaterial + "," + HoverMaterial);
        }
    }

    [HarmonyPatch(typeof(MainMenuUI), "Awake")]
    public class MainMenuUIAwakePatch()
    {
        public static void Postfix()
        {
            Plugin.Instance.ReloadMaterials();
        }
    }

    [HarmonyPatch(typeof(LEV_ColorMotherGizmo), "Start")]
    public class LEVColorMotherGizmoStartPatch
    {
        public static void Prefix(LEV_ColorMotherGizmo __instance)
        {
            __instance.redRenderers.Clear();
            __instance.greenRenderers.Clear();
            __instance.blueRenderers.Clear();
            __instance.magentaRenderers.Clear();
            __instance.singleGizmos.Clear();
        }
    }

    [HarmonyPatch(typeof(LEV_SingleGizmo), "Awake")]
    public class LEVSingleGizmoAwakePatch
    {
        public static void Postfix(LEV_SingleGizmo __instance)
        {
            if(__instance.gameObject.name.ToLower().Contains("x"))
            {
                if(Plugin.Instance.XAxisMaterial != null)
                {
                    __instance.renderdude.material = Plugin.Instance.XAxisMaterial;
                    __instance.original = Plugin.Instance.XAxisMaterial;                    
                }

                if (Plugin.Instance.HoverMaterial != null)
                {
                    __instance.hover = Plugin.Instance.HoverMaterial;
                }
            }
            else if (__instance.gameObject.name.ToLower().Contains("y"))
            {
                if (Plugin.Instance.YAxisMaterial != null)
                {
                    __instance.original = Plugin.Instance.YAxisMaterial;
                    __instance.renderdude.material = Plugin.Instance.YAxisMaterial;
                }

                if (Plugin.Instance.HoverMaterial != null)
                {
                    __instance.hover = Plugin.Instance.HoverMaterial;
                }
            }
            else if (__instance.gameObject.name.ToLower().Contains("z"))
            {
                if (Plugin.Instance.ZAxisMaterial != null)
                {
                    __instance.original = Plugin.Instance.ZAxisMaterial;
                    __instance.renderdude.material = Plugin.Instance.ZAxisMaterial;
                }

                if (Plugin.Instance.HoverMaterial != null)
                {
                    __instance.hover = Plugin.Instance.HoverMaterial;
                }
            }
        }
    }
}
