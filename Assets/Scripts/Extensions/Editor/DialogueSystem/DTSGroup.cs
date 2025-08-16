using System;
using DATN2.Editor.DialogueEditor;
using DATN2.Editor.Style;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace DATN2.Editor.DialogueSystem
{
    public class DTSGroup : Group
    {
        public string ID { get; set; }
        public string Title { get; set; }

        private Color defaultBorderColor;
        private float defaultBorderWidth;
        private DTSGraphView _graphView;

        public DTSGroup(string groupTitle, Vector2 position, DTSGraphView graphView)
        {
            ID = Guid.NewGuid().ToString();

            Title = groupTitle;
            title = Title;

            SetPosition(new Rect(position, Vector2.zero));
            _graphView = graphView;
            defaultBorderColor = contentContainer.style.borderBottomColor.value;
            defaultBorderWidth = contentContainer.style.borderBottomWidth.value;
            // Hook sự kiện double click -> attach callback
            RegisterCallback<MouseDownEvent>(OnMouseDownEvent);

            defaultBorderColor = contentContainer.style.borderBottomColor.value;
            defaultBorderWidth = contentContainer.style.borderBottomWidth.value;
        }

        private void OnMouseDownEvent(MouseDownEvent evt)
        {
            // Khi double click chuột trái
            if (evt.clickCount == 2 && evt.button == 0)
            {
                // Đợi 1 frame để TextField rename được spawn
                schedule.Execute(() =>
                {
                    var textField = this.Q<TextField>();
                    if (textField != null)
                    {
                        // Clear để tránh đăng ký trùng
                        textField.UnregisterValueChangedCallback(OnTitleChanged);
                        textField.RegisterValueChangedCallback(OnTitleChanged);
                    }
                });
            }
        }

        private void OnTitleChanged(ChangeEvent<string> evt)
        {
            Title = evt.newValue;

            // update lại title của Group
            base.title = Title;

            // gọi check duplicate
            _graphView.CheckForDuplicateNames();
        }
        public void SetErrorStyle(Color color)
        {
            contentContainer.style.borderBottomColor = color;
            contentContainer.style.borderBottomWidth = 2f;
        }

        public void ResetStyle()
        {
            contentContainer.style.borderBottomColor = defaultBorderColor;
            contentContainer.style.borderBottomWidth = defaultBorderWidth;
        }
    }
}