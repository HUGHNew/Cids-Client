# Structure of Cids-Client

## Installer



>   Run Installer
>
>   store in `%ProgramFiles%\Cids`

```mermaid
graph TD
GetUrl --fail--> Error
Download --timeout--> Error
GetJson --timeout-->retry--wait_random_time-->GetJson
GetUrl --got--> Download ==Mirror==> GetJson --Join-->set--next_time-->GetJson
```



## Single Exe

Init.cs:

-   Init

### 文件和结构

-   Image.cs
-   Json
    -   JsonCom.cs
-   Socket.cs
-   Time.cs
-   Toast.cs:对Toast消息的简单封装
    -   ToastGenerator



### 执行逻辑

```mermaid
graph TD
首次使用判断--yes-->Init---->Main
首次使用判断--no-->Main-->Mirror==>Mirror
```



#### Main&Mirror

```mermaid
graph TD
GetUrl --fail--> Error
Download --timeout--> Error
GetJson --timeout-->retry--wait_random_time-->GetJson
GetUrl --got--> Download ==Mirror==> GetJson --Join-->set--next_time-->GetJson
```

#### Download

下载到`Init.CidsPath` 图片保存在 `raw.jpg`

-   [] 拼接的图片放在`img`下 从 0 开始编号 每次拼接后选取值最大的

-   [] 每次更新时删除之前的图片 把图片名 定为 `wp.png`



### 可能问题和解决

| 类型        | 行为               | 方案                           |
| ----------- | ------------------ | ------------------------------ |
| 人为/不可控 | 一天内程序多次启动 | 加强注册表修改等敏感信息的检查 |
|             |                    |                                |
|             |                    |                                |
|             |                    |                                |



### Init

>   File stored in `%TMP%\Cids` variable in `Init.CidsPath`

```c#
public const string ClientTitle = "四川大学智慧教学系统壁纸同步工具";
        public const string deskInitConf = "Cids.txt";
        public const string imgName = "Cids.txt";
        public const string RegName = "Cids"; // add to registry
        public const string Conf = "Cids.conf"; // configuration file
        public static readonly string desktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        // Image Stored in %TMP% file
        public static readonly string CidsPath = ((Environment.GetEnvironmentVariables(EnvironmentVariableTarget.Machine)["TMP"] as string)
            ?? @"C:\Windows\Temp") +"\\Cids";
        public static readonly string ConfFile = Path.Combine(CidsPath, Conf);
```

注册表修改

1.  第一次获取`TMP`环境变量使用Machine环境
2.  配置和修改使用User环境的环境变量`Cids`

