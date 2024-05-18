using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Barracuda;
using UnityEngine;
using UnityEngine.UI;

public class MainManager : MonoBehaviour
{
    [SerializeField] private TMP_Text inputValue;
    [SerializeField] private TMP_Text outputValue;
    [SerializeField] private NNModel myModel;
    
    private int _lengthInput = 10;
    private Model _runtimeModel;
    private IWorker _worker;
    private string _output;
    
    // Start is called before the first frame update
    void Start()
    {
        _runtimeModel = ModelLoader.Load(myModel);
        _worker = WorkerFactory.CreateWorker(WorkerFactory.Type.Auto, _runtimeModel);
        var count = _runtimeModel.outputs.Count;
        _output = _runtimeModel.outputs[count - 1];
    }

    public void Predict()
    {
        float[] randomData = GenerateRandomInputData();

        inputValue.text = randomData.Aggregate("Input: \n", (current, t) => current + (t + "\n"));
        
        using var tensor = new Tensor(1, _lengthInput, randomData);
        
        _worker.Execute(tensor);
        
        Tensor outputTensor = _worker.PeekOutput(_output);
        
        float[] outputArray = outputTensor.AsFloats();
        outputValue.text = outputArray.Aggregate("Output: \n", (current, t) => current + (t + "\n"));
        
        tensor.Dispose();
        outputTensor.Dispose();
    }
    
    float[] GenerateRandomInputData()
    {
        float[] randomData = new float[_lengthInput];
        for (int i = 0; i < _lengthInput; i++)
        {
            randomData[i] = UnityEngine.Random.Range(0f, 1f);
        }
        return randomData;
    }

    private void OnDestroy()
    {
        _worker?.Dispose();
    }
}