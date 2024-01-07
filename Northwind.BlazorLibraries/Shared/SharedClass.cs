namespace Packt.Shared;

public static class NorthwindExtensionMethods
{
    public static string ConvertToBase64Jpeg(this byte[] picture)
    {
        string base64 = string.Format("data:image/jpg;base64,{0}",
            Convert.ToBase64String(picture));
        return base64;
    }
}