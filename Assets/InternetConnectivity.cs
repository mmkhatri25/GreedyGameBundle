using System.Collections;
using UnityEngine;

public class InternetConnectivity : MonoBehaviour
{
    public Transform wheelTransform; // The transform of the wheel
    public int numberOfSpots; // The total number of spots on the wheel
    public float spinDuration = 5f; // Base duration of the spin
    public AnimationCurve spinCurve; // The curve for controlling spin speed

    private bool isSpinning = false;

    public void SpinWheel(int winningIndex)
    {
        if (!isSpinning)
        {
            StartCoroutine(SpinTheWheel(winningIndex));
        }
    }

    private IEnumerator SpinTheWheel(int winningIndex)
    {
        isSpinning = true;

        float anglePerSpot = 360f / numberOfSpots;
        float targetAngle = anglePerSpot * winningIndex;

        // Adjust targetAngle to ensure it stops at the top center (e.g., 0 or 360 degrees)
        targetAngle = 360f - targetAngle;

        float currentAngle = wheelTransform.eulerAngles.z;
        float totalRotation = targetAngle - currentAngle;

        // Ensure the wheel only rotates clockwise
        if (totalRotation < 0)
        {
            totalRotation += 360f;
        }

        // Adjust duration based on the total angle to rotate
        float adjustedDuration = spinDuration * (totalRotation / 360f);

        float timeElapsed = 0f;

        while (timeElapsed < adjustedDuration)
        {
            timeElapsed += Time.deltaTime;
            float t = timeElapsed / adjustedDuration;
            float spinFactor = spinCurve.Evaluate(t);
            float rotationAmount = Mathf.Lerp(0, totalRotation, spinFactor);
            wheelTransform.rotation = Quaternion.Euler(0f, 0f, currentAngle + rotationAmount);
            yield return null;
        }

        // Ensure it stops exactly at the winning spot
        wheelTransform.rotation = Quaternion.Euler(0f, 0f, targetAngle);

        isSpinning = false;
    }
}




////using System;
////using System.Net.Sockets;
////using System.Text;
////using UnityEngine;

////public class InternetConnectivity : MonoBehaviour
////{
////    private TcpClient socketConnection;
////    private NetworkStream networkStream;

////    // Server IP and port
////    public string serverIP = "127.0.0.1";
////    public int serverPort = 8080;

////    void Start()
////    {
////        ConnectToServer();
////    }

////    void ConnectToServer()
////    {
////        try
////        {
////            socketConnection = new TcpClient(serverIP, serverPort);
////            networkStream = socketConnection.GetStream();
////            Debug.Log("Connected to server.");
////        }
////        catch (Exception e)
////        {
////            Debug.Log("Socket error: " + e);
////        }
////    }

////    void Update()
////    {
////        if (networkStream != null && networkStream.DataAvailable)
////        {
////            byte[] bytes = new byte[socketConnection.ReceiveBufferSize];
////            networkStream.Read(bytes, 0, bytes.Length);
////            string serverMessage = Encoding.UTF8.GetString(bytes).TrimEnd('\0');
////            Debug.Log("Server message received: " + serverMessage);
////        }
////    }

////    public void SendMessageToServer(string message)
////    {
////        if (networkStream != null)
////        {
////            try
////            {
////                byte[] messageBytes = Encoding.UTF8.GetBytes(message);
////                networkStream.Write(messageBytes, 0, messageBytes.Length);
////                networkStream.Flush();
////                Debug.Log("Message sent to server: " + message);
////            }
////            catch (Exception e)
////            {
////                Debug.Log("Socket send error: " + e);
////            }
////        }
////    }

////    void OnApplicationQuit()
////    {
////        if (networkStream != null)
////        {
////            networkStream.Close();
////        }
////        if (socketConnection != null)
////        {
////            socketConnection.Close();
////        }
////    }
////}

//using UnityEngine;
//using System.Collections;

//public class InternetConnectivity : MonoBehaviour
//{
//    public Transform wheel; // Assign the wheel GameObject in the Inspector
//    public GameObject[] items; // Array to hold all the items on the wheel
//    public float spinDuration = 2.0f; // Duration of the spin in seconds
//    public int i;
//    public float totalRotationAngle;
//    public void StartRotate()
//    {
//        SpinToItem(items[i]);
//    }


//    // Call this method with the GameObject of the desired item
//    public void SpinToItem(GameObject targetItem)
//    {
//        // Find the index of the target item in the array
//        int itemIndex = System.Array.IndexOf(items, targetItem);

//        if (itemIndex == -1)
//        {
//            Debug.LogError("Item not found on the wheel!");
//            return;
//        }

//        // Calculate the angle for each item
//        float anglePerItem = 360.0f / items.Length;

//        // Calculate the final rotation angle needed to bring the item to the top (aligned with the notch)
//        float targetAngle = anglePerItem * itemIndex;

//        // Normalize the target angle within the range [0, 360]
//        targetAngle = targetAngle % 360;

//        // Calculate the current angle of the wheel
//        float currentAngle = wheel.rotation.eulerAngles.z;

//        // Calculate the shortest rotation direction (clockwise or counterclockwise)
//        float rotationAngle = Mathf.DeltaAngle(currentAngle, targetAngle);

//        // Calculate the total rotation angle needed (3 full rotations + target alignment)
//         totalRotationAngle = 1080.0f + rotationAngle;

//        // Start the coroutine to spin the wheel
//        StartCoroutine(SpinWheel(totalRotationAngle));
//    }

//    private IEnumerator SpinWheel(float totalRotationAngle)
//    {
//        float elapsedTime = 0.0f;
//        float startAngle = wheel.rotation.eulerAngles.z;

//        // Determine the final angle after spinning three rounds plus target alignment
//        float finalAngle = startAngle + totalRotationAngle;

//        // Calculate the duration based on the total rotation angle
//        float rotationDuration = spinDuration; // Adjust this if needed

//        while (elapsedTime < rotationDuration)
//        {
//            elapsedTime += Time.deltaTime;
//            float newAngle = Mathf.LerpAngle(startAngle, finalAngle, elapsedTime / rotationDuration);
//            wheel.rotation = Quaternion.Euler(0, 0, newAngle);

//            // Log the new angle for debugging
//            Debug.Log($"New Angle: {newAngle}");

//            yield return null;
//        }

//        // Snap to the exact final angle
//        wheel.rotation = Quaternion.Euler(0, 0, finalAngle);

//        Debug.Log($"Final Angle: {wheel.rotation.eulerAngles.z}");
//    }

//}
