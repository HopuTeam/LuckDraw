using System;
using System.Drawing;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace LuckDraw.Handles
{
    /// <summary>
    /// 密码/验证码
    /// </summary>
    public static class Security
    {
        /// <summary>
        /// MD5加密
        /// </summary>
        /// <param name="originalString">待加密字符串</param>
        /// <returns>结果</returns>
        public static string MD5Encrypt16(string originalString)
        {
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            string t2 = BitConverter.ToString(md5.ComputeHash(Encoding.Default.GetBytes(originalString)), 4, 8);
            t2 = t2.Replace("-", "");
            return t2;
        }
        /// <summary>
        /// MD5加密
        /// </summary>
        /// <param name="originalString">待加密字符串</param>
        /// <returns>结果</returns>
        public static string MD5Encrypt32(string originalString)
        {
            string pwd = "";
            MD5 md5 = MD5.Create();
            byte[] s = md5.ComputeHash(Encoding.UTF8.GetBytes(originalString));
            for (int i = 0; i < s.Length; i++)
            {
                pwd += s[i].ToString("X");
            }
            return pwd;
        }

        /// <summary>
        /// RAS加密
        /// </summary>
        /// <param name="originalString">待加密字符串</param>
        /// <param name="key">密匙</param>
        /// <returns>结果</returns>
        public static string RSAEncrypt(string originalString, string key = "RSA")
        {
            CspParameters param = new CspParameters
            {
                KeyContainerName = key
            };
            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(param))
            {
                byte[] plaindata = Encoding.Default.GetBytes(originalString);
                byte[] encryptdata = rsa.Encrypt(plaindata, false);
                return Convert.ToBase64String(encryptdata);
            }
        }

        /// <summary>
        /// RAS解密
        /// </summary>
        /// <param name="securitylString">待加密字符串</param>
        /// <param name="key">密匙</param>
        /// <returns>结果</returns>
        public static string RSADecrypt(string securitylString, string key = "RSA")
        {
            CspParameters param = new CspParameters
            {
                KeyContainerName = key
            };
            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(param))
            {
                byte[] encryptdata = Convert.FromBase64String(securitylString);
                byte[] decryptdata = rsa.Decrypt(encryptdata, false);
                return Encoding.Default.GetString(decryptdata);
            }
        }

        /// <summary>
        /// DES加密
        /// </summary>
        /// <param name="originalString">待加密字符串</param>
        /// <param name="key">密匙</param>
        /// <param name="iv">初始化向量</param>
        /// <returns>结果</returns>
        public static string DESEncrypt(string originalString, string key = "THISDESP", string iv = "PSEDSIHT")
        {
            byte[] btKey = Encoding.UTF8.GetBytes(key);
            byte[] btIV = Encoding.UTF8.GetBytes(iv);
            byte[] inData = Encoding.UTF8.GetBytes(originalString);
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            MemoryStream ms = new MemoryStream();
            CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(btKey, btIV), CryptoStreamMode.Write);
            cs.Write(inData, 0, inData.Length);
            cs.FlushFinalBlock();
            string securtyString = Convert.ToBase64String(ms.ToArray());
            cs.Close();
            ms.Close();
            return securtyString;
        }

        /// <summary>
        /// DES解密
        /// </summary>
        /// <param name="securityString">待加密字符串</param>
        /// <param name="key">密匙</param>
        /// <param name="iv">初始化向量</param>
        /// <returns>结果</returns>
        public static string DESDecrypt(string securityString, string key = "THISDESP", string iv = "PSEDSIHT")
        {
            byte[] inData;
            try
            {
                inData = Convert.FromBase64String(securityString);
            }
            catch (Exception)
            {
                return null;
            }

            byte[] btKey = Encoding.UTF8.GetBytes(key);
            byte[] btIV = Encoding.UTF8.GetBytes(iv);
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            MemoryStream ms = new MemoryStream();
            CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(btKey, btIV), CryptoStreamMode.Write);
            cs.Write(inData, 0, inData.Length);
            try
            {
                cs.FlushFinalBlock();
            }
            catch (Exception)
            {
                ms.Close();
                return null;
            }
            string originalString = Encoding.UTF8.GetString(ms.ToArray());
            cs.Close();
            ms.Close();
            return originalString;
        }
        /// <summary>
        /// 生成验证码图片
        /// </summary>
        /// <param name="codes">需要生成的验证码</param>
        /// <param name="width">宽度</param>
        /// <param name="height">高度</param>
        /// <returns>结果图片</returns>
        //public static Bitmap VerifiedImage(string codes)
        //{
        //    Bitmap map = new Bitmap(codes.Length * 25, 36);
        //    Graphics graph = Graphics.FromImage(map);
        //    graph.Clear(Color.AliceBlue);
        //    graph.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
        //    Random rand = new Random();
        //    Pen[] blackPens = new Pen[] { new Pen(Color.Yellow, 3), new Pen(Color.LightSteelBlue, 3), new Pen(Color.MediumTurquoise, 3), new Pen(Color.Pink, 3), new Pen(Color.Gold, 3) };
        //    for (int i = 0; i < 50; i++)
        //    {
        //        int x = rand.Next(0, map.Width);
        //        int y = rand.Next(0, map.Height);
        //        graph.DrawRectangle(blackPens[rand.Next(0, blackPens.Length)], x, y, 1, 1);
        //    }
        //    char[] chars = codes.ToCharArray();
        //    StringFormat format = new StringFormat(StringFormatFlags.NoClip)
        //    {
        //        Alignment = StringAlignment.Center,
        //        LineAlignment = StringAlignment.Center
        //    };
        //    Color[] c = { Color.YellowGreen, Color.LightSlateGray, Color.MediumSpringGreen, Color.Plum, Color.Goldenrod };
        //    string[] font = { "Verdana", "Microsoft Sans Serif", "Comic Sans MS", "Arial", "宋体" };
        //    for (int i = 0; i < chars.Length; i++)
        //    {
        //        int cindex = rand.Next(c.Length);
        //        int findex = rand.Next(font.Length);
        //        Font f = new Font(font[findex], rand.Next(10, 23), FontStyle.Bold);
        //        Brush b = new SolidBrush(c[cindex]);
        //        graph.DrawString(chars[i].ToString(), f, b, i * 25 + 12, 18, format);
        //    }
        //    return map;
        //}
    }

}
