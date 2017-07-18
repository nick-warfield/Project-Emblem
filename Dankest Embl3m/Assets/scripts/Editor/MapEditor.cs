using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class MapEditor : EditorWindow
{
    //float Break = 1.23f;
    bool GenerateMap;
    bool RefreshMap;

    //for grid selection and use
    GUIContent[] Buttons = new GUIContent[1] { new GUIContent("", new Texture(), "") };
    int buttonIndex = 0;
    Vector2 scroll;

    int Width = 5;
    int Height = 5;

    Terrain Filler;
    Terrain[,] TerrainArray = new Terrain[0, 0] { };
    Terrain[,] TerrainManip = new Terrain[0, 0] { };

    //GameObject[] Tiles;

    [MenuItem("Window/Map Editor")]
    static void Init ()
    {
        MapEditor window = (MapEditor)EditorWindow.GetWindow(typeof(MapEditor));
        window.Show();
    }


    //the visual interface of the map editor
    private void OnGUI()
    {
        //First Header
        GUILayout.Label("Update Map Functions", EditorStyles.boldLabel);

        //Update Buttons, put on the same row
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Generate New Map") )
        { GenerateNewMap(); }

        if (GUILayout.Button("Refresh Current Map") )
        { RefreshCurrentMap(); }
        
        if (GUILayout.Button("Discard Changes") )
        { DiscardChanges(); }

        GUILayout.EndHorizontal();
        EditorGUILayout.Separator();

        //Second Header, for the map specifiactions (dimensions, tiles, etc)
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Map Specifications", EditorStyles.boldLabel);
        
        //Default Tile
        if (Filler == null)
        { Filler = AssetDatabase.LoadAssetAtPath<Terrain>("Assets/Prefabs/Terrain/Plain.prefab"); }
        Filler = EditorGUILayout.ObjectField("Default Tile", Filler, typeof(Terrain), true) as Terrain;

        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        //Dimensions
        Width = EditorGUILayout.IntSlider("Width", Width, 5, 40);
        Height = EditorGUILayout.IntSlider("Height", Height, 5, 40);
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.Separator();


        //Selected Terrain Interface
        int x = -1;
        int y = -1;
        string selected = "None";
        if (buttonIndex >= 0 && buttonIndex < Buttons.Length)
        {
            if (Buttons[buttonIndex].tooltip != "")
            {
                x = int.Parse(Buttons[buttonIndex].tooltip);
                y = x % 100;
                x /= 100;

                selected = Buttons[buttonIndex].text;
            }
        }

        EditorGUILayout.BeginHorizontal();

        EditorGUILayout.LabelField("Selected Terrain: " + selected);

        if (x >= 0 && x < TerrainManip.GetLength(0) && y >= 0 && y < TerrainManip.GetLength(1))
        {
            Terrain temp = TerrainManip[x, y];
            temp.Tag = (Terrain.tileTag)EditorGUILayout.EnumPopup("New Type:", temp.Tag);
        }

        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space();

        //Map Grid, for changing a tile and the individual properties of a tile
        scroll = EditorGUILayout.BeginScrollView(scroll);

        Buttons = new GUIContent[Width * Height];
        int k = 0;
        for (int i = Height - 1; i >= 0; i--)
        {
            for (int j = 0; j < Width; j++)
            {
                string txt = "";
                Texture spr = new Texture();
                string ttp = "";

                if (j < TerrainManip.GetLength(0) && i < TerrainManip.GetLength(1) )
                {
                    txt = "(" + i + ", " + j + ")";
                    spr = TerrainManip[j, i].GetComponent<SpriteRenderer>().sprite.texture;
                    int num = (j * 100 + i);
                    ttp = num.ToString();
                    //ttp.PadLeft(4, '0');
                }

                Buttons[k] = new GUIContent(txt, spr, ttp);
                k++;
            }
        }

        buttonIndex = GUILayout.SelectionGrid(buttonIndex, Buttons, Width);

        EditorGUILayout.EndScrollView();
    }


    void DiscardChanges()
    {
        TerrainManip = TerrainArray;
    }


    void RefreshCurrentMap()
    {
        //Find any of the gameobjects that have been created previously
        GameObject currentMap = GameObject.Find("Terrain");

        //Then destroy them if found
        if (currentMap != null)
        {
            foreach (Object child in currentMap.transform)
            { DestroyImmediate(child as GameObject); }

            DestroyImmediate(currentMap);
        }

        //Create a new Gameobject to hold all of the new tiles that will be generated.
        GameObject newMap = new GameObject("Terrain");

        //Loop through all of the array
        for (int i = 0; i < TerrainManip.GetLength(0); i++)
        {
            for (int j = 0; j < TerrainManip.GetLength(1); j++)
            {
                string path = "Assets/Prefabs/Terrain/" + TerrainManip[i, j].Tag.ToString() + ".prefab";
                Terrain load = AssetDatabase.LoadAssetAtPath<Terrain>(path);
                GameObject tile = PrefabUtility.InstantiatePrefab(load.gameObject) as GameObject;

                tile.transform.position = new Vector3(i, j, 0);
                tile.transform.parent = newMap.transform;
                tile.name = ("(" + i + ", " + j + ") " + tile.name);
                TerrainManip[i, j] = tile.GetComponent<Terrain>();
            }
        }

        TerrainArray = TerrainManip;
    }


    void GenerateNewMap()
    {
        //Create a new Reference.
        TerrainArray = new Terrain[Width, Height];

        //Find any of the gameobjects that have been created previously
        GameObject currentMap = GameObject.Find("Terrain");

        //Then destroy them if found
        if (currentMap != null)
        {
            foreach (Object child in currentMap.transform)
            { DestroyImmediate(child as GameObject); }

            DestroyImmediate(currentMap);
        }

        //Create a new Gameobject to hold all of the new tiles that will be generated.
        GameObject newMap = new GameObject("Terrain");

        //Loop through the 'Width' of the Terrain Array
        for (int i = 0; i < TerrainArray.GetLength(0); i++)
        {
            //Loop through the 'Height' of the Terrain Array
            for (int j = 0; j < TerrainArray.GetLength(1); j++)
            {
                GameObject tile = PrefabUtility.InstantiatePrefab(Filler.gameObject) as GameObject;
                tile.transform.position = new Vector3(i, j, 0);
                tile.transform.parent = newMap.transform;
                tile.name = ("(" + i + ", " + j + ") " + Filler.name);
                TerrainArray[i, j] = tile.GetComponent<Terrain>();
            }
        }

        TerrainManip = TerrainArray;

        if (GameObject.Find("Grid") == null)
        {
            GameObject g = new GameObject("Grid");

            for (int i = 0; i < TerrainManip.GetLength(0); i++)
            {
                for (int j = 0; j < TerrainManip.GetLength(1); j++)
                {
                    GameObject go = PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/UI/Grid.prefab")) as GameObject;
                    go.transform.position = new Vector3(i, j, -0.25f);
                    go.transform.parent = g.transform;
                }
            }
        }
    }

}
