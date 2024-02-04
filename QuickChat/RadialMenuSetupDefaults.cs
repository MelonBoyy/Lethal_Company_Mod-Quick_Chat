using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.InputSystem;

using HarmonyLib;
using GameNetcodeStuff;
using System.Xml.Linq;
using BepInEx.Configuration;
using UnityEngine.InputSystem.Composites;
using LethalConfig.ConfigItems;
using LethalConfig.ConfigItems.Options;
using LethalConfig;

namespace QuickChat.RadialMenu
{
	public class RadialMenuSetupDefaults
	{
		public static RadialMenu positionsMenu = new RadialMenu("Positions");
		public static RadialMenu playersMenu = new RadialMenu("Players");
		public static RadialMenu monstersMenu2 = new RadialMenu("Monsters 2")
		{
			saveToHistory = false,
			radialOffset = RadialMenu.UnitCircleOffset.LEFT
		};
		public static RadialMenu monstersMenu = new RadialMenu("Monsters")
		{
			radialOffset = RadialMenu.UnitCircleOffset.RIGHT,
		};
		public static RadialMenu commandsMenu = new RadialMenu("Commands");
		public static RadialMenu answersMenu = new RadialMenu("Answers");
		public static RadialMenu questionsMenu = new RadialMenu("Questions");
		public static RadialMenu greetingsMenu = new RadialMenu("Greetings");
		public static RadialMenu observationsMenu = new RadialMenu("Observations");
		public static RadialMenu urgentMenu = new RadialMenu("Urgent");
		public static RadialMenu defaultMenu = new RadialMenu("Default Menu");

		public static List<RadialMenu.RadialButton> defaultMenuButtons = new List<RadialMenu.RadialButton>();

