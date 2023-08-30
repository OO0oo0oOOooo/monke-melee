using System;
using UnityEngine;

public class DevMenuManager : MonoBehaviour
{
    [SerializeField] private GameObject _devMenu;
    [SerializeField] private GameObject _title;

    [SerializeField] MenuArray[] _menus;
    
    private int _currentMenuIndex = 0;
    private int _selectedItemIndex = 0;

    void Start()
    {
        UpdateSelection();
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.M) || Input.GetKeyDown(KeyCode.Escape))
            ToggleDevMenu();

        if(Input.GetKeyDown(KeyCode.UpArrow))
            SelectionUp();

        if(Input.GetKeyDown(KeyCode.DownArrow))
            SelectionDown();

        
        if(Input.GetKeyDown(KeyCode.LeftArrow))
            SelectionLeftCommand();

        if(Input.GetKeyDown(KeyCode.RightArrow))
            SelectionRightCommand();

        if(Input.GetKeyDown(KeyCode.Return))
            SelectionExecuteCommand();
    }

    #region Input Functions
    private void SelectionExecuteCommand()
    {
        if(_menus[_currentMenuIndex].MenuItems.Length == 0)
            return;
        _menus[_currentMenuIndex].MenuItems[_selectedItemIndex].GetComponent<IMenuItem>()?.ExecuteCommand();
    }

    private void SelectionRightCommand()
    {
        if(_menus[_currentMenuIndex].MenuItems.Length == 0)
            return;
        
        _menus[_currentMenuIndex].MenuItems[_selectedItemIndex].GetComponent<IMenuItem>()?.RightCommand();
    }

    private void SelectionLeftCommand()
    {
        if(_menus[_currentMenuIndex].MenuItems.Length == 0)
        {
            UpdateCurrentMenu(0);
            return;
        }

        _menus[_currentMenuIndex].MenuItems[_selectedItemIndex].GetComponent<IMenuItem>()?.LeftCommand();
    }
    #endregion

    #region Menu Navigation
    private void SelectionUp()
    {
        if (_selectedItemIndex == 0)
            _selectedItemIndex = _menus[_currentMenuIndex].MenuItems.Length - 1;
        else
            _selectedItemIndex--;

        UpdateSelection();
    }
    private void SelectionDown()
    {
        if (_selectedItemIndex == _menus[_currentMenuIndex].MenuItems.Length - 1)
            _selectedItemIndex = 0;
        else
            _selectedItemIndex++;

        UpdateSelection();
    }

    private void UpdateSelection()
    {
        if(_menus[_currentMenuIndex].MenuItems.Length == 0)
            return;

        foreach (var item in _menus[_currentMenuIndex].MenuItems)
        {
            item.GetComponent<IMenuItem>()?.UnhighlightSelection();
        }

        _menus[_currentMenuIndex].MenuItems[_selectedItemIndex].GetComponent<IMenuItem>()?.HighlightSelection();
    }
    
    public void UpdateCurrentMenu(int index)
    {
        _selectedItemIndex = 0;
        _currentMenuIndex = index;

        DisableAllMenus();
        EnableMenu(_currentMenuIndex);
        UpdateSelection();
    }
    
    public void ToggleDevMenu()
    {
        _selectedItemIndex = 0;
        _currentMenuIndex = 0;

        DisableAllMenus();
        EnableMenu(0);

        _devMenu.SetActive(!_devMenu.activeSelf);
    }
    #endregion

    #region Helper Functions
    private void EnableMenu(int index)
    {
        _menus[index].Menu.SetActive(true);
    }

    private void DisableAllMenus()
    {
        foreach (var menu in _menus)
        {
            menu.Menu.SetActive(false);
        }
    }
    #endregion
}

[Serializable]
public class MenuArray
{
    public GameObject Menu;
    public GameObject[] MenuItems;
}