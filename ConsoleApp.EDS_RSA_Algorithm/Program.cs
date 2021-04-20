using System;
using System.Text;
using System.Security.Cryptography;
using System.Numerics;

namespace ConsoleApp.EDS_RSA_Algorithm
{
  class Program
  {
    static void Main(string[] args)
    {
      int open_e = 5;
      int open_n = 91;
      int secure_d = 29;
      string text = "KUZ";

      Console.WriteLine("Generation", Console.ForegroundColor = ConsoleColor.Green);
      Console.ResetColor();
      Console.WriteLine($"Text:              {text}");
      // Считаем хэш сообщения
      byte[] hash = GetHash(text, open_n);
      Console.WriteLine($"Text hash (mod n): {BytesToHexString(hash)}");

      // Получаем ЭЦП
      byte[] eds = GetEDS(hash, open_n, secure_d);
      Console.WriteLine($"EDS:               {BytesToHexString(eds)}");

      // Проверяем подлинность
      Console.WriteLine("\nVerification", Console.ForegroundColor = ConsoleColor.Green);
      Console.ResetColor();
      string text_to_check = text;
      byte[] eds_to_check = eds;
      CheckEDS(text_to_check, eds_to_check, open_e, open_n);

      Console.ReadLine();
    }

    private static byte[] GetHash(string text, int n)
    {
      using (MD5 md5 = MD5.Create())
      {
        // Получаем массив байтов исходного текста
        byte[] textBytes = Encoding.ASCII.GetBytes(text);
        // Получаем MD5 хэш для нашего текста
        byte[] hashBytes = md5.ComputeHash(textBytes);
        Console.WriteLine($"Text hash:         {BytesToHexString(hashBytes)}");
        // Берем хэш по модулю n
        for (int i = 0; i < hashBytes.Length; i++)
        {
          hashBytes[i] = (byte)(hashBytes[i] % n);
        }
        return hashBytes;
      }
    }

    private static byte[] GetEDS(byte[] hash, int n, int d)
    {
      // Считаем ЭЦП по хэшу текста с использованием
      // закрытого ключа и второй части открытого ключа
      byte[] eds = hash;
      for (int i = 0; i < hash.Length; i++)
      {
        eds[i] = (byte)BigInteger.ModPow(hash[i], d, n);
      }
      return eds;
    }

    private static void CheckEDS(string text, byte[] s, int e, int n)
    {
      byte[] hash = GetHash(text, n);
      Console.WriteLine($"Text hash (mod n): {BytesToHexString(hash)}");


      byte[] eds_hash = new byte[s.Length];
      for (int i = 0; i < s.Length; i++)
      { 
        eds_hash[i] = (byte)BigInteger.ModPow(s[i], e, n);
      }

      Console.WriteLine($"EDS hash:          {BytesToHexString(eds_hash)}");
    }

    private static string BytesToHexString(byte[] arr)
    {
      StringBuilder sb = new StringBuilder();
      for (int i = 0; i < arr.Length; i++)
      {
        sb.Append(arr[i].ToString("X2"));
      }
      return sb.ToString();
    }

  }
}
