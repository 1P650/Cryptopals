using System;
using System.Collections.Generic;


public class XorBreaker {

    public static KeyValuePair<string, double> Break(byte[] c, int m){
        List<KeyValuePair<string, double>> SCORES = new List<KeyValuePair<string, double>>();
        for(byte i = 0; i < 128; i++){
            byte[] guess = XorUtil.XOR(c, i);
            double guessScore = Score(guess);
                SCORES.Add(new KeyValuePair<string, double>(HexUtil.ByteArrayToString(guess), guessScore));
            }

        SCORES.Sort(
                delegate(KeyValuePair<string, double> firstPair,
                        KeyValuePair<string, double> nextPair)
                {
                    return firstPair.Value.CompareTo(nextPair.Value);
                }
        );

        return SCORES[SCORES.Count-1];

    }

    public static byte BreakChar(byte[] c){
        Dictionary<KeyValuePair<string, double> , byte> l = new Dictionary<KeyValuePair<string, double>,byte>();
        List<KeyValuePair<string, double>> SCORES = new List<KeyValuePair<string, double>>();
        for(byte i = 0; i < 128; i++){
            byte[] guess = XorUtil.XOR(c, i);
            double guessScore = Score(guess);
            KeyValuePair<string, double> curr = new KeyValuePair<string, double>(HexUtil.ByteArrayToString(guess), guessScore);
            SCORES.Add(curr);
            l.Add( curr, i);
        }

        SCORES.Sort(
                delegate(KeyValuePair<string, double> firstPair,
                        KeyValuePair<string, double> nextPair)
                {
                    return firstPair.Value.CompareTo(nextPair.Value);
                }
        );

        return l[SCORES[SCORES.Count-1]];

    }

    public static KeyValuePair<string, double> Break(byte[] c){
        Dictionary<string, double> SCORES = new Dictionary<string, double>();
        for(byte i = 0; i < 128; i++){
            byte[] guess = XorUtil.XOR(c, i);
            double guessScore = Score(guess);

            if (!SCORES.ContainsKey(HexUtil.ByteArrayToString(guess)))
            {
                SCORES.Add(HexUtil.ByteArrayToString(guess), guessScore);
            }


        }


        List<KeyValuePair<string, double>> SCORES_SORTED = new List<KeyValuePair<string, double>>(SCORES);
        SCORES_SORTED.Sort(
                delegate(KeyValuePair<string, double> firstPair,
                        KeyValuePair<string, double> nextPair)
                {
                    return firstPair.Value.CompareTo(nextPair.Value);
                }
        );

        return SCORES_SORTED[SCORES_SORTED.Count-1];

    }

    public static KeyValuePair<string, double> Break(string[] c){
        List<KeyValuePair<string, double>> SCORES = new List<KeyValuePair<string, double>>();
        for (int k = 0; k < c.Length; k++){
            KeyValuePair<string, double> guessCurr =  Break(HexUtil.HexStrToByteArray(c[k]),k );
            SCORES.Add(guessCurr);
        }
        SCORES.Sort(
                delegate(KeyValuePair<string, double> firstPair,
                        KeyValuePair<string, double> nextPair)
                {
                    return firstPair.Value.CompareTo(nextPair.Value);
                }
        );
        int i = 0;
        foreach (KeyValuePair<string , double> q in SCORES){
            i++;
           // System.Console.WriteLine(i + ": " + q.Key);
        }

        return SCORES[SCORES.Count-1];



    }

    public static double Score(byte[] guess){
        double SCORE_ENG = 0;
        Dictionary<char, double> ENG_FREQUENCY = new Dictionary<char, double>(){
            {'a', 0.0651738}, {'b', 0.0124248}, {'c', 0.0217339}, {'d', 0.0349835}, {'e', 0.1041442}, {'f', 0.0197881}, {'g', 0.0158610},
            {'h', 0.0492888}, {'i', 0.0558094}, {'j', 0.0009033}, {'k', 0.0050529}, {'l', 0.0331490}, {'m', 0.0202124}, {'n', 0.0564513},
            {'o', 0.0596302}, {'p', 0.0137645}, {'q', 0.0008606}, {'r', 0.0497563}, {'s', 0.0515760}, {'t', 0.0729357}, {'u', 0.0225134},
            {'v', 0.0082903}, {'w', 0.0171272}, {'x', 0.0013692}, {'y', 0.0145984}, {'z', 0.0007836},
            {'A', 0.0651738}, {'B', 0.0124248}, {'C', 0.0217339}, {'D', 0.0349835}, {'E', 0.1041442}, {'F', 0.0197881}, {'G', 0.0158610},
            {'H', 0.0492888}, {'I', 0.0558094}, {'J', 0.0009033}, {'K', 0.0050529}, {'L', 0.0331490}, {'M', 0.0202124}, {'N', 0.0564513},
            {'O', 0.0596302}, {'P', 0.0137645}, {'Q', 0.0008606}, {'R', 0.0497563}, {'S', 0.0515760}, {'T', 0.0729357}, {'U', 0.0225134},
            {'V', 0.0082903}, {'W', 0.0171272}, {'X', 0.0013692}, {'Y', 0.0145984}, {'Z', 0.0007836}
        };
        foreach (byte g in guess){
            if(g == 32) SCORE_ENG+=1;
           if(g >= 65  && g <= 90) SCORE_ENG+=1;
            if(g >= 97 && g <= 122) SCORE_ENG+=2;
            char gE = Convert.ToChar(g);
            char gm = Convert.ToString(g).ToLower()[0];
            if(ENG_FREQUENCY.ContainsKey(gm)) SCORE_ENG+=ENG_FREQUENCY[gm]*10;
            else SCORE_ENG-=0.25;
        }
        //SCORE_ENG = SCORE_ENG/guess.Length * 100;
        return SCORE_ENG;
    }

}
