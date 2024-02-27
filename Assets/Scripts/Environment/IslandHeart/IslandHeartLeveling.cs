using Codice.CM.Common.Tree.Partial;
using Player;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Transactions;
using UnityEngine;
using UnityEngine.Android;

public class IslandHeartLeveling : MonoBehaviour
{
    /// <summary>
    /// This event must be subscribed to by all scripts that need to trigger when the
    /// Island Heart this script is attached to levels up
    /// </summary>
    public event EventHandler islandHeartLevelUp;

	[Tooltip("The world space where the generator is placed when attached to Island Heart")]
	[SerializeField]
	private GameObject _generatorLocation;
	private GameObject _generator;

	[Header("Island Heart Level Manager")]
	[Tooltip("Current Island Heart Level")]
	[Min(1f)]
	[SerializeField]
	private float _heartLevel = 1;

    [Tooltip("Maximum Island Heart Level")]
    [Min(2f)]
    [SerializeField]
    private float _maxHeartLevel = 2;

	[Tooltip("Multiplier to increase the experience threshold to level up the Island Heart")]
	[Min(1)]
	[SerializeField]
	private float _xpMultiplier = 1;

	[Tooltip("The experience threshold to level up the Island Heart")]
	[field: SerializeField]
	public float _xpThreshold
	{
		get;
		private set;
	} = 1;

	[Tooltip("The current amount of experience the Island Heart has")]
    [field: SerializeField]
    public float _currentXP
    {
        get;
        private set;
    } = 0;


    // Start is called before the first frame update
    void Start()
	{
		_generator = null;
	}

	public void OnTriggerEnter(Collider other)
	{
		if(other.gameObject.tag.Equals("Generator") && _generator == null)
		{
			GeneratorHandler handler = other.gameObject.GetComponent<GeneratorHandler>();
			handler.SetCharge(_currentXP);
		}
		if(!other.gameObject.tag.Equals("Player"))
		{
			return;
		}
		if(_generator == null)
		{
			InventoryScene scene = GameObject.FindGameObjectWithTag("Player").GetComponent<InventoryScene>();
			//TODO: Drone should be notified to display the Generator icon here
			Debug.Log("Displaying Inventory Scene and recommending the Generator be dropped");

		} else
		{
            //TODO: Drone should be notified to display the Jelly Dew icon here
            Debug.Log("Display jelly Dew Icon");
        }
	}

	public void OnTriggerExit(Collider other)
	{
		if(!other.gameObject.tag.Equals("Player")) {
			return;
		} else
		{
			//TODO: Notify drone to stop displaying its icon here
		}
	}

	

	/// <summary>
	/// This method is called by the entity providing the experience. 
	/// It also carries over surplus experience to the next level. 
	/// </summary>
	/// <param name="xpValue"></param>
	public void FeedIslandHeart(float xpValue)
	{
		if (_currentXP + xpValue >= _xpThreshold)
		{
			float difference = _currentXP - _xpThreshold;
			difference += xpValue;
			_currentXP = difference;
			LevelUp();
		}
		else
		{
			_currentXP += xpValue;
		}
		if(_generator != null)
		{
			_generator.GetComponent<GeneratorHandler>().AddCharge(xpValue);
		}
	}

	public bool IsMaxLevel()
	{
		return _heartLevel == _maxHeartLevel;
	}

	/// <summary>
	/// This event increments the heart's level and invokes an event. This event can be subscribed to trigger
	/// subsequent level up event sequences such as island expansion and increased production.
	/// </summary>
	private void LevelUp()
	{
		_heartLevel++;
		_xpThreshold *= _xpMultiplier;
		islandHeartLevelUp?.Invoke(this, null);
		//TODO: Add any other sequences or animations/model changes an island heart goes through upon level up
	}
}
