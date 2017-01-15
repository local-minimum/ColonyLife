﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Media : MonoBehaviour {

    [SerializeField]
    Nutrient prefab;

    [SerializeField]
    int[] nutrientSizes;

    [SerializeField]
    int[] nutrientAmmounts;

    [SerializeField]
    float[] nutrientPositive;

    [SerializeField]
    float[] nutrientDetrimental;

    [SerializeField]
    Transform beaker;

	void Start () {
		for (int i=0; i<nutrientSizes.Length; i++)
        {
            CreateNutrient(nutrientSizes[i], nutrientAmmounts[i], nutrientPositive[i], nutrientDetrimental[i]);
        }
	}


    void CreateNutrient(int size, int count, float positive, float detrimental)
    {
        Nutrient template = Instantiate(prefab);
        bool[] nutrientGenome = Nutrient.CreateNutrientGenome(size);
        template.SetNutrientGenome(nutrientGenome);
        template.beaker = beaker;
        template.transform.SetParent(transform);
        template.transform.position = beaker.TransformPoint(new Vector3(Random.Range(-1, 1), 0, Random.Range(-1, 1)));
        template.SetEffects(positive, detrimental);

        for (int i = 1; i < count; i++)
        {
            Nutrient clone = Instantiate(template);
            clone.transform.SetParent(transform);
            clone.transform.position = beaker.TransformPoint(new Vector3(Random.Range(-1, 1), 0, Random.Range(-1, 1)));
        }
        Debug.Log(Nutrient.Count);
    }	
}