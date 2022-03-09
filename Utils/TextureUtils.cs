using UnityEngine;

namespace EditorShortCuts
{
    internal static class TextureUtils
    {
        public static Texture2D Copy(this Texture2D texture)
        {
            if (texture == null)
                return null;
            RenderTexture renderTex = RenderTexture.GetTemporary(
               texture.width,
               texture.height,
               0,
               RenderTextureFormat.Default,
               RenderTextureReadWrite.Linear);

            Graphics.Blit(texture, renderTex);
            RenderTexture previous = RenderTexture.active;
            RenderTexture.active = renderTex;
            Texture2D readableText = new Texture2D(texture.width, texture.height);
            readableText.ReadPixels(new Rect(0, 0, renderTex.width, renderTex.height), 0, 0);
            readableText.Apply();
            RenderTexture.active = previous;
            RenderTexture.ReleaseTemporary(renderTex);
            return readableText;
        }
    }
}
