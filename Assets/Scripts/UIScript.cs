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

        private void Update()
        {
            if (Move)
            {
                Left.text = "Player 1";
                
            }
            
            if (!Move)
            { 
                Left.text = "Player 2";
                
            }

        }

        public void moveMade()
        {
            Move = !Move;
        }

        /// <summary>
        /// Will be called right at the end of each turn
        /// </summary>
        public void UpdateUI()
        {
            // TODO

            /* To get information from the GameManager you can call functions like
             * Player currentPlayer = gm.GetCurrentPlayer();
             * Make a comment in the GameManager issue over in GitHub requesting any
             * function you think might be useful for you. Try not to access data members.
             * It gets very ugly very fast, so don't do it.
             */
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
