using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
namespace Xeemu.PathAutoGen
{
	/// <summary>
	/// Player service.
	/// </summary>
	public class PlayerService : MonoBehaviour ,IPlayerService{	

		private readonly IPlaneHelperService _iPlaneHerplerService;

		private PlayerController _playerController;
		private GameObject _playerControllerObject;

		private GameObject _gameManagerObject;
		private PlanePathController _planePathController;

		private float _fingerStartTime  = 0.0f;
		private Vector2 _fingerStartPos = Vector2.zero;
		private bool _isSwipe = false;
		private float _minSwipeDist  = 20.0f;
		private float _maxSwipeTime = 0.2f;
		/// <summary>
		/// Initializes a new instance of the <see cref="Xeemu.PathAutoGen.PlayerService"/> class.
		/// </summary>
		public PlayerService()
		{
			_iPlaneHerplerService = new PlanePathHelperService ();

		}
			

	
		#region IPlayerService implementation

		/// <summary>
		/// Gets the player X postion.
		/// </summary>
		/// <returns>The player X postion.</returns>
		/// <param name="playerSide">Player side.</param>
		/// <param name="newXPos">New X position.</param>
		/// <param name="playerRigidBody">Player rigid body.</param>
		/// <param name="playerYPos">Player Y position.</param>
		public void keyboardMovement(ref PLAYERSIDE playerSide,ref float newXPos,ref Rigidbody playerRigidBody,ref float playerYPos)
		{
			if (_playerControllerObject == null) {
				FindPlayerControllerObject ();
			}
			if (!_playerController.isGameOver) {

				if (Input.GetKeyDown (KeyCode.LeftArrow) || Input.GetKeyDown (KeyCode.A)) {

					LeftArrowOrLeftSwipe (ref playerSide, ref newXPos);

				} else if (Input.GetKeyDown (KeyCode.RightArrow) || Input.GetKeyDown (KeyCode.D)) {
					RightArrowOrRightSwipe (ref playerSide, ref newXPos);

				} else if (Input.GetKeyDown (KeyCode.UpArrow) || Input.GetKeyDown (KeyCode.W)) {
				
					UpArrowOrJump (ref playerYPos, ref playerRigidBody);

				} else if (Input.GetKeyDown (KeyCode.DownArrow) || Input.GetKeyDown (KeyCode.S)) {
					DownArrowOrSikidding (ref  playerRigidBody);
				}
			}

		}

