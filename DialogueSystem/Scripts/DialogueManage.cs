using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueManage : MonoBehaviour
{
    /// <summary>
    /// 对话CSV数据
    /// </summary>
    public TextAsset dialogDataFile;
    /// <summary>
    /// 发言者
    /// </summary>
    public CustomGUILabel characterName;
    /// <summary>
    /// 发言内容
    /// </summary>
    public CustomGUILabel dialogContent;
    /// <summary>
    /// 选项管理器
    /// </summary>
    public ButtonGroupPanel buttongroup;
    /// <summary>
    /// 行字符
    /// </summary>
    private string[] dialogRows;
    /// <summary>
    /// 行内切割字符
    /// </summary>
    private string[] dialogContents;
    /// <summary>
    /// 目前行数下标
    /// </summary>
    private int dialogIndex = 1;
    /// <summary>
    /// 判断玩家此时正在进行对话或选择 
    /// </summary>
    private bool inSelecting = false;

    void Start()
    {
        //读表切割
        ReadText(dialogDataFile);
        //解析一行
        ShowDialogRows();
    }

    void Update()
    {
        if (inSelecting)
        {
            int selectedOption = buttongroup.GetSelected();

            if (selectedOption != 0)
            {
                inSelecting = false;

                // 因为 AddButton 里 currentIndex + 1，所以这里 -1
                int selectedRowIndex = dialogIndex + selectedOption - 1;

                if (selectedRowIndex >= dialogRows.Length)
                {
                    Debug.LogError("选择超出范围！");
                    return;
                }

                string[] selectedRow = dialogRows[selectedRowIndex].Split(',');

                if (selectedRow.Length >= 6)
                {
                    // 更新 dialogIndex，跳转到指定ID
                    dialogIndex = int.Parse(selectedRow[5]);
                    //Debug.Log($"你选择了：{selectedRow[4]}，跳转到对话 ID {dialogIndex}");
                }
                else
                {
                    Debug.LogError("CSV 选项数据不完整！");
                }

                buttongroup.RemoveButton();
                ShowDialogRows();
            }
        }
        else if (Input.GetKeyDown(KeyCode.Space))
        {
            ShowDialogRows();
        }
    }

    //没有进行多选项多结果判断方法
    // Update is called once per frame
    /*void Update()
    {
        if (inSelecting)
        {
            if ( buttongroup.GetSelected() != 0)
            {
                //状态变换
                inSelecting = false;
                dialogIndex =
                ShowDialogRows();
                buttongroup.RemoveButton();
            }
        }
        else if (Input.GetKeyDown(KeyCode.Space))
        {
            ShowDialogRows();
        }
        
    }*/

    private void UpdateDialog(string name, string content)
    {
        characterName.content = new GUIContent(name);
        dialogContent.content = new GUIContent(content);
    }
    private void ReadText(TextAsset textAsset)
    {
        dialogRows = dialogDataFile.text.Split('\n');
    }
    private void ShowDialogRows()
    {
        // 遍历整个表，查找当前 dialogIndex 匹配的行
        for (int i = 0; i < dialogRows.Length; i++)
        {
            dialogContents = dialogRows[i].Split(',');

            if (dialogContents.Length == 0) continue;

            // 对话
            if (dialogContents[0] == ">" && dialogIndex == int.Parse(dialogContents[1]))
            {
                UpdateDialog(dialogContents[2], dialogContents[4]);

                dialogIndex = int.Parse(dialogContents[5]);
                break;
            }

            // 选项处理
            else if (dialogContents[0] == "*" && dialogIndex == int.Parse(dialogContents[1]))
            {
                inSelecting = true;

                int tempIndex = i;
                int optionCount = 0;

                // 收集并添加所有选项按钮
                do
                {
                    buttongroup.AddButton(dialogContents[4]);
                    tempIndex++;
                    optionCount++;

                    if (tempIndex >= dialogRows.Length) break;

                    dialogContents = dialogRows[tempIndex].Split(',');
                }
                while (dialogContents[0] == "*");

                dialogIndex = i; // 记录当前选项开始的位置
                break;
            }
        }
    }
    /*private void ShowDialogRows()
   {
        for (int i = 0; i < dialogRows.Length; i++)
       {
           dialogContents = dialogRows[i].Split(',');
           //判断是对话剧情，并且当前指针指向这个id
           if (dialogContents[0] == ">" && dialogIndex == int.Parse(dialogContents[1]) )
           {
               UpdateDialog(dialogContents[2], dialogContents[4]);
               dialogIndex = int.Parse(dialogContents[5]);
               break;
           }
           else if (dialogContents[0] == "*" && dialogIndex == int.Parse(dialogContents[1])) 
           {
               //将选项按钮的内容传到按钮管理器,按钮管理器负责实例化按钮
               //解析选项组
               inSelecting = true;
               do
               {
                   buttongroup.AddButton(dialogContents[4]);
                   dialogContents = dialogRows[++dialogIndex].Split(",");
               }   while (dialogContents[0] == "*");
               break;
               //按钮管理器返回值进行判断当前选项，并且修改index
           }
       }
   }*/

}
