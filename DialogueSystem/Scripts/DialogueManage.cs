using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueManage : MonoBehaviour
{
    /// <summary>
    /// �Ի�CSV����
    /// </summary>
    public TextAsset dialogDataFile;
    /// <summary>
    /// ������
    /// </summary>
    public CustomGUILabel characterName;
    /// <summary>
    /// ��������
    /// </summary>
    public CustomGUILabel dialogContent;
    /// <summary>
    /// ѡ�������
    /// </summary>
    public ButtonGroupPanel buttongroup;
    /// <summary>
    /// ���ַ�
    /// </summary>
    private string[] dialogRows;
    /// <summary>
    /// �����и��ַ�
    /// </summary>
    private string[] dialogContents;
    /// <summary>
    /// Ŀǰ�����±�
    /// </summary>
    private int dialogIndex = 1;
    /// <summary>
    /// �ж���Ҵ�ʱ���ڽ��жԻ���ѡ�� 
    /// </summary>
    private bool inSelecting = false;

    void Start()
    {
        //�����и�
        ReadText(dialogDataFile);
        //����һ��
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

                // ��Ϊ AddButton �� currentIndex + 1���������� -1
                int selectedRowIndex = dialogIndex + selectedOption - 1;

                if (selectedRowIndex >= dialogRows.Length)
                {
                    Debug.LogError("ѡ�񳬳���Χ��");
                    return;
                }

                string[] selectedRow = dialogRows[selectedRowIndex].Split(',');

                if (selectedRow.Length >= 6)
                {
                    // ���� dialogIndex����ת��ָ��ID
                    dialogIndex = int.Parse(selectedRow[5]);
                    //Debug.Log($"��ѡ���ˣ�{selectedRow[4]}����ת���Ի� ID {dialogIndex}");
                }
                else
                {
                    Debug.LogError("CSV ѡ�����ݲ�������");
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

    //û�н��ж�ѡ������жϷ���
    // Update is called once per frame
    /*void Update()
    {
        if (inSelecting)
        {
            if ( buttongroup.GetSelected() != 0)
            {
                //״̬�任
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
        // �������������ҵ�ǰ dialogIndex ƥ�����
        for (int i = 0; i < dialogRows.Length; i++)
        {
            dialogContents = dialogRows[i].Split(',');

            if (dialogContents.Length == 0) continue;

            // �Ի�
            if (dialogContents[0] == ">" && dialogIndex == int.Parse(dialogContents[1]))
            {
                UpdateDialog(dialogContents[2], dialogContents[4]);

                dialogIndex = int.Parse(dialogContents[5]);
                break;
            }

            // ѡ���
            else if (dialogContents[0] == "*" && dialogIndex == int.Parse(dialogContents[1]))
            {
                inSelecting = true;

                int tempIndex = i;
                int optionCount = 0;

                // �ռ����������ѡ�ť
                do
                {
                    buttongroup.AddButton(dialogContents[4]);
                    tempIndex++;
                    optionCount++;

                    if (tempIndex >= dialogRows.Length) break;

                    dialogContents = dialogRows[tempIndex].Split(',');
                }
                while (dialogContents[0] == "*");

                dialogIndex = i; // ��¼��ǰѡ�ʼ��λ��
                break;
            }
        }
    }
    /*private void ShowDialogRows()
   {
        for (int i = 0; i < dialogRows.Length; i++)
       {
           dialogContents = dialogRows[i].Split(',');
           //�ж��ǶԻ����飬���ҵ�ǰָ��ָ�����id
           if (dialogContents[0] == ">" && dialogIndex == int.Parse(dialogContents[1]) )
           {
               UpdateDialog(dialogContents[2], dialogContents[4]);
               dialogIndex = int.Parse(dialogContents[5]);
               break;
           }
           else if (dialogContents[0] == "*" && dialogIndex == int.Parse(dialogContents[1])) 
           {
               //��ѡ�ť�����ݴ�����ť������,��ť����������ʵ������ť
               //����ѡ����
               inSelecting = true;
               do
               {
                   buttongroup.AddButton(dialogContents[4]);
                   dialogContents = dialogRows[++dialogIndex].Split(",");
               }   while (dialogContents[0] == "*");
               break;
               //��ť����������ֵ�����жϵ�ǰѡ������޸�index
           }
       }
   }*/

}
