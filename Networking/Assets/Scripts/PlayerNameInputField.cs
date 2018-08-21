/*
* Created by Daniel Mak
*/

using UnityEngine;
using TMPro;

namespace Daniel.Networkings {
    [RequireComponent(typeof(TMP_InputField))]
    public class PlayerNameInputField : MonoBehaviour {

        private static readonly string playerNamePrefKey = "PlayerName";

        void Start() {

            string defaultName = "";
            TMP_InputField inputField = GetComponent<TMP_InputField>();

            if (inputField != null) {
                if (PlayerPrefs.HasKey(playerNamePrefKey)) {
                    defaultName = PlayerPrefs.GetString(playerNamePrefKey);
                    inputField.text = defaultName;
                }
            }

            PhotonNetwork.playerName = defaultName;
        }

        /// <summary>
        /// Sets the name of the player, and save it in the PlayerPrefs for future sessions.
        /// </summary>
        /// <param name="value">The name of the Player</param>
        public void SetPlayerName(string value) {
            // #Important
            PhotonNetwork.playerName = value + " "; // force a trailing space string in case value is an empty string, else playerName would not be updated.

            PlayerPrefs.SetString(playerNamePrefKey, value);
        }
    }
}