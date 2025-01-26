using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine.UI;

namespace Xeemu.PathAutoGen
{
	public class PlanePathController : MonoBehaviour {



		#region Public variable
		//Definig Variables
		public GameObject player;
		public int rareTileRepeatInterval = 0;
		public GameObject rareTile, coins;
		//public variable using to get input from User in Inspector and assisgn these values to
		//PathModel Properties.
		public  float tileLength = 0.0f;
		public  float tileWidth = 0.0f;

		//Assiging memory to sPathModel.
		PathModel pathSize=  new PathModel();

		//Assiging memory to CoinController
		public PathController pathController = new PathController ();

		//Obstacle Types List
		public List<GameObject> singleRowBlocker = new  List<GameObject> ();
		public List<GameObject> rowBlocker = new  List<GameObject> ();

		public Text labelScore;

		//Speed At what rate Player will move (Basically in coding path is moving)
		[HideInInspector]
		public float speed=10.0f;



		#endregion

		#region Private variables
		//Helping variable for showing rare Path 
		int rarePathCount=0;

		//Help to adjust the height of coins while using the Curved type .
		float maxCoinHeight=1.5f; 

		//Start: Private variable using to calculate which path is going to generate  , Moving in Fixed Update
		int rarePathValue=0;

		/// <summary>
		/// The path game object model.
		/// </summary>
		PathGameObjectModel pathGameObjectModel=new PathGameObjectModel();

		/// <summary>
		/// The rigid body model.
		/// </summary>
		RigidBodyModel rigidBodyModel=new RigidBodyModel();

		/// <summary>
		/// The player entering path model.
		/// </summary>
		PlayerEnteringPathModel playerEnteringPathModel=new PlayerEnteringPathModel();

		/// <summary>
		/// The path position model.
		/// </summary>
		PathPositionModel pathPositionModel=new PathPositionModel();

		/// <summary>
		/// The obstacle model.
		/// </summary>
		ObstacleModel obstacleModel = new ObstacleModel ();


		//Assiging memory to pathList
		/// <summary>
		/// The path list.
		/// </summary>
		List<GameObject> pathList = new  List<GameObject> ();

		//END: Private variable using to calculate which path is going to generate  

		//It help to get the fixed postion for Path generation and Help in coin Gerartion process and Obstacle generation
		/// <summary>
		/// path will be divided into 20 part for default but its recalculated when you have selected no of coins and it will use for coin generation process.
		/// Help to get x-axis and y-axis for coins.
		/// </summary>
		private int _noOfSize=20;

		//Help to get oneunitySize of path
		float oneunitySize=0.0f;

		//Help in calculating distance.
		float distance = 0.0f;

		/// <summary>
		/// The player controller object.
		/// </summary>
		GameObject _playerControllerObject;

		/// <summary>
		/// It Will help to find Palyer Controller script.
		/// </summary>
		private PlayerController _playerController;
		#endregion

		//Interface variable
		private readonly IPlanePathService iPlanePathService;

		//Constructor
		PlanePathController()
		{			
			//Inialization 
			iPlanePathService = new PlanePathService ();
		}

