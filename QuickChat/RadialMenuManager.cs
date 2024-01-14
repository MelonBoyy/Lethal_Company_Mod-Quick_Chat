using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using UnityEngine.UI;

namespace QuickChat.RadialMenu
{
	public class RadialMenuManager
	{
		public static List<RadialMenu> RadialMenuRegistry { get; private set; } = new List<RadialMenu>();
		public static List<RadialMenu> RadialMenusHistory { get; private set; } = new List<RadialMenu>();
		public static RadialMenu CurrentMenu { get; private set; }
		public static RadialMenu LastMenu => RadialMenusHistory.Count > 0 ? RadialMenusHistory.Last() : null;

		public static RadialMenu MainMenu { get; private set; } = new RadialMenu()
		{
			name = "MainMenu"
		};

		public static List<string> ChatText { get; private set; } = new List<string>();
		public static string MostRecentText => ChatText[ChatText.Count - 1];

		public static bool FirstTimeInMenu = true;

		public delegate void RadialMenuReady();
		public static event RadialMenuReady OnRadialMenuReady;

		[HarmonyPatch(typeof(RoundManager), "Awake")]
		[HarmonyPostfix]
		static void AwakePatch()
		{
			RadialMenuHUD.Init();
			RadialMenuInput.Init();

			RegisterRadialMenu(MainMenu);

			foreach (RadialMenu menu in RadialMenuRegistry)
			{
				menu.CreateRadialMenu(RadialMenuHUD.RadialMenuHUDObject.transform);
			}

			SetCurrentMenu(MainMenu);
			RefreshMenu();

			OnRadialMenuReady?.Invoke();
			FirstTimeInMenu = false;
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

		public static void RegisterRadialMenu(RadialMenu radialMenu)
		{
			RadialMenuRegistry.Add(radialMenu);
		}

		public static void SetCurrentMenu(RadialMenu menu, bool saveLastToHistory = true)
		{
			if (CurrentMenu == menu) return;

			if (CurrentMenu != null && saveLastToHistory) RadialMenusHistory.Add(CurrentMenu);
			CurrentMenu = menu;
		}

		internal static void GoBackMenu()
		{
			if (RadialMenusHistory.Count <= 0) return;

			RadialMenu newCurrentMenu = LastMenu;
			RadialMenu oldMenu = CurrentMenu;

			RemoveChatText();

			SetCurrentMenu(newCurrentMenu, false);
			RefreshMenu(oldMenu, newCurrentMenu);

			RemoveFromHistory();
		}

		internal static void RefreshMenu()
		{
			LastMenu?.gameObject.SetActive(false);
			CurrentMenu?.gameObject.SetActive(true);
		}

		internal static void RefreshMenu(RadialMenu deactivatingMenu, RadialMenu activatingMenu)
		{
			deactivatingMenu?.gameObject.SetActive(false);
			activatingMenu?.gameObject.SetActive(true);
		}

		internal static void AddToHistory(RadialMenu radialMenu)
		{
			RadialMenusHistory.Add(radialMenu);
		}

		internal static void RemoveFromHistory()
		{
			RadialMenusHistory.RemoveAt(RadialMenusHistory.Count - 1);
		}

		internal static void AddChatText(string text)
		{
			ChatText.Add(text.Trim());
			RadialMenuHUD.UpdateChatPreview(JoinChatText());
		}

		internal static void AddChatText(string text, int i)
		{
			ChatText.Insert(i, text.Trim());
			RadialMenuHUD.UpdateChatPreview(JoinChatText());
		}

		internal static void ModifyChatText(string text, int i)
		{
			ChatText.RemoveAt(i);
			ChatText.Insert(i, text.Trim());
			RadialMenuHUD.UpdateChatPreview(JoinChatText());
		}

		internal static void RemoveChatText()
		{
			if (ChatText.Count <= 0) return;

			ChatText.RemoveAt(ChatText.Count - 1);
			RadialMenuHUD.UpdateChatPreview(JoinChatText());
		}

		internal static void RemoveChatText(int i)
		{
			if (ChatText.Count <= 0) return;

			ChatText.RemoveAt(i);
			RadialMenuHUD.UpdateChatPreview(JoinChatText());
		}

		internal static string JoinChatText()
		{
			return string.Join(" ", ChatText).Trim();
		}

		internal static void ResetChatText()
		{
			ChatText.Clear();

			RadialMenuHUD.UpdateChatPreview(string.Empty);

			SetCurrentMenu(MainMenu);
			RefreshMenu();

			RadialMenusHistory.Clear();
		}

		internal static void SendChatText()
		{
			var localPlayer = GameNetworkManager.Instance.localPlayerController;
			HUDManager.Instance.AddTextToChatOnServer(JoinChatText(), (int)localPlayer.playerClientId);

			RadialMenuHUD.ToggleRadialMenu(false);
		}
	}
}
