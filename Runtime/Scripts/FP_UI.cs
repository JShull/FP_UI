namespace FuzzPhyte.UI
{
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;
    using UnityEngine.UIElements;

    #region Data Classes for Checking Style sheet work & Interfaces
    [System.Serializable]
    public class StyleSheetData
    {
        public ComplexSelector[] m_ComplexSelectors;
    }

    [System.Serializable]
    public class ComplexSelector
    {
        public Selector[] m_Selectors;
    }

    [System.Serializable]
    public class Selector
    {
        public Part[] m_Parts;
    }

    [System.Serializable]
    public class Part
    {
        public string m_Value;
        public int m_Type;
    }

    public interface IUIDragState
    {
        void OnDragStarted();
        void OnDragging();
        void OnDragEnded();
        void OnHoverEnter();
        void OnHoverExit();
    }

    #endregion
    /// <summary>
    /// base class for UI elements for the generic Unity UI Toolkit
    /// helpful for looking up style related needs==>https://docs.unity3d.com/Manual/UIE-USS-Properties-Reference.html
    /// </summary>
    public class FP_UI : MonoBehaviour
    {
        [Tooltip("Are we using UIElements?")]
        public bool UseUIElements = true;
        #region UI Elements Parameters
        [Header("UI Element Related")]
        [Tooltip("Main UXML Document")]
        public UIDocument Document;
        [Header("UI Related Containers/Buttons")]
        public VisualElement RootContainer;
        public StyleSheet DocumentStyleSheet;
        //cached list of styles we've seen so we don't have to utilize the json convert over and over again
        protected List<string> checkedStyles = new List<string>();
        #endregion
        public virtual void Awake()
        {
            if (UseUIElements)
            {
                SetupUIElements();
            }
        }
        protected virtual void SetupUIElements()
        {
            if (Document != null)
            {
                RootContainer = Document.rootVisualElement;
            }
            else
            {
                Debug.LogError($"Missing a reference to the UIDocument");
            }
            if (DocumentStyleSheet == null)
            {
                Debug.LogError($"Missing a reference to the core Stylesheet");
            }
            else
            {
                if (!RootContainer.styleSheets.Contains(DocumentStyleSheet))
                {
                    Debug.LogError($"You're referencing a style sheet{DocumentStyleSheet.name} that isn't part of your UI Builder, check your UIDocument and see what style sheet your using and reference it here");
                }
            }
        }
        /// <summary>
        /// This applies the style to the container
        /// </summary>
        /// <param name="theContainer"></param>
        /// <param name="theStyle"></param>
        protected virtual void RemoveStyleToVisualElement(VisualElement theContainer, string theStyle)
        {
            if (ContainsStyle(theStyle))
            {
                if (theContainer.ClassListContains(theStyle))
                {
                    theContainer.RemoveFromClassList(theStyle);
                }
            }
        }

        /// <summary>
        /// THis removes the style to the container
        /// </summary>
        /// <param name="theContainer"></param>
        /// <param name="theStyle"></param>
        protected virtual void AddNewStyleToVisualElement(VisualElement theContainer, string theStyle)
        {
            if (ContainsStyle(theStyle))
            {
                if (!theContainer.ClassListContains(theStyle))
                {
                    theContainer.AddToClassList(theStyle);
                }
            }
            else
            {
                Debug.LogError($"Didn't find the {theStyle} in our current {DocumentStyleSheet.name} stylesheet");
            }
        }
        /// <summary>
        /// Just making sure we have accounts for these styles so we aren't throwing around strings that don't exist
        /// workaround as there's no way to make sure a string based style actually exists within the stylesheet
        /// </summary>
        /// <param name="theStyle"></param>
        /// <returns></returns>
        protected virtual bool ContainsStyle(string theStyle)
        {
            if (checkedStyles.Contains(theStyle))
            {
                return true;
            }
            else
            {
                string json = JsonUtility.ToJson(DocumentStyleSheet, true);
                // Deserialize the JSON into StyleSheetData
                StyleSheetData sheetData = JsonUtility.FromJson<StyleSheetData>(json);
                // this sifts through the JSON via the custom C# classes I have to find what we really care about :m_ComplexSelectors
                // this is sort of crazy that Unity doesn't provide a way to do this within their API... this is just good practice to make sure we don't throw some random string at our UIDocument that doesn't exist
                // within the UI StyleSheet type. We aren't generating anything at runtime, but again just to be on the safe side as we might have Unity Events pass information style via a string
                // totally opens up the possibility of a typo error that we wouldn't actually catch, it would just probably add some random style that has nothing different on it and would be one of those validation bugs that can be hard to identify
                if (sheetData.m_ComplexSelectors.Any(complexSelector => complexSelector.m_Selectors.Any(selector => selector.m_Parts.Any(part => part.m_Value == theStyle))))
                {
                    checkedStyles.Add(theStyle);
                    return true;
                }
                return false;
            }
        }
        /// <summary>
        /// Quick class to quit and open up a GitHub ticket
        /// format is important we are assuming the gitHubURL points to the correct repository and project
        /// </summary>
        /// <param name="gitHubURL">https://github.com/OrganizationName/Project/</param>
        /// <param name="assignee">UserName to assign the issue too</param>
        /// <param name="labels">string array with labels </param>
        /// <param name="template">name of template being used with .md</param>
        /// <param name="projectBoardName">name of the project board</param>
        /// <param name="issueTitle">title for the issue</param>
        /// <param name="versionValue">version of your game</param>

        protected virtual void UIQuitGitHubURL(string gitHubURL, string assignee, string[] labels, string template, string projectBoardName, string issueTitle, string versionValue)
        {
            var systemInfo = SystemInfo.operatingSystem;
            if (!template.Contains(".md"))
            {
                Debug.LogError($"Missing .md on end of template name");
                template += ".md";
            }
            if (!gitHubURL.EndsWith("/"))
            {
                Debug.LogError($"Missing forward slash at end of githuburl");
                gitHubURL += "/";
            }
            //replace whitespace with + on the issueTitle string
            issueTitle = issueTitle.Replace(" ", "+");
            versionValue = versionValue.Replace(" ", "+");

            var gitRoot = string.Concat(gitHubURL, @"/issues/new?assignees=", assignee, "&labels=");
            var gitLabels = string.Join(",", labels);
            gitRoot = string.Concat(gitRoot, gitLabels, @"&projects=", projectBoardName, @"&template=", template, @"&title=%5B", issueTitle, "_", versionValue, "_", systemInfo, @"%5D");

            Debug.Log($"Opening an external link to...{gitRoot}");
            //var stringURL = gitHubURL+ versionValue + "_" + systemInfo + "%5D";
            Application.OpenURL(@gitRoot);
#if UNITY_EDITOR
            UnityEditor.EditorApplication.ExitPlaymode();
#else
            Application.Quit();
#endif
        }
        /// <summary>
        /// Generic quit but open up a URL before quitting
        /// </summary>
        /// <param name="URL"></param>
        protected virtual void UIQuitOpenURL(string URL)
        {
            Application.OpenURL(@URL);
#if UNITY_EDITOR
            UnityEditor.EditorApplication.ExitPlaymode();
#else
            Application.Quit();
#endif

        }

        /// <summary>
        // Based on generating a ratio tied to a list of some sorts generally 
        /// </summary>
        /// <param name="theValue">index in list</param>
        /// <param name="maxValue">max index/length</param>
        protected virtual float UIProgressBar(int theValue, int maxValue, bool reverse = false, int startingValue = 0)
        {
            var maxRange = (maxValue - startingValue) * 1f;
            if (maxRange < 0)
            {
                Debug.LogError($"Can't handle a negative range {maxValue} - {startingValue} = {maxRange}");
                return 0;
            }
            //float ratioConversation = (1 - ((indexValue + 1) / (_currentDialogue.ConversationData.Count * 1f)));
            if (startingValue == 0)
            {
                if (reverse)
                {
                    return 1 - ((theValue + 1) / maxRange);
                }
                return (theValue + 1) / maxRange;

            }
            else
            {
                if (reverse)
                {
                    return 1 - ((theValue + 1 - startingValue) / maxRange);
                }
                return (theValue + 1 - startingValue) / maxRange;
            }
        }

        protected virtual void ApplyOneStyleToAll(FPUITStyleData styleData, IEnumerable<VisualElement> elements)
        {
            foreach (var element in elements)
            {
                if (element is IFPStyleReceiver receiver)
                {
                    receiver.ApplyFPStyle(styleData);
                }
            }
        }
    }
}
