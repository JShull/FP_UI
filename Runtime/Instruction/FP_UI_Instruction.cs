using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FuzzPhyte.Utility;
using UnityEngine.Events;
using TMPro;
using UnityEngine.UI;
using UnityEngine.InputSystem;
namespace FuzzPhyte.UI
{
    public enum FP_UI_InstructionType
    {
        None,
        Arrow,
        TextBox,
        ArrowAndTextBox
    }
    public enum FP_UI_InstructionButtonPosition
    {
        None,
        TopLeft,
        TopCenter,
        TopRight,
        MiddleLeft,
        MiddleRight,
        BottomLeft,
        BottomCenter,
        BottomRight
    }
    /// <summary>
    /// consists of a collection of UI elements that are used to instruct the user on how to use the UI
    /// Utilizes the FP_Timer Singleton to process UnityEvents and the time between those events
    /// </summary>
    public class FP_UI_Instruction : MonoBehaviour
    {
        public FP_Arrow Signal;
        public FP_UI_TextBox TextInformation;
        private RectTransform signalBox;
        private RectTransform textBox;
        public RectTransform SignalBox { get { return signalBox; } set { signalBox = value; } }
        public RectTransform TextBox { get { return textBox; } set { textBox = value; } }
        /// <summary>
        /// These events are for the UI to use to trigger different events/timed or untimed
        /// OnStart is immediate
        /// We process a delay after the start event
        /// TimedEvent is then immediate after time delay
        /// We hold on OnHold until the user does something
        /// OnEnd is activated upon said user action immediately
        /// OnComplete is activated after a timed delay
        /// </summary>
        #region Startup Events
        public UnityEvent OnStart;
        public float TimeDelayAfterStart;
        public UnityEvent TimedEvent;
        public float TimeDelayAfterTimedEvent;
        public UnityEvent OnHold;
        //if this is currently in a start/hold state
        private bool activeInstruction;
        #endregion
        #region End Events
        public UnityEvent OnEnd;
        public float TimeDelayAfterEnd;
        public UnityEvent OnComplete;
        public float CompletionClearTime;
        #endregion
        #region Input Events
        [Space]
        [Header("Input Events")]
        public float ResetUIButtonDelay=1.5f;
        private WaitForSecondsRealtime waitResetUIButtonDelay;
        public UnityEvent OnMovingForwardEvent;
        public UnityEvent OnMovingBackwardEvent;
        public UnityEvent OnMovingLeftEvent;
        public UnityEvent OnMovingRightEvent;
        public UnityEvent OnMenuEvent;
        public UnityEvent OnMouseEventOne;
        public UnityEvent OnMouseEventTwo;
        public UnityEvent OnMouseEventThree;
        /// <summary>
        /// clear button cache on interactable
        /// </summary>
        [SerializeField]
        private List<Button> cacheButtons = new List<Button>();
        private float cacheButtonTimer;
        #endregion
        private void Start()
        {
            waitResetUIButtonDelay = new WaitForSecondsRealtime(ResetUIButtonDelay);
        }
        private void FixedUpdate()
        {
            if (activeInstruction)
            {
                //check cacheButtons every unit of time for any that are enabled
                //only run this every ResetUIButtonDelay seconds*2
                if(cacheButtonTimer > ResetUIButtonDelay)
                {
                    cacheButtonTimer = 0f;
                    cacheButtons.ForEach(x =>
                    {
                        if (x.interactable)
                        {
                            ImmediateUIResetAction(x);
                        }
                    });
                    
                }
                else
                {
                    cacheButtonTimer += Time.fixedDeltaTime;
                }
            }
        }
        /// <summary>
        /// Generally called by the end of another UI instruction or some high level manager
        /// </summary>
        public void InstructionActivation()
        {
            activeInstruction = true;
            OnStart.Invoke();
            FP_Timer.CCTimer.StartTimer(TimeDelayAfterStart, TimedEvent.Invoke);
            FP_Timer.CCTimer.StartTimer(TimeDelayAfterStart+TimeDelayAfterTimedEvent, OnHold.Invoke);
        }
        /// <summary>
        /// Generally called by a UI action element like a Button
        /// </summary>
        public void InstructionDeactivation()
        {
            activeInstruction = false;
            OnEnd.Invoke();
            FP_Timer.CCTimer.StartTimer(TimeDelayAfterEnd, OnComplete.Invoke);
            FP_Timer.CCTimer.StartTimer(CompletionClearTime, ClearAndRemove);
        }
        public void ClearAndRemove()
        {
            Debug.Log($"Do we need to destroy anything?");
        }
        #region UI Monitoring Actions
        
