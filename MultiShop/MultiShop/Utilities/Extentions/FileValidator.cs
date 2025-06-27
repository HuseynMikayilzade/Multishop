namespace MultiShop.Utilities.Extentions
{
    public static class FileValidator
    {
        public static void DeleteFile(this string filename, string root, params string[] folders)
        {
            for (int i = 0; i < folders.Length; i++)
            {
                root = Path.Combine(root, folders[i]);
            }
            root = Path.Combine(root, filename);

            if (File.Exists(root))
            {
                File.Delete(root);
            }
        }
    }
}
