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
		internal static Camera RadialMenuHUDCamera;

		internal static Vector2 Center => new Vector2(Screen.width / 2, Screen.height / 2);

		internal static bool RadialMenuOpen = false;
		internal static bool RadialMenuLoaded = false;

		public static void Init()
		{
			Transform canvas = GameObject.Find("Systems").transform.Find("UI").transform.Find("Canvas");
			Transform uiCamera = GameObject.Find("Systems").transform.Find("UI").transform.Find("UICamera");

			RadialMenuHUDObject = new GameObject()
			{
				name = "QuickChat Container"
			};
			RadialMenuHUDObject.transform.SetParent(canvas, false);
			
			RectTransform rectTransform = RadialMenuHUDObject.AddComponent<RectTransform>();
			rectTransform.offsetMax = new Vector2(25, 75);
			rectTransform.offsetMin = new Vector2(0, 50);

			RadialMenuHUDCamera = uiCamera.GetComponent<Camera>();

			RadialMenuHUDObject.SetActive(false);
			RadialMenuLoaded = true;
		}

		public static void DeInit()
		{
			RadialMenuLoaded = false;
		}

		public static void ToggleRadialMenu(bool open, bool modifyInput = true)
		{
			if (!RadialMenuLoaded) return;
			if (!open) RadialMenuManager.ResetChatText();

			var localPlayer = GameNetworkManager.Instance.localPlayerController;

			RadialMenuHUDObject.SetActive(open);
			RadialMenuOpen = open;

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
			HUDManager.Instance.chatTextField.text = text == string.Empty ? "Press \"/\" to talk." : $"<color=white>{text}</color>";
			HUDManager.Instance.chatTextField.MoveToEndOfLine(false, false);

			HUDManager.Instance.PingHUDElement(HUDManager.Instance.Chat, 1f, 1f, 1f);
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
