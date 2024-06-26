const { Client } = require('whatsapp-web.js')
var qrcode = require('qrcode')

const client = new Client({
puppeteer: {
	headless: true,
	args: ['--no-sandbox', '--disable-setuid-sandbox']
},
webVersionCache: {
type: 'remote',
remotePath: 'https://raw.githubusercontent.com/wppconnect-team/wa-version/main/html/2.2412.54.html',
},
})
client.on('qr', (qr) => {
	console.log(qr)
	qrcode.toFile(
	'fqr.png',
	qr
	)
})
client.initialize()

client.on('message_create', async message => {
	console.log('Message body: '+message.body)
	var chat = await message.getChat()
	console.log('Chat name: '+chat.name)
	var participants = chat.participants
	let particpiantsJson = JSON.stringify(participants)

	await fetch('https://mumarket-api.azurewebsites.net/users/bulk', {  
		method: 'POST',
		headers: {
		  "Content-Type": "application/json",
		},
		body: particpiantsJson
	})
	.then(async function (data) {
		console.log('Participants sent')
	})  
	.catch(function (error) {  
	  console.log('Request failure: ', error)
	})

})

client.on('message_create', async message => {
	var chat = await message.getChat()
	if (!message.hasMedia && chat.isGroup && message.body.includes('/') && (/*chat.name.includes("Alfheim MARKET MuOnline") ||*/ chat.name.includes("Mu Moradito - Market Oficial") || chat.name == "Test")) {
		let msgJson = {
			raw: btoa(message),
			msg: message.body,
			author: message.author
		  }
		await sendCmd(msgJson, message)
	}
})

async function sendCmd(msg, message){
	await fetch('https://mumarket-api.azurewebsites.net/market/commands', {  
	  method: 'POST',
	  headers: {
		"Content-Type": "application/json",
	  },
	  body: JSON.stringify(msg)
  })
  .then(async function (data) {
	var cmdMsgResult = await data.text()

	if(cmdMsgResult != null && cmdMsgResult != ''){
		message.reply(cmdMsgResult)
	}
  })  
  .catch(function (error) {  
	message.reply("I'm busy re-spawning, please try again later")
	console.log('Request failure: ', error)
  })
  }