using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;

public class RoomListScript : MonoBehaviourPunCallbacks
{

    [SerializeField] private Transform content;
    [SerializeField] private RoomListing roomListing;

    [SerializeField] private GameObject screen2;
    [SerializeField] private GameObject screen3;

    private List<RoomListing> listings = new List<RoomListing>();

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {

        for (int i = 0; i < roomList.Count; i++)
        {
            if (roomList[i].RemovedFromList)
            {
                int index = listings.FindIndex(x => x.RoomInfo.Name == roomList[i].Name);
                if (index != -1)
                {
                    Destroy(listings[index].gameObject);
                    listings.RemoveAt(index);
                }
            } 
            else
            {
                RoomListing listing = Instantiate(roomListing, content);
                listing.SetRoomInfo(roomList[i]);
                listings.Add(listing);
            }
        }
    }

    public override void OnJoinedRoom()
    {
        screen2.SetActive(false);
        screen3.SetActive(true);
    }
}
