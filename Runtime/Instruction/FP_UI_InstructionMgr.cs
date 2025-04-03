using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FuzzPhyte.Utility;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;
//using UnityEngine.InputSystem;

namespace FuzzPhyte.UI
{
    /// <summary>
    /// Manages a linear sequence of UI_Instructions
    /// Manages generating UI elements based on the data
    /// </summary>
    public class FP_UI_InstructionMgr : MonoBehaviour
    {
        #region Singleton Setup
        private static FP_UI_InstructionMgr _instance;

        public static FP_UI_InstructionMgr Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<FP_UI_InstructionMgr>();
                    if (_instance == null)
                    {
                        GameObject obj = new GameObject();
                        obj.name = "InstructionMgr";
                        _instance = obj.AddComponent<FP_UI_InstructionMgr>();
                    }
                }
                return _instance;
            }
        }
        #endregion
        [SerializeField]
        [Tooltip("Flag to indicate if we've finished our instructions and retained if we reload scene")]
        private bool finishedInstructions = false;
        [SerializeField]
        private bool isActive = false;
        [Space]
        [Header("Main Instruction Data and Setup")]
        public List<FP_UI_Instruction> Instructions = new List<FP_UI_Instruction>();
        
        public FP_UI_Instruction CurrentInstruction
        {
            get
            {
                return finishedInstructions? null : Instructions[currentInstructionIndex];
            }
        }
        [Tooltip("If we want the instructions to activate upon Start")]
        public bool ActivateInstructionsOnStart = true;
        [Tooltip("If we want to reload the scene on finish")]
        public bool ReloadSceneOnFinish = false;
        public Canvas UICanvas;
        [Space]
        public Color OverlayColor;
       
        [Tooltip("Any additional OnAwake events after Instance setup we want to check/do")]
        public UnityEvent OnAwake;
        [Tooltip("Any additional on Start events we want to do")]
        public UnityEvent OnStart;
        [Tooltip("When we finish our instructions we want to do something-like reset the controllers")]
        public UnityEvent OnFinishInstructions;
        [Space]
        [Header("Next Button Setup")]
       
        public Sprite NextButtonSprite;
        public Color NextButtonFontColor;
        [Header("Colors for Buttons")]
        public ColorBlock FontButtonColorBlock;
        [Space]
        [Range(0.1f,1f)]
        public float ButtonSizeWidthPercent = 0.25f;
        [Range(0.1f,1f)]
        public float ButtonSizeHeightPercent = 0.5f;
        [Space]
        public string NextButtonText = "Click To Continue";
        public string NextButtonTextFin = "Click To Finish";
        public TMPro.TextAlignmentOptions NextButtonAlignment;
        public TMPro.FontStyles NextButtonFontStyles;
        public float ButtonFontSize = 12;
        public float ButtonFontMin = 6;
        public float ButtonFontMax = 72;
        public bool NextButtonAutoFontSize = true;
        [Space]
        [Header("Skip Button Setup")]
        public bool UseSkipButton = true;
        public Sprite SkipButtonSprite;
        public Color SkipButtonFontColor;
        public string SkipButtonText = "Click To Skip Instructions!";

        [Space]
        public Vector2 SkipButtonBottomLeft = Vector2.zero;
        public Vector2 SkipButtonTopRight = Vector2.zero;
        [Header("Colors for Skip Buttons")]
        public ColorBlock SkipButtonColorBlock;
        public TMPro.TextAlignmentOptions SkipButtonAlignment;
        public TMPro.FontStyles SkipButtonFontStyles;
        public float SkipButtonFontSize = 12;
        public float SkipButtonFontMin = 6;
        public float SkipButtonFontMax = 72;
        public bool SkipButtonAutoFontSize = true;

        //cached generated UI elements
        private Dictionary<FP_UI_Instruction, GameObject> GeneratedTextBoxes = new Dictionary<FP_UI_Instruction, GameObject>();
        private Dictionary<FP_UI_Instruction, GameObject> GeneratedSignals = new Dictionary<FP_UI_Instruction, GameObject>();
        private GameObject overlayBlock;
        private GameObject uiNextButton;
        private GameObject uiSkipButton;
        private int currentInstructionIndex = 0;

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this;
            DontDestroyOnLoad(gameObject);
            if (finishedInstructions)
            {
                Debug.LogWarning($"We've completed our Instructions, returning from Awake");
                return;
            }
            OnAwake.Invoke();
        }
        public void Start()
        {
          
            if (finishedInstructions)
            {
                Debug.LogWarning($"We've completed our Instructions, returning from Start");
                return;
            }
            if (ActivateInstructionsOnStart &&!isActive)
            {
                ProcessNextInstruction();
            }
        }

        private void ProcessNextInstruction()
        {
            //are we first
            //clear old ones
            //currentInstructionIndex
            //generate new ones
            //currentInstructionIndex++
            if (finishedInstructions)
            {
                return;
            }
            if (!isActive)
            {
                //reset index
                currentInstructionIndex = 0;
                //clear and remove old dictionary references if any
                if (GeneratedTextBoxes.Count > 0)
                {
                    foreach (var item in GeneratedTextBoxes)
                    {
                        Destroy(item.Value);
                    }
                    GeneratedTextBoxes.Clear();
                }
                //clear and remove old dictionary references for signals if any
                if (GeneratedSignals.Count > 0)
                {
                    foreach (var item in GeneratedSignals)
                    {
                        Destroy(item.Value);
                    }
                    GeneratedSignals.Clear();
                }
                //setup overlay sequence
                SetupOverlayUIBlock();
                //generate all of the UI items
                for(int i = 0; i < Instructions.Count; i++)
                {
                    var curBox = GenerateTextBox(Instructions[i]);
                    var curSig = GenerateSignal(Instructions[i]);
                    curBox.SetActive(false);
                    curSig.SetActive(false);
                }
                //turn on first one
                GeneratedTextBoxes[Instructions[currentInstructionIndex]].SetActive(true);
                GeneratedSignals[Instructions[currentInstructionIndex]].SetActive(true);
                //GenerateTextBox(Instructions[currentInstructionIndex]);
                //GenerateSignal(Instructions[currentInstructionIndex]);
                //process OnStart Event After two frame delays
                StartCoroutine(DelayStartTwoFrames());

            }
            else
            {
                //we are in the middle of the sequence
                //turn off old one
                //update index
                //process new one if it exists or end
                if (currentInstructionIndex < Instructions.Count)
                {
                    //turn off old one
                    GeneratedTextBoxes[Instructions[currentInstructionIndex]].SetActive(false);
                    GeneratedSignals[Instructions[currentInstructionIndex]].SetActive(false);
                    //update index
                    currentInstructionIndex++;
                    //process new one if it exists or end
                    if (currentInstructionIndex < Instructions.Count)
                    {
                        //turn on new ones
                        GeneratedTextBoxes[Instructions[currentInstructionIndex]].SetActive(true);
                        GeneratedSignals[Instructions[currentInstructionIndex]].SetActive(true);
                        //GenerateTextBox(Instructions[currentInstructionIndex]);
                        //GenerateSignal(Instructions[currentInstructionIndex]);
                        //process InstructionActivation functions
                        Instructions[currentInstructionIndex].InstructionActivation();
                    }
                    else
                    {
                        //we are done
                        //turn off overlay
                        overlayBlock.SetActive(false);
                        Destroy(overlayBlock);
                        //turn off button
                        uiNextButton.SetActive(false);
                        Destroy(uiNextButton);
                        //skip button
                        if (uiSkipButton != null)
                        {
                            uiSkipButton.SetActive(false);
                            Destroy(uiSkipButton);
                        }
                        //set flag
                        finishedInstructions = true;
                        isActive = false;
                        OnFinishInstructions.Invoke();
                    }
                }
                else
                {
                    //we are done
                    //turn off overlay
                    Debug.Log($"We are done with the instructions!");
                    overlayBlock.SetActive(false);
                    Destroy(overlayBlock);
                    //turn off button
                    uiNextButton.SetActive(false);
                    Destroy(uiNextButton);
                    //skip button
                    if (uiSkipButton != null)
                    {
                        uiSkipButton.SetActive(false);
                        Destroy(uiSkipButton);
                    }
                    //set flag
                    finishedInstructions = true;
                    isActive = false;
                    OnFinishInstructions.Invoke();
                }   
            }
            if(finishedInstructions && ReloadSceneOnFinish)
            {
                StartCoroutine(ReloadSceneDelayOnFinishInstructions());
            }
        }
        /// <summary>
        /// Procedural content generation sometimes takes 1-2 frames to sync/update hence why we wait two frames before calling the event
        /// </summary>
        /// <returns></returns>
        IEnumerator DelayStartTwoFrames()
        {
            Debug.Log($"Frame: {Time.frameCount}");
            yield return new WaitForEndOfFrame();
            Debug.Log($"Frame: {Time.frameCount}");
            yield return new WaitForEndOfFrame();
            Debug.Log($"Frame: {Time.frameCount}");
            OnStart.Invoke();
            isActive = true;
            //setup button
            var rectInstruction = GeneratedTextBoxes[Instructions[currentInstructionIndex]].GetComponent<RectTransform>();
            var butPosition = Instructions[currentInstructionIndex].TextInformation.ButtonPosition;
            ////this was originally theme based but we are going to use the color block instead in the editor
            //var butThemeColor = Instructions[currentInstructionIndex].TextInformation.Theme.TertiaryColor;
            //var butThemeNormalColor = Instructions[currentInstructionIndex].TextInformation.Theme.SecondaryColor;
            //var fontThemeColor = Instructions[currentInstructionIndex].TextInformation.Theme.FontPrimaryColor;
            //take 10% off color universally to get a pressed color
            //Color pressedColor = new Color(butThemeNormalColor.r*0.9f,butThemeNormalColor.g*0.9f, butThemeNormalColor.b*0.9f, butThemeNormalColor.a*0.9f);
            //Color selectColor = new Color(butThemeNormalColor.r*0.9f, butThemeNormalColor.g * 0.9f, butThemeNormalColor.b * 0.9f, butThemeNormalColor.a * 0.9f);
            /*
            ColorBlock colorBlock = new ColorBlock()
            {
                colorMultiplier = 1,
                fadeDuration = 0.1f,
                disabledColor = Color.gray,
                highlightedColor = butThemeColor,
                normalColor = butThemeNormalColor,
                selectedColor = selectColor,
                pressedColor = pressedColor,
            };
            */
            //setup button
            //FontButtonColorBlock.colorMultiplier = 1;
            GenerateNextUIButton(rectInstruction, NextButtonText, FontButtonColorBlock, NextButtonFontColor, NextButtonAutoFontSize, ButtonFontSize, ButtonFontMin, ButtonFontMax);

            // Calculate and set the position of the Button
            MoveUIButton(rectInstruction, uiNextButton.GetComponent<RectTransform>(), butPosition);
            //calculate and set the position and generate the skip button
            GenerateSkipButton(SkipButtonText, SkipButtonColorBlock, SkipButtonBottomLeft, SkipButtonTopRight, SkipButtonFontColor, SkipButtonAutoFontSize, SkipButtonFontSize, SkipButtonFontMin, SkipButtonFontMax);
            //go ahead and process the first Instruction Activation sequence
            Instructions[currentInstructionIndex].InstructionActivation();
        }
        IEnumerator ReloadSceneDelayOnFinishInstructions()
        {
            yield return new WaitForSecondsRealtime(2f);
            Debug.LogWarning($"Reload of Scene!");
            UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
        }
        /// <summary>
        /// This sets up our overlay backdrop
        /// </summary>
        private void SetupOverlayUIBlock()
        {
            overlayBlock = new GameObject("UI_Instructions_OverlayBlock");
            overlayBlock.layer = LayerMask.NameToLayer("UI");
            overlayBlock.transform.SetParent(UICanvas.transform, false);
            var rectTransform = overlayBlock.AddComponent<RectTransform>();

            Image image = overlayBlock.AddComponent<Image>();
            image.color = OverlayColor;
            // Set pivot and anchors
            rectTransform.pivot = new Vector2(0.5f, 0.5f);
            rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            rectTransform.anchorMax = new Vector2(0.5f, 0.5f);

            rectTransform.anchorMin = new Vector2(0, 0);
            rectTransform.anchorMax = new Vector2(1, 1);
            rectTransform.sizeDelta = Vector2.zero; // Stretch to fill the parent
            rectTransform.anchoredPosition = Vector2.zero;
            overlayBlock.transform.SetAsFirstSibling();
        }
        
        /// <summary>
        /// public method called via our skip button
        /// this will skip the instructions and just get right to the end
        /// </summary>
        public void SkipInstructions()
        {
            Debug.LogWarning($"Skip button pressed");
            ProcessSkipButton();
        }
        private void ProcessSkipButton()
        {
            //close whatever events we have for the current instruction
            Instructions[currentInstructionIndex].InstructionDeactivation();
            //we are skipping ahead so we need to basically turn off whatever active one we are on and just jump to the end
            //process finishing instructions for current FP_UI_Instruction
            GeneratedTextBoxes[Instructions[currentInstructionIndex]].SetActive(false);
            GeneratedSignals[Instructions[currentInstructionIndex]].SetActive(false);
            //turn off overlay
            overlayBlock.SetActive(false);
            Destroy(overlayBlock);
            //turn off button
            uiNextButton.SetActive(false);
            Destroy(uiNextButton);
            //turn off skip
            if (uiSkipButton != null)
            {
                uiSkipButton.SetActive(false);
                Destroy(uiSkipButton);
            }
            //set flag
            finishedInstructions = true;
            isActive = false;
            OnFinishInstructions.Invoke();
            if (finishedInstructions && ReloadSceneOnFinish)
            {
                StartCoroutine(ReloadSceneDelayOnFinishInstructions());
            }
        }
        
        #region Procedural UI Work
        private void GenerateNextUIButton(RectTransform textBoxRect, string buttonText, ColorBlock ButtonBackground, Color? ButtonFont=null, bool useAutoSizeFont=true,float fontSize=12, float fontMin=6, float fontMax=72)
        {
            //going to generate a UI Text Button following the FP_UI_Instruction information from the first one for theme purposes
            //then going to use the instructions information to move and relocate it's position


            // Create the Button GameObject
            uiNextButton = new GameObject("NextInstruction");
            uiNextButton.transform.SetParent(UICanvas.transform, false);
            uiNextButton.AddComponent<RectTransform>();
            Button button = uiNextButton.AddComponent<Button>();
            Image buttonImage = uiNextButton.AddComponent<Image>();
            button.image = buttonImage;
            if (NextButtonSprite != null)
            {
                buttonImage.sprite = NextButtonSprite;
                buttonImage.type = Image.Type.Sliced;
            }
            
            button.onClick.AddListener(UINextButtonClick);
            
            button.colors = ButtonBackground;
            uiNextButton.layer = LayerMask.NameToLayer("UI");
            // Add Text to the Button (using TextMeshPro if desired)
            //generate a child text object
            GameObject uiNextButtonText = new GameObject("NextInstructionText");
            uiNextButtonText.transform.SetParent(uiNextButton.transform, false);
            var backDropRect = uiNextButtonText.AddComponent<RectTransform>();
            uiNextButtonText.layer = LayerMask.NameToLayer("UI");
            //little bit of text padding for the button
            backDropRect.anchorMin = new Vector2(0.025f, 0.025f);
            backDropRect.anchorMax = new Vector2(0.975f, 0.975f);
            backDropRect.sizeDelta = Vector2.zero; // Stretch to fill the parent
            backDropRect.anchoredPosition = Vector2.zero;
            //now add the text in
            TextMeshProUGUI buttonTextComponent = uiNextButtonText.AddComponent<TextMeshProUGUI>();
            //push to match parents size

            //setup font properties
            FontTextGenerationSettings(buttonTextComponent, buttonText, NextButtonAlignment, NextButtonFontStyles,useAutoSizeFont, fontMin, fontMax, fontSize, ButtonFont);
            // Set the Button's RectTransform
            RectTransform buttonRect = button.GetComponent<RectTransform>();
            buttonRect.sizeDelta = new Vector2(textBoxRect.sizeDelta.x * ButtonSizeWidthPercent, textBoxRect.sizeDelta.y * ButtonSizeHeightPercent);
            //buttonTextComponent.alignment = NextButtonAlignment;
            if(buttonImage.type==Image.Type.Sliced)
            {
                //if our image type is sliced, if we take half of our height as our pixels per unit multiplier it usually gives us a nicer
                //when over 15 pixels it gets pretty small so we want to sort of cap it there
                buttonImage.pixelsPerUnitMultiplier = Mathf.Min(textBoxRect.sizeDelta.y * ButtonSizeHeightPercent * 0.5f,15);
            }
            //buttonRect.sizeDelta = new Vector2(100, 50); // Adjust size as needed
        }
        /// <summary>
        /// this acts in retrospect to the GenerateNextUIButton
        /// we are always going to process the current instruction index end event
        /// we then process into the future to see if we need to move locations based on the next on in the list (if we aren't at the end)
        /// 
        /// </summary>
        public void UINextButtonClick()
        {
            //process ending instructions for current FP_UI_Instruction
            //then clean it up
            //need to remove all actions/invokes currently on button
            //need to reference our internal function
            //need to move the button
            //need to correctly reference the FP_UI_Instruction methods

            //process ending instructions for current FP_UI_Instruction
            Instructions[currentInstructionIndex].InstructionDeactivation();

            //move button if we aren't at the end
            Debug.Log($"Current Instruction Index: {currentInstructionIndex} '<' Instructions.Count {Instructions.Count}");
            if (currentInstructionIndex < Instructions.Count - 1)
            {
                //move based on the next instruction
                MoveUIButton(GeneratedTextBoxes[Instructions[currentInstructionIndex + 1]].GetComponent<RectTransform>(), uiNextButton.GetComponent<RectTransform>(), Instructions[currentInstructionIndex+1].TextInformation.ButtonPosition);
                //if the button is the last one lets change the font of the button to say it
                if (currentInstructionIndex + 1 == Instructions.Count - 1)
                {
                    uiNextButton.GetComponentInChildren<TextMeshProUGUI>().text = NextButtonTextFin;
                }
            }
            else
            {
                //we are at the end disable the button
                uiNextButton.GetComponent<Button>().enabled = false;
            }
            //process the button
            ProcessNextInstruction();

        }
        private void MoveUIButton(RectTransform textBoxRect, RectTransform buttonRect, FP_UI_InstructionButtonPosition position)
        {
            Vector2 positionOffset = Vector2.zero;

            switch (position)
            {
                case FP_UI_InstructionButtonPosition.TopLeft:
                    positionOffset = new Vector2(-buttonRect.sizeDelta.x, buttonRect.sizeDelta.y);
                    buttonRect.pivot = new Vector2(1, 0);
                    break;
                case FP_UI_InstructionButtonPosition.TopCenter:
                    positionOffset = new Vector2(0, buttonRect.sizeDelta.y);
                    buttonRect.pivot = new Vector2(0.5f, 0);
                    break;
                case FP_UI_InstructionButtonPosition.TopRight:
                    positionOffset = new Vector2(buttonRect.sizeDelta.x, buttonRect.sizeDelta.y);
                    buttonRect.pivot = new Vector2(0, 0);
                    break;
                case FP_UI_InstructionButtonPosition.MiddleLeft:
                    positionOffset = new Vector2(-buttonRect.sizeDelta.x*2, 0);
                    buttonRect.pivot = new Vector2(1, 0.5f);
                    break;
                case FP_UI_InstructionButtonPosition.MiddleRight:
                    positionOffset = new Vector2(buttonRect.sizeDelta.x*2, 0);
                    buttonRect.pivot = new Vector2(0, 0.5f);
                    break;
                case FP_UI_InstructionButtonPosition.BottomLeft:
                    positionOffset = new Vector2(-buttonRect.sizeDelta.x, -buttonRect.sizeDelta.y);
                    buttonRect.pivot = new Vector2(1, 1);
                    break;
                case FP_UI_InstructionButtonPosition.BottomCenter:
                    positionOffset = new Vector2(0, -buttonRect.sizeDelta.y);
                    buttonRect.pivot = new Vector2(0.5f, 1);
                    break;
                case FP_UI_InstructionButtonPosition.BottomRight:
                    positionOffset = new Vector2(buttonRect.sizeDelta.x, -buttonRect.sizeDelta.y);
                    buttonRect.pivot = new Vector2(0, 1);
                    break;
                case FP_UI_InstructionButtonPosition.None:
                    // No positioning needed
                    break;
            }

            buttonRect.anchoredPosition = textBoxRect.anchoredPosition + positionOffset;
            //make sure the button is on top of the text box last child in parent
            buttonRect.SetAsLastSibling();
            
        }

        /// <summary>
        /// Wrapper Function to take my FP_UI_Instruction type and break it into parts for text
        /// This way later on if we need to modify and/or change this we don't have to rework the brunt of the function which is a private function
        /// </summary>
        /// <param name="instruction"></param>
        /// <returns></returns>
        public GameObject GenerateTextBox(FP_UI_Instruction instruction)
        {

            var tBox = CreateTextBox(
                instruction.name, 
                instruction.TextInformation.TextBox,
                instruction.TextInformation.BottomLeftPt, 
                instruction.TextInformation.TopRightPt,
                instruction.TextInformation.TextBoxAlignment,
                instruction.TextInformation.TextBoxFontStyle,
                instruction.TextInformation.FontSize,
                instruction.TextInformation.FontMin,
                instruction.TextInformation.FontMax,
                instruction.TextInformation.OutlineThickness,
                instruction.TextInformation.Theme.FontPrimaryColor,
                instruction.TextInformation.Theme.SecondaryColor, 
                instruction.TextInformation.Theme.BackgroundImage,
                instruction.TextInformation.UseText,
                instruction.TextInformation.UseImage,
                instruction.TextInformation.UseOutline,
                instruction.TextInformation.UseScaleFont);
            GeneratedTextBoxes.Add(instruction, tBox);
            instruction.TextBox = tBox.GetComponent<RectTransform>();
            return tBox;
        }
        /// <summary>
        /// Generates the TextBox Object with the specified parameters
        /// </summary>
        /// <param name="objRefname"></param>
        /// <param name="text"></param>
        /// <param name="bottomLeft"></param>
        /// <param name="topRight"></param>
        /// <param name="fontSize"></param>
        /// <param name="fontSizeMin"></param>
        /// <param name="fontSizeMax"></param>
        /// <param name="outlineThickness"></param>
        /// <param name="textColor"></param>
        /// <param name="outlineColor"></param>
        /// <param name="backdrop"></param>
        /// <param name="useText"></param>
        /// <param name="useBackdrop"></param>
        /// <param name="useOutline"></param>
        /// <param name="useAutoSizeFont"></param>
        /// <returns></returns>
        private GameObject CreateTextBox(string objRefname, string text, Vector2 bottomLeft, Vector2 topRight, TextAlignmentOptions txtAlign, FontStyles fontTextStyle, float fontSize=12, float fontSizeMin = 6, float fontSizeMax=72, float outlineThickness=0,Color? textColor = null, Color? outlineColor = null, Sprite backdrop = null,bool useText=true,bool useBackdrop=true, bool useOutline=true, bool useAutoSizeFont=true)
        {
            // Create the TextBox GameObject
            GameObject textBoxObj = new GameObject(objRefname);
            
            textBoxObj.layer = LayerMask.NameToLayer("UI"); 
            textBoxObj.transform.SetParent(UICanvas.transform, false);
            textBoxObj.AddComponent<RectTransform>();
            //adding a fake panel
            var hiddenImage = textBoxObj.AddComponent<Image>();
            hiddenImage.color = new Color(0, 0, 0, 1);

            // Set the size and position
            RectTransform rectTransform = textBoxObj.GetComponent<RectTransform>();
            // Calculate screen position
            Vector2 bottomLeftScreen = new Vector2(Screen.width * bottomLeft.x, Screen.height * bottomLeft.y);
            Vector2 topRightScreen = new Vector2(Screen.width * topRight.x, Screen.height * topRight.y);
            Vector2 bottomLeftCanvas, topRightCanvas;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(UICanvas.GetComponent<RectTransform>(), bottomLeftScreen, UICanvas.worldCamera, out bottomLeftCanvas);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(UICanvas.GetComponent<RectTransform>(), topRightScreen, UICanvas.worldCamera, out topRightCanvas);
            rectTransform.sizeDelta = topRightCanvas - bottomLeftCanvas;
            rectTransform.anchoredPosition = (topRightCanvas + bottomLeftCanvas) / 2;

            // Set pivot and anchors
            rectTransform.pivot = new Vector2(0.5f, 0.5f);
            rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            rectTransform.anchorMax = new Vector2(0.5f, 0.5f);

            // Add Outline if specified
            
            if (useOutline&&outlineColor.HasValue && outlineThickness > 0)
            {
                Outline outline = textBoxObj.AddComponent<Outline>();
                outline.effectColor = outlineColor.Value;
                outline.effectDistance = new Vector2(outlineThickness, -outlineThickness);
            }


            
            // Add Backdrop if specified
            if (useBackdrop && backdrop!=null)
            {
                GameObject backdropObj = new GameObject(objRefname+"_Backdrop");
                backdropObj.layer = LayerMask.NameToLayer("UI");
                backdropObj.transform.SetParent(textBoxObj.transform, false);
                var backdropRect = backdropObj.AddComponent<RectTransform>();

                Image image = backdropObj.AddComponent<Image>();
                image.sprite = backdrop;

                //RectTransform backdropRect = backdropObj.GetComponent<RectTransform>();
                backdropRect.anchorMin = new Vector2(0, 0);
                backdropRect.anchorMax = new Vector2(1, 1);
                backdropRect.sizeDelta = Vector2.zero; // Stretch to fill the parent
                backdropRect.anchoredPosition = Vector2.zero;
            }
            else
            {
                //we probably need to make sure our backdrop color which is probably black doesn't conflict with our font color
                //need a way to compare colors and make sure they aren't too close
                //if they are we need to adjust the font color
                if (useText)
                {
                    var hiddenImageColor = hiddenImage.color;
                    Color oldTextColor = textColor.HasValue ? textColor.Value : Color.black;
                    //compare to textColor
                    if (textColor.HasValue)
                    {
                        var textColorValue = textColor.Value;
                        if (Mathf.Abs(hiddenImageColor.r - textColorValue.r) < 0.1f && Mathf.Abs(hiddenImageColor.g - textColorValue.g) < 0.1f && Mathf.Abs(hiddenImageColor.b - textColorValue.b) < 0.1f)
                        {
                            //we need to adjust the font color
                            //we are going to adjust the font color to be the opposite of the hidden image color
                            //we are going to use the average of the RGB values to determine if we need to go black or white
                            var avgColor = (hiddenImageColor.r + hiddenImageColor.g + hiddenImageColor.b) / 3;
                            if (avgColor > 0.5f)
                            {
                                //we need to go black
                                textColor = Color.black;
                            }
                            else
                            {
                                //we need to go white
                                textColor = Color.white;
                            }
                        }
                        Debug.LogWarning($"We overwrote the text color from ({oldTextColor.r},{oldTextColor.g},{oldTextColor.b}) to: ({textColor.Value.r},{textColor.Value.g},{textColor.Value.b})");
                    }
                }



            }
            if (useText)
            {
                // Add the TextMeshProUGUI Component
                GameObject textObj = new GameObject(objRefname + "_Text");
                textObj.layer = LayerMask.NameToLayer("UI");
                textObj.transform.SetParent(textBoxObj.transform, false);
                //var textRect = textObj.AddComponent<RectTransform>();
                TextMeshProUGUI tmpText = textObj.AddComponent<TextMeshProUGUI>();
                FontTextGenerationSettings(tmpText, text, txtAlign, fontTextStyle,useAutoSizeFont, fontSizeMin, fontSizeMax, fontSize, textColor);

                /*
                tmpText.text = text;
                tmpText.color = textColor.HasValue ? textColor.Value : Color.black;
                
                if (useAutoSizeFont)
                {
                    //tmpText.fontSize = fontSize;
                    tmpText.enableAutoSizing = useAutoSizeFont;
                    //tmpText.autoSizeTextContainer = useAutoSizeFont;
                    Debug.Log($"Autosize is: {tmpText.autoSizeTextContainer}");
                    tmpText.fontSizeMin = fontSizeMin;
                    tmpText.fontSizeMax = fontSizeMax;
                }
                else
                {
                    tmpText.fontSize = fontSize;
                }
                */
                var textRect = tmpText.GetComponent<RectTransform>();
                textRect.anchorMin = new Vector2(0, 0);
                textRect.anchorMax = new Vector2(1, 1);
                textRect.sizeDelta = Vector2.zero; // Stretch to fill the parent
                textRect.anchoredPosition = Vector2.zero;
            }
            return textBoxObj;
        }
        /// <summary>
        /// Autoadjust font size and color
        /// </summary>
        /// <param name="tmpText"></param>
        /// <param name="text"></param>
        /// <param name="useAutoSizeFont"></param>
        /// <param name="fontSizeMin"></param>
        /// <param name="fontSizeMax"></param>
        /// <param name="fontSize"></param>
        /// <param name="textColor"></param>
        private void FontTextGenerationSettings(TextMeshProUGUI tmpText,string text, TextAlignmentOptions alignmentText, FontStyles fontStyleText, bool useAutoSizeFont =true,float fontSizeMin=6, float fontSizeMax=72, float fontSize=12,Color? textColor = null) 
        {
            tmpText.text = text;
            tmpText.color = textColor.HasValue ? textColor.Value : Color.black;
            tmpText.alignment = alignmentText;
            tmpText.fontStyle = fontStyleText;
            if (useAutoSizeFont)
            {
                //tmpText.fontSize = fontSize;
                tmpText.enableAutoSizing = useAutoSizeFont;
                //tmpText.autoSizeTextContainer = useAutoSizeFont;
                Debug.Log($"Autosize is: {tmpText.autoSizeTextContainer}");
                tmpText.fontSizeMin = fontSizeMin;
                tmpText.fontSizeMax = fontSizeMax;
            }
            else
            {
                tmpText.fontSize = fontSize;
            }
        }

        /// <summary>
        /// Wrapper Function to take my FP_UI_Instruction type and break it into its parts for the icon/signal work
        /// This way later on if we need to modify and/or change this we don't have to rework the brunt of the function which is a private function
        /// </summary>
        /// <param name="instruction"></param>
        public GameObject GenerateSignal(FP_UI_Instruction instruction)
        {
            //this assumes we are using text, outline, backdrop
            var sBox = CreateImage(
               
                instruction.Signal.ArrowLabel,
                instruction.Signal.CenterPt.x,
                instruction.Signal.CenterPt.y,
                instruction.Signal.Theme.Icon,
                instruction.Signal.PixelWidth,
                instruction.Signal.PixelHeight,
                instruction.Signal.RotationClockwise);
            instruction.SignalBox = sBox.GetComponent<RectTransform>();
            GeneratedSignals.Add(instruction, sBox);
            return sBox;
        }
        /// <summary>
        /// Generates the Image Object with the specified parameters
        /// </summary>
        /// <param name="objRefName"></param>
        /// <param name="relativeX"></param>
        /// <param name="relativeY"></param>
        /// <param name="signalImage"></param>
        /// <param name="widthPx"></param>
        /// <param name="heightPx"></param>
        /// <param name="rotationDegrees"></param>
        /// <returns></returns>
        private GameObject CreateImage(string objRefName,float relativeX, float relativeY, Sprite signalImage, int widthPx, int heightPx, float rotationDegrees)
        {
            if(UICanvas==null)
            {
                Debug.LogWarning($"No canvas found, returning null");
                return null;
            }   
            // Create the root object
            GameObject imageObj = new GameObject(objRefName);
            
            imageObj.transform.SetParent(UICanvas.transform, false);


            // Add the Image Component
            Image image = imageObj.AddComponent<Image>();
            if (signalImage != null)
            {
                image.sprite = signalImage;
            }
            

            // Set the size (adjust as needed)
            RectTransform rectTransform = image.GetComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(widthPx, heightPx); // Example size

            // Calculate screen position
            Vector2 screenPosition = new Vector2(Screen.width * relativeX, Screen.height * relativeY);

            // Convert screen position to canvas position
            Vector2 canvasPosition;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(UICanvas.GetComponent<RectTransform>(), screenPosition, UICanvas.worldCamera, out canvasPosition);

            // Set position relative to canvas
            rectTransform.anchoredPosition = canvasPosition;
           
            //Set pivot and anchors (adjust if needed)
            rectTransform.pivot = new Vector2(0.5f, 0.5f);
            rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
            rectTransform.localEulerAngles = new Vector3(0, 0, -rotationDegrees);
            return imageObj;
        }

        /// <summary>
        /// Will generate skip button and return the core gameobject
        /// </summary>
        /// <param name="buttonText"></param>
        /// <param name="ButtonBackground"></param>
        /// <param name="bottomLeft"></param>
        /// <param name="topRight"></param>
        /// <param name="ButtonFont"></param>
        /// <param name="useAutoSizeFont"></param>
        /// <param name="fontSize"></param>
        /// <param name="fontMin"></param>
        /// <param name="fontMax"></param>
        /// <returns></returns>
        private GameObject GenerateSkipButton(string buttonText, ColorBlock ButtonBackground, Vector2 bottomLeft, Vector2 topRight, Color? ButtonFont = null, bool useAutoSizeFont = true, float fontSize = 12, float fontMin = 6, float fontMax = 72)
        {
            //going to generate a UI Text Button following the FP_UI_Instruction information from the first one for theme purposes
            //then going to use the instructions information to move and relocate it's position


            // Create the Button GameObject
            uiSkipButton = new GameObject("SkipInstructions");
            uiSkipButton.transform.SetParent(UICanvas.transform, false);
            uiSkipButton.AddComponent<RectTransform>();



            Button button = uiSkipButton.AddComponent<Button>();
            Image buttonImage = uiSkipButton.AddComponent<Image>();
            button.image = buttonImage;
            if (SkipButtonSprite != null)
            {
                buttonImage.sprite = SkipButtonSprite;
                buttonImage.type = Image.Type.Sliced;
            }

            button.onClick.AddListener(SkipInstructions);

            button.colors = ButtonBackground;
            uiSkipButton.layer = LayerMask.NameToLayer("UI");
            // Add Text to the Button (using TextMeshPro if desired)
            //generate a child text object
            GameObject uiSkipButtonText = new GameObject("SkipInstructionText");
            uiSkipButtonText.transform.SetParent(uiSkipButton.transform, false);
            var backDropRect = uiSkipButtonText.AddComponent<RectTransform>();
            uiSkipButtonText.layer = LayerMask.NameToLayer("UI");
            //RectTransform backdropRect = backdropObj.GetComponent<RectTransform>();
            //little bit of text padding from parent
            backDropRect.anchorMin = new Vector2(0.05f, 0.05f);
            backDropRect.anchorMax = new Vector2(0.95f, 0.95f);
            backDropRect.sizeDelta = Vector2.zero; // Stretch to fill the parent
            backDropRect.anchoredPosition = Vector2.zero;
            //now add the text in
            TextMeshProUGUI buttonTextComponent = uiSkipButtonText.AddComponent<TextMeshProUGUI>();
            //push to match parents size

            //setup font properties
            FontTextGenerationSettings(buttonTextComponent, buttonText, SkipButtonAlignment,SkipButtonFontStyles,useAutoSizeFont, fontMin, fontMax, fontSize, ButtonFont);
            // Set the Button's RectTransform
            RectTransform buttonRect = button.GetComponent<RectTransform>();

            Vector2 bottomLeftScreen = new Vector2(Screen.width * bottomLeft.x, Screen.height * bottomLeft.y);
            Vector2 topRightScreen = new Vector2(Screen.width * topRight.x, Screen.height * topRight.y);
            Vector2 bottomLeftCanvas, topRightCanvas;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(UICanvas.GetComponent<RectTransform>(), bottomLeftScreen, UICanvas.worldCamera, out bottomLeftCanvas);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(UICanvas.GetComponent<RectTransform>(), topRightScreen, UICanvas.worldCamera, out topRightCanvas);
            buttonRect.sizeDelta = topRightCanvas - bottomLeftCanvas;
            buttonRect.anchoredPosition = (topRightCanvas + bottomLeftCanvas) / 2;

            // Set pivot and anchors
            buttonRect.pivot = new Vector2(0.5f, 0.5f);
            buttonRect.anchorMin = new Vector2(0.5f, 0.5f);
            buttonRect.anchorMax = new Vector2(0.5f, 0.5f);
          
            if (buttonImage.type == Image.Type.Sliced)
            {
                //if our image type is sliced, if we take half of our height as our pixels per unit multiplier it usually gives us a nicer
                //when over 15 pixels it gets pretty small so we want to sort of cap it there
                buttonImage.pixelsPerUnitMultiplier = Mathf.Min(buttonRect.sizeDelta.y * 0.5f, 15);
            }
            return uiSkipButton;
        }
        #endregion

    }
}
