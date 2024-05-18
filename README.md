# Loading an ONNX Deep Learning Model in Unity

## Prerequisites

Before loading your ONNX deep learning model in Unity, you need to convert your Keras model to ONNX format. Ensure you have the following requirements:

- `tensorflow==2.16.1`
- `keras==3.3.3`
- `tf2onnx==1.16.1`

### Step 1: Save Your Keras Model

First, save your Keras model in the `.h5` format.

```python
import tensorflow as tf
from tensorflow.keras import models

# Load your pre-trained Keras model
model = models.load_model("./my_model.h5")

# Save the model in TensorFlow's SavedModel format
tf.saved_model.save(model, "my_model")
```

### Step 2: Convert the SavedModel to ONNX Format

Next, convert the SavedModel to the ONNX format using `tf2onnx`.

```shell
python -m tf2onnx.convert --saved-model ./my_model --output my_model.onnx
```

## Loading the ONNX Model in Unity

After converting your model, you can load it into Unity for use in your applications. Follow these steps to integrate the ONNX model into your Unity project.

### Step 1: Set Up Unity Project

1. **Create a new Unity project or open an existing one.**

2. **Install the necessary packages:**
   - Install the Barracuda package from the Unity Package Manager. Barracuda is Unity's inference library for neural networks and supports ONNX models.

   ```json
   {
     "dependencies": {
       "com.unity.barracuda": "3.0.0"
     }
   }
   ```

### Step 2: Import the ONNX Model

1. **Copy the ONNX model file (`my_model.onnx`) to the `Assets` folder of your Unity project.**

### Step 3: Load and Use the ONNX Model in Unity

1. **Create a new C# script to handle the model loading and inference. Below is an example of how you can load the model and perform predictions using Unity's Barracuda library.**

```csharp
using System;
using System.Linq;
using TMPro;
using Unity.Barracuda;
using UnityEngine;

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
```

> [!Note]
> ## Model Input
>
> The input tensor's dimensions and data must match what your model expects. Ensure you prepare your input data accordingly.