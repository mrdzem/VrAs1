using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using Photon.Pun;


public class GameScript : MonoBehaviourPun
{
    public GameObject GameSphere;
    public GameObject gameSelectionButtons;
    public GameObject scoreBoardPrefab;

    public InputActionProperty gameActivation;
    public InputActionProperty multiGameActivation;

    public GameObject rightHandCollider;
    public GameObject leftHandCollider;

    public InputActionProperty rightHandGrab;
    public InputActionProperty leftHandGrab;

    private List<GameObject> spheres = new List<GameObject>();
    private List<Vector3> spheresOffset = new List<Vector3>();
    private float radius = 0.6f;

    private GameObject gameModeSelect;
    private Vector3 gameButtonsOffset;

    private GameObject scoreBoard;
    private Vector3 scoreBoardOffset;

    private bool isRightPressed = false;
    private bool isLeftPressed = false;

    private float countDown = 10;
    private int singlePlayerRound = 0;
    private int singlePlayerRoundStage = 0; // 0 -> Generate random time, 1 -> Awaiting sphere to hit, 2 -> time to hit
    private float timeTillNextHit;
    private float betweenRoundsTime = 3f;
    private float roundStartTime;
    private float singlePlayerTimeScore = 0f;
    private int currentHitSphere = 0;
    int selected = -1;

    private bool isPlayerOne = false;
    private bool isPlayerTwo = false;
    private int myPlayerNumber = -1;

    private float multiCountDown = 10;
    private int multiPlayerRoundStage = 0; // 0 -> Generate random time, 1 -> Awaiting sphere to hit, 2 -> time to hit
    private float multiTimeTillNextHit;
    private int currentMultiHitSphere = 0;
    int player1Score = 0;
    int player2Score = 0;
    bool didPlayer1hit = false;
    bool didPlayer2hit = false;
    float player1LastTime;
    float player2LastTime;
    float stage1Timer = 3f;

    bool isNewRound = false;

    int someTestValue = 0;



    private int gameSetupStage = 0;

    // Start is called before the first frame update
    void Start()
    {
        //Generate game spheres
        for (int i = 0; i < 5; i++)
        {
            var instance = Instantiate(GameSphere, this.transform);
            spheres.Add(instance);
            spheresOffset.Add(new Vector3());
            spheres[i].SetActive(false);
        }
        gameModeSelect = Instantiate(gameSelectionButtons, this.transform);
        gameModeSelect.SetActive(false);

        scoreBoard = Instantiate(scoreBoardPrefab, this.transform);
        scoreBoard.SetActive(false);


    }

    // Update is called once per frame

    /// ///////////////////// ///////////////////// ///////////////////// ///////////////////// ///////////////////// ///////////////////// //////////////////

    void Update()
    {
        menuHandler();

        if (multiGameActivation.action.WasPressedThisFrame())
        {
            joinGame();
        }



        //Updating position of game spheres
        if (gameActivation.action.WasPressedThisFrame())
        {



            if (gameSetupStage == 0)
            {

                activateButtons();
                gameSetupStage++;
            }
            else
            {
                resetGame();
            }
        }
        updateButtonsPossition();
        updateSpheresPosition();
        updateScoreBoardPosition();

        if (gameSetupStage == 2)
        {
            preGameCountdown();
        }
        if (gameSetupStage == 3)
        {
            singlePlayerGame();
        }
        if (gameSetupStage == 5)
        {
            

        }
        if (gameSetupStage == 8)
        {
            queue();
        }
        if(isPlayerOne && isPlayerTwo)
        {
            if (gameSetupStage == 6)
            {
                multiPreGameCountDown();
            }
            if (gameSetupStage == 7)
            {
                multiGame();
            }
        }else if(gameSetupStage > 5 && gameSetupStage < 8)
        {
            updateScoreBoardText("Other player left the game");
        }
        

    }
    /// ///////////////////// ///////////////////// ///////////////////// ///////////////////// ///////////////////// ///////////////////// //////////////////
    //Multiplayer

