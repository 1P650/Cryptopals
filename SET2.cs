using System;
using System.IO;
using System.Text;

public class SET2 {
    public static void CHALLENGE_9() {
        string PL = "YELLOW SUMBARINE";
        byte[] PLB = Encoding.ASCII.GetBytes(PL);
        byte[] PLB20 = CipherUtil_S2.pad(PLB, 20);
        Console.WriteLine(HexUtil_S1.ByteArrayToHexStr(PLB20));
    }

    public static void CHALLENGE_10() {
        string PLAINTEXT = File.ReadAllText("10.txt");
        byte[] plain = Convert.FromBase64String(PLAINTEXT);
        byte[] c = CipherUtil_S2.AES_CBC_DECRYPT(plain, Encoding.ASCII.GetBytes("YELLOW SUBMARINE"), new byte[]{0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0});
        Console.WriteLine(Encoding.ASCII.GetString(c));
    }

    public static void CHALLENGE_11() {
        string PLAINTEXT = "YELLOW SUBMARINEYELLOW SUMBARINEYELLOW SUBMARINEYELLOW SUMBARINE";
        byte[] CIPHERTEXT = CipherUtil_S2.encryption_oracle(Encoding.ASCII.GetBytes(PLAINTEXT));
        string MODE = CipherUtil_S2.detectMode(CIPHERTEXT);
        Console.WriteLine(MODE);
    }


    public static void CHALLENGE_12() {
        byte[] KEY_CONST_RANDOM = new byte[16];
        new Random(DateTime.Now.Second * DateTime.Now.Minute - DateTime.Now.Hour + DateTime.Now.Year).NextBytes(KEY_CONST_RANDOM);
        string UNKNOWN_O = Encoding.ASCII.GetString(
                Convert.FromBase64String(
                        "Um9sbGluJyBpbiBteSA1LjAKV2l0aCBteSByYWctdG9wIGRvd24gc28gbXkgaGFpciBjYW4gYmxvdwpUaGUgZ2lybGllcyBvbiBzdGFuZGJ5IHdhdmluZyBqdXN0IHRvIHNheSBoaQpEaWQgeW91IHN0b3A/IE5vLCBJIGp1c3QgZHJvdmUgYnkK"));
        string MY_STRING = "AAAAAAAAAAAAAAA";

        string guessed = "";
        string guessedAll = "";
        int INDEX = 0;
        string UNKNOWN = Encoding.ASCII.GetString(CipherUtil_S2.pad(Encoding.ASCII.GetBytes(UNKNOWN_O), 16));
        int TO_REMOVE = UNKNOWN.Length - UNKNOWN_O.Length;
        while (guessedAll.Length - UNKNOWN.Length != 0) {
            while (guessed.Length != 16) {

                byte[] encryption_oracle = CipherUtil_S2.encryption_oracle_2(Encoding.ASCII.GetBytes(MY_STRING), KEY_CONST_RANDOM, Encoding.ASCII.GetBytes(UNKNOWN.Remove(0, INDEX * 16)));
                byte[] BLOCK = new byte[16];
                Array.Copy(encryption_oracle, 0, BLOCK, 0, 16);
                for (int i = 0; i < 127; i++) {
                    string q = MY_STRING + guessed + (char)i;
                    byte[] encryption_oracle_2 = CipherUtil_S2.encryption_oracle_2(Encoding.ASCII.GetBytes(q), KEY_CONST_RANDOM, Encoding.ASCII.GetBytes(UNKNOWN.Remove(0, INDEX * 16)));
                    byte[] BLOCK_2 = new byte[16];
                    Array.Copy(encryption_oracle_2, 0, BLOCK_2, 0, 16);
                    if (HexUtil_S1.ByteArrayCompare(BLOCK_2, BLOCK)) {
                        guessed += (char)i;
                        if (MY_STRING.Length != 0) MY_STRING = MY_STRING.Remove(MY_STRING.Length - 1);
                        break;
                    }
                }
            }

            MY_STRING = "AAAAAAAAAAAAAAA";
            guessedAll += guessed;
            guessed = "";
            INDEX++;

        }
        guessedAll = guessedAll.Remove(UNKNOWN_O.Length, TO_REMOVE);


        Console.WriteLine(guessedAll);

    }


}
