using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;

using GameNetcodeStuff;
using QuickChat.RadialMenu;
using LethalConfig;

namespace QuickChat
{
	[BepInPlugin(PluginInfo.ModGUID, PluginInfo.ModName, PluginInfo.ModVersion)]
	[BepInDependency("ainavt.lc.lethalconfig")]
	[BepInDependency("atomic.terminalapi")]
	public partial class Plugin : BaseUnityPlugin
	{
		private readonly Harmony Harmony = new Harmony(PluginInfo.ModGUID);

		internal static ManualLogSource LogSource = BepInEx.Logging.Logger.CreateLogSource(PluginInfo.ModGUID);
		internal static ConfigFile ConfigF;

		private void Awake()
		{
			ConfigF = Config;
			LogSource.LogInfo($"{PluginInfo.ModName} {PluginInfo.ModVersion} has been loaded!");

			LethalConfigManager.SkipAutoGen();

			RadialMenuConfig.Init();
			RadialMenuSetupDefaults.Init();

			ShortcutHandler.Init();
			TerminalHandler.Init();

			Harmony.PatchAll(typeof(ChatPatcher));
			Harmony.PatchAll(typeof(ChatCharacterLimitPatcher));
			Harmony.PatchAll(typeof(RadialMenuHUD));
			Harmony.PatchAll(typeof(RadialMenuInput));
			Harmony.PatchAll(typeof(RadialMenuSetupDefaults));
			Harmony.PatchAll(typeof(RadialMenuManager));
		}

	}

	internal static class PluginInfo
	{
		public const string ModGUID = "alfungy.quickchat";
		public const string ModName = "Quick Chat";
		public const string ModVersion = "2.1.4";
	}
}