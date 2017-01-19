using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public delegate void BatchTransfer(List<CellMetabolism> parentals);

public class Culture : MonoBehaviour
{
    [SerializeField]
    float lagEffect = 1f;

    public static event BatchTransfer OnNewBatch;

    [SerializeField]
    CellMetabolism prefab;

    [SerializeField]
    int populationStartSize = 25;

    public int populationMaxSize = 3500;

    [SerializeField]
    Transform beaker;

    [SerializeField]
    float maxCycleDuration = 72;

    [SerializeField, Range(0, 3)]
    float startDiameter = 0.75f;

    [SerializeField]
    GraphSprite popSizePlot;

    void Start()
    {

        if (popSizePlot)
        {
            popSizePlot.SetYLim(populationStartSize * 0.75f, populationMaxSize);

        }

        StartCoroutine(CreatePopulation());
    }

    enum PopulationStatus { UnderConstruction, ReadyFirstPop, Running};
    PopulationStatus populationStatus = PopulationStatus.UnderConstruction;

    IEnumerator<WaitForSeconds> CreatePopulation() {
        populationStatus = PopulationStatus.UnderConstruction;
        for (int i=0; i<populationMaxSize; i++)
        {
            CreateFounder(i);
            if (i % 10 == 0)
            {
                yield return new WaitForSeconds(0.0016f);
            }
        }
        populationStatus = PopulationStatus.ReadyFirstPop;
    }

    CellMetabolism CreateFounder(int index)
    {
        CellMetabolism cell = Instantiate(prefab);
        cell.transform.SetParent(transform);
        cell.culture = this;
        cell.name = "Cell " + index;
        cell.SetToInactive();
        cell.gameObject.SetActive(false);
        return cell;
    }

    CellMetabolism CreateFounder(CellMetabolism cell)
    {        
        cell.transform.position = beaker.TransformPoint(new Vector3(Random.Range(-1f, 1f), Random.Range(0f, 0.2f), Random.Range(-1f, 1f)).normalized * Random.Range(1f, startDiameter));        
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
        if ((populationStatus == PopulationStatus.ReadyFirstPop || populationStatus == PopulationStatus.Running) && mode == ManipulationMode.BatchTransfer && modeType == ManipulationEventType.Instant)
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
            cell.SetNutrientState(Random.Range(-lagEffect, 1));
            CreateFounder(cell);
            cell.enabled = true;
            CellMetabolism.AddToPopulation(cell);
            cell.gameObject.SetActive(true);
        }

        if (OnNewBatch != null)
        {
            OnNewBatch(parentals);
        }
    }

    void Update()
    {
        switch (populationStatus)
        {
            case PopulationStatus.ReadyFirstPop:
                List<CellMetabolism> founders = CellMetabolism.GetInactive(populationStartSize);
                CellMetabolism template = founders[0];
                template.CreateGenome();
                for (int i=1; i<populationStartSize; i++)
                {
                    founders[i].CopyGenome(template, true);
                    
                }
                SetStartCulture(founders);
                populationStatus = PopulationStatus.Running;
                break;
            case PopulationStatus.Running:
                if (GameTimeConverter.experimentTime > maxCycleDuration)
                {
                    CultureManipulator_OnManipulation(ManipulationMode.BatchTransfer, ManipulationEventType.Instant, new Ray(), 0);
                }
                break;
                
        }
    }
}
