using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public delegate void BatchTransfer(List<CellMetabolism> parentals);

public class Culture : MonoBehaviour
{

    public static event BatchTransfer OnNewBatch;

    [SerializeField]
    CellMetabolism prefab;

    [SerializeField]
    int populationStartSize = 25;

    public int populationMaxSize = 3500;

    [SerializeField]
    Transform beaker;

    [SerializeField, Range(0, 3)]
    float startDiameter = 0.75f;

    void Start()
    {
        CellMetabolism template = null;
        List<CellMetabolism> parentals = new List<CellMetabolism>();
        for (int i = 0; i < populationStartSize; i++)
        {
            CellMetabolism cell = CreateFounder();
            if (template == null)
            {
                template = cell;
                cell.CreateGenome();
            } else
            {
                cell.CopyGenome(template, true);
            }
            parentals.Add(cell);
        }

        if (OnNewBatch != null)
        {
            OnNewBatch(parentals);
        }
    }

    CellMetabolism CreateFounder()
    {
        CellMetabolism cell = Instantiate(prefab);
        cell.transform.SetParent(transform);
        return CreateFounder(cell);
    }

    CellMetabolism CreateFounder(CellMetabolism cell)
    {        
        cell.transform.position = beaker.TransformPoint(new Vector3(Random.Range(-1f, 1f), Random.Range(0f, 0.2f), Random.Range(-1f, 1f)).normalized * Random.Range(1f, startDiameter));
        cell.culture = this;
        return cell;
    }

    void OnEnable()
    {
        CultureManipulator.OnManipulation += CultureManipulator_OnManipulation;
    }

    void OnDisable()
    {
        CultureManipulator.OnManipulation -= CultureManipulator_OnManipulation;
    }

    void OnDestroy()
    {
        CultureManipulator.OnManipulation -= CultureManipulator_OnManipulation;
    }

    private void CultureManipulator_OnManipulation(ManipulationMode mode, ManipulationEventType modeType, Ray r, LayerMask layers)
    {
        if (mode == ManipulationMode.BatchTransfer && modeType == ManipulationEventType.Instant)
        {
            List<CellMetabolism> parentals = CellMetabolism.SamplePopulation(populationStartSize).ToList();
            CellMetabolism.WipePopulation();
            SetStartCulture(parentals);
        }
    }

    void SetStartCulture(List<CellMetabolism> parentals)
    {
        foreach (CellMetabolism cell in parentals)
        {
            cell.ResetAge();
            cell.SetNutrientState(0.5f);
            CreateFounder(cell);
            cell.enabled = true;
        }

        if (OnNewBatch != null)
        {
            OnNewBatch(parentals);
        }
    }
    
}
