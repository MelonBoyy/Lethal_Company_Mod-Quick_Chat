using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using UnityEngine.UI;
using static QuickChat.RadialMenu.RadialMenu;

namespace QuickChat.RadialMenu
{
	public class RadialMenuManager
	{
		public static List<RadialMenu> RadialMenuRegistry { get; private set; } = new List<RadialMenu>();
		public static List<RadialMenu> RadialMenuHistory { get; private set; } = new List<RadialMenu>();
		public static RadialMenu CurrentMenu { get; private set; }
		public static RadialMenu LastMenu => RadialMenuHistory.Count > 0 ? RadialMenuHistory.Last() : null;

		public static RadialMenu MainMenu { get; private set; } = new RadialMenu()
		{
			name = "MainMenu"
		};

		public static List<RadialMenu.RadialButton> RadialButtonHistory { get; private set; } = new List<RadialMenu.RadialButton>();
		public static RadialMenu.RadialButton MostRecentButton => RadialButtonHistory.Count > 0 ? RadialButtonHistory.Last() : null;
		public static string MostRecentText => RadialButtonHistory.Count > 0 ? RadialButtonHistory.Last().text.Trim() : string.Empty;

		internal static bool RadialMenuLoaded = false;
		internal static bool RadialMenuOpen = false;


		public delegate void RadialMenuLoadingEvent();
		public static event RadialMenuLoadingEvent OnRadialMenuPreRegister;
		public static event RadialMenuLoadingEvent OnRadialMenuPostRegister;
		public static event RadialMenuLoadingEvent OnRadialMenuReady;

		[HarmonyPatch(typeof(RoundManager), "Awake")]
		[HarmonyPostfix]
		static void AwakePatch()
		{
			RadialMenuHUD.Init();
			RadialMenuInput.Init();

			OnRadialMenuPreRegister?.Invoke();
			RegisterRadialMenu(MainMenu);
			OnRadialMenuPostRegister?.Invoke();

			foreach (RadialMenu menu in RadialMenuRegistry)
			{
				menu.CreateRadialMenu(RadialMenuHUD.RadialMenuHUDObject.transform);
			}

			SetCurrentMenu(MainMenu);
			RefreshMenu();

			OnRadialMenuReady?.Invoke();
		}

		[HarmonyPatch(typeof(QuickMenuManager), nameof(QuickMenuManager.LeaveGameConfirm))]
		[HarmonyPostfix]
		static void ResetPatch()
		{
			foreach (RadialMenu menu in RadialMenuRegistry)
			{
				menu.RemoveRadialMenu();
			}

			RadialMenuInput.DeInit();
			RadialMenuHUD.DeInit();
		}

		/// <summary>
		/// Helper expression to simplify the creation/definition and registration of a RadialMenu.
		/// </summary>
		/// <param name="radialMenu">The RadialMenu to be created and registered automatically.</param>
		/// <returns></returns>
		public static RadialMenu ConstructAndRegisterRadialMenu(RadialMenu radialMenu)
		{
			RegisterRadialMenu(radialMenu);
			return radialMenu;
		}

		/// <summary>
		/// Method to Register an already defined RadialMenu.
		/// </summary>
		/// <param name="radialMenu">The RadialMenu to register.</param>
		public static void RegisterRadialMenu(RadialMenu radialMenu)
		{
			if (RadialMenuRegistry.Contains(radialMenu)) return;

			RadialMenuRegistry.Add(radialMenu);
		}

		/// <summary>
		/// Sets the Current RadialMenu and has an option to save the last one to the "RadialMenuHistory".
		/// </summary>
		/// <param name="menu">The RadialMenu to set as current.</param>
		/// <param name="saveLastToHistory">If the last RadialMenu should be saved to the "RadialMenuHistory"</param>
		public static void SetCurrentMenu(RadialMenu menu, bool saveLastToHistory = true)
		{
			if (CurrentMenu == menu) return;

			if (CurrentMenu != null && saveLastToHistory && CurrentMenu.saveToHistory) RadialMenuHistory.Add(CurrentMenu);
			CurrentMenu = menu;
		}

		/// <summary>
		/// Go back one menu.
		/// </summary>
		public static void GoBackMenu()
		{
			if (RadialMenuHistory.Count <= 0) return;

			RadialMenu newCurrentMenu = LastMenu;
			RadialMenu oldMenu = CurrentMenu;

			if (RadialMenuHistory.Count == RadialButtonHistory.Count) RemoveRadialButtonHistory();

			SetCurrentMenu(newCurrentMenu, false);
			RefreshMenu(oldMenu, newCurrentMenu);

			RemoveFromRadialMenuHistory();
		}

		/// <summary>
		/// Hides the Last Menu and shows the Current Menu.
		/// </summary>
		public static void RefreshMenu()
		{
			LastMenu?.gameObject.SetActive(false);
			CurrentMenu?.gameObject.SetActive(true);
		}

		/// <summary>
		/// Hides the "deactivatingMenu" and shows the "activatingMenu".
		/// </summary>
		/// <param name="deactivatingMenu"></param>
		/// <param name="activatingMenu"></param>
		public static void RefreshMenu(RadialMenu deactivatingMenu, RadialMenu activatingMenu)
		{
			deactivatingMenu?.gameObject.SetActive(false);
			activatingMenu?.gameObject.SetActive(true);
		}
		
		/// <summary>
		/// Adds a RadialMenu to the "RadialMenusHistory".
		/// </summary>
		/// <param name="radialMenu">The RadialMenu to add.</param>
		public static void AddToRadialMenuHistory(RadialMenu radialMenu)
		{
			RadialMenuHistory.Add(radialMenu);
		}

