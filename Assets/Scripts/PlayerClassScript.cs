using UnityEngine;
using System.Collections;

namespace Pool
{
    public enum BallType { SPOT, STRIPE };

    public class Player
    {
        private int mPlayerNo;
        private BallType mPlayerType;

        public int GetPlayerNo() { return mPlayerNo; }
        public BallType GetPlayerType() { return mPlayerType; }

        public void SetPlayerNo(int newVal) { mPlayerNo = newVal; }
        public void SetPlayerType(BallType newVal) { mPlayerType = newVal; }
    }
}
