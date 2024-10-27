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

        [Header ("Spin final to item")]
        public Transform wheel;
        public float spinDuration = 3f;
        public GameObject[] ItemsForSpin;
        public InputField testNumber;

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
            //Debug.Log($"needle: {needle}, winno: {winno}");
            if (needle != null)
            {
                //winno = int.Parse( testNumber.text);// test 
                if (winno != 9 && winno != 8)
                    SpinToItem(ItemsForSpin[winno].gameObject);

            }
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

        public float spinDurations = 3f; // Total duration of spin
        public int extraRounds = 2; // Number of extra 360-degree rounds before stopping
        public float maxSpeed = 500f; // Max rotation speed in degrees per second

        public float itemAngleOffset = 360f /8;
        public float rotationSpeed = 100f; // Adjust speed
        private bool isRotating = false;
        //New spin after first
        public void SpinToItem(GameObject targetItem)
        {
            //Debug.Log("SpinToItem = "+ targetItem.name);


            // Find the index of the target item in the array
            int itemIndex = System.Array.IndexOf(ItemsForSpin, targetItem);

            //if (itemIndex == -1)
            //{
            //    //Debug.LogError("Item not found on the wheel!");
            //    return;
            //}

            //// Calculate the angle for each item
            //float anglePerItem = 360.0f / ItemsForSpin.Length;
            //float targetAngle = anglePerItem * itemIndex;
            //targetAngle = targetAngle % 360;
            //float currentAngle = wheel.rotation.eulerAngles.z;
            //float rotationAngle = Mathf.DeltaAngle(currentAngle, targetAngle);
            ////Debug.Log("SpinToItem rotationAngle = " + rotationAngle);

            //StartCoroutine(SpinWheel(rotationAngle));
            // Find the index of the target item in the array


            float angle = itemIndex * itemAngleOffset;

            // Rotate the needle to the calculated angle (assuming Z-axis rotation)
            StartCoroutine(SpinWheel(angle));


            return;
            int targetIndex = System.Array.IndexOf(ItemsForSpin, targetItem);
            if (targetIndex == -1)
            {
                Debug.LogError("Target item not found!");
                return;
            }

            // Calculate angle per item and target angle
            float anglePerItem = 360f / ItemsForSpin.Length;
            float targetAngle = anglePerItem * targetIndex;

            // Normalize angles
            float currentAngle = NormalizeAngle(needle.eulerAngles.z);
            targetAngle = NormalizeAngle(anglePerItem * targetIndex);

            // Calculate the clockwise rotation only
            float rotationAngle = CalculateClockwiseAngle(currentAngle, targetAngle)
                                  + (extraRounds * 360f);

            Debug.Log($"Target Item: {targetItem.name}, Target Angle: {targetAngle}, " +
                      $"Rotation Angle: {rotationAngle}");



            // Start the spin coroutine
            StartCoroutine(SpinWheel(rotationAngle));
        }

        private IEnumerator SpinWheel(float totalRotationAngle)
        {
            //// wheel
            //float elapsedTime = 0.0f;
            //float startAngle = wheel.rotation.eulerAngles.z;
            //float finalAngle = startAngle + totalRotationAngle;
            //float rotationDuration = spinDuration; // Adjust this if needed

            //while (elapsedTime < rotationDuration)
            //{
            //    elapsedTime += Time.deltaTime;

            //    // Use an easing function to control the speed (ease-in, ease-out)
            //    float t = elapsedTime / rotationDuration;
            //    float easedT = Mathf.SmoothStep(0, 1, t);  // SmoothStep provides a smooth ease-in and ease-out

            //    float newAngle = Mathf.LerpAngle(startAngle, finalAngle, easedT);
            //    wheel.rotation = Quaternion.Euler(0, 0, newAngle);

            //    // Log the new angle for debugging
            //    //Debug.Log($"New Angle: {newAngle}");

            //    yield return null;
            //}

            //// Snap to the exact final angle
            //wheel.rotation = Quaternion.Euler(0, 0, finalAngle);

            //yield return new WaitForSeconds(3f);
            //wheel.rotation = Quaternion.Euler(0, 0, finalAngle);
            //// needle
            //float elapsedTime = 0f;
            //float startAngle = needle.eulerAngles.z;
            //float finalAngle = startAngle + totalRotationAngle;

            //while (elapsedTime < spinDuration)
            //{
            //    elapsedTime += Time.deltaTime;

            //    // Smooth interpolation for smooth rotation
            //    float t = Mathf.Clamp01(elapsedTime / spinDuration);
            //    float newAngle = Mathf.Lerp(startAngle, finalAngle, t);

            //    needle.rotation = Quaternion.Euler(0f, 0f, newAngle % 360f);
            //    yield return null;
            //}

            //// Snap to the exact final angle
            //needle.rotation = Quaternion.Euler(0f, 0f, finalAngle % 360f);

            //Debug.Log($"Needle stopped at angle: {needle.rotation.eulerAngles.z}");
            float duration = 1.5f;  // Time for the entire rotation
            //float elapsedTime = 0f;
            //float startAngle = needle.eulerAngles.z;
            //float targetAngle = startAngle - totalRotationAngle;  // Subtract since it's clockwise rotation

            //while (elapsedTime < duration)
            //{
            //    elapsedTime += Time.deltaTime;
            //    // Interpolate between the start and target angle over time
            //    float currentAngle = Mathf.Lerp(startAngle, targetAngle, elapsedTime / duration);
            //    needle.rotation = Quaternion.Euler(0, 0, currentAngle);
            //    yield return null;  // Wait for the next frame
            //}

            //// Ensure the final rotation is exactly the target angle
            //needle.rotation = Quaternion.Euler(0, 0, targetAngle);
            //yield return new WaitForSeconds(3f);
            //needle.rotation = Quaternion.Euler(0, 0, 0);
            float elapsedTime = 0f;
            float startAngle = needle.eulerAngles.z;
            float targetAngle = startAngle - totalRotationAngle;  // Clockwise rotation

            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                float t = elapsedTime / duration;

                // Smooth easing using Sine function for ease-in and ease-out
                float easedT = Mathf.Sin(t * Mathf.PI * 0.5f);  // Values range smoothly from 0 to 1

                // Interpolate between the start and target angle
                float currentAngle = Mathf.Lerp(startAngle, targetAngle, easedT);
                needle.rotation = Quaternion.Euler(0, 0, currentAngle);

                yield return null;  // Wait for the next frame
            }

            // Ensure the final rotation aligns perfectly with the target
            needle.rotation = Quaternion.Euler(0, 0, targetAngle);

            // Optional: Reset the needle after a delay (for demonstration)
            yield return new WaitForSeconds(3f);
            needle.rotation = Quaternion.Euler(0, 0, 0);



            //Debug.Log($"SpinWheel Final Angle: {wheel.rotation.eulerAngles.z}");
        }
        private float CalculateClockwiseAngle(float from, float to)
        {
            if (to < from) to += 360f; // Ensure only forward (clockwise) rotation
            return to - from;
        }

        /// <summary>
        /// Normalizes an angle to the range [0, 360).
        /// </summary>
        private float NormalizeAngle(float angle)
        {
            angle = angle % 360f;
            return (angle < 0f) ? angle + 360f : angle;
        }
    }
}
