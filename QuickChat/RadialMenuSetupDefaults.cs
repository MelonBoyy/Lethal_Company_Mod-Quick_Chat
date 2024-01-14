using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.InputSystem;

using HarmonyLib;
using GameNetcodeStuff;

namespace QuickChat.RadialMenu
{
	public class RadialMenuSetupDefaults
	{
		internal static List<string> PlayerNames = new List<string>() { "John", "Billy", "Bob" };
		public static RadialMenu positionsMenu;
		public static RadialMenu playersMenu;
		public static RadialMenu monstersMenu2;
		public static RadialMenu monstersMenu;
		public static RadialMenu commandsMenu;
		public static RadialMenu questionsMenu;
		public static RadialMenu observationsMenu;
		public static RadialMenu criesMenu;
		public static RadialMenu testMenu;

		public static void Init()
		{
			positionsMenu = new RadialMenu()
			{
				name = "Positions",
				radialButtons = new List<RadialMenu.RadialButton>()
				{
					new RadialMenu.RadialButton()
					{
						text = "that way.",
						displayText = "\"that way.\" (point)",
						postRadialButtonClicked = (radialMenu, radialButton) =>
						{
							var localPlayer = GameNetworkManager.Instance.localPlayerController;
							localPlayer.PerformEmote(new InputAction.CallbackContext(), 2);
						}
					},
					new RadialMenu.RadialButton()
					{
						text = "up ahead."
					},
					new RadialMenu.RadialButton()
					{
						text = "here."
					},
					new RadialMenu.RadialButton()
					{
						text = "to the left."
					},
					new RadialMenu.RadialButton()
					{
						text = "to the right."
					}
				}
			};

			playersMenu = new RadialMenu()
			{
				name = "Players"
			};

			monstersMenu2 = new RadialMenu()
			{
				name = "Monsters 2",
				radialOffset = RadialMenu.UnitCircleOffset.LEFT,
				radialButtons = new List<RadialMenu.RadialButton>
				{
					new RadialMenu.RadialButton()
					{
						displayText = "Last Page",
						connectingRadialMenu = () => monstersMenu
					},
					new RadialMenu.RadialButton()
					{
						text = "Nutcracker",
						connectingRadialMenu = () => positionsMenu
					},
					new RadialMenu.RadialButton()
					{
						text = "Coil-Head",
						connectingRadialMenu = () => positionsMenu
					},
					new RadialMenu.RadialButton()
					{
						text = "Jester",
						connectingRadialMenu = () => positionsMenu
					},
					new RadialMenu.RadialButton()
					{
						text = "Masked",
						connectingRadialMenu = () => positionsMenu
					},
					new RadialMenu.RadialButton()
					{
						text = "Eyeless Dog",
						preRadialButtonClicked = (radialMenu, radialButton) => RadialMenuManager.ModifyChatText("There\'s an", RadialMenuManager.ChatText.Count - 1),
						connectingRadialMenu = () => positionsMenu
					},
					new RadialMenu.RadialButton()
					{
						text = "Forest Keeper",
						connectingRadialMenu = () => positionsMenu
					},
					new RadialMenu.RadialButton()
					{
						text = "Earth Leviathan",
						preRadialButtonClicked = (radialMenu, radialButton) => RadialMenuManager.ModifyChatText("There\'s an", RadialMenuManager.ChatText.Count - 1),
						connectingRadialMenu = () => positionsMenu
					},
					new RadialMenu.RadialButton()
					{
						text = "Baboon Hawk",
						connectingRadialMenu = () => positionsMenu
					}
				}
			};

			monstersMenu = new RadialMenu()
			{
				name = "Monsters",
				radialOffset = RadialMenu.UnitCircleOffset.RIGHT,
				radialButtons = new List<RadialMenu.RadialButton>
				{
					new RadialMenu.RadialButton()
					{
						displayText = "Next Page",
						connectingRadialMenu = () => monstersMenu2
					},
					new RadialMenu.RadialButton()
					{
						text = "Snare Flea",
						connectingRadialMenu = () => positionsMenu
					},
					new RadialMenu.RadialButton()
					{
						text = "Bunker Spider",
						connectingRadialMenu = () => positionsMenu
					},
					new RadialMenu.RadialButton()
					{
						text = "Hoarding Bug",
						connectingRadialMenu = () => positionsMenu
					},
					new RadialMenu.RadialButton()
					{
						text = "Bracken",
						connectingRadialMenu = () => positionsMenu
					},
					new RadialMenu.RadialButton()
					{
						text = "Thumper",
						connectingRadialMenu = () => positionsMenu
					},
					new RadialMenu.RadialButton()
					{
						text = "Hydrogere",
						connectingRadialMenu = () => positionsMenu
					},
					new RadialMenu.RadialButton()
					{
						text = "Ghost Girl",
						connectingRadialMenu = () => positionsMenu
					},
					new RadialMenu.RadialButton()
					{
						text = "Spore Lizard",
						connectingRadialMenu = () => positionsMenu
					}
				}
			};

			commandsMenu = new RadialMenu()
			{
				name = "Commands",
				radialButtons = new List<RadialMenu.RadialButton>()
				{
					new RadialMenu.RadialButton()
					{
						text = "Help!",
					},
					new RadialMenu.RadialButton()
					{
						text = "Run!",
					},
					new RadialMenu.RadialButton()
					{
						text = "Follow me.",
					},
					new RadialMenu.RadialButton()
					{
						text = "Stop walking.",
					},
					new RadialMenu.RadialButton()
					{
						text = "Be quiet.",
					},
					new RadialMenu.RadialButton()
					{
						text = "I\'m gonna go."
					},
					new RadialMenu.RadialButton()
					{
						text = "Let\'s go",
						displayText = "\"Let\'s go {direction}\"",
						connectingRadialMenu = () => positionsMenu
					}
				}
			};

			questionsMenu = new RadialMenu()
			{
				name = "Questions",
				radialButtons = new List<RadialMenu.RadialButton>()
				{
					new RadialMenu.RadialButton()
					{
						text = "What was that?",
					},
					new RadialMenu.RadialButton()
					{
						text = "Who\'s there?",
					},
					new RadialMenu.RadialButton()
					{
						text = "Are you okay?",
					},
					new RadialMenu.RadialButton()
					{
						text = "Did you hear that?",
					},
					new RadialMenu.RadialButton()
					{
						text = "Are you real?",
					}
				}
			};

			observationsMenu = new RadialMenu()
			{
				name = "Observations",
				radialButtons = new List<RadialMenu.RadialButton>()
				{
					new RadialMenu.RadialButton()
					{
						text = "More scrap",
						displayText = "\"More scrap {direction}\"",
						connectingRadialMenu = () => positionsMenu
					},
					new RadialMenu.RadialButton()
					{
						text = "There\'s a",
						displayText = "\"There\'s a {monster} {direction}\"",
						connectingRadialMenu = () => monstersMenu
					},
					new RadialMenu.RadialButton()
					{
						text = "There\'s a trap",
						displayText = "\"There\'s a trap {direction}\"",
						connectingRadialMenu = () => positionsMenu
					},
					new RadialMenu.RadialButton()
					{
						text = "There\'s a dead end",
						displayText = "\"There\'s a dead end {direction}\"",
						connectingRadialMenu = () => positionsMenu
					},
					new RadialMenu.RadialButton()
					{
						text = "That\'s a mimic fire exit!"
					},
					new RadialMenu.RadialButton()
					{
						text = "That\'s not real!"
					}
				}
			};

			criesMenu = new RadialMenu()
			{
				name = "Cries",
				radialButtons = new List<RadialMenu.RadialButton>()
				{
					new RadialMenu.RadialButton()
					{
						text = "I\'m about to die!"
					},
					new RadialMenu.RadialButton()
					{
						text = "God please forgive me for my sins."
					}
				}
			};

			testMenu = new RadialMenu()
			{
				name = "Directions",
				radialButtons = new List<RadialMenu.RadialButton>()
				{
					new RadialMenu.RadialButton()
					{
						displayText = "Observations Menu",
						connectingRadialMenu = () => observationsMenu
					},
					new RadialMenu.RadialButton()
					{
						displayText = "Questions Menu",
						connectingRadialMenu = () => questionsMenu
					},
					new RadialMenu.RadialButton()
					{
						displayText = "Commands Menu",
						connectingRadialMenu = () => commandsMenu
					},
					new RadialMenu.RadialButton()
					{
						displayText = "Cries Menu",
						connectingRadialMenu = () => criesMenu
					},
					new RadialMenu.RadialButton()
					{
						displayText = "Players Menu",
						connectingRadialMenu = () => playersMenu
					},
				}
			};

			RadialMenuManager.MainMenu.AddRadialButton(new RadialMenu.RadialButton()
			{
				displayText = "Test Menu",
				connectingRadialMenu = () => testMenu
			});

			RadialMenuManager.RegisterRadialMenu(positionsMenu);
			RadialMenuManager.RegisterRadialMenu(playersMenu);
			RadialMenuManager.RegisterRadialMenu(monstersMenu2);
			RadialMenuManager.RegisterRadialMenu(monstersMenu);
			RadialMenuManager.RegisterRadialMenu(commandsMenu);
			RadialMenuManager.RegisterRadialMenu(questionsMenu);
			RadialMenuManager.RegisterRadialMenu(observationsMenu);
			RadialMenuManager.RegisterRadialMenu(criesMenu);
			RadialMenuManager.RegisterRadialMenu(testMenu);
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