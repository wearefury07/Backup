using UnityEngine;
using System.Collections.Generic;

public class PushNotificator : MonoBehaviour {
	// use for initialization
	//void Start () {

	//	OneSignal.StartInit("368vipEdited")
	//		.HandleNotificationReceived(HandleNotificationReceived)
	//		.EndInit();

	//	OneSignal.inFocusDisplayType = OneSignal.OSInFocusDisplayOption.Notification;
	//}

	//private static void HandleNotificationReceived(OSNotification notification) {
	//	OSNotificationPayload payload = notification.payload;
	//	string message = payload.body;

	//	print ("NOTIFY TYPE: " + notification.displayType); 
	//	print ("PUSH MESS: " + message);
	//	OGUIM.Toast.ShowNotification (message );
	//}

	/* Hidden PushWoosh
	void OnRegisteredForPushNotifications(string token)
	{
		// handle here
		Debug.Log("Received token: \n" + token);
	}

	void OnFailedToRegisteredForPushNotifications(string error)
	{
		// handle here
		Debug.Log("Error ocurred while registering to push notifications: \n" + error);
	}

	void OnPushNotificationsReceived(string payload)
	{
		// handle here
		Debug.Log("Received push notificaiton: \n" + payload);

		JSONObject json = new JSONObject (payload);
		// display content of JSON
		OGUIM.MessengerBox.Show("Thông báo", json["title"].str);
	}
	*/
}