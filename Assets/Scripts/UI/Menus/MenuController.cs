using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Handles the menu stack. This is done so that the currently active page entry/exit can be easilly handled while maintaining 
/// the previously accessed pages so that they can be returned to. (Think of clicking pause -> settings -> Key bindings and then wanting to
/// return to settings by pressing escape.)
/// </summary>
[DisallowMultipleComponent]
public class MenuController : MonoBehaviour
{
    [SerializeField]
    private Page _initialPage;
    [SerializeField]
    private GameObject _firstFocusItem;

    private Stack<Page> _pageStack = new Stack<Page>();

    private void Awake()
    {
        //_rootCanvas = GetComponent<Canvas>();
    }

    // Start is called before the first frame update
    private void Start()
    {
        if(_firstFocusItem != null)
        {
            EventSystem.current.SetSelectedGameObject(_firstFocusItem);
        }

        if(_initialPage != null)
        {
            PushPage(_initialPage);
        }
    }

    /// <summary>
    /// Pop current page.
    /// </summary>
    private void OnCancel()
    {
        if(this.gameObject.activeInHierarchy)
        {
            if (_pageStack.Count != 0)
            {
                PopPage();
            }
        }
    }

    /// <summary>
    /// Is this page in the stack of pages.
    /// </summary>
    /// <param name="page">The page to search for.</param>
    /// <returns>Is it in the stack?</returns>
    public bool IsPageInStack(Page page)
    {
        return _pageStack.Contains(page);
    }

    /// <summary>
    /// Is this page on top of the stack.
    /// </summary>
    /// <param name="page">The page to compare to the top of thes stack.</param>
    /// <returns>Is it on top of the stack</returns>
    public bool IsPageOnTopOfStack(Page page)
    {
        return _pageStack.Count>0 && page == _pageStack.Peek();
    }

    /// <summary>
    /// Add page to top of stack.
    /// </summary>
    /// <param name="page">Page to add.</param>
    public void PushPage(Page page)
    {
        page.Enter(true);

        if (_pageStack.Count > 0)
        {
            Page currentPage = _pageStack.Peek();

            if (currentPage.ExitOnNewPagePush)
            {
                currentPage.Exit(false);
            }
        }

        _pageStack.Push(page);
    }

    /// <summary>
    /// Pops the page at the top of the stack.
    /// </summary>
    public void PopPage()
    {
        if (_pageStack.Count > 0)
        {
            Page page = _pageStack.Pop();
            page.Exit(true);

            if (_pageStack.Count != 0)
            {
                Page newCurrentPage = _pageStack.Peek();
                if (newCurrentPage.ExitOnNewPagePush)
                {
                    newCurrentPage.Enter(false);
                }
            }
        }
        else
        {
            Debug.LogWarning("Trying to pop a page but (" + _pageStack.Count + ") page(s) remains in the stack");
        }
    }

    /// <summary>
    /// Clear the page stack.
    /// </summary>
    public void PopAllPages()
    {
        for(int i = 1; i<_pageStack.Count; i++)
        {
            PopPage();
        }
    }
}

