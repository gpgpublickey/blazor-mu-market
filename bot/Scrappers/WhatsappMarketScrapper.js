'use strict';

const fs = require('fs')
const puppeteer = require('puppeteer');
const fetch = (...args) => import('node-fetch').then(({default: fetch}) => fetch(...args));
const express = require('express');
const app = express();

// Serve the image file
app.get('/qr', (req, res) => {
    // Replace 'my-image.jpg' with your actual image file path
    const imagePath = __dirname + '/browser.png';
    delay(15000);
    res.sendFile(imagePath);
});

// Start the server
const PORT = 8080;
app.listen(PORT, () => {
    console.log(`Server running on port ${PORT}`);
});

(async () => {
  const browser = await puppeteer.launch({timeout:0, sessionStorage:false, headless: true, args: ['--no-sandbox', '--disable-setuid-sandbox']})//, slowMo: 500
  const page = await browser.newPage()
  await page.setUserAgent('Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/73.0.3641.0 Safari/537.36')
  await page.setViewport({ width: 1024, height: 768 })
  await page.goto('https://web.whatsapp.com/')
  await delay(15000);
  await page.screenshot({path: 'browser.png'})
  
  while(true){
    await delay(3000)
    await pickChatGroups(page, main, browser)
    console.log(" [+] Another round....")
  }
  //await browser.close()
})();

async function delay(time) {
  return new Promise(function(resolve) { 
      setTimeout(resolve, time)
  });
}

async function pickChatGroups(page, routine, brw) {
  return page.$$("span[data-icon='pinned2']").then(async o => await routine(page, o, brw))
}

async function pickMessages(page) {
  return page.$$("div[data-pre-plain-text]")
}

async function pickImages(page) {
  return page.$$("div[role=application] > div > div > div > div > div > div > div > div > div > div > img[src^=blob]")
}

async function main(page, elements, brw){
  elements.forEach(async e => {
    await e?.click()
    await delay(3000)
    let imgs = await pickImages(page)
    let splitImgs = imgs.splice(imgs.length - 1)
    await splitImgs.forEach(async element => {
      var pageImg = await brw.newPage()
      await pageImg.setCacheEnabled(false)
      let imgUri = await element.evaluate(el => el.src)
      let imgJson = {
        post: await element.evaluate(el => el.alt),
        author: await element.evaluate(el => el.parentNode.parentNode.parentNode.parentNode.parentNode.getAttribute('data-pre-plain-text'))
      }
      //console.log(imgUri)
      var imgName = new Date().getTime()+'-img.png'
      await pageImg.goto(imgUri).then(async () => {
        await pageImg.screenshot({path: imgName})
        let bitmap = fs.readFileSync('./'+imgName)
        imgJson.img = new Buffer.from(bitmap).toString('base64')
        fs.unlinkSync('./'+imgName)
      }).catch(() => {})
      await pageImg.close()
      if(imgJson.img != null) {
        //http://localhost:5253/market/sell
        await fetch('https://mumarket-api.azurewebsites.net/market/sell', {  
          method: 'POST',
          headers: {
            "Content-Type": "application/json",
          },
          body: JSON.stringify(imgJson)
        })
      }
    })

    let msgs = await pickMessages(page)
    let splitMsg = msgs.splice(msgs.length - 1)
    await splitMsg.forEach(async element => {
      let msgJson = {
        post: await element.evaluate(el => el.innerText),
        author: await element.evaluate(el => el.getAttribute('data-pre-plain-text'))
      }
      //console.log(msgJson)
      //http://localhost:5253/market/sell
      await fetch('https://mumarket-api.azurewebsites.net/market/sell', {  
          method: 'POST',
          headers: {
            "Content-Type": "application/json",
          },
          body: JSON.stringify(msgJson)
      })
      .then(function (data) {  
        // console.log('Request success: ', data);  
      })  
      .catch(function (error) {  
        console.log('Request failure: ', error);  
      });
    });
  });
}