using UnityEngine;
using Yarn.Unity;

namespace HellVillage.InteractionSystem {
    public class SomethingInteraction : TriggerInteractionBase {
        public DialogueRunner dialogue;

        public override void Interact() {
            Debug.Log("SomethingInteraction Interacted");

            dialogue.StartDialogue("Intro");
        }

    }
}
