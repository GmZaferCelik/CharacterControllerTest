using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyNetworkManager : NetworkManager
{
    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        base.OnServerAddPlayer(conn);

        DontDestroyOnLoad(conn.identity.gameObject);        
    }

    public override void OnClientConnect()
    {
        base.OnClientConnect();

        CinemachineSwitcher.Instance.OnPlayerAdded(NetworkClient.connection.identity.gameObject.GetComponent<CharacterLocomotion>().cameraTarget.gameObject);
    }
}
