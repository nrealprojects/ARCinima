using NRKernal;
using NRToolkit.Record;
using UnityEngine;

public class RecordTest : MonoBehaviour
{
    NRRecordController Recorder { get; set; }

    void Start()
    {
        Recorder = NRRecordController.Create();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R) || NRInput.GetButtonDown(ControllerButton.TRIGGER))
        {
            Recorder.StartRecord();
        }

        if (Input.GetKeyDown(KeyCode.T) || NRInput.GetButtonDown(ControllerButton.HOME))
        {
            Recorder.StopRecord();
        }
    }
}
