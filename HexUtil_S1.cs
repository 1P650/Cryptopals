using System;
using System.Collections.Generic;
using System.Text;

public static class HexUtil_S1
{
    private readonly static Dictionary<char, byte> hexmap = new Dictionary<char, byte>()
    {
        { 'a', 0xA },{ 'b', 0xB },{ 'c', 0xC },{ 'd', 0xD },
        { 'e', 0xE },{ 'f', 0xF },{ 'A', 0xA },{ 'B', 0xB },
        { 'C', 0xC },{ 'D', 0xD },{ 'E', 0xE },{ 'F', 0xF },
        { '0', 0x0 },{ '1', 0x1 },{ '2', 0x2 },{ '3', 0x3 },
        { '4', 0x4 },{ '5', 0x5 },{ '6', 0x6 },{ '7', 0x7 },
        { '8', 0x8 },{ '9', 0x9 }
    };
    public static byte[] HexStrToByteArray( string hex)
    {
        bool startsWithHexStart = hex.StartsWith("0x", StringComparison.OrdinalIgnoreCase);


        int startIndex = startsWithHexStart ? 2 : 0;

        byte[] bytesArr = new byte[(hex.Length - startIndex) / 2];

        char left;
        char right;

        try
        {
            int x = 0;
            for(int i = startIndex; i < hex.Length; i += 2, x++)
            {
                left = hex[i];
                right = hex[i + 1];
                bytesArr[x] = (byte)((hexmap[left] << 4) | hexmap[right]);
            }
            return bytesArr;
        }
        catch(KeyNotFoundException)
        {
            throw new FormatException("Hex string has non-hex character");
        }
    }

    public static string ByteArrayToHexStr(byte[] bytes){
        return BitConverter.ToString(bytes).Replace("-", String.Empty).ToLower();
    }

    public static string ByteArrayToString(byte[] bytes){
        return Encoding.ASCII.GetString(bytes);
    }

    public static bool ByteArrayCompare(byte[] a1, byte[] a2) {
        if (a1.Length != a2.Length)
            return false;

        for (int i = 0; i < a1.Length; i++)
            if (a1[i] != a2[i])
                return false;

        return true;
    }

    public static bool isSubArray(byte []A, byte []B,
            int n, int m)
    {
        // Two pointers to traverse the arrays
        int i = 0, j = 0;

        // Traverse both arrays simultaneously
        while (i < n && j < m)
        {

            // If element matches
            // increment both pointers
            if (A[i] == B[j])
            {

                i++;
                j++;

                // If array B is completely
                // traversed
                if (j == m)
                    return true;
            }

                // If not,
                // increment i and reset j
            else
            {
                i = i - j + 1;
                j = 0;
            }
        }
        return false;
    }
}
