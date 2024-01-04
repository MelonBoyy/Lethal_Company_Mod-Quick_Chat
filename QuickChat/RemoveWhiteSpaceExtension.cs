using System;
using System.Linq;

// https://stackoverflow.com/a/26435053
internal static partial class RemoveWhiteSpaceExtension
{
	public static string RemoveWhiteSpace(this string self)
	{
		return new string(self.Where(c => !char.IsWhiteSpace(c)).ToArray());
	}
}