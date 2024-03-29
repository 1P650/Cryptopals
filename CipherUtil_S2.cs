using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

public class CipherUtil_S2 {
    public static byte[] pad(byte[] input, int to) {
        int newSize = extendToSize(input.Length, to);
        byte diff = (byte) Math.Abs(newSize - input.Length);
        byte[] rtq = new byte[newSize];
        Array.Copy(input, 0, rtq, 0, input.Length);
        for (int i = input.Length; i < rtq.Length; i++) {
            rtq[i] = diff;
        }
        return rtq;
    }

    public static int extendToSize(int variable, int size) {
        if (variable % size == 0) return variable;
        return (variable - (variable % size) + size);
    }

    public static byte[] AES_ECB_ENCRYPT(byte[] plain, byte[] key) {

        RijndaelManaged rijndael = new RijndaelManaged {
            Mode = CipherMode.ECB,
            Padding = PaddingMode.None
        };
        byte[] encrypted;

        using (ICryptoTransform encryptor = rijndael.CreateEncryptor(key, null)) {

            encrypted = encryptor.TransformFinalBlock(plain, 0, plain.Length);

        }

        return encrypted;
    }

    public static byte[] AES_ECB_DECRYPT(byte[] ciph, byte[] key) {
        RijndaelManaged rijndael = new RijndaelManaged {
            Mode = CipherMode.ECB,
            Padding = PaddingMode.None

        };

        byte[] decrypted;

        using (ICryptoTransform decryptor = rijndael.CreateDecryptor(key, null)) {

            decrypted = decryptor.TransformFinalBlock(ciph, 0, ciph.Length);

        }
        return decrypted;
    }

    public static byte[] AES_CBC_ENCRYPT(byte[] input, byte[] key, byte[] IV) {
        byte blocksize = 16;

        byte last_len = (byte) (input.Length % blocksize == 0 ? 0 : extendToSize(input.Length, blocksize) - input.Length);
        byte[] input_extended = new byte[input.Length % blocksize == 0 ? input.Length + blocksize : extendToSize(input.Length, blocksize)];
        Array.Copy(input, 0, input_extended, 0, input.Length);
        for (int i = input.Length; i < input_extended.Length; i++) input_extended[i] = last_len;
        byte[] STATE = (byte[]) IV.Clone();
        byte[] encrypted = new byte[input_extended.Length];

        for (int i = 0; i < input_extended.Length; i += blocksize) {

            byte[] chunck = new byte[blocksize];

            Array.Copy(input_extended, i, chunck, 0, blocksize);
            chunck = AES_ECB_ENCRYPT(XorUtil_S1.XOR(STATE, chunck), key);

            STATE = (byte[]) chunck.Clone();
            Array.Copy(chunck, 0, encrypted, i, blocksize);
        }


        return encrypted;
    }

    public static byte[] AES_CBC_DECRYPT(byte[] input, byte[] key, byte[] IV) {
        byte blocksize = 16;
        byte[] STATE = (byte[]) IV.Clone();
        byte[] decrypted_extended = new byte[input.Length];
        for (int i = 0; i <= input.Length - blocksize; i += blocksize) {
            byte[] chunck = new byte[blocksize];
            Array.Copy(input, i, chunck, 0, chunck.Length);
            byte[] STATE_m = (byte[]) chunck.Clone();
            chunck = XorUtil_S1.XOR(STATE, AES_ECB_DECRYPT(chunck, key));
            STATE = STATE_m;
            Array.Copy(chunck, 0, decrypted_extended, i, blocksize);
        }

        byte len = decrypted_extended[decrypted_extended.Length - 1];
        if (len == 0) len = blocksize;
        byte[] decrypted = new byte[decrypted_extended.Length - len];
        Array.Copy(decrypted_extended, 0, decrypted, 0, decrypted.Length);
        return decrypted;
    }