    private void multiGame()
    {
        Debug.Log("Current stage:  " + multiPlayerRoundStage);
        if (player1Score < 10 && player2Score < 10)
        {
            if (multiPlayerRoundStage == 0)
            {
                if (myPlayerNumber == 1)
                {
                    updateScoreBoardText("Me   " + player2Score + " : " + player1Score + "   Opponent");
                }
                if (myPlayerNumber == 0)
                {
                    updateScoreBoardText("Me   " + player1Score + " : " + player2Score + "   Opponent");
                }
                spheres[currentHitSphere].GetComponent<Renderer>().material.color = Color.white;
                if (myPlayerNumber == 1)
                {
                    photonView.RPC("multiDrawHitSphere", RpcTarget.AllBuffered);
                    photonView.RPC("multiDrawTimeTillNext", RpcTarget.AllBuffered);
                    
                }
                multiPlayerRoundStage = 1;
                isNewRound = true;
                stage1Timer = 3f;
            }
            if (multiPlayerRoundStage == 1)
            {
                stage1Timer -= Time.deltaTime;
                if(stage1Timer < 0)
                {
                    if (isNewRound)
                    {
                        isNewRound = false;
                        currentHitSphere = currentMultiHitSphere;
                        timeTillNextHit = multiTimeTillNextHit;
                        Debug.Log("local sphere; " + currentMultiHitSphere + "     local time left; " + timeTillNextHit);
                    }
                    timeTillNextHit -= Time.deltaTime;
                    Debug.Log(timeTillNextHit);
                    if (timeTillNextHit < 0)
                    {
                        multiPlayerRoundStage = 2;
                        roundStartTime = Time.time;
                        spheres[currentHitSphere].GetComponent<Renderer>().material.color = Color.blue;
                    }
                }
                
            }
            if (multiPlayerRoundStage == 2)
            {
                Debug.Log("did player 1 hit: " + didPlayer1hit + "   did player 2 hit: " + didPlayer2hit);
                if (rightHandGrab.action.WasPressedThisFrame())
                {
                    if (checkSphereCollision(rightHandCollider) == currentHitSphere)
                    {
                        photonView.RPC("updatePlayerLastTime", RpcTarget.AllBuffered, myPlayerNumber, Time.time - roundStartTime);
                        singlePlayerTimeScore += Time.time - roundStartTime;
                        Debug.Log("single playewr time " + singlePlayerTimeScore);
                        photonView.RPC("updateDidPlayer", RpcTarget.All, myPlayerNumber, true);

                        spheres[currentHitSphere].GetComponent<Renderer>().material.color = Color.green;
                    }
                }
                Debug.Log("play1 time: " + player1LastTime + "   play2 time: " + player2LastTime);
                if (myPlayerNumber == 1 && didPlayer1hit && didPlayer2hit)
                {
                    if(player1LastTime > player2LastTime)
                    {
                        photonView.RPC("increasePlayerScore", RpcTarget.All, 1);
                    }
                    else
                    {
                        photonView.RPC("increasePlayerScore", RpcTarget.All, 0);
                    }
                    photonView.RPC("updateDidPlayer", RpcTarget.All, 0, false);
                    photonView.RPC("updateDidPlayer", RpcTarget.All, 1, false);
                    photonView.RPC("updateRoundStatus", RpcTarget.AllBuffered, 3);
                }
            }
            if (multiPlayerRoundStage == 3)
            {
                

                if (myPlayerNumber == 1)
                {
                    betweenRoundsTime -= Time.deltaTime;
                    if (betweenRoundsTime < 0)
                    {

                        photonView.RPC("updateRoundStatus", RpcTarget.AllBuffered, 0);
                        betweenRoundsTime = 3;
                    }
                }
                
            }
        }
        else
        {
            if(myPlayerNumber == 1)
            {
                if(player2Score == 10)
                {
                    var outputString = "WIN!!! \n Your average rection time was: " + (singlePlayerTimeScore / player2Score).ToString() + "\n The lower score the better ";
                    updateScoreBoardText(outputString);
                }
                else
                {
                    var outputString = "You´ve lost :( \n Your average rection time was: " + (singlePlayerTimeScore / player2Score).ToString() + "\n The lower score the better ";
                    updateScoreBoardText(outputString);
                }
            }
            else
            {
                if (player1Score == 10)
                {
                    var outputString = "WIN!!! \n Your average rection time was: " + (singlePlayerTimeScore / player1Score).ToString() + "\n The lower score the better ";
                    updateScoreBoardText(outputString);
                }
                else
                {
                    var outputString = "You´ve lost :( \n Your average rection time was: " + (singlePlayerTimeScore / player1Score).ToString() + "\n The lower score the better ";
                    updateScoreBoardText(outputString);
                }
            }
            
            gameSetupStage = 9;
            
        }
    }

