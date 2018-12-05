using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Vuforia;

public class UserTargetController : MonoBehaviour, IUserDefinedTargetEventHandler
{
	ImageTargetBehaviour targetBehaviour;
	UserDefinedTargetBuildingBehaviour udtbBehaviour;
	ImageTargetBuilder.FrameQuality udtbFrameQuality;
	ObjectTracker objectTracker;
	DataSet dataSet;

	Text log;
    void Start () 
	{
		targetBehaviour = GetComponent<ImageTargetBehaviour>();
		log = GameObject.Find("Log").GetComponent<Text>();

		udtbBehaviour = GetComponent<UserDefinedTargetBuildingBehaviour>();
		if(udtbBehaviour)
		{
			udtbBehaviour.RegisterEventHandler(this);
		}
		else
		{
			log.text = "UserDefinedTargetBuildingBehaviour is NULL";
			log.color = Color.red;
		}
	}

	public void BuildTarget()
	{
		if(udtbFrameQuality == ImageTargetBuilder.FrameQuality.FRAME_QUALITY_HIGH)
		{
			//Nie można zbudować nowego - wciąż wykorzystuje stary target!
			udtbBehaviour.BuildNewTarget("UserTarget", targetBehaviour.GetSize().x);
        	log.text = "Target built";
			log.color = Color.green;
		}
		else
		{
			log.text = "Frame quality too low";
			log.color = Color.red;
		}
	}

    public void OnInitialized()
    {
		//Creating dataset
        objectTracker = TrackerManager.Instance.GetTracker<ObjectTracker>();
		if(objectTracker != null)
		{
			dataSet = objectTracker.CreateDataSet();
		}
		else
		{
			log.text = "ObjectTracker is NULL";
			log.color = Color.red;
		}
    }

	// Image quality control
    public void OnFrameQualityChanged(ImageTargetBuilder.FrameQuality frameQuality)
    {
        udtbFrameQuality = frameQuality;
    }

    public void OnNewTrackableSource(TrackableSource trackableSource)
    {
        dataSet.CreateTrackable(trackableSource, targetBehaviour.gameObject);
		objectTracker.ActivateDataSet(dataSet);
		udtbBehaviour.StartScanning();
    }
}
