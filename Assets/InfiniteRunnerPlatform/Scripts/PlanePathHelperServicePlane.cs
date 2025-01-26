using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

namespace Xeemu.PathAutoGen
{
	/// <summary>
	/// Plane path helper service.
	/// </summary>
	public class PlanePathHelperService : MonoBehaviour,IPlaneHelperService {	

		/// <summary>
		/// Coins the generate process.
		/// </summary>
		/// <param name="coins">Coins.</param>
		/// <param name="xpos">Xpos.</param>
		/// <param name="coinPos">Coin position.</param>
		/// <param name="parentPath">Parent path.</param>
		/// <param name="yPos">Y position.</param>
		public void CoinGenerateProcess(GameObject coins,float xpos,float coinPos,GameObject parentPath,float yPos)
		{
			//Debug.Log ("IPlaneHelperService : enter in CoinGenerateProcess 14");
			GameObject coin = Instantiate (coins, new Vector3 (xpos, yPos, coinPos), Quaternion.identity)as GameObject;
			coin.transform.localRotation = Quaternion.Euler(new Vector3(90f,0f,0f));
			coin.transform.parent = parentPath.transform;
		}	

		//Help to get values for GetCoinControllerItem
		public PathControllerViewModel GetCoinControllerItem (PathController coinController,string pathName)
		{	//Debug.Log ("IPlaneHelperService : enter in GetCoinControllerItem 9");	
			PathControllerViewModel coinControllerViewModel = new PathControllerViewModel ();

			//Set values to CoinControllerViewModel variables
			foreach (var item in coinController.tilesQuantity) {	
				if (item.tile.name.ToLower () == pathName) {
					//set numberOfCoins
					coinControllerViewModel.numberOfCoins = coinController.numberOfCoins;
					//set values to path variables 


					coinControllerViewModel.tilesQuantity.coinController.coinsSpawn = item.coinController.coinsSpawn;
					coinControllerViewModel.tilesQuantity.coinController.coinsAlignment = item.coinController.coinsAlignment;
					coinControllerViewModel.tilesQuantity.coinController.coinsPath = item.coinController.coinsPath;

					coinControllerViewModel.tilesQuantity.obstacleController.obstacleSpawnDirection = item.obstacleController.obstacleSpawnDirection;
					coinControllerViewModel.tilesQuantity.obstacleController.obstacleTypes = item.obstacleController.obstacleTypes;
					coinControllerViewModel.tilesQuantity.obstacleController.singleRowBlocker = item.obstacleController.singleRowBlocker;
					coinControllerViewModel.tilesQuantity.obstacleController.rowBlocker = item.obstacleController.rowBlocker;
					coinControllerViewModel.tilesQuantity.obstacleController.obstacleQuantity = item.obstacleController.obstacleQuantity;
					break;
				}
			}
			return coinControllerViewModel;
		}

		//Helping to get Coing Alignment for particular path (If we are using CoinAlignment.RANDOM)
		/// <summary>
		/// Gets the length of the coin.
		/// </summary>
		/// <returns>The coin length.</returns>
		/// <param name="coinController">Coin controller.</param>
		/// <param name="coin">Coin.</param>
		public int GetCoinLength (PathController coinController,TilesQuantity coin)
		{
			//Debug.Log ("IPlaneHelperService : enter in GetCoinLength 11");	
			var coinLength = 0;
			////Debug.Log (coin.coinGenerate.ToString().ToUpper());
			if (coin.coinController.coinsSpawn != COINSPAWN.NIL) {
				if (coin.coinController.coinsSpawn != COINSPAWN.RANDOM) {			
					coinLength = (int)coinController.numberOfCoins;				

				} else {
					var radom = new System.Random ();
					coinLength = radom.Next (0, 11);
				}
			}
			return coinLength;
		}

		/// <summary>
		/// Gets the random type of the coin.
		/// </summary>
		/// <returns>The random coin type.</returns>
		/// <param name="num">Number.</param>
		//It will help to get CoinType while using Random from DropDown
		public COINPATH GetRandomCoinType(int num)
		{
			switch (num) {
			case 1:
				return COINPATH.FLAT;
			case 2:
				return COINPATH.CURVED;
			}
			return COINPATH.FLAT;
		}


