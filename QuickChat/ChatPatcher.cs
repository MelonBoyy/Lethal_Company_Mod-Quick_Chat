using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BepInEx;
using GameNetcodeStuff;
using HarmonyLib;
using TMPro;

using UnityEngine.UI;

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

				Plugin.LogSource.LogDebug($"New Chat Shortcut: \"{shortcut}\" => \"{result}\"");
				ChatShortcuts.Add(shortcut, result);
			}

		}

		internal static void ChatShortcutsGeneration(int num, List<SimpleShortcut> simpleShortcuts)
		{
			ChatShortcuts.Clear();
			for (int i = 0; i < num; i++)
			{
				string rawShortcut = simpleShortcuts[i].nameValue;
				string rawResult = simpleShortcuts[i].resultValue;

				string shortcut, result;

				if (rawShortcut.IsNullOrWhiteSpace() || rawResult.IsNullOrWhiteSpace()) continue;

				shortcut = $"{Prefix}{rawShortcut.RemoveWhiteSpace()}";
				result = rawResult.Trim();

				if (!CaseSensitive) shortcut = shortcut.ToLower();

				Plugin.LogSource.LogDebug($"New Chat Shortcut: \"{shortcut}\" => \"{result}\"");
				ChatShortcuts.Add(shortcut, result);
			}

		}

		public static string GetChatShortcutsList()
		{
			string fullList = string.Empty;

			foreach (KeyValuePair<string, string> ChatShortcut in ChatShortcuts)
			{
				int index = ChatShortcuts.Keys.ToList().IndexOf(ChatShortcut.Key);

				fullList += $"{GetChatShortcutAdvancedFormat(index, ChatShortcut.Key, ChatShortcut.Value)}\n";
			}

			return fullList;
		}

		public static string GetChatShortcutAdvancedFormat(int num, string name, string result)
		{
			return $"{num} - {name} : {result}";
		}

		[HarmonyPatch("Start")]
		[HarmonyPostfix]
		static void StopSelectionOnOpenChatPatch(ref TMP_InputField ___chatTextField)
		{
			___chatTextField.onFocusSelectAll = false;
		}

		[HarmonyPatch(nameof(HUDManager.AddTextToChatOnServer))]
		[HarmonyPrefix]
		static void ChatPatch(ref string chatMessage, ref int playerId)
		{
			string inspectChatMessage = CaseSensitive ? chatMessage : chatMessage.ToLower();

			Plugin.LogSource.LogDebug($"Tried to change command {chatMessage}");

			if (ChatShortcuts.ContainsKey(inspectChatMessage))
			{
				string resultMessage = ChatShortcuts[inspectChatMessage];
				chatMessage = resultMessage;

				Plugin.LogSource.LogDebug($"Found command {chatMessage} => {resultMessage}");
			}
		}

		[HarmonyPatch("EnableChat_performed")]
		[HarmonyPostfix]
		static void PrefixOnChatPatch(ref PlayerControllerB ___localPlayer, ref TMP_InputField ___chatTextField)
		{
			if (!ShortcutHandler.QuickChatPrefixOnChat.Value) return;

			___chatTextField.text = ShortcutHandler.QuickChatPrefix.Value;
			___chatTextField.MoveToEndOfLine(false, false);
		}

	}
}
