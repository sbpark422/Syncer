using UnityEngine;
using System.Collections;
using System.Collections.Generic;


namespace Xeemu.PathAutoGen
{
	/// <summary>
	/// I plane helper service.
	/// </summary>
	public interface IPlaneHelperService {
		/// <summary>
		/// Coins the generate process.
		/// </summary>
		/// <param name="coins">Coins.</param>
		/// <param name="xpos">Xpos.</param>
		/// <param name="coinPos">Coin position.</param>
		/// <param name="parentPath">Parent path.</param>
		/// <param name="yPos">Y position.</param>
		void CoinGenerateProcess (GameObject coins, float xpos, float coinPos, GameObject parentPath, float yPos);

		//Help to get values for GetCoinControllerItem
		PathControllerViewModel GetCoinControllerItem (PathController coinController,string pathName);

		//Helping to get Coing Alignment for particular path (If we are using CoinAlignment.RANDOM)
		/// <summary>
		/// Gets the length of the coin.
		/// </summary>
		/// <returns>The coin length.</returns>
		/// <param name="coinController">Coin controller.</param>
		/// <param name="coin">Coin.</param>
		int GetCoinLength (PathController coinController,TilesQuantity coin);


		//It will help to get CoinType while using Random from DropDown

		/// <summary>
		/// Gets the random type of the coin.
		/// </summary>
		/// <returns>The random coin type.</returns>
		/// <param name="num">Number.</param>

		COINPATH GetRandomCoinType(int num);

		/// <summary>
		/// Gets the type of the alignment.
		/// </summary>
		/// <returns>The alignment type.</returns>
		/// <param name="alignNum">Align number.</param>
		//Help to get Coin Alignment
		COINALIGNMENT GetAlignmentType (int alignNum);

		/// <summary>
		/// Gets the type of the alignment.
		/// </summary>
		/// <returns>The alignment type.</returns>
		/// <param name="coinAlignment">Coin alignment.</param>

		float GetAlignmentType(COINALIGNMENT coinAlignment,PathModel pathSize);


		/// <summary>
		/// Gets the obstacle section.
		/// </summary>
		/// <returns>The obstacle section.</returns>
		/// <param name="pathSize">Path size.</param>
		/// <param name="noOfObstaclePerPath">No of obstacle per path.</param>
		float GetObstacleSection(PathModel pathSize,int noOfObstaclePerPath);

		/// <summary>
		/// Gets the obstacle postion.
		/// </summary>
		/// <returns>The obstacle postion.</returns>
		/// <param name="obstacleSection">Obstacle section.</param>
		float GetObstaclePostion(float obstacleSection);

		

		//It will help to get GetObstacleType while we are using RANDOM type from DropDownMenu
		/// <summary>
		/// Gets the type of the obstacle.
		/// </summary>
		/// <returns>The obstacle type.</returns>
		/// <param name="num">Number.</param>
		OBSTACLETYPE GetObstacleType (int num);


		/// <summary>
		/// Gets the points for coins.
		/// </summary>
		/// <returns>The points for coins.</returns>
		/// <param name="horizontalLength">Horizontal length.</param>
		/// <param name="verticalLength">Vertical length.</param>
		/// <param name="totalSegments">Total segments.</param>
		//Give  curve points (x,y) for generating curvy coin
		List<Vector2> GetPointsForCoins (float horizontalLength, float verticalLength, float totalSegments);


		//If you are going to add More type of Obsatcakles the you nees also to create more case according to number of obstacle
		// right now SingleLane is 3 so i have generated according to length
		int GetThreeLaneIndexNumber (ROWBLOCKER threeLane);

		/// <summary>
		/// Gets the obstacle alignment value.
		/// </summary>
		/// <returns>The obstacle alignment value.</returns>
		/// <param name="obstacleAlignment">Obstacle alignment.</param>
		/// <param name="pathSize">Path size.</param>
		float GetObstacleAlignmentValue(OBSTACLESPAWNDIRECTION obstacleAlignment,PathModel pathSize);

		//If you are going to add More type of Obsatcakles the you nees also to create more case according to number of obstacle
		// right now SingleLane is 3 so i have generated according to length
		int GetIndexNumber (SINGLEROWBLOCKER singleLane);


		/// <summary>
		/// Gets the type of the obstacle.
		/// </summary>
		/// <param name="singleLaneList">Single lane list.</param>
		/// <param name="ThreeLaneList">Three lane list.</param>
		/// <param name="_coin">Coin.</param>
		/// <param name="pathSize">Path size.</param>
		/// <param name="obstaclePosition">Obstacle position.</param>
		/// <param name="_noOfSize">No of size.</param>
		/// <param name="parentPath">Parent path.</param>
		/// <param name="pathName">Path name.</param>
		/// <param name="xpos">Xpos.</param>
		void GenerateObstacle (ref ObstacleModel obstackleModel, TilesQuantity _coin, PathModel pathSize, float obstaclePosition, int _noOfSize,
		                      GameObject parentPath, string pathName, float xpos);
	}
}