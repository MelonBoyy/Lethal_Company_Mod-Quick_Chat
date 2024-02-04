using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using UnityEngine.UI;

using TMPro;
using HarmonyLib;
using UnityEngine.EventSystems;
using GameNetcodeStuff;
using UnityEngine.InputSystem;

namespace QuickChat.RadialMenu
{
	public class RadialMenuHUD
	{
		internal static GameObject RadialMenuHUDObject;
		internal static Image RadialMenuHUDBackground;
		internal static TextMeshProUGUI RadialMenuHUDRecentText;
		internal static Camera RadialMenuHUDCamera;

		internal static Vector2 Center => new Vector2(Screen.width / 2, Screen.height / 2);

		internal static void Init()
		{
			Transform canvas = GameObject.Find("Systems").transform.Find("UI").transform.Find("Canvas");
			Transform uiCamera = GameObject.Find("Systems").transform.Find("UI").transform.Find("UICamera");

			RadialMenuHUDCamera = uiCamera.GetComponent<Camera>();

			RadialMenuHUDObject = new GameObject()
			{
				name = "QuickChat Container"
			};
			RadialMenuHUDObject.transform.SetParent(canvas, false);

			RectTransform rectTransform = RadialMenuHUDObject.AddComponent<RectTransform>();
			rectTransform.offsetMax = new Vector2(25, 75);
			rectTransform.offsetMin = new Vector2(0, 50);

			RadialMenuHUDBackground = new GameObject()
			{
				name = "QuickChat Background"
			}.AddComponent<Image>();
			RadialMenuHUDBackground.transform.SetParent(RadialMenuHUDObject.transform, false);
			RadialMenuHUDBackground.transform.SetSiblingIndex(0);

			RadialMenuHUDBackground.color = new Color32(0, 0, 0, (byte)RadialMenuConfig.QuickChatRadialMenuBackgroundAlpha.Value);

			RadialMenuHUDBackground.rectTransform.anchoredPosition = RadialMenuHUDCamera.ScreenToWorldPoint(Center);
			RadialMenuHUDBackground.rectTransform.sizeDelta = new Vector2(Screen.width, Screen.height);

			RadialMenuHUDRecentText = new GameObject()
			{
				name = "Most Recent Text"
			}.AddComponent<TextMeshProUGUI>();
			RadialMenuHUDRecentText.transform.SetParent(RadialMenuHUDObject.transform, false);
			RadialMenuHUDRecentText.alignment = TextAlignmentOptions.Center;

			RadialMenuHUDRecentText.enableWordWrapping = true;
			RadialMenuHUDRecentText.enableAutoSizing = true;
			RadialMenuHUDRecentText.fontSizeMin = RadialMenuConfig.QuickChatRadialMenuRecentTextMinSize.Value;
			RadialMenuHUDRecentText.fontSizeMax = RadialMenuConfig.QuickChatRadialMenuRecentTextMaxSize.Value;

			RadialMenuHUDRecentText.rectTransform.sizeDelta = new Vector2(350, 350);
			RadialMenuHUDRecentText.rectTransform.anchoredPosition = RadialMenuHUDCamera.ScreenToWorldPoint(Center);

			RadialMenuHUDObject.SetActive(false);
			RadialMenuManager.RadialMenuLoaded = true;
		}

		public static void DeInit()
		{
			RadialMenuManager.RadialMenuLoaded = false;
		}

		public static void ToggleRadialMenu(bool open, bool modifyInput = true)
		{
			if (!RadialMenuManager.RadialMenuLoaded) return;
			if (!open) RadialMenuManager.ResetChatText();

			var localPlayer = GameNetworkManager.Instance.localPlayerController;

			RadialMenuHUDObject.SetActive(open);
			RadialMenuManager.RadialMenuOpen = open;

			if (modifyInput)
			{
				Cursor.visible = open;
				Cursor.lockState = open ? CursorLockMode.None : CursorLockMode.Locked;
				localPlayer.disableLookInput = open;
			}

			HUDManager.Instance.chatTextField.textComponent.enableWordWrapping = !open;
		}

		public static void UpdateChatPreview(string text)
		{
			HUDManager.Instance.chatTextField.text = string.Empty;
			HUDManager.Instance.chatTextField.MoveToEndOfLine(false, false);

			HUDManager.Instance.PingHUDElement(HUDManager.Instance.Chat, 1f, 1f, 1f);
		}

		public static void UpdateChatRecentText(string text)
		{
			RadialMenuHUDRecentText.text = text;
		}

		[HarmonyPatch(typeof(QuickMenuManager), nameof(QuickMenuManager.OpenQuickMenu))]
		[HarmonyPostfix]
		static void OpenQuickMenuCloseRadialMenuPatch()
		{
			ToggleRadialMenu(false, false);
		}

		[HarmonyPatch(typeof(QuickMenuManager), nameof(QuickMenuManager.CloseQuickMenu))]
		[HarmonyPostfix]
		static void CloseQuickMenuFixLookInputPatch()
		{
			var localPlayer = GameNetworkManager.Instance.localPlayerController;

			localPlayer.disableLookInput = false;
		}

		[HarmonyPatch(typeof(PlayerControllerB), nameof(PlayerControllerB.KillPlayer))]
		[HarmonyPostfix]
		static void PlayerControllerBKillPlayerCloseRadialMenuPatch()
		{
			ToggleRadialMenu(false, false);
		}
	}
}
