using GameNetcodeStuff;
using HarmonyLib;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.InputSystem;

namespace QuickChat.RadialMenu
{
	public class RadialMenuInput
	{
		internal static InputAction RadialMenuToggleAction = new InputAction("Open", type: InputActionType.Button, binding: "<Keyboard>/alt");
		internal static InputAction RadialMenuCloseAction = new InputAction("Close", type: InputActionType.Button, binding: "<Mouse>/rightButton");

		internal static void Init()
		{
			RadialMenuToggleAction.Enable();
			RadialMenuCloseAction.Enable();

			RadialMenuToggleAction.performed += RadialMenuToggleAction_performed;
			RadialMenuCloseAction.performed += RadialMenuCloseAction_performed;

			Cursor.lockState = CursorLockMode.Locked;
		}

		internal static void DeInit()
		{
			RadialMenuToggleAction.Disable();
			RadialMenuCloseAction.Disable();

			RadialMenuToggleAction.performed -= RadialMenuToggleAction_performed;
			RadialMenuCloseAction.performed -= RadialMenuCloseAction_performed;
		}

		private static void RadialMenuToggleAction_performed(InputAction.CallbackContext ctx)
		{
			var localPlayer = GameNetworkManager.Instance.localPlayerController;
			if (localPlayer.isTypingChat || localPlayer.isPlayerDead || localPlayer.quickMenuManager.isMenuOpen) return;

			RadialMenuManager.RadialMenuOpen = !RadialMenuManager.RadialMenuOpen;

			RadialMenuHUD.ToggleRadialMenu(RadialMenuManager.RadialMenuOpen);
		}

		private static void RadialMenuCloseAction_performed(InputAction.CallbackContext ctx)
		{
			RadialMenuManager.GoBackMenu();
			RadialMenuManager.UpdateChat();
		}

		[HarmonyPatch(typeof(HUDManager), "CanPlayerScan")]
		[HarmonyPrefix]
		static bool PingScanBackMenuPatch()
		{
			if (RadialMenuManager.RadialMenuOpen) return false;

			return true;
		}
	}

}