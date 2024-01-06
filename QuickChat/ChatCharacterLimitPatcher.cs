using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;

using HarmonyLib;
using TMPro;
using static UnityEngine.Scripting.GarbageCollector;

namespace QuickChat
{
	[HarmonyPatch(typeof(HUDManager))]
	internal class ChatCharacterLimitPatcher
	{
		public static int CharacterLimit = 9999;

		[HarmonyPatch("Start")]
		[HarmonyPostfix]
		static void CharacterLimitPatchTMP(ref TMP_InputField ___chatTextField)
		{
			___chatTextField.characterLimit = 0;
		}

		[HarmonyPatch("AddPlayerChatMessageServerRpc")]
		[HarmonyTranspiler]
		static IEnumerable<CodeInstruction> CharacterLimitPatchServerRPC(IEnumerable<CodeInstruction> instructions)
		{
			List<CodeInstruction> code = new List<CodeInstruction>(instructions);

			code = FindAndReplaceCharacterLimit(code);

			return code.AsEnumerable();
		}

		[HarmonyPatch("SubmitChat_performed")]
		[HarmonyTranspiler]
		static IEnumerable<CodeInstruction> CharacterLimitPatchSubmitChat(IEnumerable<CodeInstruction> instructions)
		{
			List<CodeInstruction> code = new List<CodeInstruction>(instructions);

			code = FindAndReplaceCharacterLimit(code);

			return code.AsEnumerable();
		}

		static List<CodeInstruction> FindAndReplaceCharacterLimit(List<CodeInstruction> code)
		{
			// Thank you albinogeek (https://github.com/AlbinoGeek), ayteeate, lordfirespeed, and JavidPack (https://gist.github.com/JavidPack/454477b67db8b017cb101371a8c49a1c) for helping me! 
			for (int i = 0; i < code.Count - 1; i++)
			{
				Plugin.LogSource.LogInfo($"{code[i].opcode} .. {code[i].operand}");

				if (code[i].opcode == OpCodes.Callvirt && code[i].operand.ToString() == "Int32 get_Length()" && code[i + 1].opcode == OpCodes.Ldc_I4_S)
				{
					code[i + 1].opcode = OpCodes.Ldc_I4;
					code[i + 1].operand = CharacterLimit;

					Plugin.LogSource.LogInfo($"{code[i + 1].opcode} .. {code[i + 1].operand} CHANGED");
					break;
				}
			}

			return code;
		}

	}


}