    private void multiPreGameCountDown()
    {
        if(myPlayerNumber == 1)
        {
            photonView.RPC("updateCountDownRPC", RpcTarget.AllBuffered, multiCountDown - Time.deltaTime);
        }
        
        if (multiCountDown <= 5 && multiCountDown >= 0)
        {
            updateScoreBoardText(Mathf.Ceil(multiCountDown).ToString());
        }
        if (multiCountDown < 0)
        {
            gameSetupStage = 7;
            updateScoreBoardText("");
        }
    }

    private void queue()
    {

        if(myPlayerNumber == -1)
        {
            updateScoreBoardText("Other players are currently playing");
        }
        else if (isPlayerOne && isPlayerTwo)
        {
            gameSetupStage = 6;
            updateScoreBoardText("MultiPlayer game is about to start \n Grab Blue spheres with Right Hand \n And Yellow with your left");
        }
        else
        {
            updateScoreBoardText("Waiting for second player");
        }
    }


    #region Custom Methods
    private void joinGame()
    {
        if (!isPlayerOne)
        {
            myPlayerNumber = 0;
            photonView.RPC("addPlayerRPC", RpcTarget.AllBuffered);
            
        }
        else if (!isPlayerTwo)
        {
            myPlayerNumber = 1;
            photonView.RPC("addPlayerRPC", RpcTarget.AllBuffered);
            
        }
        else
        {
            myPlayerNumber = -1;
        }
        gameSetupStage = 8;
    }
    #endregion

    #region RPCS

    [PunRPC]
    public void testPUN(int val)
    {
        someTestValue = val + 1;

    }

    [PunRPC]
    public void resetPlayersScore()
    {
        player1Score = 0;
        player2Score = 0;
    }

    [PunRPC]
    public void increasePlayerScore(int player)
    {
        if (player == 0)
        {
            player1Score++;
        }
        else
        {
            player2Score++;
        }
    }

    [PunRPC]
    public void updateDidPlayer(int player, bool b)
    {
        if (player == 0)
        {
            didPlayer1hit = b;
        }
        else
        {
            didPlayer2hit = b;
        }
    }

    [PunRPC]
    public void updatePlayerLastTime(int player, float time)
    {
        if(player == 0)
        {
            player1LastTime = time;
        }
        else
        {
            player2LastTime = time;
        }
    }

    [PunRPC]
    public void updateRoundStatus(int i)
    {
        multiPlayerRoundStage = i;
    }

    [PunRPC]
    public void multiDrawTimeTillNext()
    {
        multiTimeTillNextHit = Random.Range(2, 5);
    }

    [PunRPC]
    public void multiDrawHitSphere()
    {
        currentMultiHitSphere = (int)Mathf.Floor(Random.Range(0, 5));
    }


    [PunRPC]
    public void updateCountDownRPC(float newCount)
    {
        multiCountDown = newCount;
    }

    [PunRPC]
    public void addPlayerRPC()
    {
        if (!isPlayerOne)
        {
            Debug.Log("add playerRPC 1");
            isPlayerOne = true;
        }
        else if (!isPlayerTwo)
        {
            Debug.Log("add playerRPC 2");
            isPlayerTwo = true;
        }
    }

    [PunRPC]
    public void removePlayersRPC(int playerNum)
    {
        if (playerNum == 0)
        {
            isPlayerOne = false;
        }
        else if (playerNum == 1)
        {
            isPlayerTwo = false;
        }
    }

    

    #endregion



    /// ///////////////////// ///////////////////// ///////////////////// ///////////////////// ///////////////////// ///////////////////// //////////////////

