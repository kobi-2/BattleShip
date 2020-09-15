﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;


public static class FlagClass {

	public static bool[] rowStatus { get; set; }

}



public class PlayerGetInstance : NetworkBehaviour {

	[SerializeField]
	public Button dialogueButton;

	public DoneButtonScript doneButtonScript;
	public GameObject localTile0, localTile1, localTile2;
	public GameObject remoteTile0, remoteTile1, remoteTile2;

	public GameObject tile;
	public Color color;
	public bool hasColorValue;

	public bool isServersTurn, thisIsServer ;

	public bool isGameOver, serverWins, localPlayerWins, isTied;
	public int serverPoints, localPlayerPoints;

	void Start () {
		
		//Object[] objects = Object.FindObjectsOfType<GameObject.>;
		//GameObject ob = GameObject.FindGameObjectsWithTag;
		//this.GetInstanceID;

		hasColorValue = false;

		getTiles ();
		isServersTurn = false;

		isGameOver = false;

		serverWins = false;
		localPlayerWins = false;
		isTied = false;

		serverPoints = 0;
		localPlayerPoints = 0;


		/*
		RemotePlayer remotePlayer = new RemotePlayer();
		remotePlayer.playerInstanceID = this.GetInstanceID ();
		remotePlayer.remoteTiles [0] = localTile0;
		remotePlayer.remoteTiles [1] = localTile1;
		remotePlayer.remoteTiles [2] = localTile2;
		
		CmdSendTiles (remotePlayer);
		*/

	}


	
	void Update () {

		getTiles ();

		/*
		RemotePlayer remotePlayer = new RemotePlayer();
		remotePlayer.playerInstanceID = this.GetInstanceID ();
		remotePlayer.remoteTiles [0] = localTile0;
		remotePlayer.remoteTiles [1] = localTile1;
		remotePlayer.remoteTiles [2] = localTile2;

		CmdSendTiles (remotePlayer);
		*/

	}


	public void calculateIsServer(){
		if (isServer) {
			thisIsServer = true;
		}
	}


	void getTiles(){

		if (isLocalPlayer) {
			doneButtonScript = this.GetComponentInChildren<DoneButtonScript> ();

			localTile0 = doneButtonScript.tile0;
			localTile1 = doneButtonScript.tile1;
			localTile2 = doneButtonScript.tile2;
		}




		if (isServer) {

			thisIsServer = true;


			GameObject[] gameObjects = GameObject.FindGameObjectsWithTag ("Battleship");
			gameObjects [0].GetComponent<Renderer> ().material.color = Color.red;
			//gameObjects [1].GetComponent<Renderer> ().material.color = Color.green;
			//gameObjects [1].GetComponentInChildren<MeshRenderer> ().enabled = false;
			//gameObjects [0].GetComponentInChildren<MeshRenderer> ().enabled = false;

			Renderer[] obj = gameObjects[1].GetComponentsInChildren<MeshRenderer> ();
			for (int i = 0; i < obj.Length; i++) {
				obj [i].enabled = false;
			}

			gameObjects [0].GetComponentInChildren<MeshRenderer> ().enabled = true;


			DoneButtonScript dB = gameObjects [1].GetComponentInChildren<DoneButtonScript> ();// index 1 is the remote player for server
			dB.calculateTilePosition ();
			//dB.calculateTilePosition (FlagClass.rowStatus[0]);

			remoteTile0 = dB.tile0;
			remoteTile1 = dB.tile1;
			remoteTile2 = dB.tile2;

		}

		if (isLocalPlayer) {

			thisIsServer = false;
		

			GameObject[] gameObjects = GameObject.FindGameObjectsWithTag ("Battleship");
			//gameObjects [0].GetComponent<Renderer> ().material.color = Color.blue;
			gameObjects [1].GetComponent<Renderer> ().material.color = Color.black;
			//gameObjects [0].GetComponentInChildren<MeshRenderer> ().enabled = false;
			//gameObjects [1].GetComponentInChildren<MeshRenderer> ().enabled = false;

			if (gameObjects.Length > 1) {
				Renderer[] obj = gameObjects [0].GetComponentsInChildren<MeshRenderer> ();
				for (int i = 0; i < obj.Length; i++) {
					obj [i].enabled = false;
				}
			}

			DoneButtonScript dB = gameObjects [0].GetComponentInChildren<DoneButtonScript> ();	// index 0 is the server
			dB.calculateTilePosition ();
			//FlagClass.rowStatus[gameObjects.Length-1] = ShipOrientation.rowWise;
			//dB.calculateTilePosition (FlagClass.rowStatus[gameObjects.Length-1]);

			remoteTile0 = dB.tile0;
			remoteTile1 = dB.tile1;
			remoteTile2 = dB.tile2;

		}


		/*
		for (int i = 0; i < gameObjects.Length; i++) {
			
			DoneButtonScript dB = gameObjects [i].GetComponent<DoneButtonScript> ();

			if ((localTile0 != dB.tile0) || (localTile1 != dB.tile1) || (localTile2 != dB.tile2)) {

				remoteTile0 = dB.tile0;
				remoteTile1 = dB.tile1;
				remoteTile2 = dB.tile2;

				break;
			}
		
		}
		*/



/*
		DoneButtonScript[] doneButtonScriptsArray = this.GetComponents<DoneButtonScript> ();	

		for(int i=0; i<doneButtonScriptsArray.Length; i++){
			
			if ((localTile0 != doneButtonScriptsArray [i].tile0) || (localTile1 != doneButtonScriptsArray [i].tile1) || (localTile2 != doneButtonScriptsArray [i].tile2)) {

				remoteTile0 = doneButtonScriptsArray[i].tile0;
				remoteTile1 = doneButtonScriptsArray[i].tile1;
				remoteTile2 = doneButtonScriptsArray[i].tile2;
				
				break;
			}
		
		}

*/

	}




