using UnityEngine;
using System.Collections;

namespace Pool
{
    public class Player
    {
        private int mPlayerNo;
        private BallType mPlayerType;
        private int mScore;

        public int GetPlayerNo() { return mPlayerNo; }
        public BallType GetPlayerType() { return mPlayerType; }
        public int GetScore() { return mScore; }

        public void SetPlayerNo(int newVal) { mPlayerNo = newVal; }
        public void SetPlayerType(BallType newVal) { mPlayerType = newVal; }

        public void SetScore(int newVal) { mScore = newVal; }
    }
}
