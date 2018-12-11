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
	// On Vuforia initialization
    public void OnInitialized()
    {
        objectTracker = TrackerManager.Instance.GetTracker<ObjectTracker>();
    }

	// Image quality control
    public void OnFrameQualityChanged(ImageTargetBuilder.FrameQuality frameQuality)
    {
        udtbFrameQuality = frameQuality;
    }

	// Create trackable and activate dataset on new target
    public void OnNewTrackableSource(TrackableSource trackableSource)
    {
        dataSet.CreateTrackable(trackableSource, targetBehaviour.gameObject);
		objectTracker.ActivateDataSet(dataSet);
		udtbBehaviour.StartScanning();
    }

	public void BuildTarget()
	{
		// Every time new dataset for single target
		// TO DO dialog jeśli użytkownik chce zbudować target w jakości medium na słabym aparacie
		if(udtbFrameQuality == ImageTargetBuilder.FrameQuality.FRAME_QUALITY_HIGH)
		{
			if(objectTracker != null)
			{
				dataSet = objectTracker.CreateDataSet();
			}
			else
			{
				log.text = "ObjectTracker is NULL";
				log.color = Color.red;
				return;
			}
			udtbBehaviour.BuildNewTarget("UserTarget", targetBehaviour.GetSize().x);
        	log.text = "Target built";
			log.color = Color.green;
			TargetBuilderUIController.startButton.interactable = true;
		}
		else
		{
			log.text = "Frame quality too low";
			log.color = Color.red;
		}
	}
}
