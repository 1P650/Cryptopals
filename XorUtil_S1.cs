using System;

public class XorUtil_S1 {

    public static byte[] XOR(byte[] p, byte[] k){
        for (int i = 0; i < p.Length; i++) {
            p[i]^=k[i];
        }
        return p;
    }

    public static byte[] XOR(byte[] pO, byte k){
        byte[] p = new byte[pO.Length];
        Array.Copy(pO, p, p.Length);
        for (int i = 0; i < p.Length; i++) {
            p[i]^=k;

        }
        return p;
    }

    public static byte[] RXOR(byte[] pO, byte[] k){
        byte[] p = new byte[pO.Length];
        Array.Copy(pO, p, p.Length);
        for (int i = 0; i < p.Length; i++) {
            p[i]^=k[i%k.Length];

        }
        return p;
    }

}