    public static byte[] encryption_oracle(byte[] input) {

        byte[] key = new byte[16];
        byte[] IV = new byte[16];
        Random r = new Random(DateTime.Now.Hour + DateTime.Now.Millisecond + DateTime.Now.Year + DateTime.Now.Second);
        r.NextBytes(key);
        r.NextBytes(IV);

        byte[] R1 = new byte[r.Next(5, 10)];
        byte[] R2 = new byte[r.Next(5, 10)];
        byte[] input_extended = new byte[R1.Length + R2.Length + input.Length];
        Array.Copy(R1, 0, input_extended, 0, R1.Length);
        Array.Copy(input, 0, input_extended, R1.Length, input.Length);
        Array.Copy(R2, 0, input_extended, input.Length + R1.Length, R2.Length);



        int A = r.Next() % 2;
        if (A == 0) {
            //ECB
            // Console.WriteLine("CURR MODE: ECB");
            byte[] plain = (byte[]) input_extended.Clone();
            plain = pad(plain, 16);
            byte[] ciph = AES_ECB_ENCRYPT(plain, key);
            return ciph;
        }
        else {
            //CBC
            //  Console.WriteLine("CURR MODE: CBC");
            byte[] plain = (byte[]) input_extended.Clone();
            plain = pad(plain, 16);
            byte[] ciph = AES_CBC_ENCRYPT(plain, key, IV);
            return ciph;
        }
    }

    public static byte[] encryption_oracle_2(byte[] input, byte[] const_key) {
        string unknown_string = Encoding.ASCII.GetString(
                Convert.FromBase64String(
                        "Um9sbGluJyBpbiBteSA1LjAKV2l0aCBteSByYWctdG9wIGRvd24gc28gbXkgaGFpciBjYW4gYmxvdwpUaGUgZ2lybGllcyBvbiBzdGFuZGJ5IHdhdmluZyBqdXN0IHRvIHNheSBoaQpEaWQgeW91IHN0b3A/IE5vLCBJIGp1c3QgZHJvdmUgYnkK"));
        string FULL = Encoding.ASCII.GetString(input) + unknown_string;
        byte[] plain = Encoding.ASCII.GetBytes(FULL);
        plain = pad(plain, 16);
        byte[] ciph = AES_ECB_ENCRYPT(plain, const_key);
        return ciph;


    }


    public static byte[] encryption_oracle_4(byte[] input, byte[] const_key) {
        RNGCryptoServiceProvider random = new RNGCryptoServiceProvider();
        string unknown_string = Encoding.ASCII.GetString(
                Convert.FromBase64String(
                        "Um9sbGluJyBpbiBteSA1LjAKV2l0aCBteSByYWctdG9wIGRvd24gc28gbXkgaGFpciBjYW4gYmxvdwpUaGUgZ2lybGllcyBvbiBzdGFuZGJ5IHdhdmluZyBqdXN0IHRvIHNheSBoaQpEaWQgeW91IHN0b3A/IE5vLCBJIGp1c3QgZHJvdmUgYnkK"));


        byte[] n = new byte[4];
        random.GetBytes(n);
        byte[] rndAppend = new byte[Math.Abs(BitConverter.ToInt32(n, 0)) % 256 + 15];
        random.GetBytes(rndAppend);
        Console.WriteLine(rndAppend.Length);
        string inputAndUnknown = Encoding.ASCII.GetString(input) + unknown_string;
        byte[] inputAndUnknown_Bytes = Encoding.ASCII.GetBytes(inputAndUnknown);
        byte[] FullString_Bytes = new byte[inputAndUnknown_Bytes.Length + rndAppend.Length];
        Array.Copy(rndAppend, 0, FullString_Bytes, 0, rndAppend.Length);
        Array.Copy(inputAndUnknown_Bytes, 0, FullString_Bytes, rndAppend.Length, inputAndUnknown_Bytes.Length);


        byte[] plain = FullString_Bytes;
        plain = pad(plain, 16);
        byte[] ciph = AES_ECB_ENCRYPT(plain, const_key);
        return ciph;


    }

    public static byte[] encryption_oracle_4_fixed(byte[] input, byte[] const_key, byte[] const_random_prefix) {

        string unknown_string = Encoding.ASCII.GetString(
                Convert.FromBase64String(
                        "Um9sbGluJyBpbiBteSA1LjAKV2l0aCBteSByYWctdG9wIGRvd24gc28gbXkgaGFpciBjYW4gYmxvdwpUaGUgZ2lybGllcyBvbiBzdGFuZGJ5IHdhdmluZyBqdXN0IHRvIHNheSBoaQpEaWQgeW91IHN0b3A/IE5vLCBJIGp1c3QgZHJvdmUgYnkK"));




        string inputAndUnknown = Encoding.ASCII.GetString(input) + unknown_string;
        byte[] inputAndUnknown_Bytes = Encoding.ASCII.GetBytes(inputAndUnknown);
        byte[] FullString_Bytes = new byte[inputAndUnknown_Bytes.Length + const_random_prefix.Length];
        Array.Copy(const_random_prefix, 0, FullString_Bytes, 0, const_random_prefix.Length);
        Array.Copy(inputAndUnknown_Bytes, 0, FullString_Bytes, const_random_prefix.Length, inputAndUnknown_Bytes.Length);


        byte[] plain = FullString_Bytes;
        plain = pad(plain, 16);
        byte[] ciph = AES_ECB_ENCRYPT(plain, const_key);
        return ciph;
    }

