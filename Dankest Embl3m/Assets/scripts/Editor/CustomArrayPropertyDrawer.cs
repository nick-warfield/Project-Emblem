//https://docs.unity3d.com/Manual/editor-PropertyDrawers.html
//https://www.youtube.com/watch?v=uoHc-Lz9Lsc&index=9&t=2s&list=PLiwDqWg58CHUr6Z2RaZZ_Vb-ZdtQUGAvM

/*

using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

//letting unity know that we are creating a custom drawer for the class ArrayLayout
[CustomPropertyDrawer(typeof(ArrayLayout))]
public class CustomArrayPropertyDrawer : PropertyDrawer
{
    //override the default gui for a drawer
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.PrefixLabel(position, label);
        Rect newPosition = position;

        //gets rid of the word "element" before each gameobject
        label.text = "";

        //18 is about the number of pixels in a line on the editor
        newPosition.y += 22f;

        //7 is used becuase right now the array length is fixed,
        //but by dividing the total width by the number of elements in a row we make the rows distribute evenly
        //even if we adjust the editor width
        newPosition.width = position.width / MapWidth;

        //here we set the number of rows displayed in the editor with this for loop
        //we use 7 because we know how many rows there are
        for (int j = 0; j < MapHeight; j++)
        {
            //grab the GameObject array from ArrayLayout
            SerializedProperty data = property.FindPropertyRelative("rows");

            //grab each element from the RowData array in ArrayLayout
            SerializedProperty row = data.GetArrayElementAtIndex(j).FindPropertyRelative("column");

            //we use 7 because we know how many rows there are
            row.arraySize = MapHeight;

            //sets every line to have the same height
            newPosition.height = 18f;

            //this for loops makes the columns of the gui
            for (int i = 0; i < MapWidth; i++)
            {
                EditorGUI.PropertyField(newPosition, row.GetArrayElementAtIndex(i), label);
                newPosition.x += newPosition.width;
            }

            //reset and set up for next loop
            newPosition.x = position.x;
            newPosition.y += 22f;
        }
        //base.OnGUI(position, property, label);
    }


    //let unity know the number of pixels this stuff will take up for height
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return 22f * (MapHeight + 1);

        //return base.GetPropertyHeight(property, label);
    }

}
*/