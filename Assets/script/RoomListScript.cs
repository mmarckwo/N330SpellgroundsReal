using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;

public class RoomListScript : MonoBehaviourPun
{

    private Dictionary<string, RoomInfo> cachedRoomList = new Dictionary<string, RoomInfo>();

    private void UpdateCachedRoomList(List<RoomInfo> roomList)
    {
        for (int i = 0; i < roomList.Count; i++)
        {
            RoomInfo info = roomList[i];
            if (info.RemovedFromList)
            {
                cachedRoomList.Remove(info.Name);
            }
            else
            {
                cachedRoomList[info.Name] = info;
            }
        }
    }

    public void RoomListUpdate(List<RoomInfo> roomList)
    {
        // clear list of rooms.
        cachedRoomList.Clear();
        // here you get the response, empty list if no rooms found
        UpdateCachedRoomList(roomList);
    }

    public void RoomListClear()
    {
        cachedRoomList.Clear();
    }
}