		internal static void Init()
		{
			defaultMenuButtons = new List<RadialMenu.RadialButton>()
			{
				new RadialMenu.RadialButton(observationsMenu, Color.cyan),
				new RadialMenu.RadialButton(questionsMenu, Color.yellow),
				new RadialMenu.RadialButton(commandsMenu, Color.magenta),
				new RadialMenu.RadialButton(urgentMenu, Color.red),
				new RadialMenu.RadialButton(answersMenu, Color.green),
				new RadialMenu.RadialButton(playersMenu, Color.blue),
				new RadialMenu.RadialButton(greetingsMenu, Color.white)
			};

			positionsMenu.UpdateRadialButtons(new List<RadialMenu.RadialButton>()
			{
				new RadialMenu.RadialButton("that way", "\"that way\" (point)")
				{
					postRadialButtonClicked = (radialMenu, radialButton) => PerformEmoteFix(2)
				},
				new RadialMenu.RadialButton("to the right"),
				new RadialMenu.RadialButton("up ahead"),
				new RadialMenu.RadialButton("here"),
				new RadialMenu.RadialButton("to the left")
			});

			monstersMenu2.UpdateRadialButtons(new List<RadialMenu.RadialButton>
			{
				new RadialMenu.RadialButton()
				{
					displayText = "Last Page",
					saveToHistory = false,
					connectingRadialMenu = () => monstersMenu
				},
				new RadialMenu.RadialButton("Nutcracker", positionsMenu),
				new RadialMenu.RadialButton("Coil-Head", positionsMenu),
				new RadialMenu.RadialButton("Jester", positionsMenu),
				new RadialMenu.RadialButton("Masked", positionsMenu),
				new RadialMenu.RadialButton("Forest Keeper", positionsMenu),
				new RadialMenu.RadialButton("Baboon Hawk", positionsMenu),
				new RadialMenu.RadialButton("Eyeless Dog", positionsMenu)
				{
					preRadialButtonClicked = (radialMenu, radialButton) => RadialMenuManager.ModifyRadialButtonHistory("There's an", RadialMenuManager.RadialButtonHistory.Count - 1)
				},
				new RadialMenu.RadialButton("Earth Leviathan", positionsMenu)
				{
					preRadialButtonClicked = (radialMenu, radialButton) => RadialMenuManager.ModifyRadialButtonHistory("There's an", RadialMenuManager.RadialButtonHistory.Count - 1)
				}
			});

			monstersMenu.UpdateRadialButtons(new List<RadialMenu.RadialButton>
			{
				new RadialMenu.RadialButton()
				{
					displayText = "Next Page",
					saveToHistory = false,
					connectingRadialMenu = () => monstersMenu2
				},
				new RadialMenu.RadialButton("Snare Flea", positionsMenu),
				new RadialMenu.RadialButton("Bunker Spider", positionsMenu),
				new RadialMenu.RadialButton("Hoarding Bug", positionsMenu),
				new RadialMenu.RadialButton("Bracken", positionsMenu),
				new RadialMenu.RadialButton("Thumper", positionsMenu),
				new RadialMenu.RadialButton("Hydrogere", positionsMenu),
				new RadialMenu.RadialButton("Ghost Girl", positionsMenu),
				new RadialMenu.RadialButton("Spore Lizard", positionsMenu)
			});

			commandsMenu.UpdateRadialButtons(new List<RadialMenu.RadialButton>()
			{
				new RadialMenu.RadialButton("Follow me"),
				new RadialMenu.RadialButton("Stop walking"),
				new RadialMenu.RadialButton("Be quiet"),
				new RadialMenu.RadialButton("I\'m gonna go"),
				new RadialMenu.RadialButton("Let\'s go", "\"Let\'s go {direction}\"", positionsMenu)
			});

			answersMenu.UpdateRadialButtons(new List<RadialMenu.RadialButton>()
			{
				new RadialMenu.RadialButton("I\'m thinking", Color.gray)
				{
					textColor = Color.white
				},
				new RadialMenu.RadialButton("No", Color.red),
				new RadialMenu.RadialButton("Yes", Color.green)
			});

			questionsMenu.UpdateRadialButtons(new List<RadialMenu.RadialButton>()
			{
				new RadialMenu.RadialButton("What was that", '?'),
				new RadialMenu.RadialButton("Who\'s there", '?'),
				new RadialMenu.RadialButton("Are you okay", '?'),
				new RadialMenu.RadialButton("Did you hear that", '?'),
				new RadialMenu.RadialButton("Are you real", '?'),
				new RadialMenu.RadialButton("What\'s up", '?'),
				new RadialMenu.RadialButton("What", '?')
			});

			greetingsMenu.UpdateRadialButtons(new List<RadialMenu.RadialButton>
			{
				new RadialMenu.RadialButton("Hello", '.'),
				new RadialMenu.RadialButton("Hi", '.')
			});

			observationsMenu.UpdateRadialButtons(new List<RadialMenu.RadialButton>()
			{
				new RadialMenu.RadialButton("More scrap", "\"More scrap {direction}\"", positionsMenu),
				new RadialMenu.RadialButton("There\'s a trap", "\"There\'s a trap {direction}\"", positionsMenu),
				new RadialMenu.RadialButton("There\'s a dead end", "\"There\'s a dead end {direction}\"", positionsMenu),
				new RadialMenu.RadialButton("There\'s a", "\"There\'s a(n) {monster} {direction}\"", monstersMenu),
				new RadialMenu.RadialButton("That\'s a mimic fire exit", '!'),
				new RadialMenu.RadialButton("That\'s not real", '!')
			});

			urgentMenu.UpdateRadialButtons(new List<RadialMenu.RadialButton>()
			{
				new RadialMenu.RadialButton("I\'m about to die", '!'),
				new RadialMenu.RadialButton("Run", '!'),
				new RadialMenu.RadialButton("Help", '!'),
				new RadialMenu.RadialButton("We have to get out of here", '!')
			});

			defaultMenu.UpdateRadialButtons(defaultMenuButtons);

			foreach (RadialMenu.RadialButton button in defaultMenuButtons)
			{
				DisplayTextConfigFromButton(button, defaultMenuButtons.IndexOf(button));
				ColorConfigFromButton(button, defaultMenuButtons.IndexOf(button));
			}

			RadialMenuManager.MainMenu.AddRadialButton(new RadialMenu.RadialButton(defaultMenu));

			RadialMenuManager.OnRadialMenuReady += RadialMenuManager_OnRadialMenuReady;
			RadialMenuManager.OnRadialMenuExit += RadialMenuManager_OnRadialMenuExit;
		}

		private static void RadialMenuManager_OnRadialMenuReady()
		{
			if (GameNetworkManager.Instance.localPlayerController == null) return;

			SetupPlayerMenu(GameNetworkManager.Instance.localPlayerController.quickMenuManager);
		}

		private static void RadialMenuManager_OnRadialMenuExit()
		{
			playersMenu.radialButtons.Clear();
			playersMenu.UpdateRadialButtons();
		}

		private static void PerformEmoteFix(int emoteIndex)
		{
			var localPlayer = GameNetworkManager.Instance.localPlayerController;

			localPlayer.timeSinceStartingEmote = 0f;
			localPlayer.performingEmote = true;
			localPlayer.playerBodyAnimator.SetInteger("emoteNumber", emoteIndex);
			localPlayer.StartPerformingEmoteServerRpc();
		}

