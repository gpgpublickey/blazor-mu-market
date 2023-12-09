'use strict';

const puppeteer = require('puppeteer');
const fetch = (...args) => import('node-fetch').then(({default: fetch}) => fetch(...args));
(async () => {
  const browser = await puppeteer.launch({sessionStorage:false})
  const page = await browser.newPage()
  await page.setUserAgent('Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/73.0.3641.0 Safari/537.36')
  await page.setViewport({ width: 1024, height: 768 })
  await page.goto('https://web.whatsapp.com/')
  await delay(15000);
  await page.screenshot({path: 'browser.png'})

  while(true){
    await delay(3000)
    await pickChatGroups(page, main)
    //console.log(" [+] Another round....")
  }
  //await browser.close()
})();

function delay(time) {
  return new Promise(function(resolve) { 
      setTimeout(resolve, time)
  });
}
44
function pickChatGroups(page, routine) {
  return page.$$("span[data-icon='pinned2']").then(async o => await routine(page, o))
}

function pickMessages(page) {
  return page.$$("div[data-pre-plain-text]")
}

async function main(page, elements){
  elements.forEach(async e => {
    await e?.click()
    await delay(3000)
    let msgs = await pickMessages(page)
    let splitMsg = msgs.splice(msgs.length - 1)
    splitMsg.forEach(async element => {
      let msgJson = {
        post: await element.evaluate(el => el.innerText),
        author: await element.evaluate(el => el.getAttribute('data-pre-plain-text'))
      }
      //console.log(msgJson)
      await fetch('http://localhost:5253/market/sell', {  
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