# electron-asar-hot-updater
[![NPM](https://user-gold-cdn.xitu.io/2018/12/17/167ba2fc49bb1b2e?w=384&h=56&f=png&s=4570)](https://nodei.co/npm/electron-asar-hot-updater/)

中文文档 | [English](README.md)

## What it is

> 一个用于`electron`的NodeJs模块，用于支持app.asar的更新，基于`electron-asar-updater`重构

Github: https://github.com/yansenlei/electron-asar-hot-updater

如果`electron-updater`支持差异更新，那应该是最佳选择了，貌似目前正在尝试，期待ing...

## 如何工作 (Read this first)
* 用于处理更新Electron应用程序内app.asar文件的过程;它只是用名为“update.asar”的新文件替换app.asar文件（在/ resources /）！
* 检查“更新”必须由应用程序触发。 `EAU`不会自行进行任何形式的定期检查。
* `EAU`与API进行对话，告诉它是否有新的更新。
    * API接收来自EAU的请求，其中包含客户端当前版本的应用程序（必须在应用程序package.json文件中指定）
    * 然后，API以新更新响应，或者只是false以中止。
    * 如果有可用更新，则API应使用此更新update.asar文件的源进行响应。
    * EAU然后下载.asar文件，删除旧的app.asar并将update.asar重命名为app.asar。(为了绕开Windows下替换asar存在程序占用的问题，会在关闭Electron应用后启动`updater.exe`，5秒后替换asar)

## 为何要使用它 ? (用例)
* 如果您认为这些太复杂而无法实施:
https://www.npmjs.com/package/electron-updater
http://electron.atom.io/docs/v0.33.0/api/auto-updater/
* 如果您认为在更换一个文件（通常为40MB），.app或.exe文件（最多100MB）是不合理的。
* 需要在更新时查看进度。
* 选择使用服务器端检查或客户端检查。
* 可以使用zip压缩文件，压缩你的ASAR使其更小。

---

## 安装
```bash
$ npm install --save electron-asar-hot-updater
```
现在，在main.js文件中，调用它如下：
```js
const { app, dialog } = require('electron');
const EAU = require('electron-asar-hot-updater');

app.on('ready', function () {
  // Initiate the module
  EAU.init({
    'api': 'http://...', // The API EAU will talk to
    'server': false, // Where to check. true: server side, false: client side, default: true.
    'debug': false // Default: false.
  });

  EAU.check(function (error, last, body) {
    if (error) {
      if (error === 'no_update_available') { return false; }
      dialog.showErrorBox('info', error)
      return false
    }

    EAU.progress(function (state) {
      // The state is an object that looks like this:
      // {
      //     percent: 0.5,               
      //     speed: 554732,              
      //     size: {
      //         total: 90044871,        
      //         transferred: 27610959   
      //     },
      //     time: {
      //         elapsed: 36.235,        
      //         remaining: 81.403       
      //     }
      // }
    })

    EAU.download(function (error) {
      if (error) {
        dialog.showErrorBox('info', error)
        return false
      }
      dialog.showErrorBox('info', 'App updated successfully! Restart it please.')
    })

  })
})
```

## 服务端例子
例如，服务器可以返回版本详细信息
```js
const express = require('express')
var bodyParser = require('body-parser');
const app = express()

app.use(bodyParser.json());
app.use(bodyParser.urlencoded({ extended: false }));

var desktop_app_version = '1.0.0';
var desktop_app_URL = 'http://127.0.0.1:8083/update.asar' // or ../update.zip

app.post('/update', function (req, res) {
  if(req.body && req.body.current != desktop_app_version){ // check for server side
    res.write(JSON.stringify( {"last": desktop_app_version, "source": desktop_app_URL} ).replace(/[\/]/g, '\\/') );
  }else{
    res.write(JSON.stringify( {"last": desktop_app_version} ).replace(/[\/]/g, '\\/') );
  }
  res.end();
});

app.listen(3000)
console.log('run port: 3000')
```
或者您可以返回版本信息供客户端检查
```js
app.post('/update', function (req, res) {
  res.write(JSON.stringify( {
    "name": "app",
    "version": "0.0.1",
    "asar": "http://127.0.0.1:8083/update.asar",
    "sha1": "203448645d8a32b9a08ca9a0eb88006f874d0c78", // 可选项, 如果设置将会验证`asar`文件的合法性
    "info": "1.fix bug\n2.feat..."
  } ).replace(/[\/]/g, '\\/') );
  res.end();
});
```
## 让更新包更小
如果您使用zip文件，插件将在下载后解压缩文件，这将使你的更新文件更小，但你必须确保`update.asar`位于zip包的根目录：
```
── update.zip
   └── update.asar
```
## Windows更新
updater.exe是一个非常简单的C＃控制台应用程序，使用[Mono]（http://www.mono-project.com）编译 [源码](./updater.cs)。来自 [electron-asar-updater pull #2](https://github.com/whitesmith/electron-asar-updater/pull/2)

## License

欢迎提交Issues、PR

MIT - [yansenlei](https://github.com/yansenlei)