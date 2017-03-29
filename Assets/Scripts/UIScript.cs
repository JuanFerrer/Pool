using UnityEngine;
using System.Collections;

namespace Pool
{
    public class UIScript : MonoBehaviour
    {
        private GameManagerScript gm;

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
        public void UpdatePowerBar(float power)
        {

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
