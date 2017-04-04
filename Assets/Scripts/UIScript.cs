using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace Pool
{
    public class UIScript : MonoBehaviour
    {
        private GameManagerScript gm;
        int NumBallsIn = 0;
        private Slider powerBar;
        string player1Type = "";
        string player2Type = "";
        private Text Left;
        private Text Score;
        GameObject LeftBox;
        GameObject RightBox;
        GameObject GameManager;
        bool Move = true;

        private void Start()
        {
            Left = GameObject.Find("LeftText").GetComponent<Text>();                   
        }

        /// <summary>
        /// Update info about player
        /// </summary>
        /// <param name="currentPlayer"></param>
        public void UpdatePlayer(Player currentPlayer)
        {
            Left.text = "Player " + (currentPlayer.GetPlayerNo() + 1);
        }
        

        /// <summary>
        /// 
        /// </summary>
        /// <param name="power"></param>
        public void SetCuePower(float power)
        {
            powerBar = GetComponentInChildren<Slider>();
            powerBar.value = power;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ballNo"></param>
        public void PutOnRack(int ballNo)
        {
            if (ballNo > 0 && ballNo < 15)
            {
                string currentBall = "Ball" + ballNo;
                GameObject go = GameObject.Find(currentBall);
                float xVal = 475;
                go.transform.position = new Vector3(xVal + (25 * NumBallsIn), 25, 0);
                NumBallsIn++;
            }                  
        }
    
        /// <summary>
        /// Just for initialisation. GameManager will call this on construction
        /// </summary>
        /// <param name="newGM"></param>
        public void SetGMReference(GameManagerScript newGM)
        {
            gm = newGM;
        }
    }
}