		private static void ColorConfigFromButton(RadialMenu.RadialButton radialButton, int i)
		{
			RadialMenu connectingMenu = radialButton.connectingRadialMenu.Invoke();

			ConfigEntry<int>[] colorValues = new ConfigEntry<int>[4]
			{
				Plugin.ConfigR.Bind($"{connectingMenu.name} RadialMenu Options", "Color R", ConvertToRGB32(radialButton.buttonColor.r), "The Red of the RadialButton Color"),
				Plugin.ConfigR.Bind($"{connectingMenu.name} RadialMenu Options", "Color G", ConvertToRGB32(radialButton.buttonColor.g), "The Green of the RadialButton Color"),
				Plugin.ConfigR.Bind($"{connectingMenu.name} RadialMenu Options", "Color B", ConvertToRGB32(radialButton.buttonColor.b), "The Blue of the RadialButton Color"),
				Plugin.ConfigR.Bind($"{connectingMenu.name} RadialMenu Options", "Color A", ConvertToRGB32(radialButton.buttonColor.a), "The Alpha (Transparency) of the RadialButton Color")
			};

			var colorValuesOptions = new IntSliderOptions()
			{
				Min = 0,
				Max = 255,
				RequiresRestart = false
			};

			var colorValuesFieldR = new IntSliderConfigItem(colorValues[0], colorValuesOptions);
			colorValues[0].SettingChanged += (obj, args) =>
			{
				RadialMenu.RadialButton button = defaultMenuButtons[i];

				button.buttonColor = new Color(ConvertToRGB(colorValues[0].Value), button.buttonColor.g, button.buttonColor.b, button.buttonColor.a);
			};

			var colorValuesFieldG = new IntSliderConfigItem(colorValues[1], colorValuesOptions);
			colorValues[1].SettingChanged += (obj, args) =>
			{
				RadialMenu.RadialButton button = defaultMenuButtons[i];

				button.buttonColor = new Color(button.buttonColor.r, ConvertToRGB(colorValues[1].Value), button.buttonColor.b, button.buttonColor.a);
			};

			var colorValuesFieldB = new IntSliderConfigItem(colorValues[2], colorValuesOptions);
			colorValues[2].SettingChanged += (obj, args) =>
			{
				RadialMenu.RadialButton button = defaultMenuButtons[i];

				button.buttonColor = new Color(button.buttonColor.r, button.buttonColor.g, ConvertToRGB(colorValues[2].Value), button.buttonColor.a);
			};

			var colorValuesFieldA = new IntSliderConfigItem(colorValues[3], colorValuesOptions);
			colorValues[3].SettingChanged += (obj, args) =>
			{
				RadialMenu.RadialButton button = defaultMenuButtons[i];

				button.buttonColor = new Color(button.buttonColor.r, button.buttonColor.g, button.buttonColor.b, ConvertToRGB(colorValues[3].Value));
			};

			LethalConfigManager.AddConfigItem(colorValuesFieldR);
			LethalConfigManager.AddConfigItem(colorValuesFieldG);
			LethalConfigManager.AddConfigItem(colorValuesFieldB);
			LethalConfigManager.AddConfigItem(colorValuesFieldA);
		}

		private static void DisplayTextConfigFromButton(RadialMenu.RadialButton radialButton, int i)
		{
			RadialMenu connectingMenu = radialButton.connectingRadialMenu.Invoke();

			ConfigEntry<string> displayTextValue = Plugin.ConfigR.Bind($"{connectingMenu.name} RadialMenu Options", "Display Text", radialButton.displayText, "The Display Text of the Radial Button");

			var displayTextField = new TextInputFieldConfigItem(displayTextValue, requiresRestart: false);
			displayTextValue.SettingChanged += (obj, args) =>
			{
				RadialMenu.RadialButton button = defaultMenuButtons[i];

				button.displayText = displayTextValue.Value;
			};

			LethalConfigManager.AddConfigItem(displayTextField);
		}

		private static int ConvertToRGB32(float colorValue)
		{
			return (int)(colorValue * 255);
		}

		private static float ConvertToRGB(int colorValue)
		{
			return (float)colorValue / 255;
		}

		[HarmonyPatch(typeof(QuickMenuManager), nameof(QuickMenuManager.AddUserToPlayerList))]
		[HarmonyPostfix]
		static void SetupPlayerMenuJoin(QuickMenuManager __instance)
		{
			SetupPlayerMenu(__instance);
		}

		[HarmonyPatch(typeof(QuickMenuManager), nameof(QuickMenuManager.RemoveUserFromPlayerList))]
		[HarmonyPostfix]
		static void SetupPlayerMenuLeave(QuickMenuManager __instance)
		{
			SetupPlayerMenu(__instance);
		}

		static void SetupPlayerMenu(QuickMenuManager __instance)
		{
			List<RadialMenu.RadialButton> radialButtons = new List<RadialMenu.RadialButton>();

			foreach (PlayerListSlot playerListSlot in __instance.playerListSlots)
			{
				playersMenu.AddRadialButton(new RadialMenu.RadialButton()
				{
					text = playerListSlot.usernameHeader.text
				});
			}

			playersMenu.UpdateRadialButtons(radialButtons);
		}
	}

}