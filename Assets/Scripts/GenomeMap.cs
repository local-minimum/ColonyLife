﻿using System.Collections;
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

    private void Culture_OnNewBatch(List<CellMetabolism> parentals)
    {
        bool[][] genomes = parentals.Select(e => e.Genome).ToArray();
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

    /* Need multiple aligmnents and sorting getting them in orderish
        int dist = GenomeDistance(genomes[a], genome[b]);
        
        List<int> genomeIndices = Enumerable.Range(0, height).ToList();
        

    */

    int GenomeDistance(bool[] A, bool[] B)
    {
        int diffs = 0;
        for (int i=0; i<A.Length; i++)
        {
            if (A[i] != B[i])
            {
                diffs++;
            }
        }
        return diffs;
    }
}