    public bool menuHandler()
    {
        Renderer[] buttonColor = gameModeSelect.GetComponentsInChildren<Renderer>();
        //right press
        if (rightHandGrab.action.WasPressedThisFrame() && !isLeftPressed)
        {
            isRightPressed = true;
            if (gameSetupStage == 1)
            {
                selected = buttonPressCheck(rightHandCollider);
            }
        }
        //left press
        if (leftHandGrab.action.WasPressedThisFrame() && !isRightPressed)
        {
            isLeftPressed = true;
            if (gameSetupStage == 1)
            {
                selected = buttonPressCheck(leftHandCollider);
            }
        }

        //right release
        if (!rightHandGrab.action.IsPressed() && isRightPressed)
        {
            isRightPressed = false;
            int gameModeSelected = 1;


            if (gameSetupStage == 1)
            {
                gameModeSelected = buttonReleaseCheck(rightHandCollider, selected);
                if (gameModeSelected >= 0)
                {
                    unactivateButtons();
                    sphereActivation();
                    activateScoreBoard();
                    gameSetupStage = gameModeSelected;
                }
                
                buttonColor[0].material.color = Color.white;
                buttonColor[2].material.color = Color.white;
                
            }
            selected = -1;
            if (gameModeSelected == 5)
            {
                updateScoreBoardText("Press left menu button to enter lobby");
                return true;
            }
         



        }
        //left relese
        if (!leftHandGrab.action.IsPressed() && isLeftPressed)
        {
            
            if (gameSetupStage == 1)
            {
                int gameModeSelected = buttonReleaseCheck(leftHandCollider, selected);
                if (gameModeSelected >= 0)
                {
                    unactivateButtons();
                    sphereActivation();
                    activateScoreBoard();
                    gameSetupStage = gameModeSelected;
                }

                buttonColor[0].material.color = Color.white;
                buttonColor[2].material.color = Color.white;
            }
            isLeftPressed = false;
            selected = -1;

        }
        return false;
    }

    private void resetGame()
    {
        gameSetupStage = 0;
        unactivateButtons();
        unactivateSpheres();
        unactivateScoreBoard();
        countDown = 8;
        singlePlayerRound = 0;
        singlePlayerRoundStage = 0;
        betweenRoundsTime = 3f;
        singlePlayerTimeScore = 0f;
        for (int i = 0; i < spheres.Count; i++)
        {
            spheres[i].GetComponent<Renderer>().material.color = Color.white;
        }

        if(myPlayerNumber != -1)
        {
            photonView.RPC("removePlayersRPC", RpcTarget.AllBuffered, myPlayerNumber);
            myPlayerNumber = -1;
        }
}

    private void singlePlayerGame()
    {
        if(singlePlayerRound < 10)
        {
            if(singlePlayerRoundStage == 0)
            {
                timeTillNextHit = Random.Range(2, 5);
                drawHitSphere();
                singlePlayerRoundStage++;
            }
            if (singlePlayerRoundStage == 1)
            {
                timeTillNextHit -= Time.deltaTime;
                if(timeTillNextHit < 0)
                {
                    singlePlayerRoundStage++;
                    roundStartTime = Time.time;
                    spheres[currentHitSphere].GetComponent<Renderer>().material.color = Color.blue;
                } 
            }
            if (singlePlayerRoundStage == 2)
            {
                if (rightHandGrab.action.WasPressedThisFrame())
                {
                    if (checkSphereCollision(rightHandCollider) == currentHitSphere)
                    {
                        singlePlayerRound++;
                        singlePlayerTimeScore += Time.time - roundStartTime;
                        singlePlayerRoundStage++;
                        var outputString = "Your last reaction time was: " + (Time.time - roundStartTime).ToString();
                        updateScoreBoardText(outputString);
                        spheres[currentHitSphere].GetComponent<Renderer>().material.color = Color.green;
                    }
                }  
            }
            if(singlePlayerRoundStage == 3)
            {
                betweenRoundsTime -= Time.deltaTime;
                if(betweenRoundsTime < 0)
                {
                    spheres[currentHitSphere].GetComponent<Renderer>().material.color = Color.white;
                    singlePlayerRoundStage = 0;
                    betweenRoundsTime = 1;
                }
            }
        }
        else
        {
            var outputString = "Your average rection time was: " + (singlePlayerTimeScore/10).ToString() + "\n The lower score the better ";
            gameSetupStage = 4;
            updateScoreBoardText(outputString);
        }
    }

    private void drawHitSphere()
    {
        currentHitSphere = (int)Mathf.Floor(Random.Range(0, 5));
    }

    private void preGameCountdown()
    {
        countDown -= Time.deltaTime;
        if (countDown <= 5 && countDown >= 0)
        {
            updateScoreBoardText(Mathf.Ceil(countDown).ToString());
        }
        if (countDown < 0)
        {
            gameSetupStage = 3;
            updateScoreBoardText("");
        }
    }

