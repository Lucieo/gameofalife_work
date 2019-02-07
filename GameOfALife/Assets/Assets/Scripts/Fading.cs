using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fading : MonoBehaviour
{
    public Texture2D fadeoutTexture; //texture that will overlay the screen
    public float fadespeed = 0.8f; // fading speed

    private int drawDepth = -1000; // texture order in draw hierarchy low number renders top
    private float alpha = 1.0f; // Texture alpha value
    private int fadeDir = -1;// Direction to fade : -1 in or ou 1

    void OnGUI(){
        //fade in fade out the alpha value using a direction a speed and deltatime to convert the operation to seconds
        alpha += fadeDir * fadespeed * Time.deltaTime;
        //force (clamp) the number between 0 and 1 Gui.color uses alpha value between 0 and 1
        alpha = Mathf.Clamp01(alpha);

        //set color of our GUI (texture or black image)
        GUI.color = new Color(GUI.color.r, GUI.color.g, GUI.color.b, alpha);
        GUI.depth = drawDepth; // make the black texture render on top
        GUI.DrawTexture( new Rect (0, 0, Screen.width, Screen.height), fadeoutTexture); // draw texture on screen
    }


    public float BeginFade(int direction){
        fadeDir = direction;
        return (fadespeed);
    }

    private void OnLevelWasLoaded(int level)
    {
        BeginFade(-1); // call the fade in function
    }

}
