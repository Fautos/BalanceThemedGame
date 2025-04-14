using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniTaskController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Minigames selection
    private void MiniGameSelection(int minigame)
    {
        switch (minigame)
        {
            // Clean the yard
            case 0:
            {
                // Three piles of trash will appear in the yard and you have to pick them

                break;
            }
            // Clean the bathroom
            case 1:
            {
                // Ventana emergente en la que tienes que clickar en 3 motas de suciedad. Se activa desde el baño.

                break;
            }
            // Help in the kitchen
            case 2:
            {
                
                break;
            }
            // Help in the workshop
            case 3:
            {
                break;
            }
            // Submit letters
            case 4:
            {
                break;
            }
        }

    }
}
