using System;
using UdpKit;
using UnityEngine;
using UnityEngine.UI;

public class UIServerEntry : MonoBehaviour
{
    [SerializeField] private Text serverInfoText;
    [SerializeField] private Text slotInfoText;
    [SerializeField] private Button joinServerButton;

    public void Populate(UdpSession session, Color backgroundColor, Action clickAction)
    {
        serverInfoText.text = session.HostName;
        slotInfoText.text = string.Format("{0}/{1}", session.ConnectionsCurrent, session.ConnectionsMax);

        joinServerButton.onClick.RemoveAllListeners();
        joinServerButton.onClick.AddListener(clickAction.Invoke);

        //gameObject.GetComponent<Image>().color = backgroundColor;
    }
}
