using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class GenomeMap : MonoBehaviour {

    [SerializeField]
    Image targetImage;

    List<Sprite> genomeHistory = new List<Sprite>();

    Sprite currentFounders;

    void OnEnable()
    {
        Culture.OnNewBatch += Culture_OnNewBatch;
    }

    void OnDisable()
    {
        Culture.OnNewBatch -= Culture_OnNewBatch;
    }

    void OnDestroy()
    {
        Culture.OnNewBatch -= Culture_OnNewBatch;
    }

    [SerializeField]
    Color trueColor;

    [SerializeField]
    Color falseColor;

    private void Culture_OnNewBatch(Culture culture)
    {
        bool[][] genomes = CellMetabolism.Genomes().ToArray();

        int width = genomes[0].Length;
        int height = genomes.Length;

        Texture2D tex = new Texture2D(width, height);
        currentFounders = Sprite.Create(tex, new Rect(0, 0, width, height), Vector2.one * 0.5f);

        for (int x=0; x<width; x++)
        {
            for (int y=0; y<height; y++)
            {
                tex.SetPixel(x, y, genomes[y][x] ? trueColor : falseColor);
            }
        }
        tex.Apply();
        genomeHistory.Add(currentFounders);

        if (targetImage)
        {
            targetImage.preserveAspect = true;
            targetImage.color = Color.white;
            targetImage.sprite = currentFounders;
        }
    }
}
