using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NpcTrigger : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && NpcInteraction.Instance?.IsInteracting == false)
        {
            GameStateManager gameState = GameObject.FindObjectOfType<GameStateManager>();
            if (gameState) gameState.SetState(GameState.Dialog);

            NpcInteraction.Instance.Opening();
        }
    }
}
