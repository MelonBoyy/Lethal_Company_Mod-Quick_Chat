using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using HarmonyLib;

using TerminalApi;
using TerminalApi.Classes;
using TerminalApi.Events;
using static TerminalApi.TerminalApi;

namespace QuickChat
{
	[HarmonyPatch(typeof(Terminal))]
	internal class TerminalHandler
	{
		internal static TerminalNode ShortcutHomeNode => CreateTerminalNode($"Welcome to the Quick Chat Shortcuts Page or the Q.C.S.P. for very very long.\nHere you can currently only view your shortcuts.\n\n----------------------------\n\nConfig Type: {ShortcutHandler.GetConfigType()}\n\nShortcuts List:\n{ChatPatcher.GetChatShortcutsList()}");

		internal static TerminalNode CurrentNode => TerminalApi.TerminalApi.Terminal.currentNode;
		internal static Terminal CurrentTerminal => TerminalApi.TerminalApi.Terminal;

		internal static void Init()
		{
			AddCommand("qcview", new CommandInfo()
			{
				Title = "qcview",
				Description = "Display all shortcuts",
				Category = "Other",
				TriggerNode = ShortcutHomeNode,
				DisplayTextSupplier = () => { return ShortcutHomeNode.displayText; }
			}, clearPreviousText: true);

		}
	}
}
