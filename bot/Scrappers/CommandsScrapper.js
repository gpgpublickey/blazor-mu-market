'use strict';
const fs = require('fs')
const puppeteer = require('puppeteer');
const fetch = (...args) => import('node-fetch').then(({default: fetch}) => fetch(...args));

(async () => {
  const browser = await puppeteer.launch({sessionStorage:false, headless: false})//, slowMo: 500
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

async function main(page, elements, brw){
  elements.forEach(async e => {
    await e?.click()
    await delay(3000)
    let msgs = await pickMessages(page)
    let splitMsg = msgs.splice(msgs.length - 1)
    await splitMsg.forEach(async element => {
      let msgJson = {
        raw: await element.evaluate(el => btoa(el.innerHTML)),
        msg: await element.evaluate(el => btoa(el.innerText)),
        author: await element.evaluate(el => el.getAttribute('data-pre-plain-text'))
      }
      console.log(msgJson)
      if(atob(msgJson.msg).includes('/+1')){
        await sendCmd(msgJson, () => reply(page, 'Comandito mandado'))
      }
      else if(msgJson.msg.includes('/adduser ')){

      }
    });
  });
}



async function reply(page, msg){
  console.warn(msg)
  await delay(2000)
  var searchbar = await page.$("div[data-lexical-editor='true'][spellcheck]")
  await searchbar.press('holaaa')
  await delay(2000)
  await page.keyboard.press("Enter")
}