using UnityEngine;
using UnityEngine.InputSystem;

namespace HellVillage
{
    public class PlayerControl : MonoBehaviour
    {
        private PlayerInput playerInput;

        private void Start()
        {
            playerInput = GetComponent<PlayerInput>();
        }

        private void Update()
        {
        }
    }
}
