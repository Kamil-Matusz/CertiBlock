using System.Text.RegularExpressions;

namespace CertiBlock.Shared.Helpers;

public static class EvmAddress
{
    private static readonly Regex Format = new("^0x[0-9a-fA-F]{40}$", RegexOptions.Compiled);

    public static bool IsValid(string? address) => address is not null && Format.IsMatch(address);
}