        /// <summary>
        /// Event Driven input for moving
        /// forward, backward, left, right
        /// </summary>
        /// <param name="movementValue"></param>
        public virtual void OnMoving(InputValue movementValue)
        {
            Vector2 movementVector = movementValue.Get<Vector2>();
            if (movementVector.y > 0)
            {
                Debug.LogWarning($"W was probably pressed");
                OnMovingForwardEvent.Invoke();
            }
            if (movementVector.y < 0)
            {
                Debug.LogWarning($"S was probably pressed");
                OnMovingBackwardEvent.Invoke();
            }
            if (movementVector.x > 0)
            {
                Debug.LogWarning($"D was probably pressed");
                OnMovingRightEvent.Invoke();
            }
            if(movementVector.x < 0)
            {
                Debug.LogWarning($"A was probably pressed");
                OnMovingLeftEvent.Invoke();
            }
        }
        public virtual void OnMenuOpen()
        {
            Debug.LogWarning($"Menu pushed?-UI Instruction Action caught");
            OnMenuEvent.Invoke();
        }
        /// <summary>
        /// left mouse clicked
        /// </summary>
        public virtual void OnClick()
        {
            Debug.LogWarning($"Mouse Clicked-left?-UI Instruction Action caught");
            OnMouseEventOne.Invoke();
        }
        public virtual void OnRightClick()
        {
            Debug.LogWarning($"Mouse Clicked-right?-UI Instruction Action caught");
            OnMouseEventTwo.Invoke();
        }
        /// <summary>
        /// public access to turn on and enable a ui button
        /// </summary>
        /// <param name="uiButton"></param>
        public void EnableUIButton(Button uiButton)
        {
            uiButton.interactable = true;
            if(uiButton.gameObject.GetComponentInChildren<TextMeshProUGUI>())
            {
                uiButton.gameObject.GetComponentInChildren<TextMeshProUGUI>().enabled = true;
            }
            else
            {
                if(uiButton.gameObject.GetComponentInChildren<TextMeshPro>())
                {
                    uiButton.gameObject.GetComponentInChildren<TextMeshPro>().enabled = true;
                }
                else
                {
                    if (uiButton.gameObject.GetComponentInChildren<TextMesh>())
                    {
                        uiButton.gameObject.GetComponent<TextMeshPro>().enabled = true;
                    }
                }
            }
            if(!cacheButtons.Contains(uiButton))
            {
                cacheButtons.Add(uiButton);
            }
            //StartCoroutine(DelayResetUIButton(uiButton));
        }
        /// <summary>
        /// public access to un-enable a UI Button
        /// </summary>
        /// <param name="uiButton"></param>
        public void ResetUIDelayEnabledAction(Button uiButton)
        {
            StartCoroutine(DelayResetUIButton(uiButton));
        }
        private void ImmediateUIResetAction(Button uiButton)
        {
            uiButton.interactable = false;
            if (uiButton.gameObject.GetComponentInChildren<TextMeshProUGUI>())
            {
                uiButton.gameObject.GetComponentInChildren<TextMeshProUGUI>().enabled = false;
            }
            else
            {
                if (uiButton.gameObject.GetComponentInChildren<TextMeshPro>())
                {
                    uiButton.gameObject.GetComponentInChildren<TextMeshPro>().enabled = false;
                }
                else
                {
                    if (uiButton.gameObject.GetComponentInChildren<TextMesh>())
                    {
                        uiButton.gameObject.GetComponent<TextMeshPro>().enabled = false;
                    }
                }
            }
        }
        IEnumerator DelayResetUIButton(Button uiButton)
        {
            yield return waitResetUIButtonDelay;
            ImmediateUIResetAction(uiButton);
           
        }
        #endregion
    }
}
