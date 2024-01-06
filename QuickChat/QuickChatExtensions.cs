using QuickChat;
using System;
using System.Linq;

internal static partial class QuickChatExtensions
{
	// https://stackoverflow.com/a/26435053
	internal static string RemoveWhiteSpace(this string self)
	{
		return new string(self.Where(c => !char.IsWhiteSpace(c)).ToArray());
	}
}