		#region Predefined Methods
		// Use this for initialization
		void Start () {
			
			//checking path count 
			var pathCount = pathController.tilesQuantity.Count;
			if (pathCount > 5||pathCount <2) {
				throw new Exception ("Maximum 5 paths or minimum 2 paths are allowed or For more than 5 paths need to develop logical code for further funcionality");
			}

			// Set global coin quantity first
			pathController.numberOfCoins = NOOFCOINS._10;  // Can be _5, _10, _15, etc.

			// Configure coin spawning for each path tile
			for (int i = 0; i < pathController.tilesQuantity.Count; i++) 
			{
				var tile = pathController.tilesQuantity[i];
				
				// Coin spawn settings
				tile.coinController.coinsSpawn = COINSPAWN.YES;
				
				// Coin placement pattern
				tile.coinController.coinsAlignment = COINALIGNMENT.RANDOM;  // or LINE, ZIGZAG
				tile.coinController.coinsPath = COINPATH.RANDOM;  // or STRAIGHT, CURVE
			}

			//Getting path speed.
			speed = pathController.platformSpeed==0?10f:pathController.platformSpeed;

			//recalculate no of units and max height;
			SetMaxHeight ();

			//Calling method
			FindPlayerControllerObject ();

			//Set values of obstacleModel 
			obstacleModel.SingleLaneList = singleRowBlocker;
			obstacleModel.ThreeLaneList = rowBlocker;

			//Adding paths to PathList for creating Paths in the Environment
			foreach (var item in pathController.tilesQuantity) {
				pathList.Add (item.tile);
			} 

			//Getting the path speed using enum.
			speed = pathController.platformSpeed;

			//calling Function
			SetPathModelValues ();

			//calculate oneunitySize and its will further use in process.
			oneunitySize = (pathSize.PathSizeLength / _noOfSize);

			//calculating width section of path 
			var pathWidthSection = ((pathSize.PathSizeWidth / 3)/2);

			//assiging value to private variable for inner manupulation
			rarePathValue = rareTileRepeatInterval;

			//Set values to PathPositionModel 
			SetPathPositionModel ();

			//initialization Path Generate
			iPlanePathService.GeneratePath ( ref pathPositionModel,ref pathGameObjectModel,ref pathList,ref rigidBodyModel, ref pathSize,
				pathController,oneunitySize,coins,maxCoinHeight ,_noOfSize,ref obstacleModel,ref speed);	
		}

		void FixedUpdate()             
		{			
			if (_playerControllerObject != null) {
				if (!_playerController.isGameOver)
					speed = pathController.platformSpeed;
			}

			if (rarePathValue != rareTileRepeatInterval) {
				rarePathValue = rareTileRepeatInterval;
				rarePathCount = 0;
			}

			// recalculation for path.
			SetMaxHeight ();
			
			//calling function
			SetPathModelValues ();

			//DestroyPath function
			DestroyPath (player.transform.position.z);

			//Start: Moving Path
			var time = Time.deltaTime;
			distance+=((speed/2) * time);
			labelScore.text = string.Format ("{0} m",Convert.ToInt32(distance));
			//Moving paths according to list count.
			MovingPath (time);				
			//End: Moving Path

		}
		#endregion


		#region Destroy Section
		/// <summary>
		/// Destroies the path.
		/// </summary>
		/// <param name="playerPositionZ">Player position z.</param>
		void DestroyPath(float playerPositionZ)
		{
			for (var i=1;i<=pathList.Count;i++) {
				var destroyPathNum=i;
				var genpathNum = i;
				//calling helper Method
				DestroyService (playerPositionZ, destroyPathNum, genpathNum);
			}

		}
		#endregion

		#region HelperMethod
		void DestroyService(float playerPositionZ,int destroyPathNum,int pathNum)
		{
			//Debug.Log ("Fixed update: enter in iPlanePathService.DestroyProcess 4");
			//calling Service method
			iPlanePathService.DestroyProcess(ref  playerPositionZ,ref  playerEnteringPathModel,ref pathGameObjectModel,ref rigidBodyModel,
				ref pathSize, ref pathNum, ref  rarePathCount, ref rareTileRepeatInterval, ref rareTile, ref pathList,
				destroyPathNum,pathController, oneunitySize, coins,  maxCoinHeight , _noOfSize,ref obstacleModel,ref speed);
		}

		//Assigind values to PathModel
		void SetPathModelValues()
		{
			pathSize.PathSizeLength = tileLength;
			pathSize.PathSizeWidth = tileWidth;
		} 

