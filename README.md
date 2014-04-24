#QQHelper QQ小助手
* 功能
  * 消息助手（给群和好友群发消息） 
*注：本文假设你已经有vs2010的开发环境*
启动vs2010，导入项目，本项目是一个WebApp<br>

下面将简单的解析下项目：

## **一、项目的目录结构** ##
> 根目录<br>
> ├ App_Code---c#源码<br>
> ├ Bin---包含json、yiwoSDK .net库<br>
> ├ js---包含jquery库<br>
> ├ v2---基于dojo mobile库的v2版本<br>
> ├ Services.asmx---web服务<br>
> ├ index.html---默认应用页<br>

## **二、提示** ##
开发更多功能请详细查看Bin\yiwoSDK.chm接口文档，在App_Code\Service.cs中加入自己的服务。

