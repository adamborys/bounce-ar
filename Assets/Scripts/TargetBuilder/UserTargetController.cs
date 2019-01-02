using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Vuforia;

public class UserTargetController : MonoBehaviour, IUserDefinedTargetEventHandler
{
	public static Transform UserTargetTransform;
	public TargetBuilderUIController UIController;
	ImageTargetBehaviour targetBehaviour;
	UserDefinedTargetBuildingBehaviour udtbBehaviour;
	ImageTargetBuilder.FrameQuality udtbFrameQuality;
	ObjectTracker objectTracker;
	DataSet dataSet;

	Text log;
    void Start () 
	{
		UserTargetTransform = transform;
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
		// New dataset for every (single) new target
		ImageTargetBuilder.FrameQuality frameQuality = ImageTargetBuilder.FrameQuality.FRAME_QUALITY_MEDIUM;
		if(Application.platform == RuntimePlatform.Android)
			frameQuality = ImageTargetBuilder.FrameQuality.FRAME_QUALITY_HIGH;
		if(udtbFrameQuality == frameQuality)
		{
			if(objectTracker != null)
			{
				objectTracker.DestroyAllDataSets(true);
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
			UIController.StartButton.interactable = true;
			transform.GetChild(0).gameObject.GetComponent<MeshRenderer>().enabled = true;
		}
		else
		{
			log.text = "Frame quality too low";
			log.color = Color.red;
		}
	}
}