		//Help to get Coin Alignment
		/// <summary>
		/// Gets the type of the alignment.
		/// </summary>
		/// <returns>The alignment Name From 3 option Left,Center,Right.</returns>
		/// <param name="alignNum">Align number.</param>
		public COINALIGNMENT GetAlignmentType (int alignNum)
		{
				//Debug.Log ("IPlaneHelperService : enter in GetAlignmentType 10");	
			COINALIGNMENT alignName = COINALIGNMENT.CENTRE;
			switch (alignNum) {
			case 1:
				////Debug.Log ("CoinAlignment.LEFT");
				alignName = COINALIGNMENT.LEFT;
				break;
			case 2:
				////Debug.Log ("CoinAlignment.CENTER");
				alignName = COINALIGNMENT.CENTRE;
				break;
			case 3:
				////Debug.Log ("CoinAlignment.RIGHT");
				alignName = COINALIGNMENT.RIGHT;
				break;
			}
			return alignName;
		}

		/// <summary>
		/// Gets the type of the alignment.
		/// </summary>
		/// <returns>The alignment XPos to place game object either at Left,Center,Right .</returns>
		/// <param name="coinAlignment">Coin alignment.</param>
		public float GetAlignmentType(COINALIGNMENT coinAlignment,PathModel pathSize)
		{
			var xpos=0.0f;
			var widthSection = Convert.ToSingle (pathSize.PathSizeWidth / 3);
			switch(coinAlignment)
			{
			case COINALIGNMENT.LEFT:
				////Debug.Log ("LEFT");
				xpos = -widthSection;
				break;

			case COINALIGNMENT.CENTRE:
				////Debug.Log ("CENTER");
				xpos= Convert.ToSingle(0);
				break;

			case COINALIGNMENT.RIGHT :
				////Debug.Log ("RIGHT");
				xpos= widthSection;
				break;

			case COINALIGNMENT.RANDOM :
				////Debug.Log ("RIGHT");
				var random= new System.Random();
				var alignmentType=GetAlignmentType(random.Next(1,4));

				xpos= GetAlignmentType(alignmentType,pathSize);
				break;

			}
			return xpos;
		}


		/// <summary>
		/// Gets the obstacle section.
		/// </summary>
		/// <returns>The obstacle section.</returns>
		/// <param name="pathSize">Path size.</param>
		/// <param name="noOfObstaclePerPath">No of obstacle per path.</param>
		public float GetObstacleSection(PathModel pathSize,int noOfObstaclePerPath)
		{
			return pathSize.PathSizeLength / noOfObstaclePerPath;
		}

		/// <summary>
		/// Gets the obstacle postion.
		/// </summary>
		/// <returns>The obstacle postion.</returns>
		/// <param name="obstacleSection">Obstacle section.</param>
		public float GetObstaclePostion(float obstacleSection)
		{
			return obstacleSection / 2;

		}

		/// <summary>
		/// Gets the type of the obstacle.
		/// </summary>
		/// <returns>The obstacle type.</returns>
		/// <param name="num">Number.</param>
		//It will help to get GetObstacleType while we are using RANDOM type from DropDownMenu
		public OBSTACLETYPE GetObstacleType (int num)
		{
			switch(num)
			{
			case 1:			
				return OBSTACLETYPE.SINGLE_ROW_BLOCKER;
			case 2:			
				return OBSTACLETYPE.ROW_BLOCKER;
			}
			return OBSTACLETYPE.SINGLE_ROW_BLOCKER;
		}

		/// <summary>
		/// Gets the points for coins.
		/// </summary>
		/// <returns>The points for coins.</returns>
		/// <param name="horizontalLength">Horizontal length.</param>
		/// <param name="verticalLength">Vertical length.</param>
		/// <param name="totalSegments">Total segments.</param>
		//Give  curve points (x,y) for generating curvy coin
		public List<Vector2> GetPointsForCoins (float horizontalLength, float verticalLength, float totalSegments)
		{
			List<Vector2> listPoints = new List<Vector2> ();
			float diffX, diffY;
			float halfSegment = (float)totalSegments / 2f;
			diffX = (float)horizontalLength / (float)totalSegments;
			diffY = (float)verticalLength / halfSegment;
			float x, y;
			for (int i = 1; i <= totalSegments; i++) {
				x = diffX * i;

				if (i <= halfSegment) {
					y = diffY * i;
						
				} else {					
					y = diffY * ((2f * halfSegment) - i + 1);
				}
				listPoints.Add (new Vector2 (x, y));
			}
			//listPoints.Sort ();
			return listPoints;
		}

		//If you are going to add More type of Obsatcakles the you nees also to create more case according to number of obstacle
		// right now SingleLane is 3 so i have generated according to length
		public int GetThreeLaneIndexNumber (ROWBLOCKER threeLane)
		{
			//if you have more than 3 obstacles than you ave to add more cases in it.
			int index = 0;
			switch (threeLane) {

			case ROWBLOCKER.ROWBLOCKER_1:
				index = 0;
				break;
			case ROWBLOCKER.ROWBLOCKER_2:
				index = 1;
				break;
			case ROWBLOCKER.ROWBLOCKER_3:
				index = 2;
				break;
			}
			return index;
		}

