using System;
using System.IO;
using System.Text;

public class Challenges {


    public static void CHALLENGE_1(){
        string HEX = "49276d206b696c6c696e6720796f757220627261696e206c696b65206120706f69736f6e6f7573206d757368726f6f6d";
        byte[] plaintext = HexUtil.HexStrToByteArray(HEX);
        string base64 = Convert.ToBase64String(plaintext);
        Console.WriteLine(base64);
        Console.WriteLine("\n");

    }

    public static void CHALLENGE_2(){
      string HEX1 = "1c0111001f010100061a024b53535009181c";
        string HEX2 = "686974207468652062756c6c277320657965";
        string HEX_XORD = HexUtil.ByteArrayToHexStr(XorUtil.XOR(HexUtil.HexStrToByteArray(HEX1), HexUtil.HexStrToByteArray(HEX2)));
        Console.WriteLine(HEX_XORD);
        Console.WriteLine("\n");

    }

    public static void CHALLENGE_3(){
        string HEX = "1b37373331363f78151b7f2b783431333d78397828372d363c78373e783a393b3736";
        string Plain = XorBreaker.Break(HexUtil.HexStrToByteArray(HEX)).Key;
        Console.WriteLine(Plain);
        Console.WriteLine("\n");

    }

    public static void CHALLENGE_4(){
        string[] CONTENTS = File.ReadAllLines("PATH");

            string Plain = XorBreaker.Break(CONTENTS).Key;
        Console.WriteLine(Plain);

    }

    public static void CHALLENGE_5(){
        string PLAIN = "Burning 'em, if you ain't quick and nimble I go crazy when I hear a cymbal";
        string KEY = "ICE";
        string CIPH = HexUtil.ByteArrayToHexStr(XorUtil.RXOR(Encoding.ASCII.GetBytes(PLAIN), Encoding.ASCII.GetBytes(KEY)));
        Console.WriteLine(CIPH);

    }

    public static void CHALLENGE_6(){
        Console.WriteLine(VigenereBreaker.HammingDistance(Encoding.ASCII.GetBytes("this is a test"), Encoding.ASCII.GetBytes("wokka wokka!!!")));
        string PLAIN = File.ReadAllText(PATH);
        byte[] ciph = Convert.FromBase64String(PLAIN);
        string cipherText = Encoding.ASCII.GetString(ciph);
      //  Console.WriteLine(cipherText + "\n\n\n");
        string plaintext = Encoding.ASCII.GetString(VigenereBreaker.BREAK_XOR(ciph));
        Console.WriteLine(plaintext);


    }
}