		/// <summary>
		/// Replaces RadialMenu at a specific index.
		/// </summary>
		/// <param name="radialMenu">The new RadialMenu to replace with at the specified index.</param>
		/// <param name="i">The specified index to replace at.</param>
		public static void ModifyRadialMenuHistory(RadialMenu radialMenu, int i)
		{
			RadialMenuHistory.RemoveAt(i);
			RadialMenuHistory.Insert(i, radialMenu);
		}

		/// <summary>
		/// Removes a RadialMenu from the "RadialMenusHistory".
		/// </summary>
		public static void RemoveFromRadialMenuHistory()
		{
			RadialMenuHistory.RemoveAt(RadialMenuHistory.Count - 1);
		}

		/// <summary>
		/// Removes a RadialMenu from the "RadialMenusHistory".
		/// </summary>
		/// <param name="i">The specified index to remove.</param>
		public static void RemoveFromRadialMenuHistory(int i)
		{
			RadialMenuHistory.RemoveAt(i);
		}

		/// <summary>
		/// Adds a RadialButton to the "RadialButtonHistory".
		/// </summary>
		/// <param name="radialButton">The RadialButton to add.</param>
		public static void AddRadialButtonHistory(RadialMenu.RadialButton radialButton)
		{
			RadialButtonHistory.Add(radialButton);
			UpdateChat();
		}

		/// <summary>
		/// Adds text to the "RadialButtonHistory".
		/// </summary>
		/// <param name="text">The text to add.</param>
		public static void AddRadialButtonHistory(string text)
		{
			RadialMenu.RadialButton radialButton = new RadialMenu.RadialButton(text);

			RadialButtonHistory.Add(radialButton);
			UpdateChat();
		}

		/// <summary>
		/// Inserts a RadialButton at a specified index to the "RadialButtonHistory".
		/// </summary>
		/// <param name="radialButton">The RadialButton to insert.</param>
		/// <param name="i">The index to insert the RadialButton at.</param>
		public static void InsertRadialButtonHistory(RadialMenu.RadialButton radialButton, int i)
		{
			RadialButtonHistory.Insert(i, radialButton);
			UpdateChat();
		}

		/// <summary>
		/// Inserts text at a specified index to the "RadialButtonHistory".
		/// </summary>
		/// <param name="text">The text to insert.</param>
		/// <param name="i">The index to insert the text at.</param>
		public static void InsertRadialButtonHistory(string text, int i)
		{
			RadialMenu.RadialButton radialButton = new RadialMenu.RadialButton(text);

			RadialButtonHistory.Insert(i, radialButton);
			UpdateChat();
		}

		/// <summary>
		/// Replaces a RadialButton at a specific index in the "RadialButtonHistory".
		/// </summary>
		/// <param name="radialButton">The new RadialButton to replace with at the specified index.</param>
		/// <param name="i">The index to replace.</param>
		public static void ModifyRadialButtonHistory(RadialMenu.RadialButton radialButton, int i)
		{
			RadialButtonHistory.RemoveAt(i);
			RadialButtonHistory.Insert(i, radialButton);
			UpdateChat();
		}

		/// <summary>
		/// Replaces text at a specific index in the "RadialButtonHistory"
		/// </summary>
		/// <param name="text">The text to replace at the specified index.</param>
		/// <param name="i">The index to replace.</param>
		public static void ModifyRadialButtonHistory(string text, int i)
		{
			RadialMenu.RadialButton radialButton = new RadialMenu.RadialButton(text);

			RadialButtonHistory.RemoveAt(i);
			RadialButtonHistory.Insert(i, radialButton);
			UpdateChat();
		}

		/// <summary>
		/// Removes a RadialButton from the "RadialButtonHistory".
		/// </summary>
		public static void RemoveRadialButtonHistory()
		{
			if (RadialButtonHistory.Count <= 0) return;

			RadialButtonHistory.RemoveAt(RadialButtonHistory.Count - 1);
			UpdateChat();
		}

		/// <summary>
		/// Removes a RadialButton at a specified index from the "RadialButtonHistory".
		/// </summary>
		/// <param name="i">The index where the RadialButton was removed.</param>
		public static void RemoveRadialButtonHistory(int i)
		{
			if (RadialButtonHistory.Count <= 0) return;

			RadialButtonHistory.RemoveAt(i);
			UpdateChat();
		}

		internal static void UpdateChat()
		{
			RadialMenuHUD.UpdateChatPreview(JoinChatText().CapitalizeFirstChar());
			RadialMenuHUD.UpdateChatRecentText(MostRecentText.CapitalizeFirstChar());
		}

		internal static string JoinChatText()
		{
			return RadialButtonHistory.Join(radialButton => radialButton.text, " ").Trim();
		}

		internal static void ResetChatText()
		{
			RadialButtonHistory.Clear();

			RadialMenuHUD.UpdateChatPreview(string.Empty);
			RadialMenuHUD.UpdateChatRecentText(string.Empty);

			SetCurrentMenu(MainMenu);
			RefreshMenu();

			RadialMenuHistory.Clear();
		}

		internal static void SendChatText()
		{
			char punctuation = (char)MostRecentButton.punctuation?.Invoke();
			string wholeText = JoinChatText();
			wholeText = wholeText.CapitalizeFirstChar() + punctuation;

			var localPlayer = GameNetworkManager.Instance.localPlayerController;
			HUDManager.Instance.AddTextToChatOnServer(wholeText, (int)localPlayer.playerClientId);

			RadialMenuHUD.ToggleRadialMenu(false);
		}
	}
}
