# BasicsServices
基于.NET5的基础服务项目

#### 介绍

本项目提供基础服务API和定时任务平台，截止2020-08-21已完成的有邮件发送服务和文件上传服务

* 需要使用NuGget安装`Autofac`、`Swashbuckle.AspNetCore`、`CSRedisCore`、`MailKit`、`MongoDB.Driver.GridFS`等
* 定时任务平台使用Hangfire，利用Redis存储定时任务，需要安装包：
	- `Hangfire.Dashboard.BasicAuthorization`
	- `Hangfire.HttpJob`
	- `Hangfire.HttpJob.Agent`
	- `Hangfire.HttpJob.Client`
	- `Hangfire.Redis.StackExchange`
* 文件存储使用MongoDB的GridFS
* 邮件服务基于`MailKit`的支持
* 相关配置详见代码，展示图片：[图片1](https://note.youdao.com/yws/res/8730/WEBRESOURCE7fe0af4d127654c3235ab55bd519191a)、[图片2](http://note.youdao.com/yws/res/8732/WEBRESOURCE3c00713e9d25fa4711aa3fe9a799ca7f);
![图片1](https://note.youdao.com/yws/res/8730/WEBRESOURCE7fe0af4d127654c3235ab55bd519191a)
![图片2](http://note.youdao.com/yws/res/8732/WEBRESOURCE3c00713e9d25fa4711aa3fe9a799ca7f)

#### 版本升级

* 2022-07-02 升级为.NET 5