    private void updateScoreBoardText(string newText)
    {
        scoreBoard.GetComponentInChildren<TextMeshPro>().text = newText;
    }

    private int buttonReleaseCheck(GameObject hand, int pressedButton)
    {
        
        if (checkButtonCollision(hand) == 0 && checkButtonCollision(hand) == pressedButton)
        {
            return 2;
        }
        else if (checkButtonCollision(hand) == 1 && checkButtonCollision(hand) == pressedButton)
        {
            return 5;
        }
        return -1;
        
    }

    private int buttonPressCheck(GameObject hand)
    {
        int selectedButton = checkButtonCollision(hand);
        
        Renderer[] buttonColor = gameModeSelect.GetComponentsInChildren<Renderer>();
        if (selectedButton == 0)
        {
            buttonColor[selectedButton].material.color = Color.green;
        }
        else if (selectedButton == 1)
        {
            buttonColor[2].material.color = Color.green;
        }
        return selectedButton;
    }
    
    private int checkButtonCollision(GameObject handCollider)
    {
        Collider[] buttons = gameModeSelect.GetComponentsInChildren<Collider>();
        for (int i = 0; i < 2; i++)
        {
            if (handCollider.GetComponent<Collider>().bounds.Intersects(buttons[i].bounds))
            {
                return i;
            }
        }
        return -1;
    }

    private int checkSphereCollision(GameObject handCollider)
    {
        for (int i = 0; i < spheres.Count; i++)
        {
            if (handCollider.GetComponent<Collider>().bounds.Intersects(spheres[i].GetComponent<Collider>().bounds))
            {
                return i;
            }
        }
        return -1;
    }

    private void sphereActivation()
    {
        for (int i = 0; i < spheres.Count; i++)
        {
            spheres[i].SetActive(true);

            float cornerAngle =
                -(2f * Mathf.PI + ((2f * Mathf.PI) / ((float)spheres.Count * 3f) * i) // align
                - 23f / 30f * Mathf.PI //offset to center spheres
                + this.transform.eulerAngles.y * Mathf.Deg2Rad); //player head angle

            spheres[i].transform.position = new Vector3(Mathf.Cos(cornerAngle) * radius, -0.3f, Mathf.Sin(cornerAngle) * radius) + this.transform.position + spheres[i].transform.forward * 0.2f;

            spheresOffset[i] = this.transform.position - spheres[i].transform.position; //save offset
        }
    }

    private void updateSpheresPosition()
    {
        for (int i = 0; i < spheres.Count; i++)
        {
            spheres[i].transform.position = this.transform.position - spheresOffset[i];
        }
    }

    private void activateButtons()
    {
        gameModeSelect.transform.position = this.transform.position + this.transform.forward * 0.6f;
        gameButtonsOffset = this.transform.position - gameModeSelect.transform.position;
        gameModeSelect.SetActive(true);
    }

    private void updateButtonsPossition()
    {
        gameModeSelect.transform.position = this.transform.position - gameButtonsOffset;
        gameModeSelect.transform.LookAt(this.transform.position, Vector3.up);
        gameModeSelect.transform.Rotate(new Vector3(0, -90, 0), Space.World);
    }

    private void unactivateButtons()
    {
        gameModeSelect.SetActive(false);

    }

    private void unactivateSpheres()
    {
        for (int i = 0; i < spheres.Count; i++)
        {
            if (spheres[i].activeSelf)
            {
                spheres[i].SetActive(false);
            }
        }
    }

    private void activateScoreBoard()
    {
        updateScoreBoardText("The game is about to start \n Grab Blue spheres with Right Hand \n And Yellow with your left");
        scoreBoard.transform.position = this.transform.position + this.transform.forward;
        scoreBoardOffset = this.transform.position - scoreBoard.transform.position;
        scoreBoard.SetActive(true);
        
    }

    private void updateScoreBoardPosition()
    {
        scoreBoard.transform.position = this.transform.position - scoreBoardOffset;
        scoreBoard.transform.LookAt(this.transform.position, Vector3.up);
        scoreBoard.transform.Rotate(new Vector3(0, -180, 0), Space.World);
    }

    private void unactivateScoreBoard()
    {
        scoreBoard.SetActive(false);
    }

    
}


