using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class PaletteSwapLookup : MonoBehaviour
{
    public Texture LookupTexture;
    Material _mat;


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
        { _mat.SetTexture("_PaletteTex", LookupTexture); print("hit"); }
    }
}