# electron-asar-hot-updater

[![FOSSA Status](https://app.fossa.io/api/projects/git%2Bgithub.com%2Fyansenlei%2Felectron-asar-hot-updater.svg?type=shield)](https://app.fossa.io/projects/git%2Bgithub.com%2Fyansenlei%2Felectron-asar-hot-updater?ref=badge_shield)
[![npm version](https://badge.fury.io/js/electron-asar-hot-updater.svg)](https://badge.fury.io/js/electron-asar-hot-updater)

[![NPM](https://nodei.co/npm/electron-asar-hot-updater.png)](https://nodei.co/npm/electron-asar-hot-updater/)

[中文文档](README-CN.md) | English

[Update demo](https://github.com/yansenlei/asar-update-project)

## What it is
> A NodeJs module for Electron, that handles app.asar updates. Reconstruction of `electron-asar-updater`

## How it works (Read this first)
* EAU (Electron Asar Updater) was built upon _Electron Application Updater_ to handle the process of updating the app.asar file inside an Electron app ; **it simply replaces the app.asar file (at /resources/) with the new one called "update.asar"!**
* The check for "updates" must by triggered by the application. **EAU doesn't make any kind of periodic checks on its own**.
* EAU talks to an API (let's call it so) to tell it if there is a new update.
    * The API receives a request from EAU with the client's **current version of the application (must be specified inside the application package.json file)**.
    * The API then responds with the new update, ... or simply *false* to abort.
    * If there's an update available the API should respond with the *source* for this update **update.asar** file.
    * EAU then downloads the .asar file, deletes the old app.asar and renames the update.asar to app.asar.

## But why ? (use cases)
* If you think these are too complicated to implement:
https://www.npmjs.com/package/electron-updater
http://electron.atom.io/docs/v0.33.0/api/auto-updater/
* If you don't think it's reasonable to update the hole .app or .exe file (up to 100MB) when you're only changing one file (usually 40MB).
* If you want to see `progress` when updating.
* If you want to `check` the version on the `server side` or on the `client side`.
* If you want to use `zip` to compress files, make your ASAR file smaller.

---

## Installation
```bash
$ npm install --save electron-asar-hot-updater
```
Now, inside the *main.js* file, call it like this:
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

## The update server
The server can return the version details, for example
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
Or you can return version information for client to check
```js
app.post('/update', function (req, res) {
  res.write(JSON.stringify( {
    "name": "app",
    "version": "0.0.1",
    "asar": "http://127.0.0.1:8083/update.asar",
    "sha1": "203448645d8a32b9a08ca9a0eb88006f874d0c78", // Optional, If set, verify `asar` file legitimacy
    "info": "1.fix bug\n2.feat..."
  } ).replace(/[\/]/g, '\\/') );
  res.end();
});
```
If you use a zip file, the plug-in will unzip the file after downloading it, which will make your update file smaller, but you must make sure that `update.asar` is at the root of the zip package:
```
── update.zip
   └── update.asar
```

Now uses a really dumb-but-simple .exe to update

This is to get around the fact that the prompt text from the timeout command was always being shown, even when redirecting to NUL

The updater.exe is a really simple C# console app, compiled with [Mono](http://www.mono-project.com).
[Source code](./updater.cs). from [electron-asar-updater pull #2](https://github.com/whitesmith/electron-asar-updater/pull/2).

## License

:smiley: if you have any comments or wish to contribute to this project, you are welcome to submit Issues or PR.

MIT - [yansenlei](https://github.com/yansenlei)
