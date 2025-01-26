using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace Xeemu.PathAutoGen
{

	public class PlanePathService : MonoBehaviour , IPlanePathService{
		//Interface variable
		private readonly IPlaneHelperService iPlaneHerplerService;

		public PlanePathService()
		{
			iPlaneHerplerService = new PlanePathHelperService ();
		}	

		#region IPlanePathService implementation
		//Helping in destroy Paths
		/// <summary>
		/// Destroies the process.
		/// </summary>
		/// <param name="playerPositionZ">Player position z.</param>
		/// <param name="playerEnteringPathModel">Player entering path model.</param>
		/// <param name="pathGameObjectModel">Path game object model.</param>
		/// <param name="rigidBodyModel">Rigid body model.</param>
		/// <param name="pathModel">Path model.</param>
		/// <param name="pathNum">Path number.</param>
		/// <param name="rarePathCount">Rare path count.</param>
		/// <param name="rarePathRepeatAfterNoOfPath">Rare path repeat after no of path.</param>
		/// <param name="rarePath">Rare path.</param>
		/// <param name="pathList">Path list.</param>
		/// <param name="destroyPathNum">Destroy path number.</param>
		/// <param name="coinController">Coin controller.</param>
		/// <param name="oneunitySize">Oneunity size.</param>
		/// <param name="coins">Coins.</param>
		/// <param name="maxCoinHeight">Max coin height.</param>
		/// <param name="_noOfSize">No of size.</param>
		/// <param name="obstacleModel">Obstacle model.</param>
		public void DestroyProcess(ref float playerPositionZ,ref PlayerEnteringPathModel playerEnteringPathModel,ref PathGameObjectModel pathGameObjectModel,ref RigidBodyModel rigidBodyModel,
			ref PathModel pathModel, ref int pathNum, ref int rarePathCount, ref int rarePathRepeatAfterNoOfPath, ref GameObject rarePath, ref List<GameObject> pathList,int destroyPathNum,
			PathController pathController,float oneunitySize,GameObject coins, float maxCoinHeight ,int _noOfSize,
			ref ObstacleModel obstacleModel, ref float speed) 
		{
			switch(pathController.tilesQuantity.Count)
			{
			case 2:
				DestroyProcess (ref destroyPathNum, ref playerEnteringPathModel, ref pathGameObjectModel, playerPositionZ, ref pathModel, ref pathNum,
					ref rarePathCount, ref rarePathRepeatAfterNoOfPath, ref rarePath, ref pathList, ref rigidBodyModel,
					pathController, oneunitySize, coins, maxCoinHeight, _noOfSize, ref obstacleModel,pathController.tilesQuantity.Count,ref speed);
				break;
			case 3:
				DestroyProcess (ref destroyPathNum, ref playerEnteringPathModel, ref pathGameObjectModel, playerPositionZ, ref pathModel, ref pathNum,
					ref rarePathCount, ref rarePathRepeatAfterNoOfPath, ref rarePath, ref pathList, ref rigidBodyModel,
					pathController, oneunitySize, coins, maxCoinHeight, _noOfSize, ref obstacleModel,pathController.tilesQuantity.Count,ref speed);
				break;
			case 4:
				DestroyProcess( ref destroyPathNum,ref playerEnteringPathModel,ref pathGameObjectModel,playerPositionZ,ref pathModel,ref pathNum,ref rarePathCount,
					ref rarePathRepeatAfterNoOfPath, ref rarePath, ref pathList,ref rigidBodyModel,pathController,oneunitySize,coins, maxCoinHeight ,_noOfSize,
					ref obstacleModel,pathController.tilesQuantity.Count,ref speed);
				break;
			case 5:
				DestroyProcess( ref destroyPathNum,ref playerEnteringPathModel,ref pathGameObjectModel,playerPositionZ,ref pathModel,ref pathNum,ref rarePathCount,
					ref rarePathRepeatAfterNoOfPath, ref rarePath, ref pathList,ref rigidBodyModel,pathController,oneunitySize,coins, maxCoinHeight ,_noOfSize,
					ref obstacleModel,pathController.tilesQuantity.Count,ref speed);
				break;
			}



		}

		#region initialization for path

		public void GeneratePath ( ref PathPositionModel pathPositionModel,ref PathGameObjectModel pathGameObjectModel,ref List<GameObject> pathList,ref RigidBodyModel rigidBodyModel, ref PathModel pathModel,
			PathController pathController,float oneunitySize,GameObject coins,float maxCoinHeight ,int _noOfSize,ref ObstacleModel obstackleModel,ref float speed)
		{
			for (int i = 0; i < pathController.tilesQuantity.Count; i++) {
				switch(i){
				case 0:
					pathGameObjectModel.GeneratedPath1 = Instantiate (pathList [i], pathPositionModel.Path1PosValue, Quaternion.identity)as GameObject;
					break;
				case 1:
					pathGameObjectModel.GeneratedPath2 = Instantiate (pathList [i], pathPositionModel.Path2PosValue, Quaternion.identity)as GameObject;
					break;
				case 2:
					pathGameObjectModel.GeneratedPath3 = Instantiate (pathList [i], pathPositionModel.Path3PosValue, Quaternion.identity)as GameObject;
					break;
				case 3:
					pathGameObjectModel.GeneratedPath4 = Instantiate (pathList [i], pathPositionModel.Path4PosValue, Quaternion.identity)as GameObject;
					break;
				case 4:
					pathGameObjectModel.GeneratedPath5 = Instantiate (pathList [i], pathPositionModel.Path5PosValue, Quaternion.identity)as GameObject;
					break;

				}
			}
			//Scaling of all path
			PathSacling (ref pathGameObjectModel,ref rigidBodyModel,ref pathModel, pathController );
			//Help in generateing obstacles for the first time.
			InitialObstackleGenerater(ref pathGameObjectModel,ref pathModel,pathController,oneunitySize,coins,maxCoinHeight ,_noOfSize,ref obstackleModel,ref speed);
		}
		#endregion

		#endregion


		#region Helper Methods
		/// <summary>
		/// Initials the obstackle generater.
		/// </summary>
		/// <param name="pathGameObjectModel">Path game object model.</param>
		/// <param name="pathModel">Path model.</param>
		/// <param name="pathController">Path controller.</param>
		/// <param name="oneunitySize">Oneunity size.</param>
		/// <param name="coins">Coins.</param>
		/// <param name="maxCoinHeight">Max coin height.</param>
		/// <param name="_noOfSize">No of size.</param>
		/// <param name="obstackleModel">Obstackle model.</param>
		public void InitialObstackleGenerater(ref PathGameObjectModel pathGameObjectModel,ref PathModel pathModel,PathController pathController,float oneunitySize,GameObject coins,float maxCoinHeight ,int _noOfSize,ref ObstacleModel obstackleModel,ref float speed)
		{
			for (int i =2 ; i <= pathController.tilesQuantity.Count; i++) {


				var pathName =string.Format("tile{0}",i);
				//will return coinControllerViewModel object.
				var coinControllerViewModel =iPlaneHerplerService.GetCoinControllerItem (pathController,pathName);
				//Aliginment
				var xpos= iPlaneHerplerService.GetAlignmentType(coinControllerViewModel.tilesQuantity.coinController.coinsAlignment,pathModel);
				//return the number of coin we are goin to generate for particular path.
				var coinLength = iPlaneHerplerService.GetCoinLength (pathController, coinControllerViewModel.tilesQuantity);
				switch(i)
				{
				case 2:						
					//Generateing Coin abd obstacles as Child of path
					InstantiateCoinsAndObstacles (pathGameObjectModel.GeneratedPath2, xpos, coinLength,coinControllerViewModel.tilesQuantity,
						oneunitySize, coins, ref pathModel,  maxCoinHeight , _noOfSize,ref  obstackleModel,pathName,ref speed);
					break;
				case 3:
					//Generateing Coin abd obstacles as Child of path
					InstantiateCoinsAndObstacles (pathGameObjectModel.GeneratedPath3, xpos, coinLength,coinControllerViewModel.tilesQuantity,
						oneunitySize,  coins, ref pathModel,  maxCoinHeight , _noOfSize,ref  obstackleModel,pathName,ref speed);
					break;
				case 4:
					//Generateing Coin abd obstacles as Child of path
					InstantiateCoinsAndObstacles (pathGameObjectModel.GeneratedPath4, xpos, coinLength,coinControllerViewModel.tilesQuantity,
						oneunitySize,  coins, ref pathModel,  maxCoinHeight , _noOfSize,ref  obstackleModel,pathName,ref speed);
					break;
				case 5:
					//Generateing Coin and obstacles as Child of path
					InstantiateCoinsAndObstacles (pathGameObjectModel.GeneratedPath5, xpos, coinLength,coinControllerViewModel.tilesQuantity,
						oneunitySize,  coins, ref pathModel,  maxCoinHeight , _noOfSize,ref  obstackleModel,pathName,ref speed);
					break;
				}
			}
		}

		/// <summary>
		/// Destroies the process.
		/// </summary>
		/// <param name="destroyPathNum">Destroy path number.</param>
		/// <param name="playerEnteringPathModel">Player entering path model.</param>
		/// <param name="pathGameObjectModel">Path game object model.</param>
		/// <param name="playerPositionZ">Player position z.</param>
		/// <param name="pathModel">Path model.</param>
		/// <param name="pathNum">Path number.</param>
		/// <param name="rarePathCount">Rare path count.</param>
		/// <param name="rarePathRepeatAfterNoOfPath">Rare path repeat after no of path.</param>
		/// <param name="rarePath">Rare path.</param>
		/// <param name="pathList">Path list.</param>
		/// <param name="rigidBodyModel">Rigid body model.</param>
		/// <param name="pathController">Path controller.</param>
		/// <param name="oneunitySize">Oneunity size.</param>
		/// <param name="coins">Coins.</param>
		/// <param name="maxCoinHeight">Max coin height.</param>
		/// <param name="_noOfSize">No of size.</param>
		/// <param name="obstacleModel">Obstacle model.</param>
		/// <param name="noOfPath">No of path.</param>
		public void DestroyProcess( ref int destroyPathNum,ref PlayerEnteringPathModel playerEnteringPathModel,ref PathGameObjectModel pathGameObjectModel,float playerPositionZ,
			ref PathModel pathModel,ref int pathNum,ref int rarePathCount,ref int rarePathRepeatAfterNoOfPath, ref GameObject rarePath, ref List<GameObject> pathList,ref RigidBodyModel rigidBodyModel,
			PathController pathController,float oneunitySize,GameObject coins, float maxCoinHeight ,int _noOfSize,
			ref ObstacleModel obstacleModel,int noOfPath,ref float speed){
			//Debug.Log ("iPlanePathService : enter in DestroyProcess 5");
			switch (destroyPathNum) {
			case 1:
				//Destroy plane and generate 1 when player enter in 2 plane (5 count)
				if (!playerEnteringPathModel.IsEnterPath2) {
					var GeneratedPathPositionZ = 0.0f;
					GeneratedPathPositionZ = pathGameObjectModel.GeneratedPath2.transform.position.z;	
					var lastPathZPostion = 0.0f;
					switch (noOfPath) {
					case 2:
						lastPathZPostion = pathGameObjectModel.GeneratedPath2.transform.position.z;
						break;
					case 3:
						lastPathZPostion = pathGameObjectModel.GeneratedPath3.transform.position.z;
						break;
					case 4:
						lastPathZPostion = pathGameObjectModel.GeneratedPath4.transform.position.z;
						break;
					case 5:
						lastPathZPostion = pathGameObjectModel.GeneratedPath5.transform.position.z;
						break;

					}
					if ((Convert.ToInt32 (GeneratedPathPositionZ) == playerPositionZ)) {	
						playerEnteringPathModel.IsEnterPath1 = false;	
						playerEnteringPathModel.IsEnterPath2 = true;
						Destroy (pathGameObjectModel.GeneratedPath1);
						GeneratePath (new Vector3 (0, 0, (lastPathZPostion + pathModel.PathSizeLength)), ref  pathModel, ref  pathNum, ref  rarePathCount, ref  rarePathRepeatAfterNoOfPath,
							ref  rarePath, ref  pathList, ref  playerEnteringPathModel, ref  pathGameObjectModel, ref  rigidBodyModel, pathController, oneunitySize, coins, maxCoinHeight,
							_noOfSize, ref obstacleModel,ref speed);
					}
				}
				break;

			case 2:
				
				switch (noOfPath) {
				case 5:
				case 4:
				case 3:
					//Destroy plane and generate 2 when player enter in 3 plane (5 count ,4,3)
					if (!playerEnteringPathModel.IsEnterPath3) {
						var GeneratedPathPositionZ = pathGameObjectModel.GeneratedPath3.transform.position.z;
						var lastPathZPostion = pathGameObjectModel.GeneratedPath1.transform.position.z;
						if ((Convert.ToInt32 (GeneratedPathPositionZ) == playerPositionZ)) {		
							playerEnteringPathModel.IsEnterPath3 = true;
							playerEnteringPathModel.IsEnterPath2 = false;
							Destroy (pathGameObjectModel.GeneratedPath2);
							GeneratePath (new Vector3 (0, 0, (lastPathZPostion + pathModel.PathSizeLength)), ref  pathModel, ref  pathNum, ref  rarePathCount, ref  rarePathRepeatAfterNoOfPath, 
								ref  rarePath, ref  pathList, ref  playerEnteringPathModel, ref  pathGameObjectModel, ref  rigidBodyModel, pathController, 
								oneunitySize, coins, maxCoinHeight, _noOfSize, ref obstacleModel,ref speed);
						}	
					}
					break;
				case 2:
					//Destroy plane and generate 2 when player enter in 1 plane
					if (!playerEnteringPathModel.IsEnterPath1) {
						var GeneratedPathPositionZ = pathGameObjectModel.GeneratedPath1.transform.position.z;
						var lastPathZPostion = pathGameObjectModel.GeneratedPath1.transform.position.z;
						if ((Convert.ToInt32 (GeneratedPathPositionZ) == playerPositionZ)) {		
							playerEnteringPathModel.IsEnterPath1 = true;
							playerEnteringPathModel.IsEnterPath2 = false;
							Destroy (pathGameObjectModel.GeneratedPath2);
							GeneratePath (new Vector3 (0, 0, (lastPathZPostion + pathModel.PathSizeLength)), ref  pathModel, ref  pathNum, 
								ref  rarePathCount, ref  rarePathRepeatAfterNoOfPath, ref  rarePath, ref  pathList,
								ref  playerEnteringPathModel, ref  pathGameObjectModel,
								ref  rigidBodyModel, pathController, oneunitySize, coins, maxCoinHeight, _noOfSize, ref obstacleModel,ref speed);
						}	
					}
					break;	
				

				}
				break;
			case 3:
				switch (noOfPath) {
				case 5:
				case 4:
				//Destroy plane and generate 3 when player enter in 4 plane
					if (!playerEnteringPathModel.IsEnterPath4) {
						var GeneratedPathPositionZ = pathGameObjectModel.GeneratedPath4.transform.position.z;
						var lastPathZPostion = pathGameObjectModel.GeneratedPath2.transform.position.z;
						if ((Convert.ToInt32 (GeneratedPathPositionZ) == playerPositionZ)) {		
							playerEnteringPathModel.IsEnterPath4 = true;
							playerEnteringPathModel.IsEnterPath3 = false;
							Destroy (pathGameObjectModel.GeneratedPath3);
							GeneratePath (new Vector3 (0, 0, (lastPathZPostion + pathModel.PathSizeLength)), ref  pathModel, ref  pathNum, ref  rarePathCount, 
								ref  rarePathRepeatAfterNoOfPath, ref  rarePath, ref  pathList, ref  playerEnteringPathModel, 
								ref  pathGameObjectModel, ref  rigidBodyModel, pathController, oneunitySize, coins, maxCoinHeight, _noOfSize, ref obstacleModel,ref speed);
						}
					}
					break;
				case 3:
					//Destroy plane and generate 3 when player enter in 1 plane
					if (!playerEnteringPathModel.IsEnterPath1) {
						var GeneratedPathPositionZ = pathGameObjectModel.GeneratedPath1.transform.position.z;
						var lastPathZPostion = pathGameObjectModel.GeneratedPath2.transform.position.z;
						if ((Convert.ToInt32 (GeneratedPathPositionZ) == playerPositionZ)) {		
							playerEnteringPathModel.IsEnterPath1 = true;
							playerEnteringPathModel.IsEnterPath3 = false;
							Destroy (pathGameObjectModel.GeneratedPath3);
							GeneratePath (new Vector3 (0, 0, (lastPathZPostion + pathModel.PathSizeLength)), ref  pathModel, ref  pathNum, 
								ref  rarePathCount, ref  rarePathRepeatAfterNoOfPath, ref  rarePath, ref  pathList,
								ref  playerEnteringPathModel, ref  pathGameObjectModel,
								ref  rigidBodyModel, pathController, oneunitySize, coins, maxCoinHeight, _noOfSize, ref obstacleModel,ref speed);
						}
					}
					break;			
				}
				break;
			case 4:
				switch (noOfPath) {
				case 5:
				//Destroy plane and generate 4 when player enter in 5 plane
					if (!playerEnteringPathModel.IsEnterPath5) {
						var GeneratedPathPositionZ = pathGameObjectModel.GeneratedPath5.transform.position.z;
						var lastPathZPostion = pathGameObjectModel.GeneratedPath3.transform.position.z;
						if ((Convert.ToInt32 (GeneratedPathPositionZ) == playerPositionZ)) {		
							playerEnteringPathModel.IsEnterPath5 = true;
							playerEnteringPathModel.IsEnterPath4 = false;
							Destroy (pathGameObjectModel.GeneratedPath4);
							GeneratePath (new Vector3 (0, 0, (lastPathZPostion + pathModel.PathSizeLength)), ref  pathModel, ref  pathNum, ref  rarePathCount,
								ref  rarePathRepeatAfterNoOfPath, ref  rarePath, ref  pathList,	ref  playerEnteringPathModel, ref  pathGameObjectModel,
								ref  rigidBodyModel, pathController, oneunitySize, coins, maxCoinHeight, _noOfSize, ref obstacleModel,ref speed);
						}
					}	
					break;
				case 4:
					//Destroy plane and generate 4 when player enter in 1 plane
					if (!playerEnteringPathModel.IsEnterPath1) {
						var GeneratedPathPositionZ = pathGameObjectModel.GeneratedPath1.transform.position.z;
						var lastPathZPostion = pathGameObjectModel.GeneratedPath3.transform.position.z;
						if ((Convert.ToInt32 (GeneratedPathPositionZ) == playerPositionZ)) {		
							playerEnteringPathModel.IsEnterPath1 = true;
							playerEnteringPathModel.IsEnterPath4 = false;
							Destroy (pathGameObjectModel.GeneratedPath4);
							GeneratePath (new Vector3 (0, 0, (lastPathZPostion + pathModel.PathSizeLength)), ref  pathModel, ref  pathNum, 
								ref  rarePathCount, ref  rarePathRepeatAfterNoOfPath, ref  rarePath, ref  pathList,
								ref  playerEnteringPathModel, ref  pathGameObjectModel,
								ref  rigidBodyModel, pathController, oneunitySize, coins, maxCoinHeight, _noOfSize, ref obstacleModel,ref speed);
						}
					}	
					break;

				}
				break;
			case 5:
				//Destroy plane and generate 5 when player enter in 1 plane	
				if (!playerEnteringPathModel.IsEnterPath1) {
					var GeneratedPathPositionZ = pathGameObjectModel.GeneratedPath1.transform.position.z;
					var lastPathZPostion = pathGameObjectModel.GeneratedPath4.transform.position.z;
					if ((Convert.ToInt32 (GeneratedPathPositionZ) == playerPositionZ)) {		
						playerEnteringPathModel.IsEnterPath1 = true;
						Destroy (pathGameObjectModel.GeneratedPath5);
						GeneratePath (new Vector3 (0, 0, (lastPathZPostion + pathModel.PathSizeLength)), ref  pathModel, ref  pathNum,
							ref  rarePathCount, ref  rarePathRepeatAfterNoOfPath, ref  rarePath, ref  pathList,	ref  playerEnteringPathModel,
							ref  pathGameObjectModel,ref  rigidBodyModel,pathController, oneunitySize, coins, maxCoinHeight, _noOfSize,ref obstacleModel,ref speed);
					}
				}		
				break;
			}
		}	

		//Help In scaling of all path
		/// <summary>
		/// Paths the sacling.
		/// </summary>
		/// <param name="pathGameObjectModel">Path game object model.</param>
		/// <param name="rigidBodyModel">Rigid body model.</param>
		/// <param name="pathModel">Path model.</param>
		/// <param name="pathController">Path controller.</param>
		void PathSacling(ref PathGameObjectModel pathGameObjectModel,ref RigidBodyModel rigidBodyModel,ref PathModel pathModel ,PathController pathController)
		{
			var pathLength = pathModel.PathSizeLength;
			var pathWidth = pathModel.PathSizeWidth;

			for (int i = 0; i < pathController.tilesQuantity.Count; i++) {
				switch (i) {
				case 0:
					pathGameObjectModel.GeneratedPath1.transform.localScale = new Vector3 (pathWidth, 1, pathLength);
					rigidBodyModel.RigidbodyPath1 = pathGameObjectModel.GeneratedPath1.GetComponent<Rigidbody> ();
					break;
				case 1:
					pathGameObjectModel.GeneratedPath2.transform.localScale = new Vector3 (pathWidth, 1, pathLength);
					rigidBodyModel.RigidbodyPath2 = pathGameObjectModel.GeneratedPath2.GetComponent<Rigidbody> ();
					break;
				case 2:
					pathGameObjectModel.GeneratedPath3.transform.localScale = new Vector3 (pathWidth, 1, pathLength);
					rigidBodyModel.RigidbodyPath3 = pathGameObjectModel.GeneratedPath3.GetComponent<Rigidbody> ();
					break;
				case 3:
					pathGameObjectModel.GeneratedPath4.transform.localScale = new Vector3 (pathWidth, 1, pathLength);
					rigidBodyModel.RigidbodyPath4 = pathGameObjectModel.GeneratedPath4.GetComponent<Rigidbody> ();
					break;
				case 4:
					pathGameObjectModel.GeneratedPath5.transform.localScale = new Vector3 (pathWidth, 1, pathLength);
					rigidBodyModel.RigidbodyPath5 = pathGameObjectModel.GeneratedPath5.GetComponent<Rigidbody> ();
					break;

				}
			}
		}

		/// <summary>
		/// Generates the path.
		/// </summary>
		/// <param name="newPathPos">New path position.</param>
		/// <param name="pathModel">Path model.</param>
		/// <param name="pathNum">Path number.</param>
		/// <param name="rarePathCount">Rare path count.</param>
		/// <param name="rarePathRepeatAfterNoOfPath">Rare path repeat after no of path.</param>
		/// <param name="rarePath">Rare path.</param>
		/// <param name="pathList">Path list.</param>
		/// <param name="playerEnteringPathModel">Player entering path model.</param>
		/// <param name="pathGameObjectModel">Path game object model.</param>
		/// <param name="rigidBodyModel">Rigid body model.</param>
		/// <param name="pathController">Path controller.</param>
		/// <param name="oneunitySize">Oneunity size.</param>
		/// <param name="coins">Coins.</param>
		/// <param name="maxCoinHeight">Max coin height.</param>
		/// <param name="_noOfSize">No of size.</param>
		/// <param name="obstacleModel">Obstacle model.</param>
		void GeneratePath (Vector3 newPathPos,ref PathModel pathModel, ref int pathNum, ref int rarePathCount,ref int rarePathRepeatAfterNoOfPath,ref GameObject rarePath,ref List<GameObject> pathList,
			ref  PlayerEnteringPathModel playerEnteringPathModel,ref  PathGameObjectModel pathGameObjectModel,
			ref  RigidBodyModel rigidBodyModel,PathController pathController,float oneunitySize,GameObject coins, float maxCoinHeight ,int _noOfSize,ref ObstacleModel obstacleModel,ref float speed)
		{
			
			GameObject newpath;

			//Check When Rare Path Will Introduce 
			if (rarePathCount == rarePathRepeatAfterNoOfPath&&rarePathRepeatAfterNoOfPath!=0) {			
				newpath = Instantiate (rarePath, newPathPos, Quaternion.identity)as GameObject;
				rarePathCount = 0;
			}
			//Instantiate a Noramal Paths
			else 
			{			
				var probalityNumber = pathNum - 1;
				newpath = Instantiate (pathList [probalityNumber], newPathPos, Quaternion.identity)as GameObject;
				rarePathCount++;	
				////Debug.Log ("rarePathCount : "+rarePathCount);
			}
			//Debug.Log ("iPlanePathService : enter in GeneratePath 6");
			//assign a path to Variable
			GeneratePath( ref  pathNum,ref  newpath,ref   pathGameObjectModel,ref rigidBodyModel,ref  pathModel,ref  playerEnteringPathModel,
				pathController, oneunitySize, coins,  maxCoinHeight , _noOfSize,ref obstacleModel,ref speed);

		}

		/// <summary>
		/// Generates the path.
		/// </summary>
		/// <param name="pathNum">Path number.</param>
		/// <param name="newpath">Newpath.</param>
		/// <param name="pathGameObjectModel">Path game object model.</param>
		/// <param name="rigidBodyModel">Rigid body model.</param>
		/// <param name="pathModel">Path model.</param>
		/// <param name="playerEnteringPathModel">Player entering path model.</param>
		/// <param name="pathController">Path controller.</param>
		/// <param name="oneunitySize">Oneunity size.</param>
		/// <param name="coins">Coins.</param>
		/// <param name="maxCoinHeight">Max coin height.</param>
		/// <param name="_noOfSize">No of size.</param>
		/// <param name="obstacleModel">Obstacle model.</param>
		void GeneratePath(ref int pathNum,ref GameObject newpath,ref  PathGameObjectModel pathGameObjectModel,
			ref  RigidBodyModel rigidBodyModel,ref PathModel pathModel,
			ref  PlayerEnteringPathModel playerEnteringPathModel,
			PathController pathController,float oneunitySize,GameObject coins, float maxCoinHeight ,int _noOfSize,
			ref ObstacleModel obstacleModel,ref float speed) 
		{
			//Debug.Log ("iPlanePathService : enter in GeneratePath 7");
			var pathLength = pathModel.PathSizeLength;
			var pathWidth = pathModel.PathSizeWidth;

			switch(pathNum)
			{
			case 1:
				pathGameObjectModel.GeneratedPath1 = newpath as GameObject;
				pathGameObjectModel.GeneratedPath1.transform.localScale = new Vector3 (pathWidth,1,pathLength);
				rigidBodyModel.RigidbodyPath1 = pathGameObjectModel.GeneratedPath1.GetComponent<Rigidbody> ();
				//Calling Function
				GenerateCoins (pathGameObjectModel.GeneratedPath1,"tile1", pathController, oneunitySize, coins, ref pathModel,  maxCoinHeight , _noOfSize,ref obstacleModel,ref speed);
				playerEnteringPathModel.IsEnterPath1 = false;


				break;
			case 2:
				pathGameObjectModel.GeneratedPath2 = newpath as GameObject;
				pathGameObjectModel.GeneratedPath2.transform.localScale = new Vector3 (pathWidth,1,pathLength);
				rigidBodyModel.RigidbodyPath2 = pathGameObjectModel.GeneratedPath2.GetComponent<Rigidbody> ();
				//Calling Function
				GenerateCoins (pathGameObjectModel.GeneratedPath2,"tile2", pathController, oneunitySize, coins, ref pathModel,  maxCoinHeight , _noOfSize,ref obstacleModel,ref speed);
				playerEnteringPathModel.IsEnterPath2 = false;

				break;
			case 3:
				pathGameObjectModel.GeneratedPath3 = newpath as GameObject;
				pathGameObjectModel.GeneratedPath3.transform.localScale = new Vector3 (pathWidth,1,pathLength);
				rigidBodyModel.RigidbodyPath3 = pathGameObjectModel.GeneratedPath3.GetComponent<Rigidbody> ();
				playerEnteringPathModel.IsEnterPath3 = false;
				//Calling Function
				GenerateCoins (pathGameObjectModel.GeneratedPath3,"tile3",pathController, oneunitySize, coins, ref pathModel,  maxCoinHeight , _noOfSize,ref obstacleModel,ref speed);

				break;
			case 4:
				pathGameObjectModel.GeneratedPath4 = newpath as GameObject;
				pathGameObjectModel.GeneratedPath4.transform.localScale = new Vector3 (pathWidth,1,pathLength);
				rigidBodyModel.RigidbodyPath4 = pathGameObjectModel.GeneratedPath4.GetComponent<Rigidbody> ();
				////Debug.Log ("going to generate Coins");
				GenerateCoins (pathGameObjectModel.GeneratedPath4,"tile4", pathController, oneunitySize,  coins, ref pathModel,  maxCoinHeight , _noOfSize,ref obstacleModel,ref speed);
				playerEnteringPathModel.IsEnterPath4 = false;

				break;
			case 5:

				pathGameObjectModel.GeneratedPath5 = newpath as GameObject;
				pathGameObjectModel.GeneratedPath5.transform.localScale = new Vector3 (pathWidth,1,pathLength);
				rigidBodyModel.RigidbodyPath5 = pathGameObjectModel.GeneratedPath5.GetComponent<Rigidbody> ();
				//Calling Function
				GenerateCoins (pathGameObjectModel.GeneratedPath5,"tile5", pathController, oneunitySize,  coins, ref pathModel,  maxCoinHeight , _noOfSize,ref obstacleModel,ref speed);
				playerEnteringPathModel.IsEnterPath5 = false;

				break;
			}
		}

		/// <summary>
		/// Generates the coins.
		/// </summary>
		/// <param name="parentPath">Parent path.</param>
		/// <param name="pathName">Path name.</param>
		/// <param name="pathController">Path controller.</param>
		/// <param name="oneunitySize">Oneunity size.</param>
		/// <param name="coins">Coins.</param>
		/// <param name="pathModel">Path model.</param>
		/// <param name="maxCoinHeight">Max coin height.</param>
		/// <param name="_noOfSize">No of size.</param>
		/// <param name="obstacleModel">Obstacle model.</param>
		void GenerateCoins(GameObject parentPath,string pathName,PathController pathController,float oneunitySize,GameObject coins,ref PathModel pathModel,
			float maxCoinHeight ,int _noOfSize,ref ObstacleModel obstacleModel,ref float speed)
		{
			//Debug.Log ("iPlanePathService : enter in GeneratePath 8");
			//will return coinControllerViewModel object.
			var coinControllerViewModel =iPlaneHerplerService.GetCoinControllerItem (pathController,pathName);
			//Aliginment
			var xpos= iPlaneHerplerService.GetAlignmentType(coinControllerViewModel.tilesQuantity.coinController.coinsAlignment,pathModel);
			//return the number of coin we are goin to generate for particular path.
			var coinLength = iPlaneHerplerService.GetCoinLength (pathController, coinControllerViewModel.tilesQuantity);
			//Generateing Coin as Child of path
			InstantiateCoinsAndObstacles (parentPath, xpos, coinLength,coinControllerViewModel.tilesQuantity,
				oneunitySize, coins, ref pathModel,  maxCoinHeight , _noOfSize,ref obstacleModel,pathName,ref speed);
		}


		//Helping in Instantiate Coins when new Path have generated
		/// <summary>
		/// Instantiates the coins and obstacles.
		/// </summary>
		/// <param name="parentPath">Parent path.</param>
		/// <param name="xpos">Xpos.</param>
		/// <param name="coinLength">Coin length.</param>
		/// <param name="pathQuantity">Path quantity.</param>
		/// <param name="oneunitySize">Oneunity size.</param>
		/// <param name="coins">Coins.</param>
		/// <param name="pathModel">Path model.</param>
		/// <param name="maxCoinHeight">Max coin height.</param>
		/// <param name="_noOfSize">No of size.</param>
		/// <param name="obstackleModel">Obstackle model.</param>
		/// <param name="pathName">Path name.</param>
		void InstantiateCoinsAndObstacles(GameObject parentPath,float xpos,int coinLength,TilesQuantity pathQuantity,
			float oneunitySize,GameObject coins,ref PathModel pathModel, float maxCoinHeight ,int _noOfSize,ref ObstacleModel obstackleModel, string pathName ,ref float speed)
		{
			//Debug.Log ("IPlaneService : enter in InstantiateCoinsAndObstacles 12");

			var coinPos = Convert.ToSingle (parentPath.transform.position.z - (parentPath.transform.localScale.z / 2)+1);
			var obstaclePosition = coinPos;				
			CoinGenerateProcess (parentPath, pathQuantity, coinLength, coins, xpos, coinPos, oneunitySize, ref pathModel, maxCoinHeight, _noOfSize,ref speed);
			ObstaclesGerneratingProcess (ref obstackleModel, pathQuantity, parentPath, xpos, obstaclePosition, _noOfSize, pathName, ref pathModel);
		}

		/// <summary>
		/// Coins the generate process.
		/// </summary>
		/// <param name="parentPath">Parent path.</param>
		/// <param name="pathQuantity">Path quantity.</param>
		/// <param name="coinLength">Coin length.</param>
		/// <param name="coins">Coins.</param>
		/// <param name="xpos">Xpos.</param>
		/// <param name="coinPos">Coin position.</param>
		/// <param name="oneunitySize">Oneunity size.</param>
		/// <param name="pathModel">Path model.</param>
		/// <param name="maxCoinHeight">Max coin height.</param>
		/// <param name="_noOfSize">No of size.</param>
		void CoinGenerateProcess(GameObject parentPath,TilesQuantity pathQuantity,int coinLength,GameObject coins,float xpos,float coinPos,
			float oneunitySize,ref PathModel pathModel, float maxCoinHeight ,int _noOfSize,ref float speed)
		{
			//Debug.Log ("IPlaneService : enter in CoinGenerateProcess 13");
			switch (pathQuantity.coinController.coinsPath) {

			case COINPATH.RANDOM:
				var random = new System.Random ();
				//Calling Sevices Method and Replace Coin type from random to Either Flat or Curved
				pathQuantity.coinController.coinsPath = iPlaneHerplerService.GetRandomCoinType (random.Next (1, 3));;
				//If randonm is selected then it call by self 
				CoinGenerateProcess (parentPath,pathQuantity, coinLength, coins, xpos, coinPos,oneunitySize,ref pathModel, maxCoinHeight ,_noOfSize,ref speed);
				break;

			case COINPATH.FLAT:
				coinPos = coinPos + (oneunitySize / 2);
				for (int i = 0; i < coinLength; i++) {

					//Calling Sevices Method
					iPlaneHerplerService.CoinGenerateProcess (coins, xpos, coinPos, parentPath,1.2f);
					coinPos = coinPos + oneunitySize;
				}
				break;
			default:
				//Calling Sevices Method and Get Coins points for generation on path
				var list = iPlaneHerplerService.GetPointsForCoins (pathModel.PathSizeLength, maxCoinHeight, _noOfSize);
				var len = coinLength;
				foreach (var item in list) {
					var yPos = item.y;
					while (coinLength > 0) {	
						    //to make curve asiiging 1.2 
						if (len == (int)NOOFCOINS._20) {
							if (yPos < 1.2f)
								yPos = 1.2f;
						} else {
							if (yPos == 1.2f) {
								yPos = 1.4f;
							}
							if (yPos < 1f)
								yPos = 1.2f;

						}

						//Calling Sevices Method
						iPlaneHerplerService.CoinGenerateProcess (coins, xpos, coinPos, parentPath, yPos);

						if(speed>=10&&speed<=20)
							coinPos = (len==((int)NOOFCOINS._20))?(coinPos + (oneunitySize/2f)):(coinPos + (oneunitySize/1.5f));
						else if(speed>20&&speed<30)
							coinPos = (len==((int)NOOFCOINS._20))?(coinPos + (oneunitySize/2f)):(coinPos + (oneunitySize/1.5f));
						else if(speed>=30&&speed<=40)
							coinPos = (len==((int)NOOFCOINS._20))?(coinPos + (oneunitySize/1.1f)):(coinPos + (oneunitySize/1.1f));
//						switch(Convert.ToInt32(speed))
//						{
//						case 10:
//						case 20:
//							coinPos = (len==((int)NOOFCOINS._20))?(coinPos + (oneunitySize/2f)):(coinPos + (oneunitySize/1.5f));
//							break;
//						case 30:
//							coinPos = (len==((int)NOOFCOINS._20))?(coinPos + (oneunitySize/2f)):(coinPos + (oneunitySize/1.1f));
//							break;
//						}
						//coinPos = (len==((int)NOOFCOINS._20))?(coinPos + (oneunitySize/2f)):(coinPos + (oneunitySize/1.5f));
						//coinPos = coinPos + oneunitySize;
						coinLength--;
						break;
					}
				}
				break;
			}
		}

		/// <summary>
		/// Obstacleses the gernerating process.
		/// </summary>
		/// <param name="obstackleModel">Obstackle model.</param>
		/// <param name="pathQuantity">Path quantity.</param>
		/// <param name="parentPath">Parent path.</param>
		/// <param name="xpos">Xpos.</param>
		/// <param name="obstaclePosition">Obstacle position.</param>
		/// <param name="_noOfSize">No of size.</param>
		/// <param name="pathName">Path name.</param>
		/// <param name="pathModel">Path model.</param>
		void ObstaclesGerneratingProcess (ref ObstacleModel obstackleModel,TilesQuantity pathQuantity,GameObject parentPath,float xpos,
			float obstaclePosition,int _noOfSize,string pathName,ref PathModel pathModel){
			//Debug.Log ("IPlaneService : enter in ObstaclesGerneratingProcess 15");
			if (pathQuantity.obstacleController.obstacleTypes != OBSTACLETYPE.NIL) {
				//Calling Sevices Method
				iPlaneHerplerService.GenerateObstacle (ref obstackleModel, pathQuantity, pathModel, obstaclePosition, _noOfSize, parentPath, pathName, xpos);
			}
		}

		#endregion

	}
}
