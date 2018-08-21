/*
* Created by Daniel Mak
*/

using Photon;
using TMPro;

public class NameDisplay : PunBehaviour, IPunObservable {

    public TextMeshProUGUI nameTextField;

    public void SetName(string name) {
        nameTextField.text = name;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
        if (stream.isWriting) {
            stream.SendNext(nameTextField.text);
        } else {
            nameTextField.text = (string)stream.ReceiveNext();
        }
    }
}