using BepInEx;
using HarmonyLib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace QuickChat
{
	[HarmonyPatch(typeof(HUDManager))]
	internal class ChatPatcher
	{
		internal static Dictionary<string, string> ChatShortcuts = new Dictionary<string, string>();

		internal static string Prefix = "/";
		internal static bool CaseSensitive = false;

		internal static void ChatCommandsGen(string rawShortcuts)
		{
			string[] rawShortcutArray = rawShortcuts.Split(',');

			ChatShortcuts.Clear();
			foreach (string rawShortcut in rawShortcutArray)
			{
				string[] splitCommand = rawShortcut.Split(':');
				string shortcut, result;

				if (splitCommand[0].IsNullOrWhiteSpace() || splitCommand[1].IsNullOrWhiteSpace()) continue;

				shortcut = $"{Prefix}{splitCommand[0].RemoveWhiteSpace()}";
				result = splitCommand[1].Trim();

				if (!CaseSensitive) shortcut = shortcut.ToLower();

				QuickChatBase.LogSource.LogDebug($"New Chat Shortcut: \"{shortcut}\" => \"{result}\"");
				ChatShortcuts.Add(shortcut, result);
			}

		}

		internal static void ChatShortcutsGeneration(int num, List<SimpleShortcut> simpleShortcuts)
		{
			ChatShortcuts.Clear();
			for (int i = 0; i < num; i++)
			{
				string rawShortcut = simpleShortcuts[i].shortcutValue;
				string rawResult = simpleShortcuts[i].resultValue;

				string shortcut, result;

				if (rawShortcut.IsNullOrWhiteSpace() || rawResult.IsNullOrWhiteSpace()) continue;

				shortcut = $"{Prefix}{rawShortcut.RemoveWhiteSpace()}";
				result = rawResult.Trim();

				if (!CaseSensitive) shortcut = shortcut.ToLower();

				QuickChatBase.LogSource.LogDebug($"New Chat Shortcut: \"{shortcut}\" => \"{result}\"");
				ChatShortcuts.Add(shortcut, result);
			}

		}

		[HarmonyPatch(nameof(HUDManager.AddTextToChatOnServer))]
		[HarmonyPrefix]
		static void ChatPatch(ref string chatMessage, ref int playerId)
		{
			string inspectChatMessage = CaseSensitive ? chatMessage : chatMessage.ToLower();

			QuickChatBase.LogSource.LogDebug($"Tried to change command {chatMessage}");

			if (ChatShortcuts.ContainsKey(inspectChatMessage))
			{
				string resultMessage = ChatShortcuts[inspectChatMessage];
				chatMessage = resultMessage;

				QuickChatBase.LogSource.LogDebug($"Found command {chatMessage} => {resultMessage}");
			}
		}

	}
}
