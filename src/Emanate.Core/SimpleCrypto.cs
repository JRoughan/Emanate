using System.Text;

namespace Emanate.Core
{
    public static class SimpleCrypto
    {
        // TODO: Extrmemely simplistic encrytion used here - will keep honest people honest but not much else
        public static string EncryptDecrypt(string text)
        {
            var outSb = new StringBuilder(text.Length);
            foreach (var c in text)
            {
                var xored = (char)(c ^ 129);
                outSb.Append(xored);
            }
            return outSb.ToString();
        }
    }
}
