using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class WindowSceneLogic : MonoBehaviour
{
    [DllImport("user32.dll", CharSet = CharSet.Unicode)] //내부적으로 uint로 정리되어 있어서 무조건 써야함
    private static extern int MessageBox(IntPtr hwnd, string lpText, string lpCaption, uint flags);

    private static extern IntPtr GetActiveWindow();

    private static IntPtr windowHandle;
    private bool isfirewall = true;

    public bool isExecute;
    private bool isMoveTime;

    private string serchFile;
    public GameObject windowFinder;

    public UnityEvent OnTextEvent;

    [SerializeField]
    private TMP_InputField inputField;
    public string fileName;


    public string[] tip;

    [field: SerializeField]
    private GameObject[] app;
    private List<Vector3> appPos = new();

    [SerializeField] private GameEventChannelSO textChannel;

    string desktop_path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);


    private void Awake()
    {
        foreach (GameObject pos in app)
        {
            appPos.Add(pos.transform.position);
        }
    }

    public static IntPtr GetWindowHandle()
    {
        if (windowHandle == null)
        {
            windowHandle = GetActiveWindow();
        }
        return windowHandle;
    }

    private int Answer(string appName, string appDescript)
    {
        int answer;
        answer = MessageBox(GetWindowHandle(), appName, appDescript, (uint)(0x00000001L | 0x00000030L));
        return answer;
    }

    public bool DuplicateCheck()
    {
        if (isExecute) return false;
        isExecute = true;
        return isExecute;
    }

    public void GGMSite(string appName,string appDescript)
    {
        if (DuplicateCheck() == false) return;

        if (Answer(appDescript, appName) == 1)
        {
            Process.Start("https://ggm-h.goeay.kr/ggm-h/main.do");
            isExecute = false;
        }
        else
        {
            isExecute = false;
        }
    }

    public void FishProfile(string appName, string appDescript)
    {
        if (DuplicateCheck() == false) return;

        if (Answer(appDescript, appName) == 1)
        {
            Process.Start("https://ggm.gondr.net/user/profile/352");
            isExecute = false;
        }
        else
        {
            isExecute = false;
        }
    }

    public void LCMProfile(string appName, string appDescript)
    {
        if (DuplicateCheck() == false) return;

        if (Answer(appDescript, appName) == 1)
        {
            Process.Start("https://ggm.gondr.net/user/profile/327");
            isExecute = false;
        }
        else
        {
            isExecute = false;
        }
    }
    public void Tip(string appName, string appDescript)
    {
        if (DuplicateCheck() == false) return;
        TextEvent events = UIEvent.ErrorTextEvect;
        events.Text = tip[FindAnyObjectByType<Player>().TipCount];
        events.textType = TextType.Help;
        events.isDefunct = true;
        events.TextSkipKey = KeyCode.Mouse0;
        isExecute = false;
        textChannel.RaiseEvent(events);
    }

    public void Rewind(string appName, string appDescript)
    {
        if (DuplicateCheck() == false) return;
        for(int i =0; i<app.Length; i++)
        {
            app[i].transform.position = appPos[i];
            app[i].GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;
            isExecute = false;
        }
    }

    public void ComeBack()
    {
        for (int i = 0; i < app.Length; i++)
        {
            app[i].transform.position = appPos[i];
            app[i].GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;
            isExecute = false;
        }
    }

    public void Mandle(string appName, string appDescript)
    {
        if (DuplicateCheck() == false) return;

        if (Answer(appDescript, appName) == 1)
        {
            Process.Start("https://mandlemandle.com/");
            isExecute = false;
        }
        else
        {
            isExecute = false;
        }
    }
    private void Start()
    {
        FileInfo file = new FileInfo(desktop_path + @"\" + @"마왕의 위치정보\item.txt");
        if (file.Exists)
        {
            File.Delete(desktop_path + @"\" + @"마왕의 위치정보\item.txt");
        }
    }

    private void Update()
    {
        if (isMoveTime)
        {
            FileInfo file = new FileInfo(serchFile);
            if (file.Exists)  //해당 파일이 없으면 생성하고 파일 닫기
            {
                OnTextEvent?.Invoke();
                UnityEngine.Debug.Log("성공");
                isMoveTime = false;
                
                FindAnyObjectByType<Player>().isDialogue =false;
            }
        }
        if (fileName.Length > 0 && Input.GetKeyDown(KeyCode.Return))
        {
            switch (fileName)
            {
                case "마왕의 위치정보":
                    MakeDirectory();
                    WindowBoxSetFalse();
                    break;
                case "마왕의 위치정":
                    MakeDirectory();
                    WindowBoxSetFalse();
                    break;
                default:
                    inputField.placeholder.GetComponent<TextMeshProUGUI>().text = "오류난 폴더를 입력하세요";
                    break;
            };
        }
    }

    public void InputFile()
    {
        fileName = inputField.text;
    }

    public void WindowBoxSetFalse()
    {
        windowFinder.SetActive(false);
        FindAnyObjectByType<Player>().isDialogue = false;
        isExecute = false;
    }

    public void SerchFile(string appName, string appDescript)
    {
        if (DuplicateCheck() == false) return;
        windowFinder.SetActive(true);
        FindAnyObjectByType<Player>().isDialogue = true;
        isExecute = false;
    }

    private void MakeDirectory()
    {
        print("됐냐");
        string npcFolder_path = CreateDirectory("마왕의 위치정보");
        string itemFolder_path = CreateDirectory("버려진 폴더");

        //string itemFolder_path = desktop_path + @"\버려진 폴더";
        //DirectoryInfo itemDirectoryInfo = new DirectoryInfo(itemFolder_path);
        //if (itemDirectoryInfo.Exists != true)
        //    itemDirectoryInfo.Create();

        string path = itemFolder_path + @"\item.txt";
        //var writer = new StreamWriter(File.Open(path, FileMode.OpenOrCreate));
        //writer.Close();

        var writer = new StreamWriter(File.Open(path, FileMode.OpenOrCreate));
        writer.WriteLine("이 파일을 '마왕의 위치정보' 폴더로 옮겨주세요");
        writer.Close();

        FileInfo file = new FileInfo(path);
        if (!file.Exists)  //해당 파일이 없으면 생성하고 파일 닫기
        {
            FileStream fs = file.Create();
            fs.Close();
        }

        isMoveTime = true;
        SetSeachFile(npcFolder_path + @"\item.txt");
        //string file_name = "ScreenShot.png";
        //string source_path = Application.dataPath + @"\ScreenShot.png";
        //string target_path = itemFolder_path;

        //string source_file = Path.Combine(source_path, file_name);
        //string dest_file = Path.Combine(target_path, file_name);

        //File.Copy(source_file, dest_file, true);

        //System.IO.File.Move(source_file, dest_file);

        //ProcessStartInfo startInfo = new ProcessStartInfo(path)
        //{
        //    UseShellExecute = true
        //};
        //Process.Start(startInfo);

        UnityEngine.Debug.Log(npcFolder_path + @"\item.txt");
        Process.Start("explorer.exe", $"{npcFolder_path}");
        Process.Start("explorer.exe", $"{itemFolder_path}");
    }

    private void SetSeachFile(string ss)
    {
        serchFile = ss;
    }

    private string CreateDirectory(string directoryName)
    {
        string folder_path = desktop_path + @"\"+directoryName;
        DirectoryInfo npcDirectoryInfo = new DirectoryInfo(folder_path);
        if (npcDirectoryInfo.Exists != true)
            npcDirectoryInfo.Create();

        return folder_path;
    }

    public void Password(string appName, string appDescript)
    {
        if (DuplicateCheck() == false) return;

        if (isfirewall)
        {
            MessageBox(GetWindowHandle(), appName, appDescript, (uint)(0x00000000L | 0x00000030L));
            isExecute = false;
            return;
        }
    }

    public void Photo(string appName, string appDescript)
    {
        if (DuplicateCheck() == false) return;
        //if (Answer(appDescript, appName) == 1)
        //{
        //    string filePath = Application.dataPath + @"\ScreenShot.png";
        //    ProcessStartInfo startInfo = new ProcessStartInfo(filePath)
        //    {
        //        UseShellExecute = true
        //    };
        //    Process.Start(startInfo);

        //    string strPath = Application.dataPath + @"\ScreenShot.png";

        //    print(strPath);
        if (Answer(appDescript, appName) == 1)
        {
            Process.Start("https://www.youtube.com/@jjangfish");
            isExecute = false;
        }
        else
            isExecute = false;

            //Process.Start($"ms-photos:viewer?fileName={strPath}");
        }
    }
