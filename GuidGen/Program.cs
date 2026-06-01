using System;
using System.Security.Cryptography;
using System.Text;

class Program
{
    static void Main()
    {
        Console.WriteLine($"Venue 1: {StableGuid("venue:1")}");
        Console.WriteLine($"Field 1: {StableGuid("field:1")}");
        for(int i = 1; i <= 5; i++)
            Console.WriteLine($"Slot {i}: {StableGuid("slot:" + i)}");
    }

    private static Guid StableGuid(string key)
    {
        var bytes = MD5.HashData(Encoding.UTF8.GetBytes($"CourtManager.SampleData:{key}"));
        bytes[6] = (byte)((bytes[6] & 0x0F) | 0x50);
        bytes[8] = (byte)((bytes[8] & 0x3F) | 0x80);
        return new Guid(bytes);
    }
}
