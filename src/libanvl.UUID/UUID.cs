using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;

namespace libanvl;

/// <summary>
/// An immutable Universally Unique ID.
/// </summary>
[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public readonly struct UUID : IEquatable<UUID>
{
    private static readonly Range UUID_Range = new(0, 16);
    private static readonly byte[] _nil = new byte[16];

    private readonly ReadOnlyMemory<byte> _data;

    /// <summary>
    /// The range of bytes for the first record.
    /// </summary>
    public static readonly Range Data1_Range = new(0, 4);

    /// <summary>
    /// The range of bytes for the second record.
    /// </summary>
    public static readonly Range Data2_Range = new(4, 6);

    /// <summary>
    /// The range of bytes for the third record.
    /// </summary>
    public static readonly Range Data3_Range = new(6, 8);

    /// <summary>
    /// The range of bytes for the fourth record.
    /// </summary>
    public static readonly Range Data4_Range = new(8, 10);

    /// <summary>
    /// The range of bytes for the fifth record.
    /// </summary>
    public static readonly Range Data5_Range = new(10, 16);

    /// <summary>
    /// Known UUID namespaces.
    /// </summary>
    public static class KnownNamespaces
    {
        /// <summary>
        /// The DNS namespace. The name is a fully-qualified domain name.
        /// </summary>
        public static readonly UUID DNS = new(
            0x6ba7b810,
            0x9dad,
            0x11d1,
            new byte[] { 0x80, 0xb4 },
            new byte[] { 0x00, 0xc0, 0x4f },
            isLittleEndian: false);

        /// <summary>
        /// The URL namespace. The name is a URL.
        /// </summary>
        public static readonly UUID URL = new(
            0x6ba7b811,
            0x9dad,
            0x11d1,
            new byte[] { 0x80, 0xb4 },
            new byte[] { 0x00, 0xc0, 0x4f },
            isLittleEndian: false);

        /// <summary>
        /// The ISO Object ID namespace. The name is an ISO OID.
        /// </summary>
        public static readonly UUID OID = new(
            0x6ba7b812,
            0x9dad,
            0x11d1,
            new byte[] { 0x80, 0xb4 },
            new byte[] { 0x00, 0xc0, 0x4f },
            isLittleEndian: false);

        /// <summary>
        /// The X.500 namespace. The name is an X.500 DN.
        /// </summary>
        public static readonly UUID X500 = new(
            0x6ba7b814,
            0x9dad,
            0x11d1,
            new byte[] { 0x80, 0xb4 },
            new byte[] { 0x00, 0xc0, 0x4f },
            isLittleEndian: false);
    }

    /// <summary>
    /// Initializes an instance of the Nil <see cref="UUID"/>.
    /// </summary>
    public UUID()
    {
        _data = _nil;
        IsLittleEndian = BitConverter.IsLittleEndian;
    }

    /// <summary>
    /// Initializes an instance of <see cref="UUID"/>.
    /// </summary>
    /// <param name="data1"></param>
    /// <param name="data2"></param>
    /// <param name="data3"></param>
    /// <param name="data4"></param>
    /// <param name="data5"></param>
    /// <param name="isLittleEndian">Whether the data provided for <paramref name="data1"/>, <paramref name="data2"/> and <paramref name="data3"/> has little-endian byte order.</param>
    /// <exception cref="ArgumentException"></exception>
    public UUID(UInt32 data1, UInt16 data2, UInt16 data3, ReadOnlyMemory<byte> data4, ReadOnlyMemory<byte> data5, bool isLittleEndian)
    {
        if (data4.Length != 2)
        {
            throw new ArgumentException($"{nameof(data4)} must have Length of 2", nameof(data4));
        }

        if (data5.Length != 6)
        {
            throw new ArgumentException($"{nameof(data5)} must have Length of 6", nameof(data5));
        }

        _data = BitConverter.GetBytes(data1)
            .Concat(BitConverter.GetBytes(data2))
            .Concat(BitConverter.GetBytes(data3))
            .Concat(data4.ToArray())
            .Concat(data5.ToArray())
            .ToArray();

        IsLittleEndian = isLittleEndian;
    }

    /// <summary>
    /// Initializes an instance of <see cref="UUID"/>.
    /// Assumes <paramref name="data1"/>, <paramref name="data2"/> and <paramref name="data3"/> have platform byte order.
    /// </summary>
    /// <param name="data1"></param>
    /// <param name="data2"></param>
    /// <param name="data3"></param>
    /// <param name="data4"></param>
    /// <param name="data5"></param>
    /// <exception cref="ArgumentException"></exception>
    public UUID(UInt32 data1, UInt16 data2, UInt16 data3, ReadOnlyMemory<byte> data4, ReadOnlyMemory<byte> data5)
        : this(data1, data2, data3, data4, data5, BitConverter.IsLittleEndian)
    {
    }

    /// <summary>
    /// Initializes an instance of <see cref="UUID"/>.
    /// </summary>
    /// <param name="data"></param>
    /// <param name="isLittleEndian">Whether the data provided for the first 8 bytes have little-endian byte order.</param>
    /// <exception cref="ArgumentException"></exception>
    public UUID(ReadOnlyMemory<byte> data, bool isLittleEndian)
    {
        if (data.Length < 16)
        {
            throw new ArgumentException($"{nameof(data)} must have Length of at least 16", nameof(data));
        }

        _data = data;
        IsLittleEndian = isLittleEndian;
    }

    /// <summary>
    /// Initializes an instance of <see cref="UUID"/>.
    /// Assumes the first 8 bytes have platform byte order.
    /// </summary>
    /// <param name="data"></param>
    /// <exception cref="ArgumentException"></exception>
    public UUID(ReadOnlyMemory<byte> data)
        : this(data, BitConverter.IsLittleEndian)
    {
    }

    /// <summary>
    /// Initializes an instance of <see cref="UUID"/>.
    /// Assumes the <paramref name="guid"/> has platform byte order.
    /// </summary>
    /// <remarks>
    /// <para>
    ///     <paramref name="guid"/> data is first copied to a new array.
    /// </para>
    /// </remarks>
    /// <param name="guid"></param>
    /// <exception cref="ArgumentException"></exception>
    public UUID(Guid guid)
        : this(guid.ToByteArray(), BitConverter.IsLittleEndian)
    {
    }

#if NET
    /// <summary>
    /// Initialized an instance of <see cref="UUID"/>.
    /// </summary>
    /// <param name="hexString"></param>
    /// <exception cref="ArgumentException"></exception>
    public UUID(string hexString)
        : this(Convert.FromHexString(hexString))
    {
    }
#endif

    /// <summary>
    /// Copies the <paramref name="source"/>, optionally swapping the endianess.
    /// </summary>
    /// <param name="source">The source <see cref="UUID"/></param>
    /// <param name="endianSwap">Whether to perform an endian swap</param>
    public UUID(UUID source, bool endianSwap)
        : this(
               endianSwap ? source.Data1.Swap() : source.Data1,
               endianSwap ? source.Data2.Swap() : source.Data2,
               endianSwap ? source.Data3.Swap() : source.Data3,
               source.Data4,
               source.Data5,
               endianSwap ? !source.IsLittleEndian : source.IsLittleEndian)
    {
    }

    /// <summary>
    /// Copies the <paramref name="source"/>, with the same endianess.
    /// </summary>
    /// <param name="source">The source <see cref="UUID"/></param>
    public UUID(UUID source)
        : this(source, endianSwap: false)
    {
    }

    /// <summary>
    /// Implicit conversion to <see cref="Guid"/>. The resulting <see cref="Guid"/> will
    /// have platform byte order.
    /// </summary>
    /// <remarks>
    /// <para>
    ///     For netstandard2.0, an additional allocation is made to copy the data to a new array.
    /// </para>
    /// </remarks>
    /// <param name="uuid"></param>
    public static implicit operator Guid(UUID uuid)
    {
        if (uuid.IsLittleEndian != BitConverter.IsLittleEndian)
        {
            uuid = uuid.EndianSwap();
        }

#if NET
        return new Guid(uuid._data[UUID_Range].Span);
#else
        return new Guid(uuid._data[UUID_Range].ToArray());
#endif
    }

    /// <summary>
    /// Implicit conversion from <see cref="Guid"/>.
    /// </summary>
    /// <param name="guid"></param>
    public static implicit operator UUID(Guid guid) => new(guid);

    /// <summary>
    /// Implicit conversion to <see cref="ReadOnlyMemory{Byte}"/>.
    /// </summary>
    /// <param name="uuid"></param>
    public static implicit operator ReadOnlyMemory<byte>(UUID uuid) => uuid._data[UUID_Range];

    /// <inheritdoc />
    public static bool operator ==(UUID left, UUID right) => left.Equals(right);

    /// <inheritdoc />
    public static bool operator !=(UUID left, UUID right) => !(left == right);

    /// <summary>
    /// Creates a new instance of the Nil <see cref="UUID"/>.
    /// </summary>
    public static UUID Nil => new();

    /// <summary>
    /// Creates a new instance of the Max <see cref="UUID"/>, per RFC 9562.
    /// </summary>
    public static UUID Max => new(
        0xFFFFFFFF,
        0xFFFF,
        0xFFFF,
        new byte[] { 0xFF, 0xFF },
        new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF });

    /// <summary>
    /// Creates a new instance of Version 8 <see cref="UUID"/> with custom data.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Per the draft RFC, Version 8 UUIDs are generated using custom data.
    /// </para>
    /// </remarks>
    /// <param name="customData">The custom data to include in the UUID. Must be 16 bytes.</param>
    /// <exception cref="ArgumentException"></exception>
    public static UUID VIII(ReadOnlySpan<byte> customData)
    {
        if (customData.Length != 16)
        {
            throw new ArgumentException($"{nameof(customData)} must have Length of 16", nameof(customData));
        }

        var result = customData.ToArray();

        // Set the version to 8
        result[6] = (byte)((result[6] & 0x0F) | 0x80);

        // Set the variant to RFC 4122
        result[8] = (byte)((result[8] & 0x3F) | 0x80);

        return new UUID(result, BitConverter.IsLittleEndian);
    }

    /// <summary>
    /// Creates a new instance of Version 7 <see cref="UUID"/>.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Per RFC 9562, Version 7 UUIDs are generated using a timestamp based on Unix time.
    /// </para>
    /// </remarks>
    public static UUID VII()
    {
        var timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        var randomBytes = new byte[10];
#if NET
        RandomNumberGenerator.Fill(randomBytes);
#else
        RandomNumberGenerator.Create().GetBytes(randomBytes);
#endif
        var result = new byte[16];
        Array.Copy(BitConverter.GetBytes(timestamp), 0, result, 0, 6);
        Array.Copy(randomBytes, 0, result, 6, 10);

        // Set the version to 7
        result[6] = (byte)((result[6] & 0x0F) | 0x70);

        // Set the variant to RFC 4122
        result[8] = (byte)((result[8] & 0x3F) | 0x80);

        return new UUID(result, BitConverter.IsLittleEndian);
    }

    /// <summary>
    /// Creates a new instance of Version V(5) <see cref="UUID"/>.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Per RFC 4122, Section 4.3:
    /// Version 5 UUIDs are generated using a SHA-1 hash of a namespace <see cref="UUID"/> and a name.
    ///     * The UUIDs generated at different times from the same name in the same namespace MUST be equal.
    ///     * The UUIDs generated from two different names in the same namespace should be different(with very high probability).
    ///     * The UUIDs generated from the same name in two different namespaces should be different with(very high probability).
    ///     * If two UUIDs that were generated from names are equal, then they were generated from the same name in the same namespace (with very high probability).
    /// </para>
    /// </remarks>
    /// <param name="namespace">The namespace <see cref="UUID"/></param>
    /// <param name="name">The name</param>
    public static UUID V(UUID @namespace, string name)
    {
        // ensure network byte order (big endian)
        if (@namespace.IsLittleEndian)
        {
            @namespace = @namespace.EndianSwap();
        }

        var namespacedName = @namespace._data
            .ToArray()
            .Concat(Encoding.Unicode.GetBytes(name))
            .ToArray();

        byte[] hash = GetHashData(namespacedName);
        byte[] result = new byte[16];

        //Copy first 16-bytes of the hash into our future Guid result
        Array.Copy(hash, result, 16);

        //set high-nibble to 5 to indicate type 5
        result[6] &= 0x0F;
        result[6] |= 0x50;

        //set upper two bits to 2 for native variant
        result[8] &= 0x3F;
        result[8] |= 0x80;

        return new UUID(result, @namespace.IsLittleEndian);
    }

    /// <summary>
    /// Creates a new instance of a Version IV(4) <see cref="UUID"/>.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Per RFC 4122, Section 4.4: Version 4 UUIDs are pseudo-random.
    /// </para>
    /// </remarks>
    public static UUID IV()
    {
        byte[] result = Guid.NewGuid().ToByteArray();

        //set high-nibble to 4 to indicate type 4
        result[6] &= 0x0F;
        result[6] |= 0x40;

        //set upper two bits to 2 for native variant
        result[8] &= 0x3F;
        result[8] |= 0x80;

        return new UUID(result);
    }

    private static byte[] GetHashData(byte[] bytes)
    {
#if NET
        return SHA1.HashData(bytes);
#else
        using var sha1 = SHA1.Create();
        return sha1.ComputeHash(bytes);
#endif
    }

    private static string GetHexString(ReadOnlyMemory<byte> data)
    {
#if NET
        return Convert.ToHexString(data.ToArray()).ToLowerInvariant();
#else
        return BitConverter.ToString(data.ToArray()).Replace("-", "").ToLowerInvariant();
#endif
    }

    /// <summary>
    /// The first record. Also called 'time_low'.
    /// </summary>
    public UInt32 Data1 =>
#if NET
        BitConverter.ToUInt32(_data[Data1_Range].Span);
#else

        BitConverter.ToUInt32(_data[Data1_Range].ToArray(), 0);
#endif

    /// <summary>
    /// The second record. Also called 'time_mid'.
    /// </summary>
    public UInt16 Data2 =>
#if NET
        BitConverter.ToUInt16(_data[Data2_Range].Span);
#else
        BitConverter.ToUInt16(_data[Data2_Range].ToArray(), 0);
#endif

    /// <summary>
    /// The third record. Also called 'time_hi_and_version'.
    /// </summary>
    public UInt16 Data3 =>
#if NET
        BitConverter.ToUInt16(_data[Data3_Range].Span);
#else
        BitConverter.ToUInt16(_data[Data3_Range].ToArray(), 0);
#endif

    /// <summary>
    /// The fourth record. Also called 'clock_seq_hi_and_res_clock_seq_low'.
    /// </summary>
    /// <remarks>
    /// These are uninterpreted bytes, not subject to swapping.
    /// </remarks>
    public ReadOnlyMemory<byte> Data4 => _data[Data4_Range];

    /// <summary>
    /// The fifth record. Also called 'node'.
    /// </summary>
    /// <remarks>
    /// These are uninterpreted bytes, not subject to swapping.
    /// </remarks>
    public ReadOnlyMemory<byte> Data5 => _data[Data5_Range];

    /// <summary>
    /// Gets a value indicating whether the stored data has little endian byte order.
    /// </summary>
    public bool IsLittleEndian { get; }

    /// <summary>
    /// Gets a value indicating whether the stored data has big endian byte order.
    /// </summary>
    public bool IsNetworkByteOrder => !IsLittleEndian;

    /// <summary>
    /// Gets a value indicating whether the stored data has platform byte order.
    /// </summary>
    public bool IsPlatformEndian => IsLittleEndian == BitConverter.IsLittleEndian;

    /// <summary>
    /// Creates a new <see cref="UUID"/> with the byte order swapped.
    /// </summary>
    public UUID EndianSwap() => new(this, endianSwap: true);

    /// <summary>
    /// Copies the data to a new array.
    /// </summary>
    public byte[] ToByteArray() => _data[UUID_Range].ToArray();

    /// <inheritdoc />
    public ReadOnlySpan<byte>.Enumerator GetEnumerator() => _data[UUID_Range].Span.GetEnumerator();

    /// <inheritdoc />
    public override bool Equals(object? obj) => obj is UUID uUID && Equals(uUID);

    /// <inheritdoc />
    public bool Equals(UUID other)
    {
        return _data[..16].Span.SequenceEqual(other._data[..16].Span)
            && IsLittleEndian == other.IsLittleEndian;
    }

    /// <inheritdoc />
    public override int GetHashCode() =>
#if NET
        HashCode.Combine(_data[UUID_Range], IsLittleEndian);
#else
        _data[UUID_Range].GetHashCode() ^ IsLittleEndian.GetHashCode();
#endif

    /// <inheritdoc />
    public override string? ToString() => GetHexString(_data);

    private string GetDebuggerDisplay() => $"{_data}, IsLittleEndian={IsLittleEndian}";
}
