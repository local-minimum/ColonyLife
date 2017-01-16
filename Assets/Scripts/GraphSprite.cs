using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GraphSprite : MonoBehaviour {

    [SerializeField]
    Image targetImage;

    [SerializeField]
    int imageWidth = 200;

    [SerializeField]
    int imageHeight = 200;

    Texture2D tex;

    [SerializeField]
    Color backgroundColor = Color.white;

    List<float[]> LinesX = new List<float[]>();
    List<float[]> LinesY = new List<float[]>();

    void Start()
    {
        tex = new Texture2D(imageWidth, imageHeight);
        if (targetImage)
        {
            targetImage.sprite = Sprite.Create(tex, new Rect(0, 0, imageWidth, imageHeight), Vector2.one * 0.5f);
        }
        Redraw();
    }

    public void Plot(float[] X, float[] Y)
    {

    }

    public void Plot(float[] X, float[] Y, Color color)
    {
        SetColor(color);
        Plot(X, Y);
    }

    public void ClearPlot()
    {
        LinesX.Clear();
        LinesY.Clear();
        Redraw();    
    }

    [SerializeField]
    Color curveColor;

    public void SetColor(Color color)
    {
        curveColor = color;
    }

    public void SetBackroundColor(Color color)
    {
        backgroundColor = color;
    }

    [SerializeField]
    float xMin = 0;

    [SerializeField]
    float xMax = 72;

    public void SetXLim(float xmin, float xmax)
    {
        xMin = xmin;
        xMax = xmax;
    }


    [SerializeField]
    float yMin = 0;

    [SerializeField]
    float yMax = 72;

    public void SetYLim(float ymin, float ymax)
    {
        yMin = ymin;
        yMax = ymax;
    }

    [SerializeField]
    bool yLog2 = true;

    public void Redraw()
    {

    }

}
