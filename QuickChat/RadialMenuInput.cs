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
		internal static InputAction RadialMenuGoBackAction = new InputAction("Close", type: InputActionType.Button, binding: "<Mouse>/rightButton");

		public delegate void RadialMenuInputLoadingEvent();
		public static event RadialMenuInputLoadingEvent OnInputPreLoaded;
		public static event RadialMenuInputLoadingEvent OnInputPostLoaded;
		public static event RadialMenuInputLoadingEvent OnInputPreUnloaded;
		public static event RadialMenuInputLoadingEvent OnInputPostUnloaded;

		public delegate void RadialMenuInputEvent();
		public static event RadialMenuInputEvent OnInputGoBack;

		public delegate void RadialMenuToggleInputEvent(bool activated);
		public static event RadialMenuToggleInputEvent OnInputToggle;

		internal static void Init()
		{
			OnInputPreLoaded?.Invoke();

			RadialMenuConfig.UpdateBindings();

			RadialMenuToggleAction.Enable();
			RadialMenuGoBackAction.Enable();

			RadialMenuToggleAction.performed += RadialMenuToggleAction_performed;
			RadialMenuGoBackAction.performed += RadialMenuGoBackAction_performed;

			Cursor.lockState = CursorLockMode.Locked;

			OnInputPostLoaded?.Invoke();
		}

		internal static void DeInit()
		{
			OnInputPreUnloaded?.Invoke();

			RadialMenuToggleAction.Disable();
			RadialMenuGoBackAction.Disable();

			RadialMenuToggleAction.performed -= RadialMenuToggleAction_performed;
			RadialMenuGoBackAction.performed -= RadialMenuGoBackAction_performed;

			OnInputPostUnloaded?.Invoke();
		}

		private static void RadialMenuToggleAction_performed(InputAction.CallbackContext ctx)
		{
			var localPlayer = GameNetworkManager.Instance.localPlayerController;
			if (localPlayer.isTypingChat || localPlayer.isPlayerDead || localPlayer.quickMenuManager.isMenuOpen) return;

			RadialMenuManager.RadialMenuOpen = !RadialMenuManager.RadialMenuOpen;

			RadialMenuHUD.ToggleRadialMenu(RadialMenuManager.RadialMenuOpen);

			OnInputToggle?.Invoke(RadialMenuManager.RadialMenuOpen);
		}

		private static void RadialMenuGoBackAction_performed(InputAction.CallbackContext ctx)
		{
			RadialMenuManager.GoBackMenu();
			RadialMenuManager.UpdateChat();

			OnInputGoBack?.Invoke();
		}

		[HarmonyPatch(typeof(HUDManager), "CanPlayerScan")]
		[HarmonyPrefix]
		static bool PingScanRadialMenuOpenPatch()
		{
			if (RadialMenuManager.RadialMenuOpen) return false;

			return true;
		}

		[HarmonyPatch(typeof(PlayerControllerB), "CanUseItem")]
		[HarmonyPrefix]
		static bool ActivateItemRadialMenuOpenPatch()
		{
			if (RadialMenuManager.RadialMenuOpen) return false;

			return true;
		}
	}

}