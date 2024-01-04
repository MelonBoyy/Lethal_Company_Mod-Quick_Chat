using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using System.Collections.Generic;
using System.Diagnostics;

namespace QuickChat
{
	[BepInPlugin(PluginInfo.ModGUID, PluginInfo.ModName, PluginInfo.ModVersion)]
	[BepInDependency("ainavt.lc.lethalconfig")]
	public class QuickChatBase : BaseUnityPlugin
	{
		private readonly Harmony Harmony = new Harmony(PluginInfo.ModGUID);

		internal static ManualLogSource LogSource = BepInEx.Logging.Logger.CreateLogSource(PluginInfo.ModGUID);
		internal static ConfigFile ConfigR; 

		private void Awake()
		{
			ConfigR = Config;
			LogSource.LogDebug($"{PluginInfo.ModName} {PluginInfo.ModVersion} has been loaded!");

			ConfigLoader.Init();

			Harmony.PatchAll(typeof(ChatPatcher));
		}

	}

	internal static class PluginInfo
	{
		public const string ModGUID = "AlFungy.QuickChat";
		public const string ModName = "Quick Chat";
		public const string ModVersion = "1.0.0";
	}
}