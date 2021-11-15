using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

public class SET1 {


    public static void CHALLENGE_1() {
        string HEX = "49276d206b696c6c696e6720796f757220627261696e206c696b65206120706f69736f6e6f7573206d757368726f6f6d";
        byte[] plaintext = HexUtil_S1.HexStrToByteArray(HEX);
        string base64 = Convert.ToBase64String(plaintext);
        Console.WriteLine(base64);
        Console.WriteLine("\n");

    }

    public static void CHALLENGE_2() {
        string HEX1 = "1c0111001f010100061a024b53535009181c";
        string HEX2 = "686974207468652062756c6c277320657965";
        string HEX_XORD = HexUtil_S1.ByteArrayToHexStr(XorUtil_S1.XOR(HexUtil_S1.HexStrToByteArray(HEX1), HexUtil_S1.HexStrToByteArray(HEX2)));
        Console.WriteLine(HEX_XORD);
        Console.WriteLine("\n");

    }

    public static void CHALLENGE_3() {
        string HEX = "1b37373331363f78151b7f2b783431333d78397828372d363c78373e783a393b3736";
        string Plain = XorBreaker_S1.Break(HexUtil_S1.HexStrToByteArray(HEX)).Key;
        Console.WriteLine(Plain);
        Console.WriteLine("\n");

    }

    public static void CHALLENGE_4() {
        string[] CONTENTS = File.ReadAllLines("4.txt");

        string Plain = XorBreaker_S1.Break(CONTENTS).Key;
        Console.WriteLine(Plain);

    }

    public static void CHALLENGE_5() {
        string PLAIN = "Burning 'em, if you ain't quick and nimble I go crazy when I hear a cymbal";
        string KEY = "ICE";
        string CIPH = HexUtil_S1.ByteArrayToHexStr(XorUtil_S1.RXOR(Encoding.ASCII.GetBytes(PLAIN), Encoding.ASCII.GetBytes(KEY)));
        Console.WriteLine(CIPH);

    }

    public static void CHALLENGE_6() {

        string PLAIN = File.ReadAllText("6.txt");
        byte[] ciph = Convert.FromBase64String(PLAIN);
        string cipherText = Encoding.ASCII.GetString(ciph);
        //  Console.WriteLine(cipherText + "\n\n\n");
        string plaintext = Encoding.ASCII.GetString(VigenereBreaker_S1.BREAK_XOR(ciph));
        Console.WriteLine(plaintext);


    }

    public static void CHALLENGE_7() {
        string PLAIN = File.ReadAllText("7.txt");

        byte[] ciph = Convert.FromBase64String(PLAIN);

        Console.WriteLine(ciph.Length);
        RijndaelManaged rijndael = new RijndaelManaged {
            Mode = CipherMode.ECB
        };
        byte[] key = Encoding.ASCII.GetBytes("YELLOW SUBMARINE");

        using (ICryptoTransform decryptor = rijndael.CreateDecryptor(key, null)) {

            byte[] decryptedText = decryptor.TransformFinalBlock(ciph, 0, ciph.Length);
            Console.WriteLine(Encoding.UTF8.GetString(decryptedText));
        }

    }


    public static void CHALLENGE_8() {
        string[] PLAIN = File.ReadAllLines("8.txt");
        for (byte i = 0; i < PLAIN.Length; i++) {
            byte[] guess;
            guess = HexUtil_S1.HexStrToByteArray(PLAIN[i]);
            if(CipherUtil_S2.detectECB(guess)) Console.WriteLine(HexUtil_S1.ByteArrayToHexStr(guess));
//            List<byte[]> chuncks = new List<byte[]>();
//            for (int j = 0; j < guess.Length; j += 16) {
//                byte[] chunck = new byte[16];
//                Array.Copy(guess, j, chunck, 0, 16);
//                chuncks.Add(chunck);
//            }
//            bool flag = false;
//            for (int k = 0; k < chuncks.Count; k++) {
//                for (int l = 0; l < chuncks.Count; l++) {
//                    if (k != l && HexUtil_S1.ByteArrayCompare(chuncks[k], chuncks[l])) {
//                        flag = true;
//                        Console.WriteLine("FOUND: " + PLAIN[i] + "{ \n " + HexUtil_S1.ByteArrayToHexStr(chuncks[k]) + " EQUALS TO " + HexUtil_S1.ByteArrayToHexStr(chuncks[l]));
//                        break;
//                    }
//                }
//                if (flag == true) break;
//            }
//            if (flag == true) break;
        }
    }


}
