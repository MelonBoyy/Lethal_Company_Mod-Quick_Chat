﻿using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;

using GameNetcodeStuff;

namespace QuickChat
{
	[BepInPlugin(PluginInfo.ModGUID, PluginInfo.ModName, PluginInfo.ModVersion)]
	[BepInDependency("ainavt.lc.lethalconfig")]
	[BepInDependency("atomic.terminalapi")]
	public partial class Plugin : BaseUnityPlugin
	{
		private readonly Harmony Harmony = new Harmony(PluginInfo.ModGUID);

		internal static ManualLogSource LogSource = BepInEx.Logging.Logger.CreateLogSource(PluginInfo.ModGUID);
		internal static ConfigFile ConfigR;
		internal static PlayerControllerB localPlayerController => GameNetworkManager.Instance.localPlayerController;

		private void Awake()
		{
			ConfigR = Config;
			LogSource.LogInfo($"{PluginInfo.ModName} {PluginInfo.ModVersion} has been loaded!");

			ShortcutHandler.Init();
			TerminalHandler.Init();

			Harmony.PatchAll(typeof(ChatPatcher));
			Harmony.PatchAll(typeof(ChatCharacterLimitPatcher));
		}

	}

	internal static class PluginInfo
	{
		public const string ModGUID = "alfungy.quickchat";
		public const string ModName = "Quick Chat";
		public const string ModVersion = "1.0.0";
	}
}