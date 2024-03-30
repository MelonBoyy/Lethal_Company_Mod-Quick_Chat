using System.Collections.Generic;

using UnityEngine;

using HarmonyLib;
using LethalConfig.ConfigItems;
using LethalConfig;

namespace QuickChat.RadialMenu
{
	public class RadialMenuSetupDefaults
	{
		public static RadialMenu positionsMenu = new RadialMenu("Positions");
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

			// The item containers so we can loop through them later
			List<TextInputFieldConfigItem> displayTextConfigItems = new List<TextInputFieldConfigItem>();
			List<IntSliderConfigItem[]> colorConfigItems = new List<IntSliderConfigItem[]>();

			foreach (RadialMenu.RadialButton button in defaultMenuButtons)
			{
				displayTextConfigItems.Add(RadialMenuConfig.DisplayTextConfigFromButton(Plugin.ConfigF, button));
				colorConfigItems.Add(RadialMenuConfig.ColorConfigFromButton(Plugin.ConfigF, button));
			}

			// Adding the items to the Config manually
			foreach (TextInputFieldConfigItem item in displayTextConfigItems)
			{
				LethalConfigManager.AddConfigItem(item);
			}

			// Adding the items to the Config manually
			foreach (IntSliderConfigItem[] arr in colorConfigItems)
			{
				foreach (IntSliderConfigItem item in arr)
				{
					LethalConfigManager.AddConfigItem(item);
				}
			}

			RadialMenuManager.MainMenu.AddRadialButton(new RadialMenu.RadialButton(defaultMenu));
		}

		private static void PerformEmoteFix(int emoteIndex)
		{
			var localPlayer = GameNetworkManager.Instance.localPlayerController;

			localPlayer.timeSinceStartingEmote = 0f;
			localPlayer.performingEmote = true;
			localPlayer.playerBodyAnimator.SetInteger("emoteNumber", emoteIndex);
			localPlayer.StartPerformingEmoteServerRpc();
		}
	}

}