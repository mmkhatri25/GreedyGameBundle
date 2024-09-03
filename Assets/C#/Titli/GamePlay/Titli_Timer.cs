using System;
using System.Collections;
using UnityEngine;
using Titli.Utility;
using Newtonsoft.Json;
using System.Collections.Generic;
using Titli.UI;
using Titli.ServerStuff;
using UnityEngine.UI;
using KhushbuPlugin;
using TMPro;

namespace Titli.Gameplay
{
    public class Titli_Timer : MonoBehaviour
    {
        public static Titli_Timer Instance;
        public GameObject waitForBetScreen;
        int bettingTime = 30;
        int timeUpTimer = 5;
        // int waitTimer = 3;
        public Action onTimeUp;
        public Action onCountDownStart;
        public Action startCountDown;
        public static gameState gamestate;
       public TMP_Text countdownTxt, waittext;
        public Text todaywin;
        [Header ("First Time rotate wheel")]
        public GameObject WheelToRotate;
        // [SerializeField] TMP_Text messageTxt;
        private void Awake()
        {
            Instance = this;
            
            //Titli_ServerRequest.instance.socket.Emit(Events.winnerList);
        }
        void onWinnerListREceived()
        {

        }
        void Start()
        {
            // Titli_UiHandler.Instance.ShowMessage("please wait for next round...");
            gamestate = gameState.cannotBet;
            // onTimeUp?.Invoke();
            // onTimeUp();
            if(is_a_FirstRound)
            {
                print("this is fists time  - "+ is_a_FirstRound);
                // Titli_CardController.Instance._winNo = true;
                // for(int i = 0; i < Titli_CardController.Instance.TableObjs.Count; i++)
                // {
                //     Titli_CardController.Instance.TableObjs[i].GetComponent<BoxCollider2D>().enabled = false;
                // }
            }
        }    

        public void OnCurrentTime(object data = null)
        {
            // is_a_FirstRound = true;
            //if (is_a_FirstRound)
            //{
            //    waitForBetScreen.SetActive(true);
            //}
            //else
            {
            waitForBetScreen.SetActive(false);
           // onTimeUp();
           
            InitialData init = new InitialData();
            try
            {
                init = Utility.Utility.GetObjectOfType<InitialData>(data.ToString());

            }
            catch (Exception e)
            {
                Debug.Log(e.Message);
            }
                //if (init.gametimer > 5)
                //{
                // Titli_UiHandler.Instance.lessthanFiveSec = false;
                //print("Here getting gametimer... - "+ init.gametimer + " round first "+ is_a_FirstRound);
                // here is setting the stop bet....
                if (init.gametimer >=2)
                {
                   is_a_FirstRound = false;
                //print("Here setting true"+ init.gametimer + " round first "+ is_a_FirstRound);

                }
                OnTimerStart(init.gametimer);
                waitForBetScreen.SetActive(false);
                
            //}
            //else if (init.gametimer < 5)
            //{
            //    Titli_UiHandler.Instance.lessthanFiveSec = true;
            //    onTimeUp();
            //    StartCoroutine(currentTimer(init.gametimer));
            //}
                todaywin.text = init.currentWin.ToString();
                
         }
       }

        IEnumerator currentTimer(int currentGametimer)
        {
            for (int i = currentGametimer; i >= 0; i--)
            {
                // Titli_UiHandler.Instance.ShowMessage("please wait for next round... " + i.ToString() );
                yield return new WaitForSecondsRealtime(1f);
            }
        }

        public void OnTimerStart(int time)
        {
            print("OnTimerStart...");
            // here is setting the stop bet....
            // if (is_a_FirstRound)
            // {
            //     Titli_UiHandler.Instance.HideMessage();
            // }
            // is_a_FirstRound = false;
            Titli_UiHandler.Instance.ResetUi();
            Titli_CardController.Instance._startCardBlink = false;
            Titli_CardController.Instance._canPlaceBet = true;
            // Titli_UiHandler.Instance.ResetUi();
            StartCoroutine(timerStartCountdown(time));

            for(int i = 0; i < Titli_CardController.Instance.TableObjs.Count; i++)
            {
                Titli_CardController.Instance.TableObjs[i].GetComponent<BoxCollider2D>().enabled = true;
            }

            if (time >= 2)
            {
                print("Time available - " + time);
            }
            else
            {
                print("Time not available - " + time);
                for (int i = 0; i < Titli_CardController.Instance.TableObjs.Count; i++)
                {
                    print(" button " + Titli_CardController.Instance.TableObjs[i].name);
                    Titli_CardController.Instance.TableObjs[i].GetComponent<BoxCollider2D>().enabled = false;

                }
            }

            // StopCoroutines();
        }

