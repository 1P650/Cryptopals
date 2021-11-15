using System;
using System.Collections.Generic;
using System.Text;

public class VigenereBreaker_S1 {

    public static int HammingDistance(byte[] a, byte[] b) {
        int totalD = 0;

        for (int i = 0; i < a.Length; i++) {
            int g = (int)(a[i])^(int)(b[i]);
            string J = Convert.ToString(g, 2);
            int m =J.Replace("0","").Length;
            totalD+=m;

        }
        return totalD;
    }

    public static byte[] BREAK_XOR(byte[] ciph) {
        int MIN_KEYSIZE = 2;
        int MAX_KEYSIZE = 40;
        double MIN_KEYSIZE_G = 1000;
        int TRUE_KEYSIZE = 0;
        Dictionary<int, double> KEYSIZES = new Dictionary<int, double>();
        for (int KEYSIZE = MIN_KEYSIZE; KEYSIZE <= MAX_KEYSIZE; KEYSIZE++) {
            byte[] FIRST_BLOCK = new byte[KEYSIZE], SECOND_BLOCK = new byte[KEYSIZE], THIRD_BLOCK = new byte[KEYSIZE], FOURTH_BLOCK = new byte[KEYSIZE];
            Array.Copy(ciph, 0, FIRST_BLOCK, 0, KEYSIZE);
            Array.Copy(ciph, KEYSIZE, SECOND_BLOCK, 0, KEYSIZE);
            Array.Copy(ciph, KEYSIZE*2, THIRD_BLOCK, 0, KEYSIZE);
            Array.Copy(ciph, KEYSIZE*3, FOURTH_BLOCK, 0, KEYSIZE);

            double HamD12 = HammingDistance(FIRST_BLOCK, SECOND_BLOCK);
            double HamD13 = HammingDistance(FIRST_BLOCK, THIRD_BLOCK);
            double HamD14 = HammingDistance(FIRST_BLOCK, FOURTH_BLOCK);
            double HamD23 = HammingDistance(SECOND_BLOCK, THIRD_BLOCK);
            double HamD24 = HammingDistance(SECOND_BLOCK, FOURTH_BLOCK);
            double HamD34 = HammingDistance(THIRD_BLOCK, FOURTH_BLOCK);
            double HamD = HamD12 + HamD13 + HamD14 + HamD23 + HamD24 + HamD34;
            HamD = HamD / 6.0;
            double KEYSIZE_G = HamD/(KEYSIZE);
            if (MIN_KEYSIZE_G >= KEYSIZE_G) {
                MIN_KEYSIZE_G = KEYSIZE_G;

                TRUE_KEYSIZE = KEYSIZE;
            }
            KEYSIZES.Add(KEYSIZE, KEYSIZE_G);
        }




        List<byte[]> blocks = new List<byte[]>();
        byte[] ciph_extd = new byte[extendToSize(ciph.Length, TRUE_KEYSIZE)];

        int DIFF = ciph_extd.Length - ciph.Length;
        Array.Copy(ciph, 0, ciph_extd, 0, ciph.Length);
        for (int i = 0; i < ciph_extd.Length; i += TRUE_KEYSIZE) {
            byte[] block_tmp = new byte[TRUE_KEYSIZE];
            Array.Copy(ciph_extd, i, block_tmp, 0, TRUE_KEYSIZE);
            blocks.Add(block_tmp);
        }

            if (DIFF != 0) {
                byte[] lastBlock = new byte[TRUE_KEYSIZE - DIFF];
                Array.Copy(blocks[blocks.Count - 1], 0, lastBlock, 0, lastBlock.Length);
                blocks.RemoveAt(blocks.Count - 1);
                blocks.Add(lastBlock);
            }


        List<byte[]> TRANS_BLOCKS = new List<byte[]>();
        for (int i = 0; i < TRUE_KEYSIZE; i++) {
            byte[] block_tmp = new byte[blocks.Count];
            for (int k = 0, j = 0; k < blocks.Count; k++, j++) {
                byte[] e = blocks[k];
                bool ifOK = i >= e.Length;
                byte set = 0;
                if (ifOK == false) set = e[i];
                block_tmp[j] = set;
            }
            TRANS_BLOCKS.Add(block_tmp);

        }



        byte [] resultKey = new byte[TRUE_KEYSIZE];
        int rK = 0;
        Console.WriteLine(TRANS_BLOCKS.Count);
        foreach (byte[] block in TRANS_BLOCKS) {
            byte g = XorBreaker_S1.BreakChar(block);
            resultKey[rK] = g;
            rK++;
        }


            return XorUtil_S1.RXOR(ciph, resultKey);
    }

    public static int extendToSize(int variable, int size) {
        if (variable % size == 0) return variable;
        return (variable - (variable % size) + size);
    }
}
