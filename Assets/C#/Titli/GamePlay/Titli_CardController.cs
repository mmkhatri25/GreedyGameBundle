using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Shared;
using KhushbuPlugin;
using Titli.UI;
using Titli.Utility;

namespace Titli.Gameplay
{
    public class Titli_CardController : MonoBehaviour
    {
        public static Titli_CardController Instance;
        public List<Image> _cardsImage;
        public List<Image> owlimages;
        public Titli_BetManager betManager;
        public Action<Transform, Vector3> OnUserInput;
        public Dictionary<Spots, Transform> chipHolder = new Dictionary<Spots, Transform>();
        public bool _startCardBlink, _canPlaceBet;
        public AudioClip AddChip;
        public AudioSource CoinMove_AudioSource;
        public List<GameObject> TableObjs;
        public Transform needle;
        public Transform wheel;
        public float spinDuration = 1f;
        public GameObject[] ItemsForSpin;

        void Awake()
        {
            Instance = this;

        }
        void Start()
        {
            _canPlaceBet = false;
            OnUserInput += CreateChip;
            _startCardBlink = true;
        }

        public IEnumerator CardsBlink(int winno)
        {
            int round = 0;
            if (needle != null)
                SpinToItem(ItemsForSpin[winno].gameObject);
            else
            while ( round <= _cardsImage.Count + winno)
            {   for (int i = 0; i < _cardsImage.Count; i++)
                {
                    
                     owlimages[i].gameObject.SetActive(true);

                    yield return new WaitForSeconds(0.25f);
                    if (round == _cardsImage.Count + winno) yield break;
                    owlimages[i].gameObject.SetActive(false);
                    round ++;
                }
            }
        }

        public void StopCardsBlink()
        {
            _startCardBlink = false;
        }

        public void CreateChipOnReconnet(Spots spott, Chip chipp)
        {
            if (!Titli_UiHandler.Instance.IsEnoughBalancePresent()) return;
            Chip chip = chipp;
            Spots spot = spott;
        }
        void CreateChip(Transform bettingSpot, Vector3 target)
        {
            if (!Titli_UiHandler.Instance.IsEnoughBalancePresent()) return;
            Chip chip = Titli_UiHandler.Instance.currentChip;
            Spots spot = bettingSpot.GetComponent<BettingSpot>().spotType;
            betManager.AddBets(spot, Titli_UiHandler.Instance.currentChip);
        }
        public IEnumerator PlayAudioClip()
        {
            yield return new WaitForSeconds(0.1f);
            CoinMove_AudioSource.clip = AddChip;
            CoinMove_AudioSource.Play();
        }


        //New spin after
        public void SpinToItem(GameObject targetItem)
        {
            Debug.Log("SpinToItem = "+ targetItem.name);

            
            // Find the index of the target item in the array
            int itemIndex = System.Array.IndexOf(ItemsForSpin, targetItem);

            if (itemIndex == -1)
            {
                Debug.LogError("Item not found on the wheel!");
                return;
            }

            // Calculate the angle for each item
            float anglePerItem = 360.0f / ItemsForSpin.Length;

            // Calculate the final rotation angle needed to bring the item to the top (aligned with the notch)
            float targetAngle = anglePerItem * itemIndex;

            // Normalize the target angle within the range [0, 360]
            targetAngle = targetAngle % 360;

            // Calculate the current angle of the wheel
            float currentAngle = wheel.rotation.eulerAngles.z;

            // Calculate the shortest rotation direction (clockwise or counterclockwise)
            float rotationAngle = Mathf.DeltaAngle(currentAngle, targetAngle);

            // Calculate the total rotation angle needed (3 full rotations + target alignment)
            //totalRotationAngle = 1080.0f + rotationAngle;

            // Start the coroutine to spin the wheel
            Debug.Log("SpinToItem rotationAngle = " + rotationAngle);

            StartCoroutine(SpinWheel(rotationAngle));
        }

        private IEnumerator SpinWheel(float totalRotationAngle)
        {
            float elapsedTime = 0.0f;
            float startAngle = wheel.rotation.eulerAngles.z;

            // Determine the final angle after spinning three rounds plus target alignment
            float finalAngle = startAngle + totalRotationAngle;

            // Calculate the duration based on the total rotation angle
            float rotationDuration = spinDuration; // Adjust this if needed

            while (elapsedTime < rotationDuration)
            {
                elapsedTime += Time.deltaTime;

                // Use an easing function to control the speed (ease-in, ease-out)
                float t = elapsedTime / rotationDuration;
                float easedT = Mathf.SmoothStep(0, 1, t);  // SmoothStep provides a smooth ease-in and ease-out

                float newAngle = Mathf.LerpAngle(startAngle, finalAngle, easedT);
                wheel.rotation = Quaternion.Euler(0, 0, newAngle);

                // Log the new angle for debugging
                Debug.Log($"New Angle: {newAngle}");

                yield return null;
            }

            // Snap to the exact final angle
            wheel.rotation = Quaternion.Euler(0, 0, finalAngle);


            Debug.Log($"SpinWheel Final Angle: {wheel.rotation.eulerAngles.z}");
        }


        //public void RotateNeedleToTarget(GameObject target, float duration)
        //{
        //    StartCoroutine(RotateNeedleCoroutine(target.transform.position, duration));
        //}

        //private IEnumerator RotateNeedleCoroutine(Vector3 targetPosition, float duration)
        //{
        //    Vector3 directionToTarget = targetPosition - needle.position;
        //    float targetAngle = Mathf.Atan2(directionToTarget.y, directionToTarget.x) * Mathf.Rad2Deg - 90f;
        //    float startAngle = needle.eulerAngles.z;
        //    float elapsedTime = 0f;

        //    while (elapsedTime < duration)
        //    {
        //        elapsedTime += Time.deltaTime;
        //        float currentAngle = Mathf.LerpAngle(startAngle, targetAngle, elapsedTime / duration);
        //        needle.rotation = Quaternion.Euler(0, 0, currentAngle);
        //        yield return null;
        //    }
        //    needle.rotation = Quaternion.Euler(0, 0, targetAngle);
        //    yield return new WaitForSeconds(6f);
        //    needle.rotation = Quaternion.Euler(0, 0, 0);
        //}
    }
}
