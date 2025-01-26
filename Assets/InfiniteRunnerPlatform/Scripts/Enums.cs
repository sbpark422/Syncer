namespace Xeemu.PathAutoGen
{
	#region CoinSection
	/// <summary>
	/// COINALIGNMENT.
	/// </summary>
	public enum COINALIGNMENT{
		LEFT,
		CENTRE,
		RIGHT,
		RANDOM
	}
	/// <summary>
	/// NOOFCOIN.
	/// </summary>
	public enum NOOFCOINS{
		_10=10,
		_20=20,
	}

	/// <summary>
	/// COINTYPE.
	/// </summary>
	public enum COINPATH{
		FLAT,
		CURVED,
		RANDOM
	}
	/// <summary>
	/// COINGENERATION.
	/// </summary>
	public enum COINSPAWN{
		NIL,
		YES,
		RANDOM
	}

	#endregion

	#region Obstacle section
	/// <summary>
	/// OBSTACLEALIGNMENT.
	/// </summary>
	public enum OBSTACLESPAWNDIRECTION{
		LEFT,
		CENTRE,
		RIGHT,
		RANDOM
	}
	/// <summary>
	/// OBSTACLETYPE.
	/// </summary>
	public enum OBSTACLETYPE{
		NIL,
		SINGLE_ROW_BLOCKER,
		ROW_BLOCKER,
		RANDOM
	 
	}
	//Note: If you want increase the obstacle The you have to make case in switch statement for the 
	//      added once other wise you will get error at runtime.

	/// <summary>
	/// SINGLELANE.
	/// </summary>
	public enum SINGLEROWBLOCKER{
		NIL,
		SINGLEROWBLOCKER_1,
		SINGLEROWBLOCKER_2,
		SINGLEROWBLOCKER_3,
		RANDOM
	}
	//Note: If you want increase the obstackle The you have to make case for the 
	//      added once other wise you will get error at runtime.

	/// <summary>
	/// THREELANE.
	/// </summary>
	public enum ROWBLOCKER{
		NIL,
		ROWBLOCKER_1,
		ROWBLOCKER_2,
		ROWBLOCKER_3,
		RANDOM
	}

	#endregion.

	#region Player Section
	/// <summary>
	/// PLAYERSIDE.
	/// </summary>
	public enum PLAYERSIDE
	{
		LEFT,
		CENTER,
		RIGHT
	}
	#endregion
//
//	/// <summary>
//	/// Speed.
//	/// </summary>
//	public enum Speed{
//		_10 = 10,
//		_20 = 20,
//		_30 = 30,
//	};
}
