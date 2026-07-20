namespace WebApp.Services;

public static class Helper
{
    public static string RandomString(int len)
    {
        Random rand = new Random();
        string pattern = "123456789qwertyuiopasdfghjklzxcvbnm";
        char[] arr = new char[len];

        for (int i = 0; i < len; i++)
        {
            arr[i] = pattern[rand.Next(pattern.Length)];
        }

        return new string(arr);
    }
}