		/// <summary>
		/// Gets the obstacle alignment value.
		/// </summary>
		/// <returns>The obstacle alignment value.</returns>
		/// <param name="obstacleAlignment">Obstacle alignment.</param>
		/// <param name="pathSize">Path size.</param>
		public float GetObstacleAlignmentValue(OBSTACLESPAWNDIRECTION obstacleAlignment,PathModel pathSize)
		{
			var xpos = 0.0f;
			switch(obstacleAlignment)
			{
				case OBSTACLESPAWNDIRECTION.LEFT:
				xpos = GetAlignmentType (COINALIGNMENT.LEFT,pathSize);
				break;
			case OBSTACLESPAWNDIRECTION.CENTRE:
				xpos= GetAlignmentType(COINALIGNMENT.CENTRE,pathSize);
				break;
				case OBSTACLESPAWNDIRECTION.RIGHT:
				xpos= GetAlignmentType(COINALIGNMENT.RIGHT,pathSize);
				break;		

			}
			return xpos;
		}

		//If you are going to add More type of Obsatcakles the you nees also to create more case according to number of obstacle
		// right now SingleLane is 3 so i have generated according to length
		public int GetIndexNumber (SINGLEROWBLOCKER singleLane)
		{
			//if you have more than 3 obstacles than you have to add more cases in it.
			int index = 0;
			switch (singleLane) {

			case SINGLEROWBLOCKER.SINGLEROWBLOCKER_1:
				index = 0;
				break;
			case SINGLEROWBLOCKER.SINGLEROWBLOCKER_2:
				index = 1;
				break;
			case SINGLEROWBLOCKER.SINGLEROWBLOCKER_3:
				index = 2;
				break;
			}
			return index;
		}

		/// <summary>
		/// Gets the type of the obstacle.
		/// </summary>
		/// <param name="singleLaneList">Single lane list.</param>
		/// <param name="ThreeLaneList">Three lane list.</param>
		/// <param name="pathQuantity">Coin.</param>
		/// <param name="pathSize">Path size.</param>
		/// <param name="obstaclePosition">Obstacle position.</param>
		/// <param name="_noOfSize">No of size.</param>
		/// <param name="parentPath">Parent path.</param>
		/// <param name="pathName">Path name.</param>
		/// <param name="xpos">Xpos.</param>
		public void GenerateObstacle(ref ObstacleModel obstackleModel,TilesQuantity pathQuantity, PathModel pathSize,float obstaclePosition,int _noOfSize,
		GameObject parentPath,string pathName,float xpos)
		{
			var noOfObstaclePerPath =pathQuantity.obstacleController.obstacleQuantity;
			switch(pathQuantity.obstacleController.obstacleTypes)
			{
			case OBSTACLETYPE.SINGLE_ROW_BLOCKER:
					if (pathQuantity.obstacleController.singleRowBlocker != SINGLEROWBLOCKER.NIL) {

					if (noOfObstaclePerPath > 0) {
						var obstacleSection = GetObstacleSection( pathSize, noOfObstaclePerPath);
						var obstaclePos = GetObstaclePostion (obstacleSection);
						obstaclePosition = obstaclePosition + obstaclePos;
						//Calling by Self 
							GerneratingObstacles (ref  obstackleModel.SingleLaneList, pathQuantity, _noOfSize, ref parentPath, obstaclePosition, pathName, xpos, obstacleSection,
							noOfObstaclePerPath,true,pathSize);
					}
				}
				break;
				case OBSTACLETYPE.ROW_BLOCKER:
				if (noOfObstaclePerPath > 0) {
					var obstacleSection = GetObstacleSection( pathSize, noOfObstaclePerPath);
					var obstaclePos = GetObstaclePostion (obstacleSection);
					obstaclePosition = obstaclePosition + obstaclePos;
					//Calling Function
						GerneratingObstacles (ref  obstackleModel.ThreeLaneList, pathQuantity, _noOfSize, ref parentPath, obstaclePosition, pathName, xpos, obstacleSection, 
						noOfObstaclePerPath,false,pathSize);
				}
				break;
				case OBSTACLETYPE.RANDOM:
				var random = new System.Random ();
					pathQuantity.obstacleController.obstacleTypes = GetObstacleType (random.Next (1, 3));
					pathQuantity.obstacleController.singleRowBlocker = SINGLEROWBLOCKER.RANDOM;
					pathQuantity.obstacleController.rowBlocker = ROWBLOCKER.RANDOM;
				//Calling Function
					GenerateObstacle (ref obstackleModel, pathQuantity, pathSize, obstaclePosition, _noOfSize,parentPath, pathName, xpos);
				break;
			}
		}


