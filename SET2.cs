using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;


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
        //        string MY_STRING = "ABCDEFGHIJKLMNO";
        //        string BLOCK_BUFF = "ABCDEFGHIJKLMNO";

        string guessedAll = "";
        byte[] ENCRYPTED_L = CipherUtil_S2.encryption_oracle_2(Encoding.ASCII.GetBytes(MY_STRING + "A"), KEY_CONST_RANDOM);
        int BLOCKS_TO_COPY = 1;
        while (BLOCKS_TO_COPY * 16 < ENCRYPTED_L.Length) {
            string guess = "";
            while (guess.Length != 16) {
                byte[] ENCRYPTED = CipherUtil_S2.encryption_oracle_2(Encoding.ASCII.GetBytes(MY_STRING), KEY_CONST_RANDOM);
                byte[] BLOCK = new byte[16 * BLOCKS_TO_COPY];
                Array.Copy(ENCRYPTED, 0, BLOCK, 0, 16 * BLOCKS_TO_COPY);
                bool found = false;

                for (int i = 0; i < 127; i++) {
                    string q = MY_STRING + guessedAll + guess + (char)i;
                    byte[] ENCRYPTION_GUESS = CipherUtil_S2.encryption_oracle_2(Encoding.ASCII.GetBytes(q), KEY_CONST_RANDOM);

                    byte[] BLOCK_2 = new byte[16 * BLOCKS_TO_COPY];
                    Array.Copy(ENCRYPTION_GUESS, 0, BLOCK_2, 0, 16 * BLOCKS_TO_COPY);
                    if (HexUtil_S1.ByteArrayCompare(BLOCK_2, BLOCK)) {
                        guess += (char)i;
                        if (MY_STRING.Length != 0) MY_STRING = MY_STRING.Remove(MY_STRING.Length - 1);
                        found = true;
                        break;
                    }
                }
                if (found == false)break;


            }
            MY_STRING = BLOCK_BUFF;
            guessedAll += guess;
            BLOCKS_TO_COPY++;

        }
        byte PADDING = Encoding.ASCII.GetBytes(guessedAll)[guessedAll.Length - 1];
        string DECRYPTED = guessedAll.Substring(0, guessedAll.Length - 1 - PADDING);
        Console.WriteLine(DECRYPTED);

    }

    public static void CHALLENGE_12_ALT() {
        byte[] KEY_CONST_RANDOM = new byte[16];
        new Random(DateTime.Now.Second * DateTime.Now.Minute - DateTime.Now.Hour + DateTime.Now.Year).NextBytes(KEY_CONST_RANDOM);

        string MY_STRING = "AAAAAAAAAAAAAAA";
        string BLOCK_BUFF = "AAAAAAAAAAAAAAA";
        //        string MY_STRING = "ABCDEFGHIJKLMNO";
        //        string BLOCK_BUFF = "ABCDEFGHIJKLMNO";

        string guessedAll = "";
        byte[] ENCRYPTED_L = CipherUtil_S2.encryption_oracle_2(Encoding.ASCII.GetBytes(MY_STRING + "A"), KEY_CONST_RANDOM);
        int BLOCKS_TO_COPY = 1;
        while (BLOCKS_TO_COPY * 16 < ENCRYPTED_L.Length) {
            string guess = "";
            while (guess.Length != 16) {
                byte[] ENCRYPTED = CipherUtil_S2.encryption_oracle_2(Encoding.ASCII.GetBytes(MY_STRING), KEY_CONST_RANDOM);
                byte[] BLOCK = new byte[16];
                Array.Copy(ENCRYPTED, 16 * (BLOCKS_TO_COPY - 1), BLOCK, 0, 16);
                bool found = false;

                for (int i = 0; i < 127; i++) {
                    string q = MY_STRING + guess + (char)i;
                    byte[] ENCRYPTION_GUESS = CipherUtil_S2.encryption_oracle_2(Encoding.ASCII.GetBytes(q), KEY_CONST_RANDOM);

                    byte[] BLOCK_2 = new byte[16];
                    Array.Copy(ENCRYPTION_GUESS, 0, BLOCK_2, 0, 16);
                    if (HexUtil_S1.ByteArrayCompare(BLOCK_2, BLOCK)) {
                        guess += (char)i;
                        if (MY_STRING.Length != 0) MY_STRING = MY_STRING.Remove(0, 1);
                        found = true;
                        break;
                    }
                }
                if (found == false)break;


            }
            MY_STRING = guess.Substring(1, guess.Length - 1);
            guessedAll += guess;
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
        Array.Copy(admin, 0, hacked, 32, 16);
        Profile_S2 PARSE_DATA = Profile_S2.ParseData(hacked, key);
        Console.WriteLine(Profile_S2.ToData(PARSE_DATA));


    }

    public static void CHALLENGE_14_NotFixedPrefix() {
        byte[] KEY_CONST_RANDOM = new byte[16];
        new Random(DateTime.Now.Second * DateTime.Now.Minute - DateTime.Now.Hour + DateTime.Now.Year).NextBytes(KEY_CONST_RANDOM);
        string SIGNATURE_CHECK = new StringBuilder("").Append('A', 48).ToString();
        byte[] enc = CipherUtil_S2.encryption_oracle_4(Encoding.ASCII.GetBytes(SIGNATURE_CHECK), KEY_CONST_RANDOM);
        List<byte[]> POSSIBLE_SIGNATURES_ENCRYPTED = new List<byte[]>();
        byte[] SIGNATURE_ENCRYPTED = new byte[16];
        for (int i = 0; i < enc.Length; i += 16) {
            byte[] chunck = new byte[16];
            Array.Copy(enc, i, chunck, 0, 16);
            bool me = false;
            foreach (byte[] POSIG in POSSIBLE_SIGNATURES_ENCRYPTED) {
                if (HexUtil_S1.ByteArrayCompare(POSIG, chunck)) {
                    me = true;
                    Array.Copy(chunck, 0, SIGNATURE_ENCRYPTED, 0, 16);
                    chunck = null;
                    break;
                }

            }
            if (me == false) POSSIBLE_SIGNATURES_ENCRYPTED.Add(chunck);
            else break;


        }
        Console.WriteLine(HexUtil_S1.ByteArrayToHexStr(SIGNATURE_ENCRYPTED));

        string MY_STRING = "CCCCCCCCCCCCCCC";
        string BLOCK_BUFF = "CCCCCCCCCCCCCCC";
        byte[] ENCRYPTED_L = new byte[0];


        while (true) {
            string SIGNATURE = "AAAAAAAAAAAAAAAA";
            bool did = false;
            byte[] encrypted = CipherUtil_S2.encryption_oracle_4(Encoding.ASCII.GetBytes(SIGNATURE), KEY_CONST_RANDOM);



            //if (HexUtil_S1.ByteArrayToHexStr(encrypted).Contains(HexUtil_S1.ByteArrayToHexStr(SIGNATURE_ENCRYPTED))) {
            if (HexUtil_S1.isSubArray(encrypted, SIGNATURE_ENCRYPTED, encrypted.Length, SIGNATURE_ENCRYPTED.Length)) {
                int index = HexUtil_S1.ByteArrayToHexStr(encrypted).IndexOf(HexUtil_S1.ByteArrayToHexStr(SIGNATURE_ENCRYPTED));
                ENCRYPTED_L = new byte[encrypted.Length - index + 16];
                Array.Copy(encrypted, index - 16, ENCRYPTED_L, 0, ENCRYPTED_L.Length);
                break;
            }


        }

        Console.WriteLine("BINGO");


        string guessedAll = "";
        int BLOCKS_TO_COPY = 1;
        int guess_previous = -1;

        while (BLOCKS_TO_COPY * 16 < ENCRYPTED_L.Length) {
            Console.WriteLine("STARTED UP AT BLOCK = " + BLOCKS_TO_COPY);
            string guess = "";
            bool found = false;
            int break_counter = 0;
            while (guess.Length != 16) {
                byte[] ENCRYPTED = null;


                while (true) {

                    string SIGNATURE = "AAAAAAAAAAAAAAAA";
                    byte[] encrypted = CipherUtil_S2.encryption_oracle_4(Encoding.ASCII.GetBytes(SIGNATURE + MY_STRING), KEY_CONST_RANDOM);
                    //                    if (HexUtil_S1.ByteArrayToHexStr(encrypted).Contains(HexUtil_S1.ByteArrayToHexStr(SIGNATURE_ENCRYPTED))) {
                    //                        int index = HexUtil_S1.ByteArrayToHexStr(encrypted).IndexOf(HexUtil_S1.ByteArrayToHexStr(SIGNATURE_ENCRYPTED));
                    //                        ENCRYPTED = new byte[encrypted.Length - index];
                    //                        Array.Copy(encrypted, index, ENCRYPTED, 0, ENCRYPTED.Length);
                    //                        break;
                    //                    }
                    if (HexUtil_S1.isSubArray(encrypted, SIGNATURE_ENCRYPTED, encrypted.Length, SIGNATURE_ENCRYPTED.Length)) {
                        int index = HexUtil_S1.ByteArrayToHexStr(encrypted).IndexOf(HexUtil_S1.ByteArrayToHexStr(SIGNATURE_ENCRYPTED));
                        ENCRYPTED = new byte[encrypted.Length - index];
                        Array.Copy(encrypted, index, ENCRYPTED, 0, ENCRYPTED.Length);
                        break;
                    }


                }



                byte[] BLOCK = new byte[16 * BLOCKS_TO_COPY];
                Array.Copy(ENCRYPTED, 0, BLOCK, 0, 16 * BLOCKS_TO_COPY);
                // bool found = false;


                for (int i = 0; i < 128; i++) {
                    string q = MY_STRING + guessedAll + guess + (char)i;
                    byte[] ENCRYPTION_GUESS;
                    while (true) {
                        string SIGNATURE = "AAAAAAAAAAAAAAAA";
                        byte[] encrypted = CipherUtil_S2.encryption_oracle_4(Encoding.ASCII.GetBytes(SIGNATURE + q), KEY_CONST_RANDOM);
                        //if (HexUtil_S1.ByteArrayToHexStr(encrypted).Contains(HexUtil_S1.ByteArrayToHexStr(SIGNATURE_ENCRYPTED))) {
                        if (HexUtil_S1.isSubArray(encrypted, SIGNATURE_ENCRYPTED, encrypted.Length, SIGNATURE_ENCRYPTED.Length)) {
                            int index = HexUtil_S1.ByteArrayToHexStr(encrypted).IndexOf(HexUtil_S1.ByteArrayToHexStr(SIGNATURE_ENCRYPTED));
                            ENCRYPTION_GUESS = new byte[encrypted.Length - index];
                            Array.Copy(encrypted, index, ENCRYPTION_GUESS, 0, ENCRYPTION_GUESS.Length);

                            break;
                        }
                    }



                    byte[] BLOCK_2 = new byte[16 * BLOCKS_TO_COPY];
                    Array.Copy(ENCRYPTION_GUESS, 0, BLOCK_2, 0, 16 * BLOCKS_TO_COPY);
                    if (HexUtil_S1.ByteArrayCompare(BLOCK_2, BLOCK)) {
                        guess += (char)i;

                        if (MY_STRING.Length != 0) MY_STRING = MY_STRING.Remove(MY_STRING.Length - 1);
                        found = true;
                        break;
                    }
                }
                if (found == false)break;
                break_counter++;
                if ((BLOCKS_TO_COPY + 1) * 16 > ENCRYPTED_L.Length) break;


            }
            MY_STRING = BLOCK_BUFF;
            guessedAll += guess;
            if (found == false) break;
            BLOCKS_TO_COPY++;


        }
        string DECRYPTED = guessedAll;
        Console.WriteLine(DECRYPTED);


    }

    public static void CHALLENGE_14_FixedPrefix() {
        byte[] KEY_CONST_RANDOM = new byte[16];
        new Random(725).NextBytes(KEY_CONST_RANDOM);

        //Generating constant random prefix
        RNGCryptoServiceProvider random = new RNGCryptoServiceProvider();
        byte[] n = new byte[4];
        random.GetBytes(n);
        byte[] rndAppend = new byte[Math.Abs(BitConverter.ToInt32(n, 0)) % 256 + 15];
        random.GetBytes(rndAppend);
//        Console.WriteLine(rndAppend.Length);



        int OBTAINED_PREFIX_LENGTH = 0;
        int BLOCK_APPLIER = 0;

        //Get Encrypted "AAAAAAAA..." signature to obtain the random prefix length in further

        string SIGNATURE_CHECK = new StringBuilder("").Append('A', 48).ToString();
        byte[] enc = CipherUtil_S2.encryption_oracle_4_fixed(Encoding.ASCII.GetBytes(SIGNATURE_CHECK), KEY_CONST_RANDOM, rndAppend);

//        string hex = HexUtil_S1.ByteArrayToHexStr(enc);
//        hex = Regex.Replace(hex, "(.{32})", "$& ");
//        Console.WriteLine(hex);


        int CHUNCK_INDEX = 0;
        List<byte[]> POSSIBLE_SIGNATURES_ENCRYPTED = new List<byte[]>();
        byte[] SIGNATURE_ENCRYPTED = new byte[16];
        for (int i = 0; i < enc.Length; i += 16) {
            byte[] chunck = new byte[16];
            Array.Copy(enc, i, chunck, 0, 16);
            bool me = false;
            foreach (byte[] POSIG in POSSIBLE_SIGNATURES_ENCRYPTED) {
                if (HexUtil_S1.ByteArrayCompare(POSIG, chunck)) {
                    me = true;
                    Array.Copy(chunck, 0, SIGNATURE_ENCRYPTED, 0, 16);
                    chunck = null;
                    CHUNCK_INDEX = i - 16;
                    break;
                }

            }
            if (me == false) POSSIBLE_SIGNATURES_ENCRYPTED.Add(chunck);
            else break;


        }



        int ENC_LENGTH = 0;

        byte[] SIGNANTURE_16 = new byte[48];
        Array.Copy(SIGNATURE_ENCRYPTED, 0, SIGNANTURE_16, 0, 16);
        Array.Copy(SIGNATURE_ENCRYPTED, 0, SIGNANTURE_16, 16, 16);
        Array.Copy(SIGNATURE_ENCRYPTED, 0, SIGNANTURE_16, 32, 16);
        if (HexUtil_S1.isSubArray(enc, SIGNANTURE_16, enc.Length, SIGNANTURE_16.Length)) {
            OBTAINED_PREFIX_LENGTH = CHUNCK_INDEX;
            BLOCK_APPLIER = 0;
            ENC_LENGTH = enc.Length - CHUNCK_INDEX + 32;
           // Console.WriteLine("LENGTH - " + OBTAINED_PREFIX_LENGTH);
        }
        else {

            int L = 1;
            while(L!=32){
                string SIGNATURE_TEST = new StringBuilder("").Append('A', L).ToString();
                byte[] ENCRYPTED_L = CipherUtil_S2.encryption_oracle_4_fixed(Encoding.ASCII.GetBytes(SIGNATURE_TEST), KEY_CONST_RANDOM, rndAppend);
                byte[] SIGNATURE_TEST_ENCRYPTED = new byte[16];
                Array.Copy(ENCRYPTED_L, CHUNCK_INDEX, SIGNATURE_TEST_ENCRYPTED, 0 ,16);
                if(HexUtil_S1.ByteArrayCompare(SIGNATURE_TEST_ENCRYPTED, SIGNATURE_ENCRYPTED)){

                    OBTAINED_PREFIX_LENGTH =  CHUNCK_INDEX + (16 - L);
                    BLOCK_APPLIER = L-16;
                    ENC_LENGTH = ENCRYPTED_L.Length - CHUNCK_INDEX + 16;
                    //Console.WriteLine(L-16 + " " + rndAppend.Length % 16);
                    break;
                }
                else L++;
            }
           // Console.WriteLine(OBTAINED_PREFIX_LENGTH);

        }


        //Now knowing PREFIX_LENGTH and BLOCK_APPLIER, it is possible to apply 12-th oracle solution

        string PREFIX_DESTROYER = new StringBuilder("").Append('A', BLOCK_APPLIER).ToString();
        string MY_STRING = "AAAAAAAAAAAAAAA";
        string guessedAll = "";
        int BLOCKS_TO_COPY = 1;




        while (BLOCKS_TO_COPY * 16 < ENC_LENGTH) {
            string guess = "";
            while (guess.Length != 16) {
                byte[] ENCRYPTED = CipherUtil_S2.encryption_oracle_4_fixed(Encoding.ASCII.GetBytes(PREFIX_DESTROYER + MY_STRING), KEY_CONST_RANDOM, rndAppend);
                byte[] BLOCK = new byte[16];
                Array.Copy(ENCRYPTED, CHUNCK_INDEX + 16 * (BLOCKS_TO_COPY - 1), BLOCK, 0, 16);
                bool found = false;

                for (int i = 0; i < 127; i++) {
                    string q = PREFIX_DESTROYER +  MY_STRING + guess + (char)i;
                    byte[] ENCRYPTION_GUESS = CipherUtil_S2.encryption_oracle_4_fixed(Encoding.ASCII.GetBytes(q), KEY_CONST_RANDOM, rndAppend);

                    byte[] BLOCK_2 = new byte[16];
                    Array.Copy(ENCRYPTION_GUESS, CHUNCK_INDEX, BLOCK_2, 0, 16);
                    if (HexUtil_S1.ByteArrayCompare(BLOCK_2, BLOCK)) {
                        guess += (char)i;
                        if (MY_STRING.Length != 0) MY_STRING = MY_STRING.Remove(0, 1);
                        found = true;
                        break;
                    }
                }
                if (found == false)break;



            }
            MY_STRING = guess.Substring(1, guess.Length - 1);
            guessedAll += guess;

            BLOCKS_TO_COPY++;
           if((BLOCKS_TO_COPY + 1 )* 16 >= ENC_LENGTH) break;;


        }
        byte PADDING = Encoding.ASCII.GetBytes(guessedAll)[guessedAll.Length - 1];
        string DECRYPTED = guessedAll.Substring(0, guessedAll.Length - 1 - PADDING);
        Console.WriteLine(DECRYPTED);







    }


}
