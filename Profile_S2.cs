using System;
using System.Security.Cryptography;
using System.Text;

public class Profile_S2 {

    public string EMAIL;
    public uint ID;
    public string ROLE;


    public Profile_S2(string EMAIL, uint ID, string ROLE) {
        this.EMAIL = EMAIL;
        this.ID = ID;
        this.ROLE = ROLE;
    }

    public static Profile_S2 ParseData(byte[] PROFILE_DATA_O, byte[] key) {
        string PROFILE_DATA = Encoding.ASCII.GetString(CipherUtil_S2.decryption_oracle_3(PROFILE_DATA_O, key));
        string[] TYPES = PROFILE_DATA.Split(new string[]{"=", "&"}, StringSplitOptions.None);
        string EMAIL = TYPES[1];
        uint ID = Convert.ToUInt32(TYPES[3]);
        string ROLE = TYPES[5];
        return new Profile_S2(EMAIL, ID, ROLE);
    }



    public static string ToData(Profile_S2 PROFILE) {
       return "email=" + PROFILE.EMAIL + "&uid="+PROFILE.ID+"&role="+PROFILE.ROLE;
    }

    public static byte[] ProfileFor(string EMAIL, byte[] key) {
        if (EMAIL.Contains("&") || EMAIL.Contains("=")) {
          throw new ArgumentException("INVALID ID");
        }
        byte[] buffer = new byte[sizeof(uint)];
        new RNGCryptoServiceProvider().GetBytes(buffer);
        uint ID_C = BitConverter.ToUInt32(buffer,0);
        ID_C = (uint) Math.Abs(ID_C) % 9;
        string ROLE = "user";
        return CipherUtil_S2.encryption_oracle_3(Encoding.ASCII.GetBytes(ToData(new Profile_S2(EMAIL, ID_C, ROLE))), key);
    }


}
