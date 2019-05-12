using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameState : MonoBehaviour
{
    public enum GameStates { Menu, Paused, Starting, Playing, Ending, Setup };
    public static GameStates gameState;
}
