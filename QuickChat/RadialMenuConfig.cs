using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using UnityEngine.InputSystem;

using BepInEx.Configuration;

using LethalConfig.ConfigItems;
using LethalConfig.ConfigItems.Options;
using LethalConfig;

using QuickChat.RadialMenu;
using UnityEngine.InputSystem.LowLevel;

namespace QuickChat
{
	internal class RadialMenuConfig
	{

		internal static ConfigEntry<bool> QuickChatRadialMenuToggleUseMouse;
		internal static ConfigEntry<Key> QuickChatRadialMenuToggleKey;
		internal static ConfigEntry<MouseButton> QuickChatRadialMenuToggleMouseButton;

		internal static ConfigEntry<bool> QuickChatRadialMenuGoBackUseMouse;
		internal static ConfigEntry<Key> QuickChatRadialMenuGoBackKey;
		internal static ConfigEntry<MouseButton> QuickChatRadialMenuGoBackMouseButton;

		internal static ConfigEntry<int> QuickChatRadialMenuBackgroundAlpha;
		internal static ConfigEntry<int> QuickChatRadialMenuRecentTextMinSize;
		internal static ConfigEntry<int> QuickChatRadialMenuRecentTextMaxSize;

		internal static void Init()
		{
			Binds();

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

		internal static void Binds()
		{
			QuickChatRadialMenuToggleUseMouse = Plugin.ConfigR.Bind("Radial Menu Binds", "Toggle Open & Close Use Mouse", false, "Whether or not you use the mouse to open the radial menu.");
			QuickChatRadialMenuToggleKey = Plugin.ConfigR.Bind("Radial Menu Binds", "Toggle Open & Close Key", Key.LeftAlt, "The key used to open and close the radial menu.");
			QuickChatRadialMenuToggleMouseButton = Plugin.ConfigR.Bind("Radial Menu Binds", "Toggle Open & Close Mouse", MouseButton.Middle, "The mouse button used to open and close the radial menu.");

			var quickChatRadialMenuToggleUseMouseField = new BoolCheckBoxConfigItem(QuickChatRadialMenuToggleUseMouse, false);
			QuickChatRadialMenuToggleUseMouse.SettingChanged += (obj, args) =>
			{
				if (!QuickChatRadialMenuToggleUseMouse.Value)
					ChangeBinding(RadialMenuInput.RadialMenuToggleAction, Keyboard.current[QuickChatRadialMenuToggleKey.Value].path, !QuickChatRadialMenuToggleUseMouse.Value);
				else
					ChangeBinding(RadialMenuInput.RadialMenuToggleAction, GetMousePath(QuickChatRadialMenuToggleMouseButton.Value), QuickChatRadialMenuToggleUseMouse.Value);
			};

			var quickChatRadialMenuToggleKeyField = new EnumDropDownConfigItem<Key>(QuickChatRadialMenuToggleKey, new EnumDropDownOptions()
			{
				RequiresRestart = false,
				CanModifyCallback = () =>
				{
					return !QuickChatRadialMenuToggleUseMouse.Value;
				}
			});
			QuickChatRadialMenuToggleKey.SettingChanged += (obj, args) =>
			{
				ChangeBinding(RadialMenuInput.RadialMenuToggleAction, Keyboard.current[QuickChatRadialMenuToggleKey.Value].path, !QuickChatRadialMenuToggleUseMouse.Value);
			};

			var quickChatRadialMenuToggleMouseButtonField = new EnumDropDownConfigItem<MouseButton>(QuickChatRadialMenuToggleMouseButton, new EnumDropDownOptions()
			{
				RequiresRestart = false,
				CanModifyCallback = () =>
				{
					return QuickChatRadialMenuToggleUseMouse.Value;
				}
			});
			QuickChatRadialMenuToggleKey.SettingChanged += (obj, args) =>
			{
				ChangeBinding(RadialMenuInput.RadialMenuToggleAction, GetMousePath(QuickChatRadialMenuToggleMouseButton.Value), QuickChatRadialMenuToggleUseMouse.Value);
			};

			QuickChatRadialMenuGoBackUseMouse = Plugin.ConfigR.Bind("Radial Menu Binds", "Go Back Use Mouse", true, "Whether or not you use the mouse to go back one menu in the radial menu.");
			QuickChatRadialMenuGoBackKey = Plugin.ConfigR.Bind("Radial Menu Binds", "Go Back Key", Key.R, "The key used to go back one menu in the radial menu.");
			QuickChatRadialMenuGoBackMouseButton = Plugin.ConfigR.Bind("Radial Menu Binds", "Go Back Mouse", MouseButton.Right, "The mouse button used to go back one menu in the radial menu.");

			var quickChatRadialMenuGoBackUseMouseField = new BoolCheckBoxConfigItem(QuickChatRadialMenuGoBackUseMouse, false);
			QuickChatRadialMenuGoBackUseMouse.SettingChanged += (obj, args) =>
			{
				if (!QuickChatRadialMenuGoBackUseMouse.Value)
					ChangeBinding(RadialMenuInput.RadialMenuGoBackAction, Keyboard.current[QuickChatRadialMenuGoBackKey.Value].path, !QuickChatRadialMenuGoBackUseMouse.Value);
				else
					ChangeBinding(RadialMenuInput.RadialMenuGoBackAction, GetMousePath(QuickChatRadialMenuGoBackMouseButton.Value), QuickChatRadialMenuGoBackUseMouse.Value);
			};

			var quickChatRadialMenuGoBackKeyField = new EnumDropDownConfigItem<Key>(QuickChatRadialMenuGoBackKey, new EnumDropDownOptions()
			{
				RequiresRestart = false,
				CanModifyCallback = () =>
				{
					return !QuickChatRadialMenuGoBackUseMouse.Value;
				}
			});
			QuickChatRadialMenuGoBackKey.SettingChanged += (obj, args) =>
			{
				ChangeBinding(RadialMenuInput.RadialMenuGoBackAction, Keyboard.current[QuickChatRadialMenuGoBackKey.Value].path, !QuickChatRadialMenuGoBackUseMouse.Value);
			};

			var quickChatRadialMenuGoBackMouseButtonField = new EnumDropDownConfigItem<MouseButton>(QuickChatRadialMenuGoBackMouseButton, new EnumDropDownOptions()
			{
				RequiresRestart = false,
				CanModifyCallback = () =>
				{
					return QuickChatRadialMenuGoBackUseMouse.Value;
				}
			});
			QuickChatRadialMenuGoBackMouseButton.SettingChanged += (obj, args) =>
			{
				ChangeBinding(RadialMenuInput.RadialMenuGoBackAction, GetMousePath(QuickChatRadialMenuGoBackMouseButton.Value), QuickChatRadialMenuGoBackUseMouse.Value);
			};

			LethalConfigManager.AddConfigItem(quickChatRadialMenuToggleUseMouseField);
			LethalConfigManager.AddConfigItem(quickChatRadialMenuToggleKeyField);
			LethalConfigManager.AddConfigItem(quickChatRadialMenuToggleMouseButtonField);

			LethalConfigManager.AddConfigItem(quickChatRadialMenuGoBackUseMouseField);
			LethalConfigManager.AddConfigItem(quickChatRadialMenuGoBackKeyField);
			LethalConfigManager.AddConfigItem(quickChatRadialMenuGoBackMouseButtonField);

			UpdateBindings();
		}

		internal static void ChangeBinding(InputAction action, string bindingPathOverride, bool allowChange)
		{
			if (!allowChange || action == null) return;

			action.ApplyBindingOverride(bindingPathOverride);
		}

		internal static string GetMousePath(MouseButton button)
		{
			var mouse = Mouse.current;
			string path = string.Empty;

			switch (button)
			{
				case MouseButton.Left: path = mouse.leftButton.path; break;
				case MouseButton.Right: path = mouse.rightButton.path; break;
				case MouseButton.Middle: path = mouse.middleButton.path; break;
				case MouseButton.Forward: path = mouse.forwardButton.path; break;
				case MouseButton.Back: path = mouse.backButton.path; break;
			}

			return path;
		}

		internal static void UpdateBindings()
		{
			try
			{
				if (!QuickChatRadialMenuToggleUseMouse.Value)
					ChangeBinding(RadialMenuInput.RadialMenuToggleAction, Keyboard.current[QuickChatRadialMenuToggleKey.Value].path, !QuickChatRadialMenuToggleUseMouse.Value);
				else
					ChangeBinding(RadialMenuInput.RadialMenuToggleAction, GetMousePath(QuickChatRadialMenuToggleMouseButton.Value), QuickChatRadialMenuToggleUseMouse.Value);
			}
			catch { }

			try
			{
				if (!QuickChatRadialMenuGoBackUseMouse.Value)
					ChangeBinding(RadialMenuInput.RadialMenuGoBackAction, Keyboard.current[QuickChatRadialMenuGoBackKey.Value].path, !QuickChatRadialMenuGoBackUseMouse.Value);
				else
					ChangeBinding(RadialMenuInput.RadialMenuGoBackAction, GetMousePath(QuickChatRadialMenuGoBackMouseButton.Value), QuickChatRadialMenuGoBackUseMouse.Value);
			}
			catch { }
		}
	}
}
