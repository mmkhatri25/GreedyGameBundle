using System.Collections;
using System.Collections.Generic;
using Mosframe;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
namespace Titli.Gameplay
{
    public class RankitemSetup : UIBehaviour, IDynamicScrollViewItem
    {
        public Image dp;
        public Text winAmount, Username, UserRank;

        public void onUpdateItem(int index)
        {
            if (LeaderBoardScreen.instance.isWeekly)
            {
                Username.text = LeaderBoardScreen.instance.weeklyUsers[index].name;
                winAmount.text = LeaderBoardScreen.instance.weeklyUsers[index].amount.ToString();
                UserRank.text = (index + 1).ToString();
                LeaderBoardScreen.instance.SetImageFromURL(LeaderBoardScreen.instance.weeklyUsers[index].profile_pic, dp, null);
            }
            else
            {
                Username.text = LeaderBoardScreen.instance.dailyUsers[index].name;
                winAmount.text = LeaderBoardScreen.instance.dailyUsers[index].amount.ToString();
                UserRank.text = (index + 1).ToString();
                LeaderBoardScreen.instance.SetImageFromURL(LeaderBoardScreen.instance.dailyUsers[index].profile_pic, dp, null);
            }
        }
    }
}