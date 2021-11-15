using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

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

    public static byte[] AES_ECB_ENCRYPT(byte[] plain, byte[] key){

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

    public static byte[] AES_ECB_DECRYPT(byte[] ciph, byte[] key){
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

    public static byte[] AES_CBC_ENCRYPT(byte[] input, byte[] key, byte[] IV){
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

    public static byte[] AES_CBC_DECRYPT(byte[] input, byte[] key, byte[] IV){
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

    public static byte[] encryption_oracle(byte[] input){

        byte[] key = new byte[16];
        byte[] IV = new byte[16];
        Random r = new Random(DateTime.Now.Hour + DateTime.Now.Millisecond + DateTime.Now.Year + DateTime.Now.Second);
        r.NextBytes(key);
        r.NextBytes(IV);

        byte[] R1 = new byte[r.Next(5,10)];
        byte[] R2 = new byte[r.Next(5,10)];
        byte[] input_extended = new byte[R1.Length + R2.Length + input.Length];
        Array.Copy(R1, 0, input_extended, 0, R1.Length);
        Array.Copy(input, 0, input_extended, R1.Length, input.Length);
        Array.Copy(R2, 0, input_extended, input.Length + R1.Length, R2.Length);



        int A = r.Next() % 2;
        if(A == 0){
            //ECB
           // Console.WriteLine("CURR MODE: ECB");
            byte[] plain = (byte[]) input_extended.Clone();
            plain = pad(plain,16);
            byte[] ciph = AES_ECB_ENCRYPT(plain, key);
            return ciph;
        }
        else{
            //CBC
          //  Console.WriteLine("CURR MODE: CBC");
            byte[] plain = (byte[]) input_extended.Clone();
            plain = pad(plain,16);
            byte[] ciph = AES_CBC_ENCRYPT(plain, key, IV);
            return ciph;
        }
    }

    public static byte[] encryption_oracle_2(byte[] input, byte[] const_key, byte[] unknown_string){
       string UNKNOWN = Encoding.ASCII.GetString(
               Convert.FromBase64String(
                       "Um9sbGluJyBpbiBteSA1LjAKV2l0aCBteSByYWctdG9wIGRvd24gc28gbXkgaGFpciBjYW4gYmxvdwpUaGUgZ2lybGllcyBvbiBzdGFuZGJ5IHdhdmluZyBqdXN0IHRvIHNheSBoaQpEaWQgeW91IHN0b3A/IE5vLCBJIGp1c3QgZHJvdmUgYnkK"));

            string FULL = Encoding.ASCII.GetString(input) + Encoding.ASCII.GetString(unknown_string);

            byte[] plain = Encoding.ASCII.GetBytes(FULL);
            plain = pad(plain,16);
            byte[] ciph = AES_ECB_ENCRYPT(plain, const_key);
            return ciph;


    }




    public static string detectMode(byte[] ciph){
        if(detectECB(ciph)) return "ECB";
        return "CBC";
    }

    public static int detectBlockSize(byte[] const_key){
        string UNKNOWN = Encoding.ASCII.GetString(
                Convert.FromBase64String(
                        "Um9sbGluJyBpbiBteSA1LjAKV2l0aCBteSByYWctdG9wIGRvd24gc28gbXkgaGFpciBjYW4gYmxvdwpUaGUgZ2lybGllcyBvbiBzdGFuZGJ5IHdhdmluZyBqdXN0IHRvIHNheSBoaQpEaWQgeW91IHN0b3A/IE5vLCBJIGp1c3QgZHJvdmUgYnkK"));
        string IN = "A";
        int length = 0;
        for(int i = 0;; i++){
            byte[] ENC =encryption_oracle_2(Encoding.ASCII.GetBytes(IN), const_key, Encoding.ASCII.GetBytes(UNKNOWN));
            if(ENC.Length - length > 1 && length!=0) return ENC.Length -length;
            length = ENC.Length;
            IN+="A";
        }
    }

    public static bool detectECB(byte[] ciph){
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


}
