using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMenuItem
{
    void HighlightSelection();
    void UnhighlightSelection();
    void ExecuteCommand();
    void RightCommand();
    void LeftCommand();
}
