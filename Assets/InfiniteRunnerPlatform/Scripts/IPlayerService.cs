using UnityEngine;
using System.Collections;
using UnityEngine.UI;


namespace Xeemu.PathAutoGen
{

public interface IPlayerService {	
	/// <summary>
	/// Keyboards the movement.
	/// </summary>
	/// <param name="playerSide">Player side.</param>
	/// <param name="newXPos">New X position.</param>
	/// <param name="playerRigidBody">Player rigid body.</param>
	/// <param name="playerYPos">Player Y position.</param>
	void keyboardMovement (ref PLAYERSIDE playerSide,ref float newXPos,ref Rigidbody playerRigidBody,ref float playerYPos);

	/// <summary>
	/// Touchs the movement.
	/// </summary>
	/// <param name="playerSide">Player side.</param>
	/// <param name="newXPos">New X position.</param>
	/// <param name="playerRigidBody">Player rigid body.</param>
	/// <param name="playerYPos">Player Y position.</param>
	void touchMovement (ref PLAYERSIDE playerSide, ref float newXPos, ref Rigidbody playerRigidBody, ref float playerYPos);

	/// <summary>
	/// Enters the trigger.
	/// </summary>
	/// <param name="coinValue">Coin value.</param>
	/// <param name="coinLabel">Coin label.</param>
	/// <param name="triggerCollider">Trigger collider.</param>
	/// <param name="restartButton">Restart button.</param>
	/// <param name="currentPathSpeed">Current path speed.</param>
	void EnterTrigger (ref int coinValue, ref Text coinLabel, ref Collider triggerCollider,ref GameObject restartButton,ref float currentPathSpeed);

}
}
