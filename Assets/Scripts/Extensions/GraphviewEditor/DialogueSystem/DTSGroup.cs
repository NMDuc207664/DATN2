using System;
using System.Linq;
using DATN2.GraphviewEditor.DialogueEditor;
using DATN2.GraphviewEditor.Style;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace DATN2.GraphviewEditor.DialogueSystem
{
    public class DTSGroup : Group
    {
        public string ID { get; set; }
        public string Title { get; set; }
        public bool HasADialogueTalked { get; set; }

        private Color defaultBorderColor;
        private float defaultBorderWidth;
        private DTSGraphView _graphView;
        public Toggle hasDialogueTalkedToggle;

        public DTSGroup(string groupTitle, Vector2 position, DTSGraphView graphView)
        {
            ID = Guid.NewGuid().ToString();

            Title = groupTitle;
            title = Title;
            HasADialogueTalked = false;
            Debug.Log($"[DTSGroup Constructor] Created group '{groupTitle}' with HasADialogueTalked = {HasADialogueTalked}");
            CreateToggleUI();
            SetPosition(new Rect(position, Vector2.zero));
            _graphView = graphView;
            defaultBorderColor = contentContainer.style.borderBottomColor.value;
            defaultBorderWidth = contentContainer.style.borderBottomWidth.value;
            // Hook sự kiện double click -> attach callback
            RegisterCallback<MouseDownEvent>(OnMouseDownEvent);

            defaultBorderColor = contentContainer.style.borderBottomColor.value;
            defaultBorderWidth = contentContainer.style.borderBottomWidth.value;
        }
        private void CreateToggleUI()
        {
            // Tạo container cho toggle
            VisualElement toggleContainer = new VisualElement();
            toggleContainer.style.flexDirection = FlexDirection.Row;
            toggleContainer.style.alignItems = Align.Center;
            toggleContainer.style.marginTop = 5;
            toggleContainer.style.marginLeft = 5;
            toggleContainer.style.marginRight = 5;

            // Tạo toggle
            hasDialogueTalkedToggle = new Toggle("Has Dialogue Talked")
            {
                value = HasADialogueTalked
            };

            // Disable toggle vì nó sẽ được tự động cập nhật
            hasDialogueTalkedToggle.SetEnabled(false);
            hasDialogueTalkedToggle.style.opacity = 0.7f;

            // Thêm tooltip để giải thích
            hasDialogueTalkedToggle.tooltip = "Automatically checked when at least one node in this group has HasTalked = true";

            toggleContainer.Add(hasDialogueTalkedToggle);

            // Thêm vào headerContainer của Group
            headerContainer.Add(toggleContainer);
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
            _graphView.validator.CheckForDuplicateNames(_graphView);
        }
        public void SetErrorStyle(Color color)
        {
            contentContainer.style.borderBottomColor = color;
            contentContainer.style.borderBottomWidth = 2f;
        }
        public void UpdateHasDialogueTalkedStatus()
        {
            // Tìm tất cả các DTSNode trong group này
            var nodesInGroup = _graphView.graphElements
                .OfType<DTSNode>()
                .Where(node => node.Group == this)
                .ToList();

            // Kiểm tra xem có ít nhất 1 node có HasTalked = true không
            bool hasAnyTalkedNode = nodesInGroup.Any(node => node.HasTalked);

            // Cập nhật trạng thái
            if (HasADialogueTalked != hasAnyTalkedNode)
            {
                HasADialogueTalked = hasAnyTalkedNode;

                // Cập nhật UI toggle
                if (hasDialogueTalkedToggle != null)
                {
                    hasDialogueTalkedToggle.value = HasADialogueTalked;
                }

                // Log để debug
                Debug.Log($"[DTSGroup] Group '{Title}' HasADialogueTalked updated to: {HasADialogueTalked}");
                hasDialogueTalkedToggle.RegisterValueChangedCallback(evt =>
              {
                  HasADialogueTalked = evt.newValue;

              });

            }
        }
        public void ResetStyle()
        {
            contentContainer.style.borderBottomColor = defaultBorderColor;
            contentContainer.style.borderBottomWidth = defaultBorderWidth;
        }
    }
}