		/// <summary>
		/// Gerneratings the obstacles.
		/// </summary>
		/// <param name="obstacleList">Obstacle list.</param>
		/// <param name="pathQuantity">Coin.</param>
		/// <param name="_noOfSize">No of size.</param>
		/// <param name="parentPath">Parent path.</param>
		/// <param name="obstaclePosition">Obstacle position.</param>
		/// <param name="pathName">Path name.</param>
		/// <param name="xpos">Xpos.</param>
		/// <param name="obstacleSection">Obstacle section.</param>
		/// <param name="noOfObstaclePerPath">No of obstacle per path.</param>
		/// <param name="isSingleLane">If set to <c>true</c> is single lane.</param>
		/// <param name="pathSize">Path size.</param>
			void GerneratingObstacles (ref List<GameObject> obstacleList,TilesQuantity pathQuantity,int _noOfSize,ref GameObject parentPath,
			float obstaclePosition, string pathName,float xpos,float obstacleSection,int noOfObstaclePerPath,bool isSingleLane,PathModel pathSize)
			{
				//Debug.Log ("IPlaneHelperService : enter in GerneratingObstacles 16");
				GameObject obstacle;
				var indexNumber = 0;
				//var yPos = 0.0f;

				var obstacleScaleX = ((pathSize.PathSizeWidth / 3));
				var singleLane = SINGLEROWBLOCKER.SINGLEROWBLOCKER_1;
				var threeLane = ROWBLOCKER.ROWBLOCKER_1;
				var obstacleobject = obstacleList [indexNumber];

				var random = new System.Random ();
				// Gernerating Obstacles on path.
				for (int i = 0; i < noOfObstaclePerPath; i++) {
					if (isSingleLane) {
						singleLane = pathQuantity.obstacleController.singleRowBlocker;

						switch (singleLane) {
						case SINGLEROWBLOCKER.RANDOM:						
							//indexNumber is basially the index of list 
							//Right now i have only 3 objects in list which i am using for recreation on path
							// So if you have more than 3 like say 5 then you just need to change the 
							indexNumber = random.Next (0, 3);//from random.Next (0, 3) to random.Next (0, 4) and so on;
							var radNum = random.Next (1, 4);//from random.Next (1, 4) to random.Next (0, 5) and so on.
							obstacleobject = obstacleList [indexNumber];
							if (indexNumber == 0) {
								obstacleScaleX = obstacleobject.transform.localScale.x;
								xpos = 0.0f;
							} else {
								obstacleScaleX = obstacleobject.transform.localScale.x;
								var alignmentType = GetAlignmentType (radNum);
								xpos = GetAlignmentType (alignmentType, pathSize);
							}
							break;
						case SINGLEROWBLOCKER.SINGLEROWBLOCKER_1:
							indexNumber = GetIndexNumber (singleLane);
							obstacleobject = obstacleList [indexNumber];
							obstacleScaleX = obstacleobject.transform.localScale.x;
							xpos = 0.0f;
							break;	
						default:				
							indexNumber = GetIndexNumber (singleLane);
							obstacleobject = obstacleList [indexNumber];
							obstacleScaleX = obstacleobject.transform.localScale.x;
							xpos = GetObstacleAlignmentValue (pathQuantity.obstacleController.obstacleSpawnDirection, pathSize);
							break;			
						}
					} else {
						threeLane = pathQuantity.obstacleController.rowBlocker;
						//yPos = 1.0f;
						xpos = 0.0f;
				
						switch (threeLane) {
						case ROWBLOCKER.RANDOM:						
							//indexNumber is basially the index of list 
							//Right now i have only 3 objects in list which i am using for recreation on path
							// So if you have more than 3 like say 5 then you just need to change the 
							indexNumber = random.Next (0, 3);//from random.Next (0, 3) to random.Next (0, 4);
							obstacleobject = obstacleList [indexNumber];
							break;
						default:				
							indexNumber = GetThreeLaneIndexNumber (threeLane);
							obstacleobject = obstacleList [indexNumber];
							break;			
						}
						obstacleScaleX = obstacleobject.transform.localScale.x;
					}	
					obstacle = Instantiate (obstacleobject, new Vector3 (xpos, obstacleobject.transform.position.y, obstaclePosition), Quaternion.identity)as GameObject;
					obstacle.transform.localScale = new Vector3 (obstacleScaleX, obstacle.transform.localScale.y, obstacle.transform.localScale.z);
					obstacle.transform.parent = parentPath.transform;
					obstaclePosition = obstaclePosition + obstacleSection;
				}
			}
	}
}