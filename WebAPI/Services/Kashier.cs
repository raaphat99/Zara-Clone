namespace WebAPI.Services
{
    using System.Security.Cryptography;


    public class Kashier
    {
        public static string create_hash(string _amount, string _orderId)
        {
            string mid = "MID-29117-281"; //your merchant id
            string amount = _amount;
            string currency = "EGP";
            string orderId = _orderId;
            string secret = "ece99206-54c5-44d8-9a83-e16f1c0ec09d";
            string path = "/?payment=" + mid + "." + orderId + "." + amount + "." + currency;
            string message;
            string key;
            key = secret;
            message = path;
            System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();
            byte[] keyByte = encoding.GetBytes(key);
            byte[] messageBytes = encoding.GetBytes(message);
            HMACSHA256 hmacmd256 = new HMACSHA256(keyByte);
            byte[] hashmessage = hmacmd256.ComputeHash(messageBytes);
            return ByteToString(hashmessage).ToLower();
        }
        public static string ByteToString(byte[] buff)
        {
            string sbinary = "";
            for (int i = 0; i < buff.Length; i++)
            {
                sbinary += buff[i].ToString("X2"); // hex format
            }
            return (sbinary);
        }
    }
}

