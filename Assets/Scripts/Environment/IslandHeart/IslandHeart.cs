using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Android;

public class IslandHeart : MonoBehaviour
{
	[Header("Island Heart UI")]
	[Tooltip("The Island Heart interaction prompt")]
	[SerializeField]
	private GameObject _islandHeartPromptUI;

	[Tooltip("The Island Heart level indicator")]
	[SerializeField]
	private GameObject _islandHeartLevelIndicator;

	[Tooltip("The Island Heart experience indicator")]
	[SerializeField]
	private GameObject _islandHeartExperienceIndicator;

	[Tooltip("The world space where the generator is placed when attached to Island Heart")]
	[SerializeField]
	private GameObject _generatorLocation;
	private GameObject _generator;

	[Header("Island Heart Leveling System")]
	[Tooltip("Current Island Heart Level")]
	[Min(1)]
	[SerializeField]
	private int _heartLevel;

	[Tooltip("The experience threshold to level up the Island Heart")]
	[Min(1)]
	[SerializeField]
	private float _xpThreshold;

	[Tooltip("The current amount of experience the Island Heart has")]
	[Min(0f)]
	[SerializeField]
	private float _currentXP;


	// Start is called before the first frame update
	void Start()
	{
		_generator = null;
	}

	// Update is called once per frame
	void Update()
	{

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
	}

	private void LevelUp()
	{
		_heartLevel++;
		//TODO: Trigger any events or methods an island heart level up needs to
	}
}