		/// <summary>
		/// Touchs the movement.
		/// </summary>
		/// <param name="playerSide">Player side.</param>
		/// <param name="newXPos">New X position.</param>
		/// <param name="playerRigidBody">Player rigid body.</param>
		/// <param name="playerYPos">Player Y position.</param>
		public void touchMovement(ref PLAYERSIDE playerSide,ref float newXPos,ref Rigidbody playerRigidBody,ref float playerYPos)
		{
			if (_playerControllerObject == null) {
				FindPlayerControllerObject ();
			}
			if (Input.touchCount > 0 && !_playerController.isGameOver)
			{
				foreach (Touch touch in Input.touches)
				{  
					switch (touch.phase)
					{
					case TouchPhase.Began :
						/* this is a new touch */
						_isSwipe = true;
						_fingerStartTime = Time.time;
						_fingerStartPos = touch.position;
						break;
					case TouchPhase.Canceled :
						/* The touch is being canceled */
						_isSwipe = false;
						break;
					case TouchPhase.Moved:
						float gestureTime = Time.time - _fingerStartTime;
						float gestureDist = (touch.position - _fingerStartPos).magnitude;
						if (_isSwipe && gestureTime < _maxSwipeTime && gestureDist > _minSwipeDist) {
							Vector2 direction = touch.position - _fingerStartPos;
							Vector2 swipeType = Vector2.zero;
							if (Mathf.Abs (direction.x) > Mathf.Abs (direction.y)) {
								// the swipe is horizontal:
								swipeType = Vector2.right * Mathf.Sign (direction.x);
							} else {
								// the swipe is vertical:
								swipeType = Vector2.up * Mathf.Sign (direction.y);
							}
							if (swipeType.x != 0.0f) {
								if (swipeType.x > 0.0f) {
									_isSwipe = false;
									// MOVE RIGHT
									RightArrowOrRightSwipe (ref playerSide, ref newXPos);

								} else {
									_isSwipe = false;
									// MOVE LEFT
									LeftArrowOrLeftSwipe (ref playerSide, ref newXPos);
								}
							}
							if (swipeType.y != 0.0f) {
								if (swipeType.y > 0.0f) {
									_isSwipe = false;
									// MOVE UP
									UpArrowOrJump(ref playerYPos,ref playerRigidBody);//								

								} else if (swipeType.y < 0.0f) {   
									_playerController.StopCoroutine (CanWait (0.01f));

									_isSwipe = false;
									Debug.Log ("<color=blue> this is jump back  hit    </color>         " + "MOVE DOWN");
									DownArrowOrSikidding (ref  playerRigidBody);

								} else {
									_isSwipe = false;
									// MOVE DOWN
									DownArrowOrSikidding (ref  playerRigidBody);
								}

							}

						}

						break;
					}
				}
			}

		} 

		
		/// <summary>
		/// Enters the trigger.
		/// </summary>
		/// <param name="coinValue">Coin value.</param>
		/// <param name="coinLabel">Coin label.</param>
		/// <param name="triggerCollider">Trigger collider.</param>
		/// <param name="restartButton">Restart button.</param>
		/// <param name="currentPathSpeed">Current path speed.</param>
		public void EnterTrigger (ref int coinValue ,ref Text coinLabel,ref Collider triggerCollider,ref GameObject restartButton,ref float currentPathSpeed){
			if (_playerControllerObject == null) {
				FindPlayerControllerObject ();
			}
			if (triggerCollider.gameObject.tag.ToLower () == "obstacle" ) 
			{
				_playerController.obstacleParticle.SetActive (true);
				GameOverMethod (ref restartButton,ref currentPathSpeed);
			}

			if (triggerCollider.gameObject.tag.ToLower () == "skidding") 
			{
				if (!_playerController.isSikdding) {
					_playerController.obstacleParticle.SetActive (true);
					GameOverMethod (ref restartButton, ref currentPathSpeed);
				}
			}
			if (triggerCollider.gameObject.tag.ToLower () == "coins") 
			{
				
				_playerController.coinParticle.SetActive (true);
				coinValue++;
				coinLabel.text = coinValue.ToString ();
				triggerCollider.gameObject.SetActive (false);

			}
		}

		#endregion
			


		#region Helper Section
		/// <summary>
		/// Finds the P layer controller object.
		/// </summary>
		void FindPlayerControllerObject()
		{
			_playerControllerObject = GameObject.FindWithTag ("Player");
			if (_playerControllerObject != null) {
				_playerController = _playerControllerObject.GetComponent <PlayerController> ();					
			}
		}

		/// <summary>
		/// Finds the game manager controller.
		/// </summary>
		void FindGameManagerController()
		{
			_gameManagerObject = GameObject.FindWithTag ("GameManger");
			if (_gameManagerObject != null) {
				_planePathController = _gameManagerObject.GetComponent <PlanePathController> ();
			}
		}

		/// <summary>
		/// Gets the player X postion.
		/// </summary>
		/// <returns>The player X postion.</returns>
		/// <param name="playerSide">Player side.</param>
		public float GetPlayerXPostion (PLAYERSIDE playerSide)
		{
			COINALIGNMENT coinAlignment = COINALIGNMENT.CENTRE;

			switch(playerSide){
			case PLAYERSIDE.CENTER:
				coinAlignment = COINALIGNMENT.CENTRE;
				break;
			case PLAYERSIDE.LEFT:
				coinAlignment = COINALIGNMENT.LEFT;
				break;
			case PLAYERSIDE.RIGHT:
				coinAlignment = COINALIGNMENT.RIGHT;
				break;

			}
			var pathModel = new PathModel();
			if (_gameManagerObject == null) {
				FindGameManagerController ();
			}
			if (_gameManagerObject != null) {			
			pathModel.PathSizeLength = _planePathController.tileLength;
				pathModel.PathSizeWidth = _planePathController.tileWidth;
			}

			//Aliginment
			return _iPlaneHerplerService.GetAlignmentType(coinAlignment,pathModel);

		}

		/// <summary>
		/// Lefts the arrow or left swipe.
		/// </summary>
		/// <param name="playerSide">Player side.</param>
		/// <param name="newXPos">New X position.</param>
		void LeftArrowOrLeftSwipe(ref PLAYERSIDE playerSide,ref float newXPos)
		{
			if (playerSide == PLAYERSIDE.CENTER) {				
				playerSide = PLAYERSIDE.LEFT;
				newXPos = GetPlayerXPostion (playerSide);
			} else if (playerSide == PLAYERSIDE.RIGHT) {				
				playerSide = PLAYERSIDE.CENTER;
				newXPos = GetPlayerXPostion (playerSide);
			}
		}

