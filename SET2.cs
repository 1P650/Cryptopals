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
        string PLAINTEXT = File.ReadAllText("/home/jewpigeon/Рабочий стол/10.txt");
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

        string MY_STRING = "AAAAAAAAAAAAAAA";
        string BLOCK_BUFF = "AAAAAAAAAAAAAAA";

        string guessedAll = "";
        byte[] ENCRYPTED_L = CipherUtil_S2.encryption_oracle_2(Encoding.ASCII.GetBytes(MY_STRING+"A"), KEY_CONST_RANDOM);
        int BLOCKS_TO_COPY = 1;
        while (BLOCKS_TO_COPY*16 < ENCRYPTED_L.Length -15){
            string guess = "";
            while (guess.Length!=16){
                byte[] ENCRYPTED = CipherUtil_S2.encryption_oracle_2(Encoding.ASCII.GetBytes(MY_STRING), KEY_CONST_RANDOM);
                byte[] BLOCK = new byte[16*BLOCKS_TO_COPY];
                Array.Copy(ENCRYPTED, 0, BLOCK, 0, 16*BLOCKS_TO_COPY);
                bool found = false;

                for(int i = 0; i < 127; i++){
                    string q = MY_STRING + guessedAll + guess + (char)i;
                    byte[] ENCRYPTION_GUESS = CipherUtil_S2.encryption_oracle_2(Encoding.ASCII.GetBytes(q), KEY_CONST_RANDOM);

                    byte[] BLOCK_2 = new byte[16*BLOCKS_TO_COPY];
                    Array.Copy(ENCRYPTION_GUESS, 0, BLOCK_2, 0, 16*BLOCKS_TO_COPY);
                    if (HexUtil_S1.ByteArrayCompare(BLOCK_2, BLOCK)) {
                        guess += (char)i;
                        if (MY_STRING.Length != 0) MY_STRING = MY_STRING.Remove(MY_STRING.Length - 1);
                        found = true;
                        break;
                    }
                }
                if(found == false)break;


            }
            MY_STRING = BLOCK_BUFF;
            guessedAll+=guess;
            BLOCKS_TO_COPY++;

        }
        byte PADDING = Encoding.ASCII.GetBytes(guessedAll)[guessedAll.Length - 1];
        string DECRYPTED = guessedAll.Substring(0, guessedAll.Length - 1 - PADDING);
        Console.WriteLine(DECRYPTED);

    }

    public static void CHALLENGE_13() {
        byte[] key = new byte[16];
        new Random(DateTime.Now.Hour + DateTime.Now.Minute + DateTime.Now.Second * DateTime.Now.Millisecond).NextBytes(key);
        byte[] orig = Profile_S2.ProfileFor("AAAAAAAAAAAAAA", key);
        byte[] adminFull = Profile_S2.ProfileFor("AAAAAAAAAA" +
        Encoding.ASCII.GetString(CipherUtil_S2.pad(
            Encoding.ASCII.GetBytes("admin"), 16)
        ), key);

        byte[] admin = new byte[16];

        Array.Copy(adminFull, 16, admin, 0, 16);
        byte[] hacked = new byte[48];
        Array.Copy(orig, 0, hacked, 0, 32);
        Array.Copy(admin, 0, hacked, 32,16);
        Profile_S2 PARSE_DATA = Profile_S2.ParseData(hacked, key);
        Console.WriteLine(Profile_S2.ToData(PARSE_DATA));



    }

    public static void CHALLENGE_14(){

    }


}