        //this will run once it connected to the server
        //it will carry the time and state of server
        IEnumerator timerStartCountdown(int time)
        {
            onCountDownStart?.Invoke();
            gamestate = gameState.canBet;
            Titli_CardController.Instance._canPlaceBet = true;
            for (int i = time; i >= 0; i--)
            {
                if (i == 1)
                {
                    Debug.Log("1 countdown - " + i);

                    startCountDown?.Invoke();
                    //countdownTxt.text = "wait..";
                    //print("here countdown become 0 ...");
                countdownTxt.text = i.ToString();


                }
                else
                {
                    //waittext.gameObject.SetActive(false);
                countdownTxt.text = i.ToString();
                }

                if (i <= 0)
                {
                    Debug.Log("2 countdown - "+ i);
                    countdownTxt.text = "";

                    //waittext.gameObject.SetActive(true);
                    //countdownTxt.gameObject.SetActive(false);
                }
                // Debug.Log("Timer:" +i);
                if (i > 5)
                    Titli_Timer.Instance.waitForBetScreen.SetActive(false);
                yield return new WaitForSecondsRealtime(1f);
            }
            
            // Titli_ServerResponse.Instance.TimerUpFunction();
            onTimeUp?.Invoke();

        }


        public void OnTimeUp(object data)
        {
            if (is_a_FirstRound) return;
            
            Titli_CardController.Instance._canPlaceBet = false;
                waitForBetScreen.SetActive(false);
            
            // for(int i = 0; i < Titli_CardController.Instance.TableObjs.Count; i++)
            // {
            //     Titli_CardController.Instance.TableObjs[i].GetComponent<BoxCollider2D>().enabled = false;
            // }
            // StopCoroutines();
            StartCoroutine(TimeUpCountdown());
        }

        IEnumerator TimeUpCountdown(int time = -1)
        {
            gamestate = gameState.cannotBet;
            onTimeUp?.Invoke();
            Titli_CardController.Instance._startCardBlink = true;
            Titli_CardController.Instance._canPlaceBet = false;

            // foreach(var item in Titli_CardController.Instance._cardsImage)
            // {
            //     item.GetComponent<Button>().interactable = false;
            // }
            // StartCoroutine(Titli_CardController.Instance.CardsBlink());
            if(WheelToRotate!=null)
                StartCoroutine(Rotate360Degrees());
            for (int i = time != -1 ? time : timeUpTimer; i >= 0; i--)
            {
                // messageTxt.text = "Time Up";
                //countdownTxt.text = "Time Up";//i.ToString();
                print("======here counting ... " +countdownTxt.text );
                
                yield return new WaitForSecondsRealtime(1f);
            }
            countdownTxt.text = "";

        }


        //first round of wheel fast rotation.
        public AnimationCurve rotationCurve;
        private IEnumerator Rotate360Degrees()
        {
            float totalRotation = 0f;
            float duration = 9f; // Reduced total time for the rotation to make it faster
            float elapsedTime = 0f;

            while (elapsedTime < duration)
            {
                // Calculate the percentage of completion
                float t = elapsedTime / duration;

                // Get the speed factor from the animation curve
                float speedFactor = rotationCurve.Evaluate(t);

                // Calculate rotation for this frame
                float rotationThisFrame = speedFactor * 360 * Time.deltaTime * 1; // Increase rotation speed

                // Apply rotation to the GameObject
                WheelToRotate.transform.Rotate(0, 0, -rotationThisFrame);

                // Update total rotation
                totalRotation += rotationThisFrame;

                // Update elapsed time
                elapsedTime += Time.deltaTime;

                // Wait until next frame
                yield return null;
            }


        }



        public bool is_a_FirstRound = true;
   
    }
    [Serializable]
    public class CurrentTimer
    {
        public gameState gameState;
        public int timer;
        public List<int> lastWins;
        public int LeftBets;
        public int MiddleBets;
        public int RightBets;
    }
    public enum gameState
    {
        canBet = 0,
        cannotBet = 1,
        wait = 2,
    }

    public class InitialData
    {
        public List<int> previousWins;
        public List<BotsBetsDetail> botsBetsDetails;
        public string balance;
        public int gametimer;
        public int userDailyWin;
        public double currentWin;
    }
}
