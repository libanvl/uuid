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
    /// The range of byted for the fifth record.
    /// </summary>
    public static readonly Range Data5_Range = new(10, 16);

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
    /// <paramref name="guid"/> data is first copied to a new array.
    /// </remarks>
    /// <param name="guid"></param>
    /// <exception cref="ArgumentException"></exception>
    public UUID(Guid guid)
        : this(guid.ToByteArray(), BitConverter.IsLittleEndian)
    {
    }

    /// <summary>
    /// Initialized an instance of <see cref="UUID"/>.
    /// </summary>
    /// <param name="hexString"></param>
    /// <exception cref="ArgumentException"></exception>
    public UUID(string hexString)
        : this(Convert.FromHexString(hexString))
    {
    }

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
    /// <param name="uuid"></param>
    public static implicit operator Guid(UUID uuid)
    {
        if (uuid.IsLittleEndian != BitConverter.IsLittleEndian)
        {
            uuid = uuid.EndianSwap();
        }

        return new Guid(uuid._data[UUID_Range].Span);
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
    /// Creates a new instance of Version V(5) <see cref="UUID"/>.
    /// </summary>
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

        byte[] hash = SHA1.HashData(namespacedName);
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

    /// <summary>
    /// The first record. Also called 'time_low'.
    /// </summary>
    public UInt32 Data1 => BitConverter.ToUInt32(_data[Data1_Range].Span);

    /// <summary>
    /// The second record. Also called 'time_mid'.
    /// </summary>
    public UInt16 Data2 => BitConverter.ToUInt16(_data[Data2_Range].Span);

    /// <summary>
    /// The third record. Also called 'time_hi_and_version'.
    /// </summary>
    public UInt16 Data3 => BitConverter.ToUInt16(_data[Data3_Range].Span);

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
    public override int GetHashCode() => HashCode.Combine(_data[UUID_Range].ToArray(), IsLittleEndian);

    /// <inheritdoc />
    public override string? ToString() => Convert.ToHexString(_data.ToArray()).ToLowerInvariant();

    private string GetDebuggerDisplay() => $"{_data}, IsLittleEndian={IsLittleEndian}";
}
