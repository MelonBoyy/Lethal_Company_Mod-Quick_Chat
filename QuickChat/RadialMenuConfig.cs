using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;

using BepInEx.Configuration;

using LethalConfig.ConfigItems;
using LethalConfig.ConfigItems.Options;
using LethalConfig;

using QuickChat.RadialMenu;

namespace QuickChat
{
	internal class RadialMenuConfig
	{
		internal static ConfigEntry<int> QuickChatRadialMenuBackgroundAlpha;
		internal static ConfigEntry<int> QuickChatRadialMenuRecentTextMinSize;
		internal static ConfigEntry<int> QuickChatRadialMenuRecentTextMaxSize;

		internal static void Init()
		{
			QuickChatRadialMenuBackgroundAlpha = Plugin.ConfigR.Bind("Radial Menu Background", "Alpha of Radial Menu background", 200, "The alpha (transparency) of the background when opening the Radial Menu.");

			var quickChatRadialMenuBackgroundAlphaField = new IntSliderConfigItem(QuickChatRadialMenuBackgroundAlpha, new IntSliderOptions()
			{
				RequiresRestart = false,
				Min = 0,
				Max = 255,
			});
			QuickChatRadialMenuBackgroundAlpha.SettingChanged += (obj, args) =>
			{
				if (!RadialMenuManager.RadialMenuLoaded) return;

				RadialMenuHUD.RadialMenuHUDBackground.color = new Color32(0, 0, 0, (byte)QuickChatRadialMenuBackgroundAlpha.Value);
			};

			QuickChatRadialMenuRecentTextMinSize = Plugin.ConfigR.Bind("Radial Menu Recent Text", "Radial Menu Text Minimum Size", 8, "The minimum size that the recent text can be.");
			QuickChatRadialMenuRecentTextMaxSize = Plugin.ConfigR.Bind("Radial Menu Recent Text", "Radial Menu Text Maximum Size", 18, "The maximum size that the recent text can be.");

			var quickChatRadialMenuRecentTextMinSizeField = new IntSliderConfigItem(QuickChatRadialMenuRecentTextMinSize, new IntSliderOptions()
			{
				RequiresRestart = false,
				Min = 1,
				Max = 24,
			});
			QuickChatRadialMenuRecentTextMinSize.SettingChanged += (obj, args) =>
			{
				if (!RadialMenuManager.RadialMenuLoaded) return;

				RadialMenuHUD.RadialMenuHUDRecentText.fontSizeMin = QuickChatRadialMenuRecentTextMinSize.Value;
			};

			var quickChatRadialMenuRecentTextMaxSizeField = new IntSliderConfigItem(QuickChatRadialMenuRecentTextMaxSize, new IntSliderOptions()
			{
				RequiresRestart = false,
				Min = 1,
				Max = 24,
			});
			QuickChatRadialMenuRecentTextMaxSize.SettingChanged += (obj, args) =>
			{
				if (!RadialMenuManager.RadialMenuLoaded) return;

				RadialMenuHUD.RadialMenuHUDRecentText.fontSizeMax = QuickChatRadialMenuRecentTextMaxSize.Value;
			};

			LethalConfigManager.AddConfigItem(quickChatRadialMenuBackgroundAlphaField);

			LethalConfigManager.AddConfigItem(quickChatRadialMenuRecentTextMinSizeField);
			LethalConfigManager.AddConfigItem(quickChatRadialMenuRecentTextMaxSizeField);
		}
	}
}