    public static byte[] encryption_oracle_3(byte[] input, byte[] const_key) {


        string FULL = Encoding.ASCII.GetString(input);

        byte[] plain = Encoding.ASCII.GetBytes(FULL);
        plain = pad(plain, 16);
        byte[] ciph = AES_ECB_ENCRYPT(plain, const_key);
        return ciph;


    }

    public static byte[] decryption_oracle_3(byte[] input, byte[] const_key) {


        byte[] plain = AES_ECB_DECRYPT(input, const_key);


        byte PADDING = plain[plain.Length - 1];
        byte ToRemove = 0;
        if (PADDING == 0) ToRemove = 16;
        else ToRemove = PADDING;
        byte[] plain_RmPad = new byte[plain.Length - ToRemove];
        Array.Copy(plain, 0, plain_RmPad, 0, plain_RmPad.Length);
        return plain_RmPad;


    }


    public static string detectMode(byte[] ciph) {
        if (detectECB(ciph)) return "ECB";
        return "CBC";
    }

    public static int detectBlockSize(byte[] const_key) {
        string IN = "A";
        int length = 0;
        for (int i = 0;; i++) {
            byte[] ENC = encryption_oracle_2(Encoding.ASCII.GetBytes(IN), const_key);
            if (ENC.Length - length > 1 && length != 0) return ENC.Length - length;
            length = ENC.Length;
            IN += "A";
        }
    }

    public static bool detectECB(byte[] ciph) {
        List<byte[]> chuncks = new List<byte[]>();
        for (int j = 0; j < ciph.Length; j += 16) {
            byte[] chunck = new byte[16];
            Array.Copy(ciph, j, chunck, 0, 16);
            chuncks.Add(chunck);
        }
        bool flag = false;
        for (int k = 0; k < chuncks.Count; k++) {
            for (int l = 0; l < chuncks.Count; l++) {
                if (k != l && HexUtil_S1.ByteArrayCompare(chuncks[k], chuncks[l])) {
                    return true;
                }
            }

        }
        return false;
    }

    public static byte[] validatePKCS7(byte[] PADDED) {
        if (PADDED.Length % 16 != 0) throw new Exception("BAD PADDING");
        byte PAD_VAL = PADDED[PADDED.Length - 1];
        if (PAD_VAL > 15) throw new Exception("BAD PADDING");
        for (int i = PADDED.Length - PAD_VAL; i < PADDED.Length; i++) {
            if (PADDED[i] != PAD_VAL) throw new Exception("BAD PADDING");
        }
        byte[] UNPADDED = new byte[PADDED.Length - PAD_VAL];
        Array.Copy(PADDED, 0, UNPADDED, 0, UNPADDED.Length);
        return UNPADDED;

    }

    public static byte[] CH16_EncryptFunction(string userdata, byte[] key, byte[] IV) {
        userdata = userdata.Replace("=", "");
        userdata = userdata.Replace(";", "");
        string comment1 = "comment1=cooking%20MCs;userdata=";
        string comment2 = ";comment2=%20like%20a%20pound%20of%20bacon";
        string input = comment1 + userdata + comment2;
        byte [] inputB = Encoding.ASCII.GetBytes(input);
        byte[] enc = CipherUtil_S2.AES_CBC_ENCRYPT(inputB, key, IV);
        return enc;
    }

    public static bool CH16_FindAdmin(byte[] enc, byte[] key, byte[] IV) {
        byte[] dec = CipherUtil_S2.AES_CBC_DECRYPT(enc, key, IV);
        string decS = Encoding.ASCII.GetString(dec);



        if (decS.Contains(";admin=true;")) return true;
        return false;
    }


}
