using BepInEx;

using LethalConfig;
using LethalConfig.ConfigItems.Options;
using LethalConfig.ConfigItems;

using System.Collections;
using System.Collections.Generic;
using BepInEx.Logging;
using BepInEx.Configuration;
using System;

namespace QuickChat
{

	internal class ConfigLoader
	{

		internal static ConfigEntry<bool> QuickChatUseAdvanced;
		internal static ConfigEntry<bool> QuickChatCaseSensitive;
		internal static ConfigEntry<string> QuickChatPrefix;
		internal static ConfigEntry<int> QuickChatSimpleShortcutsNumber;
		internal static ConfigEntry<string> QuickChatShortcutsAdvanced;

		internal static List<SimpleShortcut> SimpleShortcuts = new List<SimpleShortcut>();

		internal static bool UseAdvanced => QuickChatUseAdvanced.Value;
		internal static int SimpleShortcutsNum => QuickChatSimpleShortcutsNumber.Value;
		internal static string AdvancedShortcutsRaw => QuickChatShortcutsAdvanced.Value;

		internal static void Init()
		{
			QuickChatUseAdvanced = QuickChatBase.ConfigR.Bind("General", "Use Advanced Shortcut Definition", false, "Allows user to define shortcuts with one Text Field (No game restarts needed).");
			QuickChatCaseSensitive = QuickChatBase.ConfigR.Bind("Shortcuts", "Is Case Sensitive?", false, "Are the shortcuts case sensitive? (Requires shortcut to be UPPERCASE or lowercase depending on definition of shortcut).");
			QuickChatPrefix = QuickChatBase.ConfigR.Bind("Shortcuts", "Chat Prefix", "/", "The prefix to use before a shortcut (say the prefix was \"/\": [/SHORTCUT_NAME => MESSAGE].");

			var quickChatUseAdvancedField = new BoolCheckBoxConfigItem(QuickChatUseAdvanced, true);
			var quickChatCaseSensitiveField = new BoolCheckBoxConfigItem(QuickChatCaseSensitive, false);
			QuickChatCaseSensitive.SettingChanged += (obj, args) => SaveCaseSensitive();

			var quickChatPrefixField = new TextInputFieldConfigItem(QuickChatPrefix, new TextInputFieldOptions
			{
				RequiresRestart = false
			});
			QuickChatPrefix.SettingChanged += (obj, args) => SaveChatShortcuts(SimpleShortcutsNum, SimpleShortcuts);

			LethalConfigManager.AddConfigItem(quickChatUseAdvancedField);

			LethalConfigManager.AddConfigItem(quickChatCaseSensitiveField);
			LethalConfigManager.AddConfigItem(quickChatPrefixField);
			if (!UseAdvanced)
			{
				QuickChatSimpleShortcutsNumber = QuickChatBase.ConfigR.Bind("Shortcuts", "Simple Shortcuts Amount", 4, "Amount of simple shortcuts (REQUIRES RESTART TO UPDATE).");
				var quickChatSimpleShortcutsNumberField = new IntInputFieldConfigItem(QuickChatSimpleShortcutsNumber, new IntInputFieldOptions()
				{
					RequiresRestart = true,
					Min = 1,
					Max = 20
				});

				LethalConfigManager.AddConfigItem(quickChatSimpleShortcutsNumberField);

				for (int i = 0; i < SimpleShortcutsNum; i++)
				{
					var simpleShortcut = QuickChatBase.ConfigR.Bind($"Simple Shortcut {i}", "Shortcut", $"Shortcut{i}", "The Shortcut that will be changed to the Result on enter.");
					var simpleShortcutField = new TextInputFieldConfigItem(simpleShortcut, new TextInputFieldOptions
					{
						RequiresRestart = false
					});
					simpleShortcut.SettingChanged += (obj, args) => SaveChatShortcuts(SimpleShortcutsNum, SimpleShortcuts);

					var simpleResult = QuickChatBase.ConfigR.Bind($"Simple Shortcut {i}", "Result", $"Result{i}", "The Result that will be shown after entering the Shortcut.");
					var simpleResultField = new TextInputFieldConfigItem(simpleResult, new TextInputFieldOptions
					{
						RequiresRestart = false
					});
					simpleResult.SettingChanged += (obj, args) => SaveChatShortcuts(SimpleShortcutsNum, SimpleShortcuts);

					LethalConfigManager.AddConfigItem(simpleShortcutField);
					LethalConfigManager.AddConfigItem(simpleResultField);

					SimpleShortcuts.Add(new SimpleShortcut()
					{
						shortcut = simpleShortcut,
						result = simpleResult
					});
				}
			}
			else
			{
				QuickChatShortcutsAdvanced = QuickChatBase.ConfigR.Bind("Shortcuts", "Advanced Chat Shortcuts", "a:My Balls", "Shortcuts in format of [SHORTCUT_NAME : MESSAGE, SHORTCUT_NAME2 : MESSAGE2] without the [].");

				var quickChatShortcutsAdvancedField = new TextInputFieldConfigItem(QuickChatShortcutsAdvanced, new TextInputFieldOptions
				{
					RequiresRestart = false
				});
				QuickChatShortcutsAdvanced.SettingChanged += (obj, args) => SaveChatShortcuts(AdvancedShortcutsRaw);

				LethalConfigManager.AddConfigItem(quickChatShortcutsAdvancedField);
			}

			SaveCaseSensitive();
			SaveBasedOnUseAdvanced();

			QuickChatBase.LogSource.LogDebug("QuickChat Config Successfully Loaded!");
		}

		internal static void SaveCaseSensitive()
		{
			ChatPatcher.CaseSensitive = QuickChatCaseSensitive.Value;
		}

		internal static void SavePrefix()
		{
			ChatPatcher.Prefix = QuickChatPrefix.Value;
		}

		internal static void SaveChatShortcuts(string rawShortcuts)
		{
			SavePrefix();
			ChatPatcher.ChatCommandsGen(rawShortcuts);
		}

		internal static void SaveChatShortcuts(int num, List<SimpleShortcut> simpleShortcuts)
		{
			if (simpleShortcuts.Count == 0 || simpleShortcuts == null)
			{
				SaveChatShortcuts(AdvancedShortcutsRaw);
			}

			SavePrefix();
			ChatPatcher.ChatShortcutsGeneration(num, simpleShortcuts);
		}

		internal static void SaveBasedOnUseAdvanced()
		{
			if (UseAdvanced)
			{
				SaveChatShortcuts(AdvancedShortcutsRaw);
			}
			else
			{
				SaveChatShortcuts(SimpleShortcutsNum, SimpleShortcuts);
			}
		}

	}

	internal class SimpleShortcut
	{
		internal ConfigEntry<string> shortcut;
		internal ConfigEntry<string> result;

		internal string shortcutValue => shortcut.Value;
		internal string resultValue => result.Value;
	}
}