		/// <summary>
		/// Movings the path.
		/// </summary>
		void MovingPath(float time)
		{
			//perform according to count
			switch(pathController.tilesQuantity.Count)
			{
			case 2:
				rigidBodyModel.RigidbodyPath1.MovePosition (pathGameObjectModel.GeneratedPath1.transform.position - new Vector3 (0, 0, speed * time));
				rigidBodyModel.RigidbodyPath2.MovePosition (pathGameObjectModel.GeneratedPath2.transform.position - new Vector3 (0, 0, speed * time));					
				break;

			case 3:
				rigidBodyModel.RigidbodyPath1.MovePosition (pathGameObjectModel.GeneratedPath1.transform.position - new Vector3 (0, 0, speed * time));
				rigidBodyModel.RigidbodyPath2.MovePosition (pathGameObjectModel.GeneratedPath2.transform.position - new Vector3 (0, 0, speed * time));
				rigidBodyModel.RigidbodyPath3.MovePosition (pathGameObjectModel.GeneratedPath3.transform.position - new Vector3 (0, 0, speed * time));					
				break;

			case 4:
				rigidBodyModel.RigidbodyPath1.MovePosition (pathGameObjectModel.GeneratedPath1.transform.position - new Vector3 (0, 0, speed * time));
				rigidBodyModel.RigidbodyPath2.MovePosition (pathGameObjectModel.GeneratedPath2.transform.position - new Vector3 (0, 0, speed * time));
				rigidBodyModel.RigidbodyPath3.MovePosition (pathGameObjectModel.GeneratedPath3.transform.position - new Vector3 (0, 0, speed * time));
				rigidBodyModel.RigidbodyPath4.MovePosition (pathGameObjectModel.GeneratedPath4.transform.position - new Vector3 (0, 0, speed * time));					
				break;

			case 5:
				rigidBodyModel.RigidbodyPath1.MovePosition (pathGameObjectModel.GeneratedPath1.transform.position - new Vector3 (0, 0, speed * time));
				rigidBodyModel.RigidbodyPath2.MovePosition (pathGameObjectModel.GeneratedPath2.transform.position - new Vector3 (0, 0, speed * time));
				rigidBodyModel.RigidbodyPath3.MovePosition (pathGameObjectModel.GeneratedPath3.transform.position - new Vector3 (0, 0, speed * time));
				rigidBodyModel.RigidbodyPath4.MovePosition (pathGameObjectModel.GeneratedPath4.transform.position - new Vector3 (0, 0, speed * time));
				rigidBodyModel.RigidbodyPath5.MovePosition (pathGameObjectModel.GeneratedPath5.transform.position - new Vector3 (0, 0, speed * time));	
				break;

			}
		}

		/// <summary>
		/// Sets the height of the max.
		/// </summary>
		void SetMaxHeight()
		{
			// recalculation for path.
			_noOfSize = (int)pathController.numberOfCoins;
			//assign max height.
			if(_noOfSize==20)				
				maxCoinHeight=3.5f;
			else
				maxCoinHeight=3f;
		}

		/// <summary>
		/// Finds the player controller object.
		/// </summary>
		void FindPlayerControllerObject()
		{
			_playerControllerObject = GameObject.FindWithTag ("Player");
			if (_playerControllerObject != null) {
				_playerController = _playerControllerObject.GetComponent <PlayerController> ();					
			}
		}

		/// <summary>
		/// Sets the path position model.
		/// </summary>
		void SetPathPositionModel()
		{
			//initialize Z postion Of path
			float   path1ZPosValue =((pathSize.PathSizeLength / 2)-2),
			path2ZPosValue = (path1ZPosValue + pathSize.PathSizeLength),
			path3ZPosValue = (path2ZPosValue + pathSize.PathSizeLength),
			path4ZPosValue = (path3ZPosValue + pathSize.PathSizeLength),
			path5ZPosValue = (path4ZPosValue + pathSize.PathSizeLength);

			//assiging Values to Path Position Model Property (po)
			pathPositionModel.Path1PosValue = new Vector3 (0, 0, path1ZPosValue);
			pathPositionModel.Path2PosValue = new Vector3 (0, 0, path2ZPosValue);
			pathPositionModel.Path3PosValue = new Vector3 (0, 0, path3ZPosValue);
			pathPositionModel.Path4PosValue = new Vector3 (0, 0, path4ZPosValue);
			pathPositionModel.Path5PosValue = new Vector3 (0, 0, path5ZPosValue);
		}
		#endregion
	}
}
