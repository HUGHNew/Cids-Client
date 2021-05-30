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
    -   大量的全局变量 定义路径

### 文件和结构

-   Image.cs:图片合成和存储
-   Json
    -   JsonCom.cs:Json类
-   Socket.cs:UdpClient的实现
    -   SendMain:给Main服务器发送数据包 直到收到IP后才返回IP
    -   SendMirror:给Mirror服务器发送数据报 直到收到数据报才会返回包含json数据类
    -   如果一定时间内 没有任何回应 则认为Mirror挂掉
        -   抛出异常
-   Time.cs
    -   获取协议格式化的数据
    -   基本不用
-   Toast.cs:对Toast消息的简单封装
    -   ToastGenerator



## 执行逻辑

```mermaid
graph TD
首次使用判断--yes-->Init---->Client <--QueryIp--> Main
首次使用判断--no-->Client--HearBeat-->Mirror
Mirror --FirstSet--> Client --mirror_time_out-->Main
```

### 局部分解图

```mermaid
sequenceDiagram
loop The Whole Sequence
Main->>Main: 
Client->>Main: Query Mirror Ip
Main->>Client: Send Mirror Ip

Client->>Mirror: Request Courses Json
Mirror->>Client: Send First Json  With ImgUrl

loop HeartBeat
    Client->>Mirror: Usual Query For Data
end

Note left of Client: Time Out For Recv HeartBeat
end
```



#### Main&Mirror

```mermaid
graph TD
GetIp --fail--> Error
Download --timeout--> Error
GetIp --got--> Download ==Mirror==> GetJson
```

#### HeartBeat

```mermaid
sequenceDiagram
loop ResponseInLimitedTime
	Client ->> Mirror: update
	Mirror -->> Client: Json(maybe no response)
	Client ->> Client: Wait for Json
	Client ->> Client: do something maybe
end
```

#### MessageDisplay

```mermaid
graph LR
JsonRecv-->ImageSynthesis-->SetWallpaper
JsonRecv-->MessageShow-->TimedDisplay-->Toast
JsonRecv-->FileDownload-->SetWallpaper
TimedDisplay-->MessageBox
```

#### IntergrityCheck



```mermaid
graph LR

```

```json
{// --file:%Cids%\CidsConf.json
// 环境变量为 Machine 项为 Cids 以下涉及路径均使用相对路径
    "net":{
        "main_ip":"",// IP Addr
        // 带.就IPv4 不然IPv6
        "main_port":_num,
        "mirror_port":_num,
	},
    "time":{// milli sec
        // the delay of each udp package
        "delay":_num,
        // the interval of heartbeat
        "heartbeat":_num,
        // heartbeat limit times to regard the mirror as a dead
        "limit":__num,
        "random":{
            "min":_num,
            "max":_num
        },
	"logo":""//toast 的 logo 就是弹出消息的一个图片 可以 install 的时候放进去 似乎定为默认字段就好了
    }
}
```



### 类介绍

#### Download

下载到`Init.CidsPath` 图片保存在 `raw.bmp`

-   [] 拼接的图片放在`img`下 从 0 开始编号 每次拼接后选取值最大的

-   [] 每次更新后保存两次的图片
    -   命名为`{wp0.bmp,wp1.bmp}`
    -   两次图片循环使用
    -   直接复写上一次未使用文件





#### Init

>   File stored in `%TMP%\Cids` variable in `Init.CidsPath`

```c#
public const string ClientTitle = "四川大学智慧教学系统壁纸同步工具";
public const string deskInitConf = "Cids.txt";
public const string imgName = "Cids.txt";
public const string RegName = "Cids"; // add to registry
public const string Conf = "Cids.conf"; // configuration file
// Get Set Using User Level Registry
public const EnvironmentVariableTarget Target = EnvironmentVariableTarget.User;
public static readonly string desktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
// Image Stored in %TMP% file
public static readonly string CidsPath = // Get Path First
    $"{Environment.GetEnvironmentVariable("TMP", EnvironmentVariableTarget.Machine)?? "C:\\Windows\\Temp"}\\Cids";
public static readonly string ConfFile = Path.Combine(CidsPath, Conf); // where to get uuid
private static string ValueOfCids = null; // store value
```

注册表修改

1.  第一次获取`TMP`环境变量使用Machine环境
2.  配置和修改使用User环境的环境变量`Cids`

### 可能问题和解决

| 类型        | 行为               | 方案      |
| ----------- | ------------------ | --------- |
| 人为/不可控 | 一天内程序多次启动 | Installer |
| 不可控      | 断网               |           |
|             |                    |           |
|             |                    |           |

