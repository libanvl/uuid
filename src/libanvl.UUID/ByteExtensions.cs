namespace libanvl;

/// <summary>
/// Some useful swapping extensions
/// </summary>
public static class ByteExtensions
{
    /// <summary>
    /// Swaps the byte-order
    /// </summary>
    public static UInt16 Swap(this UInt16 value)
    {
        return (UInt16)((value & 0xFFU) << 8 | (value & 0xFF00U) >> 8);
    }

    /// <summary>
    /// Swaps the byte-order
    /// </summary>
    public static UInt32 Swap(this UInt32 value)
    {
        return (value & 0x000000FFU) << 24 | (value & 0x0000FF00U) << 8 |
               (value & 0x00FF0000U) >> 8 | (value & 0xFF000000U) >> 24;
    }

    /// <summary>
    /// Swaps the byte-order
    /// </summary>
    public static UInt64 Swap(this UInt64 value)
    {
        return (value & 0x00000000000000FFUL) << 56 | (value & 0x000000000000FF00UL) << 40 |
               (value & 0x0000000000FF0000UL) << 24 | (value & 0x00000000FF000000UL) << 8 |
               (value & 0x000000FF00000000UL) >> 8 | (value & 0x0000FF0000000000UL) >> 24 |
               (value & 0x00FF000000000000UL) >> 40 | (value & 0xFF00000000000000UL) >> 56;
    }
}
