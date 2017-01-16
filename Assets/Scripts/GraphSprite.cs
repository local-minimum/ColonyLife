using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

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
            targetImage.preserveAspect = true;
        }
        Redraw();
    }

    public void SetData(float[] X, float[] Y)
    {
        int n = LinesX.Count - 1;
        LinesX[n] = X;
        if (yLog2)
        {
            LinesY[n] = Y.Select(e => Mathf.Log(e) / log2).ToArray();
        }
        else {
            LinesY[n] = Y;
        }
        Redraw();

    }

    public void Plot(float[] X, float[] Y)
    {
        LinesX.Add(X);
        if (yLog2)
        {
            LinesY.Add(Y.Select(e => Mathf.Log(e) / log2).ToArray());
        }
        else {
            LinesY.Add(Y);
        }
        Redraw();
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

    [SerializeField]
    Color curveGradientColor;

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
        SetXTicks();
    }

    [SerializeField]
    float[] xTicks;

    public void SetXTicks()
    {
        float minVal = xMin;
        float maxVal = xMax;

        int firstTick = Mathf.FloorToInt(minVal);
        int lastTick = Mathf.CeilToInt(maxVal);
        xTicks = Enumerable.Range(firstTick, lastTick - firstTick + 1).Select(e => (float)e).ToArray();
    }

    [SerializeField]
    float yMin = 0;

    [SerializeField]
    float yMax = 5;

    public void SetYLim(float ymin, float ymax)
    {
        if (yLog2)
        {
            yMin = Mathf.Log(ymin) / log2;
            yMax = Mathf.Log(ymax) / log2;
        }
        else {
            yMin = ymin;
            yMax = ymax;
        }
        
        SetYTicks();
    }

    [SerializeField]
    float[] yTicks;

    float log2 = Mathf.Log(2);

    public void SetYTicks()
    {
        float minVal = yMin;
        float maxVal = yMax;
        int firstTick = Mathf.FloorToInt(minVal);
        int lastTick = Mathf.CeilToInt(maxVal);
        yTicks = Enumerable.Range(firstTick, lastTick - firstTick + 1).Select(e => (float)e).ToArray();
    }
    
    [SerializeField]
    bool yLog2 = true;

    public void Redraw()
    {
        int spineWidth = 1;
        int tickLength = 2;
        int padding = 2;

        int lower = padding + tickLength + spineWidth;

        ClearImage();
        DrawCurves(lower, imageWidth - lower - padding, lower, imageHeight - lower - padding);
        DrawSpines(padding, spineWidth, tickLength);

        tex.Apply();

    }

    void ClearImage()
    {
        for (int y=0; y<imageHeight; y++)
        {
            for (int x=0; x<imageWidth; x++)
            {
                tex.SetPixel(x, y, backgroundColor);
            }
        }
    }

    bool curveGradient = false;

    void DrawCurves(int left, int right, int bottom, int top)
    {
        int xSpan = right - left;
        int ySpan = top - bottom;
        
        for (int c=0, nC=LinesX.Count(); c < nC; c++)
        {
            Color lineColor = curveGradient ? Color.Lerp(curveColor, curveGradientColor, c / (float)(nC - 1)) : (c == nC - 1 ? curveColor : curveGradientColor);
            float[] X = LinesX[c];
            float[] Y = LinesY[c];
            int l = X.Length - 1;
            for (int i=0; i< l; i++)
            {
                DrawInterpolation(X[i], X[i + 1], Y[i], Y[i + 1], lineColor, left, bottom, xSpan, ySpan);
            }
        }
    }

    void DrawInterpolation(float x1, float x2, float y1, float y2, Color c, int left, int bottom, int xSpan, int ySpan, int depth=0)
    {

        int xLeft = ClampInterpolate(x1, xMin, xMax, xSpan) + left;
        int xRight = ClampInterpolate(x2, xMin, xMax, xSpan) + left;
        int yLeft = ClampInterpolate(y1, yMin, yMax, ySpan) + bottom;
        int yRight = ClampInterpolate(y2, yMin, yMax, ySpan) + bottom;
        //Debug.Log(xLeft + " " + xRight + ", " + yLeft + " " + yRight + " : " + depth);
        if ((xLeft != xRight || yLeft != yRight) && depth < 5)
        {
            DrawInterpolation(x1, Mathf.Lerp(x1, x2, 0.5f), y1, Mathf.Lerp(y1, y2, 0.5f), c, left, bottom, xSpan, ySpan, depth + 1);
            DrawInterpolation(Mathf.Lerp(x1, x2, 0.5f), x2, Mathf.Lerp(y1, y2, 0.5f), y2, c, left, bottom, xSpan, ySpan, depth + 1);
        } else
        {
            tex.SetPixel(xLeft, yLeft, c);
        }
    }

    Color spineColor = Color.black;

    void DrawSpines(int padding, int spineWidth, int tickLength)
    {
        int spinePos = padding + tickLength;
        for (int y=spinePos, l=imageHeight - padding - spinePos; y< l; y++)
        {
            tex.SetPixel(spinePos, y, spineColor);
        }

        for (int x = spinePos, l = imageWidth - padding - spinePos; x < l; x++)
        {
            tex.SetPixel(x, spinePos, spineColor);
        }
    }

    int ClampInterpolate(float val, float min, float max, int pixels)
    {
        return Mathf.RoundToInt((Mathf.Clamp(val, min, max) - min) / (max - min) * pixels);
    }
}