	[Command]
	public void CmdSendTiles(RemotePlayer remotePlayer){
		RpcGetRemoteTiles (remotePlayer);
	}



	[ClientRpc]
	void RpcGetRemoteTiles (RemotePlayer remotePlayer){

		if (remotePlayer.playerInstanceID != this.GetInstanceID ()) {
		
			remoteTile0 = remotePlayer.remoteTiles [0];
			remoteTile1 = remotePlayer.remoteTiles [1];
			remoteTile2 = remotePlayer.remoteTiles [2];

		}
	
	}



	[Command]
	public void CmdSendUpdates(GameObject m_tile, Color m_color, bool turnStatus, int m_serverPoints, int m_localPlayerPoints){
		RpcReceiveUpdate (m_tile, m_color, turnStatus, m_serverPoints, m_localPlayerPoints);
	}


	[ClientRpc]
	public void RpcReceiveUpdate(GameObject m_tile, Color m_color, bool turnStatus,int m_serverPoints, int m_localPlayerPoints){
		isServersTurn = turnStatus;

		hasColorValue = true;
		tile = m_tile;
		color = m_color;

		serverPoints = m_serverPoints;
		bool hasScored = false;
		if (m_localPlayerPoints > localPlayerPoints) {
			hasScored = true;
		}
		localPlayerPoints = m_localPlayerPoints;

		showPointTurnMessage ();
	}


	public void showPointTurnMessage (){
		string PointTurnMessage = "";
		if (isServer) {
			PointTurnMessage += "Opponent's points: " + localPlayerPoints + "\n Your points: " + serverPoints + "\n";
			if (isServersTurn) {
				PointTurnMessage += "YOUR TURN";
			} else {
				PointTurnMessage += "OPPONENT'S TURN";
			}
		} else {
			PointTurnMessage += "Opponent's points: " + serverPoints + "\n Your points: " + localPlayerPoints + "\n";
			if (!isServersTurn) {
				PointTurnMessage += "YOUR TURN";
			} else {
				PointTurnMessage += "OPPONENT'S TURN";
			}
		}
		dialogueButton.GetComponentInChildren<Text>().text = PointTurnMessage;

		/*GameObject[] arr = GameObject.FindGameObjectsWithTag ("dialogue");
		for (int i = 0; i < arr.Length; i++) {
			string PointTurnMessage = "";
			if (isServer) {
				PointTurnMessage += "Opponent's points: " + localPlayerPoints + "\n Your points: " + serverPoints + "\n";
				if (isServersTurn) {
					PointTurnMessage += "YOUR TURN";
				} else {
					PointTurnMessage += "OPPONENT'S TURN";
				}
			} else {
				PointTurnMessage += "Opponent's points: " + serverPoints + "\n Your points: " + localPlayerPoints + "\n";
				if (!isServersTurn) {
					PointTurnMessage += "YOUR TURN";
				} else {
					PointTurnMessage += "OPPONENT'S TURN";
				}
			}
			arr[i].GetComponentInChildren<Text>().text = PointTurnMessage;
		}*/
		/*GameObject.Find ("DialogueButton").SetActive (true);
		string PointTurnMessage = "";
		if (isServer) {
			PointTurnMessage += "Opponent's points: " + localPlayerPoints + "\n Your points: " + serverPoints + "\n";
			if (isServersTurn) {
				PointTurnMessage += "YOUR TURN";
			} else {
				PointTurnMessage += "OPPONENT'S TURN";
			}
		} else {
			PointTurnMessage += "Opponent's points: " + serverPoints + "\n Your points: " + localPlayerPoints + "\n";
			if (!isServersTurn) {
				PointTurnMessage += "YOUR TURN";
			} else {
				PointTurnMessage += "OPPONENT'S TURN";
			}
		}
		GameObject.Find("DialogueButton").GetComponentInChildren<Text>().text = PointTurnMessage;*/
	}



	[Command]
	public void CmdSendGameOver(bool m_isTied, bool m_serverWins, bool m_localPlayerWins){
		RpcUpdateGameOver (m_isTied, m_serverWins, m_localPlayerWins);
	}


	[ClientRpc]
	public void RpcUpdateGameOver(bool m_isTied, bool m_serverWins, bool m_localPlayerWins){
		isGameOver = true;
		isTied = m_isTied;
		serverWins = m_serverWins;
		localPlayerWins = m_localPlayerWins;
		//showGameOverMessage ();
	}

	public void showGameOverMessage(){
		GameObject.Find ("DialogueButton").SetActive (true);
		string GameOverMessage = "Game Over\n";
		if (isServer) {
			if (serverWins) {
				GameOverMessage += "YOU WIN!!!";
			} else {
				GameOverMessage += "OPPONENT WINS :(";
			}
		} else {
			if (!serverWins) {
				GameOverMessage += "YOU WIN!!!";
			} else {
				GameOverMessage += "OPPONENT WINS :(";
			}
		}
		GameObject.Find("DialogueButton").GetComponentInChildren<Text>().text = GameOverMessage;
	}



	[Command]
	public void CmdSendUpdateColor(bool m_hasColorValue, GameObject m_tile, Color m_color){
		RpcReceiveUpdateColor (m_hasColorValue, m_tile, m_color);
	}

	[ClientRpc]
	public void RpcReceiveUpdateColor(bool m_hasColorValue, GameObject m_tile, Color m_color){
		hasColorValue = m_hasColorValue;
		tile = m_tile;
		color = m_color;
	}





}




public class RemotePlayer {

	public int playerInstanceID;
	public GameObject[] remoteTiles;

}



