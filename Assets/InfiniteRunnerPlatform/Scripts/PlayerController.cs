using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
namespace Xeemu.PathAutoGen
{

public class PlayerController : MonoBehaviour {
	

		#region Private variable
		private readonly IPlayerService iPlayerService;
		private float newXPos = 0.0f;
		private Rigidbody playerRigidBody;
		private float _currentPathSpeed=0.0f;
		private int coinValue=0;
		#endregion

		#region Public Vaiables
		public Animator animatorPlayer;
		public Camera mainCamera;
		public GameObject restartButton;
		[HideInInspector]
		public  bool isSikdding=false;
		[HideInInspector]
		public  bool isGameOver=false;
		[HideInInspector]
		public  bool isJump=false;
		[HideInInspector]
		public  bool isGround= true;
		PLAYERSIDE playerSide = PLAYERSIDE.CENTER;
		public Text coinLabel;
		public GameObject coinParticle;
		public GameObject obstacleParticle;
		#endregion 

		//Constructor
		PlayerController()
		{
			iPlayerService = new PlayerService ();
		}	

		// Use this for initialization
		void Start () 
		{
			playerSide = PLAYERSIDE.CENTER;
			playerRigidBody = GetComponent<Rigidbody> ();	
			coinParticle.SetActive (false);
			obstacleParticle.SetActive (false);
		}
	
		// Update is called once per frame
		void Update () {
			
			#if UNITY_EDITOR_WIN   || UNITY_EDITOR_OSX || UNITY_WEBPLAYER
			keyboardMovement();
			#endif 
			#if UNITY_ANDROID
			TouchMovement();
			#endif 
			#if UNITY_IPHONE
			TouchMovement();
			#endif 
			var pos=Mathf.Lerp(transform.position.x, newXPos, Time.deltaTime * 15);
			// Its help the player to reatin the same position i.e on ground 
			if (!isJump && isGround) {	
				//if player is on ground then its y postion always freeze to 0.59.
				transform.position = new Vector3 (pos, (transform.position.y > 0.5f ? 0.59f : transform.position.y), transform.position.z);
			} else {
				//help player to jump properly.
				transform.position = new Vector3 (pos, transform.position.y, transform.position.z);
			}
			mainCamera.transform.position = new Vector3(pos, mainCamera.transform.position.y, mainCamera.transform.position.z);
		}

		#if  UNITY_EDITOR_WIN  || UNITY_EDITOR_OSX
		/// <summary>
		/// Keyboards the movement.
		/// </summary>
		private void  keyboardMovement()
		{
			var yPos = Convert.ToSingle (Math.Round (transform.position.y, 2));
			//calling Services Method
			//iPlayerService.keyboardMovement (ref playerSide, ref newXPos, ref playerRigidBody, ref yPos);
		}
		#endif 

		#if UNITY_ANDROID || UNITY_IPHONE  
		/// <summary>
		/// Touchs the movement.
		/// </summary>
		private void TouchMovement()
		{
			if (Input.touchCount > 0)
			{
				var yPos = Convert.ToSingle (Math.Round (transform.position.y, 2));
				//calling Services Method
				iPlayerService.touchMovement (ref playerSide, ref newXPos, ref playerRigidBody, ref yPos);
			}

		}
		#endif

		/// <summary>
		/// Raises the trigger enter event.
		/// </summary>
		/// <param name="triggerCollider">Trigger collider.</param>
		void OnTriggerEnter(Collider triggerCollider)
		{
			coinParticle.SetActive (false);
			obstacleParticle.SetActive (false);
			//calling Services Method
			iPlayerService.EnterTrigger(ref coinValue,ref coinLabel,ref triggerCollider,ref restartButton, ref _currentPathSpeed);
		}
			
		/// <summary>
		/// Restarts the game.
		/// </summary>
		public void RestartGame()
		{
			//load scene
			SceneManager.LoadScene (0);
		}
	}
}