		/// <summary>
		/// Rights the arrow or right swipe.
		/// </summary>
		/// <param name="playerSide">Player side.</param>
		/// <param name="newXPos">New X position.</param>
		void RightArrowOrRightSwipe(ref PLAYERSIDE playerSide,ref float newXPos)
		{
			if (playerSide == PLAYERSIDE.LEFT) {
				playerSide = PLAYERSIDE.CENTER;
				newXPos = GetPlayerXPostion (playerSide);
			} else if (playerSide == PLAYERSIDE.CENTER) {				
				playerSide = PLAYERSIDE.RIGHT;
				newXPos = GetPlayerXPostion (playerSide);
			}
		}

		/// <summary>
		/// Ups the arrow or jump.
		/// </summary>
		/// <param name="playerYPos">Player Y position.</param>
		/// <param name="playerRigidBody">Player rigid body.</param>
		void UpArrowOrJump(ref float playerYPos,ref Rigidbody playerRigidBody)
		{
			
			if (_gameManagerObject == null) {
				FindGameManagerController ();
			}
			if (!_playerController.isJump && _playerController.isGround) {
				
				_playerController.isJump = true;
				_playerController.isGround = false;
				_playerController.animatorPlayer.SetBool ("SkiddingDown", false);

				if (_playerControllerObject != null) {
					playerRigidBody.velocity = new Vector3 (0, 5.58f, 0);
					_playerController.animatorPlayer.SetBool ("Jump", true);
					_playerController.StartCoroutine (CanWait (0.85f));
				}
			}

		}

		/// <summary>
		/// Downs the arrow or sikidding.
		/// </summary>
		void DownArrowOrSikidding(ref Rigidbody playerRigidBody)
		{
				_playerController.animatorPlayer.SetBool ("Jump", false);
			    _playerController.isJump = false;
			
			if (_playerControllerObject != null) {
				///Debug.Log ("Before : isSikdding".ToUpper()+playerController.isSikdding);
				_playerController.isSikdding = true;
				playerRigidBody.velocity = new Vector3 (0, -2.5f, 0);

				//Debug.Log ("After : isSikdding".ToUpper()+playerController.isSikdding);
				_playerController.animatorPlayer.SetBool ("SkiddingDown", true);
				//playerController.Invoke ("StopSkidding", 0.5f);
				_playerController.StartCoroutine (StopSkidding (0.5f));
			}
		}

		/// <summary>
		/// Determines whether this instance can wait the specified jumpTime.
		/// </summary>
		/// <returns><c>true</c> if this instance can wait the specified jumpTime; otherwise, <c>false</c>.</returns>
		/// <param name="jumpTime">Jump time.</param>
		IEnumerator CanWait(float jumpTime)
		{
			yield return new WaitForSeconds (jumpTime);
			_playerController.animatorPlayer.SetBool ("Jump", false);
//			if (_planePathController.speed >= 10 && _planePathController.speed < 20)
//				_playerController.StartCoroutine (IsGrounded (0.5f));
//			 else {
				_playerController.StartCoroutine (IsGrounded (0.2f));
			//}
		}

		IEnumerator IsGrounded(float jumpTime)
		{
			yield return new WaitForSeconds (jumpTime);
			_playerController.isJump = false;
			_playerController.isGround = true;
		}

		/// <summary>
		/// Stops the skidding.
		/// </summary>
		/// <returns>The skidding.</returns>
		/// <param name="skiddingTime">Skidding time.</param>
		IEnumerator StopSkidding(float skiddingTime)
		{   
			
			yield return new WaitForSeconds (skiddingTime);
			_playerController.isSikdding = false;
			_playerController.animatorPlayer.SetBool ("SkiddingDown", false);

		}


		void GameOverMethod(ref GameObject restartButton,ref float currentPathSpeed)
		{
			_playerController.animatorPlayer.SetBool ("HitAndFall", true);
			if (_gameManagerObject == null) {
				FindGameManagerController ();
			}
			if (_gameManagerObject != null) {	
				_playerController.isGameOver = true;
				currentPathSpeed = _planePathController.speed;
				_planePathController.speed = 0.0f;
				restartButton.SetActive (true);
			}
		}

			#endregion

	}
}