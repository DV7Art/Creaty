using UnityEngine;
using System.Collections;
using System.IO;

public static class TextureSaver
{
    public static IEnumerator SaveTextureToFile(Texture2D savedTexture, string fileName)
    {
        string fullPath = Path.Combine(Application.dataPath, fileName);
        var bytes = savedTexture.EncodeToPNG();
        File.WriteAllBytes(fullPath, bytes);

        Debug.Log("<color=orange>Saved Successfully!</color> " + fullPath);
        yield return null;
    }

    public static Texture2D LoadTextureFromFile(string filePath)
    {

        if (File.Exists(filePath))
        {
            byte[] fileData = File.ReadAllBytes(filePath);
            Texture2D texture = new Texture2D(2, 2);
            if (texture.LoadImage(fileData))
            {
                Debug.Log("Loaded texture from file: " + filePath);
                return texture;
            }
            else
            {
                Debug.LogError("Failed to load texture from file: " + filePath);
                return null;
            }
        }
        else
        {
            Debug.LogError("Texture file not found: " + filePath);
            return null;
        }
    }
}
