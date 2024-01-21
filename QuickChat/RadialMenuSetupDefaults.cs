using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.InputSystem;

using HarmonyLib;
using GameNetcodeStuff;
using System.Xml.Linq;

namespace QuickChat.RadialMenu
{
	public class RadialMenuSetupDefaults
	{
		internal static List<string> PlayerNames = new List<string>();
		public static RadialMenu positionsMenu;
		public static RadialMenu playersMenu;
		public static RadialMenu monstersMenu2;
		public static RadialMenu monstersMenu;
		public static RadialMenu commandsMenu;
		public static RadialMenu answersMenu;
		public static RadialMenu questionsMenu;
		public static RadialMenu observationsMenu;
		public static RadialMenu urgentMenu;
		public static RadialMenu defaultMenu;

		internal static void Init()
		{
			positionsMenu = new RadialMenu("Positions")
			{
				radialButtons = new List<RadialMenu.RadialButton>()
				{
					new RadialMenu.RadialButton("that way", "\"that way\" (point)")
					{
						postRadialButtonClicked = (radialMenu, radialButton) => PerformEmoteFix(2)
					},
					new RadialMenu.RadialButton("to the right"),
					new RadialMenu.RadialButton("up ahead"),
					new RadialMenu.RadialButton("here"),
					new RadialMenu.RadialButton("to the left")
				}
			};

			playersMenu = new RadialMenu("Players");

			monstersMenu2 = new RadialMenu("Monsters 2")
			{
				saveToHistory = false,
				radialOffset = RadialMenu.UnitCircleOffset.LEFT,
				radialButtons = new List<RadialMenu.RadialButton>
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
				}
			};

			monstersMenu = new RadialMenu("Monsters")
			{
				radialOffset = RadialMenu.UnitCircleOffset.RIGHT,
				radialButtons = new List<RadialMenu.RadialButton>
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
				}
			};

			commandsMenu = new RadialMenu("Commands")
			{
				radialButtons = new List<RadialMenu.RadialButton>()
				{
					new RadialMenu.RadialButton("Follow me"),
					new RadialMenu.RadialButton("Stop walking"),
					new RadialMenu.RadialButton("Be quiet"),
					new RadialMenu.RadialButton("I\'m gonna go"),
					new RadialMenu.RadialButton("Let\'s go", "\"Let\'s go {direction}\"", positionsMenu)
				}
			};

			answersMenu = new RadialMenu("Answers")
			{
				radialButtons = new List<RadialMenu.RadialButton>()
				{
					new RadialMenu.RadialButton("I\'m thinking", Color.gray)
					{
						textColor = Color.white
					},
					new RadialMenu.RadialButton("No", Color.red),
					new RadialMenu.RadialButton("Yes", Color.green)
				}
			};

			questionsMenu = new RadialMenu("Questions")
			{
				radialButtons = new List<RadialMenu.RadialButton>()
				{
					new RadialMenu.RadialButton("What was that", '?'),
					new RadialMenu.RadialButton("Who\'s there", '?'),
					new RadialMenu.RadialButton("Are you okay", '?'),
					new RadialMenu.RadialButton("Did you hear that", '?'),
					new RadialMenu.RadialButton("Are you real", '?')
				}
			};

			observationsMenu = new RadialMenu("Observations")
			{
				radialButtons = new List<RadialMenu.RadialButton>()
				{
					new RadialMenu.RadialButton("More scrap", "\"More scrap {direction}\"", positionsMenu),
					new RadialMenu.RadialButton("There\'s a trap", "\"There\'s a trap {direction}\"", positionsMenu),
					new RadialMenu.RadialButton("There\'s a dead end", "\"There\'s a dead end {direction}\"", positionsMenu),
					new RadialMenu.RadialButton("There\'s a", "\"There\'s a(n) {monster} {direction}\"", monstersMenu),
					new RadialMenu.RadialButton("That\'s a mimic fire exit", '!'),
					new RadialMenu.RadialButton("That\'s not real", '!')
				}
			};

			urgentMenu = new RadialMenu("Urgent")
			{
				radialButtons = new List<RadialMenu.RadialButton>()
				{
					new RadialMenu.RadialButton("I\'m about to die", '!'),
					new RadialMenu.RadialButton("Run", '!'),
					new RadialMenu.RadialButton("Help", '!'),
					new RadialMenu.RadialButton("We have to get out of here", '!')
				}
			};

			defaultMenu = new RadialMenu("Default Menu")
			{
				radialButtons = new List<RadialMenu.RadialButton>()
				{
					new RadialMenu.RadialButton(observationsMenu, Color.cyan),
					new RadialMenu.RadialButton(questionsMenu, Color.yellow),
					new RadialMenu.RadialButton(commandsMenu, Color.magenta),
					new RadialMenu.RadialButton(urgentMenu, Color.red),
					new RadialMenu.RadialButton(answersMenu, Color.green),
					new RadialMenu.RadialButton(playersMenu, Color.blue)
				}
			};

			RadialMenuManager.MainMenu.AddRadialButton(new RadialMenu.RadialButton(defaultMenu));

			RadialMenuManager.RegisterRadialMenu(positionsMenu);
			RadialMenuManager.RegisterRadialMenu(playersMenu);
			RadialMenuManager.RegisterRadialMenu(monstersMenu2);
			RadialMenuManager.RegisterRadialMenu(monstersMenu);
			RadialMenuManager.RegisterRadialMenu(commandsMenu);
			RadialMenuManager.RegisterRadialMenu(questionsMenu);
			RadialMenuManager.RegisterRadialMenu(observationsMenu);
			RadialMenuManager.RegisterRadialMenu(urgentMenu);
			RadialMenuManager.RegisterRadialMenu(answersMenu);
			RadialMenuManager.RegisterRadialMenu(defaultMenu);
		}

		private static void PerformEmoteFix(int emoteIndex)
		{
			var localPlayer = GameNetworkManager.Instance.localPlayerController;

			localPlayer.timeSinceStartingEmote = 0f;
			localPlayer.performingEmote = true;
			localPlayer.playerBodyAnimator.SetInteger("emoteNumber", emoteIndex);
			localPlayer.StartPerformingEmoteServerRpc();
		}

		[HarmonyPatch(typeof(PlayerControllerB), nameof(PlayerControllerB.ConnectClientToPlayerObject))]
		[HarmonyPostfix]
		static void SetupPlayerMenu(PlayerControllerB __instance)
		{
			List<RadialMenu.RadialButton> radialButtons = new List<RadialMenu.RadialButton>();

			foreach (PlayerControllerB i in __instance.playersManager.allPlayerScripts)
			{
				radialButtons.Add(new RadialMenu.RadialButton()
				{
					text = i.playerUsername
				});
			}

			playersMenu.UpdateRadialButtons(radialButtons);
		}
	}

}