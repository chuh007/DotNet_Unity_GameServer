using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI
{
    [RequireComponent(typeof(UIDocument))]
    public abstract class AbstractUIScreenController : MonoBehaviour
    {
        protected VisualElement Root { get; private set; }

        protected void OnEnable()
        {
            Root = GetComponent<UIDocument>().rootVisualElement;
            Bind();
        }

        //각 UI 요소에 맞는 연결, OnEnable의 Hook함수.
        protected abstract void Bind();


        #region 조회 Query 헬퍼 함수들

        protected T Q<T>(string elemName) where T : VisualElement => Root.Q<T>(elemName);
        protected Button Btn(string elemName) => Root.Q<Button>(elemName);
        protected Label Lbl(string elemName) => Root.Q<Label>(elemName);

        #endregion

        #region 메시징 헬퍼

        protected const string SuccessClass = "message-label--success";

        protected static void SetMessage(Label label, string text, bool success = false)
        {
            if (label == null) return;
            label.text = text;
            label.EnableInClassList(SuccessClass, success);
        }

        protected static void ClearMessage(Label label)
        {
            SetMessage(label, string.Empty, false);
        }

        #endregion

    }
}