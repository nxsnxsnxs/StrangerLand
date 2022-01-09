using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI
{
    [RequireComponent(typeof(CanvasGroup))]
    public class UIButton : Button, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {

    }
}