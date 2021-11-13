using System;
using Xunit;

namespace libanvl.Test;

public class UUIDTests
{
    public static readonly Guid TerminalNamespace = new("2BDE4A90-D05F-401C-9492-E40884EAD1D8");
    public static readonly Guid UbuntuExpected    = new("2C4DE342-38B7-51CF-B940-2309A097F518");

    [Fact]
    public void UUID_V_Ubuntu_Profile()
    {
        var ubuntuGenerated = UUID.V(TerminalNamespace, "Ubuntu");
        Assert.Equal<Guid>(UbuntuExpected, ubuntuGenerated);
    }

    [Fact]
    public void UUID_V_Is_BigEndian()
    {
        var x = UUID.V(TerminalNamespace, "Test");
        Assert.False(x.IsLittleEndian);
    }

    [Fact]
    public void Double_Swap_Is_Equal()
    {
        var a = UUID.IV();
        Assert.Equal(a, a.EndianSwap().EndianSwap());
    }

    [Fact]
    public void Default_Endianess_Is_Platform()
    {
        var x = new UUID(Guid.NewGuid());
        Assert.Equal(BitConverter.IsLittleEndian, x.IsLittleEndian);
    }

    [Fact]
    public void DefaultCtor_Same_As_Nil()
    {
        var x = new UUID();
        Assert.Equal(x, UUID.Nil);
    }

    [Fact]
    public void Double_Guid_Conversion_Is_Equal()
    {
        var a = Guid.NewGuid();
        UUID x = a;
        Guid y = x;
        Assert.Equal(a, y);
    }

    [Fact]
    public void Converted_Guid_Has_Platform_Endianess()
    {
        UUID x = Guid.NewGuid();
        Assert.Equal(BitConverter.IsLittleEndian, x.IsLittleEndian);
    }

    [Fact]
    public void Data4_Has_Length_2()
    {
        UUID x = UUID.IV();
        Assert.Equal(2, x.Data4.Length);
    }

    [Fact]
    public void Data5_Has_Length_6()
    {
        UUID x = UUID.IV();
        Assert.Equal(6, x.Data5.Length);
    }

    [Fact]
    public void VersionIV_Are_Not_Equal()
    {
        Assert.NotEqual(UUID.IV(), UUID.IV());
    }

    [Fact]
    public void Data4_Is_Same_Memory()
    {
        UUID a = UUID.IV();
        ReadOnlyMemory<byte> b = a;
        Assert.Equal(b[8..10], a.Data4);
    }

    [Fact]
    public void Data5_Is_Same_Memory()
    {
        UUID a = UUID.IV();
        ReadOnlyMemory<byte> b = a;
        Assert.Equal(b[10..], a.Data5);
    }

    [Fact]
    public void ByteArray_Has_Length_16()
    {
        var a = UUID.IV();
        Assert.Equal(16, a.ToByteArray().Length);
    }

    [Fact]
    public void Can_String_And_Back()
    {
        var a = UUID.IV();
        var a_string = a.ToString();

        var b = new UUID(Convert.FromHexString(a_string));
        Assert.Equal(a, b);
    }

    [Fact]
    public void LargerMemory_DoesNotThrow()
    {
        var a = new ReadOnlyMemory<byte>(new byte[18]);
        var x = new UUID(a);
    }

    [Fact]
    public void SmallerMemory_Throws()
    {
        var a = new ReadOnlyMemory<byte>(new byte[15]);
        Assert.Throws<ArgumentException>(() => new UUID(a));
    }
}
