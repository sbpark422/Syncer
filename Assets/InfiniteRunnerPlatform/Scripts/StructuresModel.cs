using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
namespace Xeemu.PathAutoGen
{

	//defining structures 

	/// <summary>
	/// Path controller.
	/// </summary>
	[Serializable]
	public struct PathController {
		public NOOFCOINS numberOfCoins;
		[Range(10.0f, 40.0f)]
		public float platformSpeed;
		//public Speed speed;
		public List<TilesQuantity> tilesQuantity;
	}

	/// <summary>
	/// Coin controller view model.
	/// </summary>
	[Serializable]
	public struct PathControllerViewModel {
		public NOOFCOINS numberOfCoins;
		public TilesQuantity tilesQuantity;
	}

	#region Helping Structures for CoinConroller
	/// <summary>
	/// Path quantity.
	/// </summary>
	[Serializable]
	public struct TilesQuantity {
		public GameObject tile;
		public CoinController coinController;
		public ObstacleController obstacleController;
	}

	/// <summary>
	/// Obstacle controller.
	/// </summary>
	[Serializable]
	public struct ObstacleController {
		//public Path path;
		public int obstacleQuantity;
		public OBSTACLETYPE obstacleTypes ;
		public SINGLEROWBLOCKER singleRowBlocker ;
		public ROWBLOCKER rowBlocker ;
		public OBSTACLESPAWNDIRECTION obstacleSpawnDirection ;

	}

	/// <summary>
	/// Coin controller.
	/// </summary>
	[Serializable]
	public struct CoinController {
		//public Path path;
		public COINALIGNMENT coinsAlignment ;
		public COINSPAWN coinsSpawn ;
		public COINPATH coinsPath ;
	}
	#endregion



}


	
