using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class PaletteSwapLookup : MonoBehaviour
{
    public Texture FactionPalette, SkinPalette, FixedPalette, LookupTexture;
    Material _mat;


    Texture AssemblePalette (Texture2D Faction, Texture2D Skin, Texture2D Fixed)
    {
        Texture2D tex = new Texture2D(Faction.width + Skin.width + Fixed.width, 2, TextureFormat.RGBA32, false);
        tex.wrapMode = TextureWrapMode.Clamp;
        tex.filterMode = FilterMode.Point;

        Color grabedColor = Color.clear;
        for (int i = 0; i < tex.width; i++)
        {
            if (i < Faction.width)
            { grabedColor = Faction.GetPixel(i, 0); }
            else if (i < Faction.width + Skin.width)
            { grabedColor = Skin.GetPixel(i - Faction.width, 0); }
            else
            { grabedColor = Fixed.GetPixel(i - Faction.width - Skin.width, 0); }

            tex.SetPixel(i, 1, grabedColor);
            tex.SetPixel(i, 0, Color.clear);
        }
        tex.Apply(false);
        return tex;
    }

    void OnEnable()
    {
        Shader shader = Shader.Find("Sprite/PaletteSwapLookup");

        if (_mat == null)
        { _mat = new Material(shader); }

        _mat.SetTexture("_PaletteTex", LookupTexture);
        GetComponent<SpriteRenderer>().material = _mat;
    }
    void OnDisable()
    { if (_mat != null) { DestroyImmediate(_mat); }  }

    private void Update()
    {
        if (_mat.GetTexture("_PaletteTex") != LookupTexture)
        {
            LookupTexture = AssemblePalette((Texture2D)FactionPalette, (Texture2D)SkinPalette, (Texture2D)FixedPalette);
            _mat.SetTexture("_PaletteTex", LookupTexture);
        }
    }
}